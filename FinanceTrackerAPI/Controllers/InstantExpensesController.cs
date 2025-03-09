using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.DTOs;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/instant-expenses")]
    [ApiController]
    public class InstantExpensesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InstantExpensesController(FinanceContext context)
        {
            _context = context;
        }

        // Получить все разовые расходы пользователя
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InstantExpense>>> GetUserExpenses(int userId)
        {
            var expenses = await _context.InstantExpenses.Where(e => e.UserId == userId).ToListAsync();
            return Ok(expenses);
        }

        // Добавить разовый расход
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] InstantExpenseCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var expense = new InstantExpense
                {
                    UserId = dto.UserId,
                    ShopName = dto.ShopName,
                    PurchaseDate = dto.PurchaseDate,
                    Amount = dto.Amount,
                    Description = dto.Description
                };

                _context.InstantExpenses.Add(expense);
                await _context.SaveChangesAsync();

                // Обновляем баланс пользователя
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user != null)
                {
                    user.CurrentBalance -= dto.Amount;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    id = expense.Id, // Возвращаем ID, который присвоила база
                    userId = expense.UserId,
                    shopName = expense.ShopName,
                    purchaseDate = expense.PurchaseDate,
                    amount = expense.Amount,
                    description = expense.Description
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ошибка при добавлении расхода.");
            }
        }

    }
}
