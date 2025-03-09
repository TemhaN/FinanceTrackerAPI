using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FinanceContext _context;

        public AuthController(FinanceContext context)
        {
            _context = context;
        }

            [HttpPost("register")]
            public async Task<ActionResult<AuthResponse>> Register(RegisterModel model)
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Email и пароль обязательны.");
                }

                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    if (!VerifyPassword(model.Password, existingUser.PasswordHash))
                    {
                        return Unauthorized("Неверный пароль.");
                    }

                    existingUser.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    var session = await CreateSession(existingUser.Id);
                    var userParameter = await _context.UserParameters // Замена UserCurrencies на UserParameters
                        .Where(up => up.UserId == existingUser.Id)
                        .Select(up => up.CurrencyId)
                        .FirstOrDefaultAsync();

                    return Ok(new AuthResponse
                    {
                        UserId = existingUser.Id,
                        Username = existingUser.Username,
                        Email = existingUser.Email,
                        CurrentBalance = existingUser.CurrentBalance,
                        CreatedAt = existingUser.CreatedAt,
                        LastLogin = existingUser.LastLogin,
                        SessionToken = session.SessionToken,
                        ExpiresAt = session.ExpiresAt,
                        CurrencyId = userParameter,
                        ImageUrl = existingUser.ImageUrl // Возвращаем путь к изображению
                    });
                }

                string username = string.IsNullOrEmpty(model.Username) ? model.Email.Split('@')[0] : model.Username;
                int currencyId = model.CurrencyId ?? 1;

                bool currencyExists = await _context.Currencies.AnyAsync(c => c.Id == currencyId);
                if (!currencyExists)
                {
                    return BadRequest("Выбранная валюта не существует.");
                }

                decimal initialBalance = model.CurrentBalance ?? 0; // Если не передано, ставим 0

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        string hashedPassword = HashPassword(model.Password);
                        DateTime now = DateTime.UtcNow;

                        var newUser = new User
                        {
                            Username = username,
                            Email = model.Email,
                            PasswordHash = hashedPassword,
                            CreatedAt = now,
                            LastLogin = now,
                            CurrentBalance = initialBalance, // Сохраняем баланс
                            ImageUrl = model.ImageFile != null ? await SaveImage(model.ImageFile) : null // Сохраняем изображение, если оно есть
                        };

                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();

                        var userParameter = new UserParameter // Замена UserCurrency на UserParameter
                        {
                            UserId = newUser.Id,
                            CurrencyId = currencyId
                            // Новые поля (MaxMonthlySpending, MaxEntertainmentSpending, MaxSavingsGoal) остаются NULL по умолчанию
                        };

                        _context.UserParameters.Add(userParameter); // Замена UserCurrencies на UserParameters
                        await _context.SaveChangesAsync();

                        var sessionNew = await CreateSession(newUser.Id);
                        await transaction.CommitAsync();

                        return Ok(new AuthResponse
                        {
                            UserId = newUser.Id,
                            Username = newUser.Username,
                            Email = newUser.Email,
                            CurrentBalance = newUser.CurrentBalance,
                            CreatedAt = newUser.CreatedAt,
                            LastLogin = newUser.LastLogin,
                            SessionToken = sessionNew.SessionToken,
                            ExpiresAt = sessionNew.ExpiresAt,
                            CurrencyId = currencyId,
                            ImageUrl = newUser.ImageUrl // Возвращаем путь к изображению в ответе
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(500, "Ошибка регистрации: " + ex.Message);
                    }
                }
            }

            // Метод для сохранения изображения и возвращения пути
            private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine("wwwroot/images", uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "images/" + uniqueFileName; // Возвращаем относительный путь к изображению
        }



        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Неверный email или пароль.");
            }

            // Получаем валюту пользователя
            var userCurrency = await _context.UserParameters
                .Where(uc => uc.UserId == user.Id)
                .Select(uc => uc.CurrencyId)
                .FirstOrDefaultAsync();

            // Обновляем время последнего входа
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Проверяем, есть ли активная сессия
            var activeSession = await _context.UserSessions
                .Where(s => s.UserId == user.Id && s.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (activeSession != null)
            {
                activeSession.LastAccessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CurrentBalance = user.CurrentBalance,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    SessionToken = activeSession.SessionToken,
                    ExpiresAt = activeSession.ExpiresAt,
                    CurrencyId = userCurrency,
                    ImageUrl = user.ImageUrl
                });
            }

            // Создаем новую сессию
            var session = await CreateSession(user.Id);

            return Ok(new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                CurrentBalance = user.CurrentBalance,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                SessionToken = session.SessionToken,
                ExpiresAt = session.ExpiresAt,
                CurrencyId = userCurrency,
                ImageUrl = user.ImageUrl
            });
        }


        // Метод для создания новой сессии
        private async Task<AuthResponse> CreateSession(int userId)
        {
            string token = GenerateSessionToken();
            var expiresAt = DateTime.UtcNow.AddHours(2);

            var session = new UserSession
            {
                UserId = userId,
                SessionToken = token,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                SessionToken = token,
                UserId = userId,
                ExpiresAt = expiresAt
            };
        }

        // Генерация случайного токена
        private string GenerateSessionToken()
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[32];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        // Хеширование пароля
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Проверка пароля
        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == storedHash;
        }
    }
}
