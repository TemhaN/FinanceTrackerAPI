using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.DTOs;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/expenses")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public ExpensesController(FinanceContext context)
        {
            _context = context;
        }

        // Получить все постоянные расходы пользователя
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ExpenseWithCategoryDto>>> GetUserExpenses(int userId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.Category) // Подгружаем связанную категорию
                .ToListAsync();

            var expenseDtos = expenses.Select(e => new ExpenseWithCategoryDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Amount = e.Amount,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Frequency = e.Frequency,
                RepeatCount = e.RepeatCount,
                Description = e.Description,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.CategoryName ?? "Без категории" // Безопасная проверка null в памяти
            }).ToList();

            return Ok(expenseDtos);
        }

        // Добавить постоянный расход
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseDto expenseDto)
        {
            if (expenseDto == null || expenseDto.Amount <= 0)
            {
                return BadRequest("Некорректные данные о расходе.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var expense = new Expense
                {
                    UserId = expenseDto.UserId,
                    Amount = expenseDto.Amount,
                    StartDate = expenseDto.StartDate,
                    EndDate = expenseDto.EndDate,
                    Frequency = expenseDto.Frequency,
                    RepeatCount = expenseDto.RepeatCount,
                    Description = expenseDto.Description,
                    CategoryId = expenseDto.CategoryId
                };

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                // Обновляем баланс пользователя
                var user = await _context.Users.FindAsync(expense.UserId);
                if (user != null)
                {
                    user.CurrentBalance -= expense.Amount;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { message = "Постоянный расход добавлен и баланс обновлен." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Ошибка при добавлении расхода: {ex.Message}");
            }
        }

    }
}
