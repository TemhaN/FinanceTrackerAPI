using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly FinanceContext _context;

        public CurrencyController(FinanceContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCurrencies()
        {
            var currencies = await _context.Currencies
                .Select(c => new
                {
                    c.Id,
                    c.CurrencyCode,
                    c.CurrencyName,
                    c.ExchangeRate
                })
                .ToListAsync();

            return Ok(currencies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCurrencyById(int id)
        {
            var currency = await _context.Currencies
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.CurrencyCode,
                    c.CurrencyName,
                    c.ExchangeRate
                })
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                return NotFound("Валюта с таким ID не найдена.");
            }

            return Ok(currency);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserCurrency([FromBody] UpdateCurrencyModel model)
        {
            if (model.UserId <= 0 || model.CurrencyId <= 0)
            {
                return BadRequest("Некорректные данные.");
            }

            // Проверяем, существует ли валюта
            var currencyExists = await _context.Currencies.AnyAsync(c => c.Id == model.CurrencyId);
            if (!currencyExists)
            {
                return BadRequest("Выбранная валюта не существует.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Получаем запись о параметрах пользователя (было UserCurrencies)
                var userParameter = await _context.UserParameters
                    .FirstOrDefaultAsync(up => up.UserId == model.UserId);

                if (userParameter != null)
                {
                    // Обновляем валюту
                    userParameter.CurrencyId = model.CurrencyId;
                    _context.UserParameters.Update(userParameter);
                }
                else
                {
                    // Если у пользователя нет параметров, создаем новую запись
                    var newUserParameter = new UserParameter
                    {
                        UserId = model.UserId,
                        CurrencyId = model.CurrencyId
                        // Новые поля (MaxMonthlySpending, MaxEntertainmentSpending, MaxSavingsGoal) остаются NULL по умолчанию
                    };
                    _context.UserParameters.Add(newUserParameter);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Валюта успешно обновлена." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ошибка при обновлении валюты.");
            }
        }
    }
}