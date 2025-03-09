using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.DTOs;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/incomes")]
    [ApiController]
    public class IncomesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public IncomesController(FinanceContext context)
        {
            _context = context;
        }

        // Получить все доходы пользователя
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<IncomeWithCategoryDto>>> GetUserIncomes(int userId)
        {
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId)
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
                    Description = x.Income.Description,
                    OperationCategoryId = x.Income.OperationCategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : "Без категории"
                })
                .ToListAsync();

            return Ok(incomes);
        }


        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] IncomeCreateDto incomeDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Создаём объект Income на основе DTO
                var income = new Income
                {
                    UserId = incomeDto.UserId,
                    OperationCategoryId = incomeDto.OperationCategoryId,
                    Amount = incomeDto.Amount,
                    StartDate = incomeDto.StartDate,
                    EndDate = incomeDto.EndDate,
                    Frequency = incomeDto.Frequency,
                    RepeatCount = incomeDto.RepeatCount,
                    Description = incomeDto.Description
                };

                _context.Incomes.Add(income);
                await _context.SaveChangesAsync();

                // Обновляем баланс пользователя
                var user = await _context.Users.FindAsync(income.UserId);
                if (user != null)
                {
                    user.CurrentBalance += income.Amount;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    id = income.Id, // Отправляем ID обратно, если нужно
                    userId = income.UserId,
                    operationCategoryId = income.OperationCategoryId,
                    amount = income.Amount,
                    startDate = income.StartDate,
                    endDate = income.EndDate,
                    frequency = income.Frequency,
                    repeatCount = income.RepeatCount,
                    Description = income.Description
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ошибка при добавлении дохода.");
            }
        }






    }
}
