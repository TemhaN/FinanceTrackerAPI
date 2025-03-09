using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.DTOs;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/financial-operations")]
    [ApiController]
    public class FinancialOperationsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public FinancialOperationsController(FinanceContext context)
        {
            _context = context;
        }

        // Получить все операции пользователя
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<FinancialOperationWithCategoryDto>>> GetUserOperations(int userId)
        {
            var operations = await _context.FinancialOperations
                .Where(o => o.UserId == userId)
                .GroupJoin(
                    _context.OperationCategories,
                    operation => operation.CategoryId,
                    category => category.Id,
                    (operation, categories) => new { Operation = operation, Category = categories.FirstOrDefault() }
                )
                .Select(x => new FinancialOperationWithCategoryDto
                {
                    Id = x.Operation.Id,
                    UserId = x.Operation.UserId,
                    OperationName = x.Operation.OperationName,
                    Description = x.Operation.Description,
                    OperationType = x.Operation.OperationType,
                    CategoryId = x.Operation.CategoryId,
                    OperationDate = x.Operation.OperationDate,
                    Amount = x.Operation.Amount,
                    CategoryName = x.Category != null ? x.Category.CategoryName : "Без категории"
                })
                .ToListAsync();

            return Ok(operations);
        }

        // Добавить финансовую операцию
        [HttpPost]
        public async Task<IActionResult> AddFinancialOperation([FromBody] FinancialOperationCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var operation = new FinancialOperation
                {
                    UserId = dto.UserId,
                    OperationName = dto.OperationName,
                    Description = dto.Description,
                    OperationType = dto.OperationType,
                    CategoryId = dto.CategoryId,
                    OperationDate = dto.OperationDate,
                    Amount = dto.Amount
                };

                _context.FinancialOperations.Add(operation);
                await _context.SaveChangesAsync();

                // Обновляем баланс пользователя
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user != null)
                {
                    if (dto.OperationType == "income")
                        user.CurrentBalance += dto.Amount;
                    else if (dto.OperationType == "expense")
                        user.CurrentBalance -= dto.Amount;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    id = operation.Id,
                    userId = operation.UserId,
                    operationName = operation.OperationName,
                    description = operation.Description,
                    operationType = operation.OperationType,
                    categoryId = operation.CategoryId,
                    category = operation.CategoryId,
                    operationDate = operation.OperationDate,
                    amount = operation.Amount
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ошибка при добавлении финансовой операции.");
            }
        }


    }
}
