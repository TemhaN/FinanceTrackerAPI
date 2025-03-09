using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.DTOs;
using System.Text;
using System;


namespace FinanceTrackerAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly FinanceContext _context;

        public UsersController(FinanceContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CurrentBalance = u.CurrentBalance,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    CurrencyId = u.UserParameters.FirstOrDefault().CurrencyId,
                    ImageUrl = u.ImageUrl // добавляем путь к изображению
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            // Проверка на null для ImageUrl
            if (string.IsNullOrEmpty(user.ImageUrl))
            {
                user.ImageUrl = null;
            }

            return Ok(user);
        }

        // Models/UserResponse.cs (или в существующем файле моделей)
        public class UserResponse
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public decimal CurrentBalance { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? LastLogin { get; set; }
            public int? CurrencyId { get; set; }
            public string ImageUrl { get; set; }
            public string PasswordHash { get; set; } // Может быть null, если пароль не обновлялся
        }

        // Controllers/UserController.cs (или ваш контроллер)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserParameters) // Включаем UserCurrencies для доступа к валюте
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Флаг для отслеживания изменений пароля
            bool passwordUpdated = false;
            string newPasswordHash = null;

            // Если в запросе указаны старый и новый пароли, проверяем старый пароль
            if (!string.IsNullOrEmpty(request.OldPassword) || !string.IsNullOrEmpty(request.NewPassword))
            {
                if (string.IsNullOrEmpty(request.OldPassword))
                {
                    return BadRequest("Old password is required when updating password.");
                }

                // Проверяем старый пароль
                if (!VerifyPassword(request.OldPassword, user.PasswordHash))
                {
                    return Unauthorized("Неверный старый пароль");
                }

                // Обновляем пароль, если новый пароль указан
                if (!string.IsNullOrEmpty(request.NewPassword))
                {
                    newPasswordHash = HashPassword(request.NewPassword); // Хешируем новый пароль
                    user.PasswordHash = newPasswordHash;
                    passwordUpdated = true;
                }
            }

            // Обновляем имя пользователя, если оно указано
            if (!string.IsNullOrEmpty(request.Username))
            {
                user.Username = request.Username;
            }

            // Обновляем email, если он указан (если пустое значение не передано)
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            // Обработка загрузки изображения, если оно указано
            string newImageUrl = user.ImageUrl; // Сохраняем текущий URL изображения
            if (request.ImageFile != null)
            {
                // Генерируем уникальное имя для файла
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);

                // Определяем путь для сохранения изображения в папке wwwroot/images
                var filePath = Path.Combine("wwwroot/images", uniqueFileName);

                // Сохраняем файл на сервере
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                // Сохраняем только относительный путь (без wwwroot) в базе данных
                newImageUrl = "images/" + uniqueFileName; // Путь будет относительным
                user.ImageUrl = newImageUrl;
            }

            // Сохраняем изменения в базе данных
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            int? currencyId = user.UserParameters?.FirstOrDefault()?.CurrencyId;

            // Подготавливаем ответ с обновлёнными данными пользователя
            var responseUser = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CurrentBalance = user.CurrentBalance,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                CurrencyId = currencyId,
                ImageUrl = newImageUrl
            };

            // Если пароль был обновлён, добавляем его в ответ
            if (passwordUpdated)
            {
                responseUser.PasswordHash = newPasswordHash; // Добавляем хешированный пароль
            }

            return Ok(new
            {
                message = "Профиль успешно обновлен",
                user = responseUser
            });
        }

        // Метод для хеширования пароля через SHA256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Метод для проверки пароля
        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == storedHash;
        }

            // Получение настроек пользователя
            [HttpGet("{id}/parameters")]
            public async Task<IActionResult> GetUserParameters(int id)
            {
                var userParameters = await _context.UserParameters
                    .Where(up => up.UserId == id)
                    .Select(up => new UserParameterDto
                    {
                        Id = up.Id,
                        UserId = up.UserId,
                        CurrencyId = up.CurrencyId,
                        MaxMonthlySpending = up.MaxMonthlySpending,
                        MaxEntertainmentSpending = up.MaxEntertainmentSpending,
                        MaxSavingsGoal = up.MaxSavingsGoal
                    })
                    .FirstOrDefaultAsync();

                if (userParameters == null)
                {
                    return NotFound("Настройки пользователя не найдены");
                }

                return Ok(userParameters);
            }

        // Обновление настроек пользователя
        [HttpPut("{id}/parameters")]
        public async Task<IActionResult> UpdateUserParameters(int id, [FromBody] UpdateUserParameterRequest request)
        {
            // Проверяем, существует ли пользователь
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
            {
                return NotFound("Пользователь не найден");
            }

            var userParameters = await _context.UserParameters
                .FirstOrDefaultAsync(up => up.UserId == id);

            if (userParameters == null)
            {
                // Если настройки не существуют, создаем новые
                userParameters = new UserParameter
                {
                    UserId = id
                };
                _context.UserParameters.Add(userParameters);
            }

            // Обновляем только те поля, которые переданы в запросе и не равны 0
            if (request.CurrencyId.HasValue && request.CurrencyId.Value != 0)
            {
                var currencyExists = await _context.Currencies.AnyAsync(c => c.Id == request.CurrencyId.Value);
                if (!currencyExists)
                {
                    return BadRequest($"Валюта с ID {request.CurrencyId.Value} не существует");
                }
                userParameters.CurrencyId = request.CurrencyId.Value;
            }

            if (request.MaxMonthlySpending.HasValue && request.MaxMonthlySpending.Value != 0)
            {
                userParameters.MaxMonthlySpending = request.MaxMonthlySpending.Value;
            }

            if (request.MaxEntertainmentSpending.HasValue && request.MaxEntertainmentSpending.Value != 0)
            {
                userParameters.MaxEntertainmentSpending = request.MaxEntertainmentSpending.Value;
            }

            if (request.MaxSavingsGoal.HasValue && request.MaxSavingsGoal.Value != 0)
            {
                userParameters.MaxSavingsGoal = request.MaxSavingsGoal.Value;
            }

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            // Подготавливаем ответ
            return Ok(new
            {
                message = "Настройки пользователя успешно обновлены",
                id = userParameters.Id,
                userId = userParameters.UserId,
                currencyId = userParameters.CurrencyId,
                maxMonthlySpending = userParameters.MaxMonthlySpending,
                maxEntertainmentSpending = userParameters.MaxEntertainmentSpending,
                maxSavingsGoal = userParameters.MaxSavingsGoal
            });
        }

        // Получение общей суммы доходов пользователя
        [HttpGet("{id}/income")]
        public async Task<IActionResult> GetTotalIncome(int id)
        {
            var totalIncome = await _context.Incomes
                .Where(i => i.UserId == id)
                .SumAsync(i => i.Amount);

            return Ok(new { totalIncome });
        }

        // Получение общей суммы расходов пользователя
        [HttpGet("{id}/expenses")]
        public async Task<IActionResult> GetTotalExpenses(int id)
        {
            var totalExpenses = await _context.Expenses
                .Where(e => e.UserId == id)
                .SumAsync(e => e.Amount);

            return Ok(new { totalExpenses });
        }

        // Получение общей суммы разовых расходов пользователя
        [HttpGet("{id}/instant-expenses")]
        public async Task<IActionResult> GetTotalInstantExpenses(int id)
        {
            var totalInstantExpenses = await _context.InstantExpenses
                .Where(i => i.UserId == id)
                .SumAsync(i => i.Amount);

            return Ok(new { totalInstantExpenses });
        }

        // Получение общей суммы финансовых операций пользователя (доходы и расходы)
        [HttpGet("{id}/financial-operations")]
        public async Task<IActionResult> GetTotalFinancialOperations(int id)
        {
            var totalFinancialOperations = await _context.FinancialOperations
                .Where(f => f.UserId == id)
                .SumAsync(f => f.Amount);

            return Ok(new { totalFinancialOperations });
        }

        // Метод для получения доходов по категориям

        [HttpGet("{id}/income-by-category")]
        public async Task<IActionResult> GetIncomeByCategory(int id)
        {
            // Получаем все доходы для пользователя
            var incomeData = await _context.Incomes
                .Where(i => i.UserId == id)
                .ToListAsync();

            // Группируем их по категории
            var incomeByCategory = incomeData
                .GroupBy(i => i.OperationCategoryId)
                .Join(
                    _context.OperationCategories, // соединяем с таблицей категорий
                    income => income.Key, // связываем по OperationCategoryId
                    category => category.Id, // связываем с id категории
                    (incomeGroup, category) => new
                    {
                        CategoryId = category.Id,
                        CategoryName = category.CategoryName,
                        TotalAmount = incomeGroup.Sum(i => i.Amount)
                    })
                .ToList();

            return Ok(incomeByCategory);
        }


        // Метод для получения статистики по операциям за последний месяц
        [HttpGet("{id}/operations-last-month")]
        public async Task<IActionResult> GetOperationsLastMonth(int id)
        {
            var lastMonthDate = DateTime.Now.AddMonths(-1);
            var operationsLastMonth = await _context.FinancialOperations
                .Where(f => f.UserId == id && f.OperationDate >= lastMonthDate)
                .Select(f => new
                {
                    f.OperationName,
                    f.Amount,
                    f.OperationDate
                })
                .ToListAsync();

            return Ok(operationsLastMonth);
        }

        // Метод для получения среднего дохода за период
        [HttpGet("{id}/average-income-last-year")]
        public async Task<IActionResult> GetAverageIncomeLastYear(int id)
        {
            var lastYearDate = DateTime.Now.AddYears(-1);
            var incomeLastYear = await _context.Incomes
                .Where(i => i.UserId == id && i.StartDate >= lastYearDate)
                .ToListAsync();

            if (incomeLastYear.Count == 0)
            {
                return Ok(new { averageIncome = 0 });
            }

            var averageIncome = incomeLastYear.Average(i => i.Amount);
            return Ok(new { averageIncome });
        }

        // Метод для получения самых больших расходов
        [HttpGet("{id}/largest-expenses")]
        public async Task<IActionResult> GetLargestExpenses(int id)
        {
            var largestExpenses = await _context.Expenses
                .Where(e => e.UserId == id)
                .GroupJoin(
                    _context.OperationCategories,
                    expense => expense.CategoryId,
                    category => category.Id,
                    (expense, categories) => new { Expense = expense, Category = categories.FirstOrDefault() }
                )
                .Select(x => new ExpenseWithCategoryDto
                {
                    Id = x.Expense.Id,
                    UserId = x.Expense.UserId,
                    Amount = x.Expense.Amount,
                    StartDate = x.Expense.StartDate,
                    EndDate = x.Expense.EndDate,
                    Frequency = x.Expense.Frequency,
                    RepeatCount = x.Expense.RepeatCount,
                    Description = x.Expense.Description,
                    CategoryId = x.Expense.CategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : "Без категории"
                })
                .OrderByDescending(e => e.Amount)
                .Take(5)
                .ToListAsync();

            return Ok(largestExpenses);
        }

        // Метод для получения самых крупных доходов
        [HttpGet("{id}/largest-incomes")]
        public async Task<IActionResult> GetLargestIncomes(int id)
        {
            var largestIncomes = await _context.Incomes
                .Where(i => i.UserId == id)
                .GroupJoin(
                    _context.OperationCategories,
                    income => income.OperationCategoryId,
                    category => category.Id,
                    (income, categories) => new { Income = income, Category = categories.FirstOrDefault() }
                )
                .Select(x => new IncomeWithCategoryDto
                {
                    Id = x.Income.Id,
                    UserId = x.Income.UserId,
                    Amount = x.Income.Amount,
                    StartDate = x.Income.StartDate,
                    EndDate = x.Income.EndDate,
                    Frequency = x.Income.Frequency,
                    RepeatCount = x.Income.RepeatCount,
                    OperationCategoryId = x.Income.OperationCategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : "Без категории"
                })
                .OrderByDescending(i => i.Amount)
                .Take(5)
                .ToListAsync();

            return Ok(largestIncomes);
        }

        // Метод для получения общего количества операций
        [HttpGet("{id}/total-operations-count")]
        public async Task<IActionResult> GetTotalOperationsCount(int id)
        {
            var totalOperationsCount = await _context.FinancialOperations
                .Where(f => f.UserId == id)
                .CountAsync();

            return Ok(new { totalOperationsCount });
        }



    }
}
