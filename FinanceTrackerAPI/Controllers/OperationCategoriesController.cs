using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/operation-categories")]
    [ApiController]
    public class OperationCategoriesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public OperationCategoriesController(FinanceContext context)
        {
            _context = context;
        }

        // Получить все категории операций
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationCategory>>> GetCategories()
        {
            var categories = await _context.OperationCategories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationCategory>> GetCategoryById(int id)
        {
            // Ищем категорию операции по заданному ID
            var category = await _context.OperationCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            // Если категория не найдена, возвращаем ошибку 404
            if (category == null)
            {
                return NotFound("Категория операции с таким ID не найдена.");
            }

            return Ok(category);
        }

    }
}
