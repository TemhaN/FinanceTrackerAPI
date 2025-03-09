using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Controllers
    {
        [Route("api/courses")]
        [ApiController]
        public class CoursesController : ControllerBase
        {
            private readonly FinanceContext _context;

            public CoursesController(FinanceContext context)
            {
                _context = context;
            }

            // 1. Подписка на платный доступ (оставляем без изменений)
            [HttpPost("subscribe")]
            public async Task<IActionResult> SubscribeToPaidCourses([FromQuery] int userId)
            {
                if (userId <= 0)
                {
                    return BadRequest("Неверный UserId.");
                }

                var existingSubscription = await _context.PaidSubscribers
                    .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.SubscriptionEndDate > DateTime.UtcNow);

                if (existingSubscription != null)
                {
                    return BadRequest("У вас уже есть активная подписка до " + existingSubscription.SubscriptionEndDate);
                }

                var newSubscription = new PaidSubscriber
                {
                    UserId = userId,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddMonths(1)
                };

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.PaidSubscribers.Add(newSubscription);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Подписка успешно оформлена до " + newSubscription.SubscriptionEndDate });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ошибка при оформлении подписки.");
                }
            }

        // 2. Получение всех курсов по категориям (исправлено)
        [HttpGet("by-category")]
        public async Task<ActionResult<IEnumerable<object>>> GetCoursesByCategories()
        {
            // Загружаем все курсы и категории в память
            var allCourses = await _context.Courses
                .Include(c => c.Category)
                .ToListAsync();

            var allCategories = await _context.CourseCategories
                .ToListAsync();

            // Выполняем фильтрацию на стороне клиента
            var coursesByCategory = allCategories
                .Select(cc => new
                {
                    CategoryId = cc.Id,
                    CategoryName = cc.CategoryName,
                    Courses = allCourses
                        .Where(c => c.CategoryId == cc.Id)
                        .Select(c => new
                        {
                            c.Id,
                            c.CourseName,
                            c.IsPaid,
                            Description = c.IsPaid ? "Требуется подписка" : c.Description,
                            VideoUrl = c.IsPaid ? null : c.VideoUrl
                        })
                        .ToList()
                })
                .ToList();

            // Добавляем курсы без категорий в отдельную категорию (опционально)
            var uncategorizedCourses = allCourses
                .Where(c => c.CategoryId == null)
                .Select(c => new
                {
                    c.Id,
                    c.CourseName,
                    c.IsPaid,
                    Description = c.IsPaid ? "Требуется подписка" : c.Description,
                    VideoUrl = c.IsPaid ? null : c.VideoUrl
                })
                .ToList();

            if (uncategorizedCourses.Any())
            {
                coursesByCategory.Add(new
                {
                    CategoryId = 0, // Условный ID для "Без категории"
                    CategoryName = "Без категории",
                    Courses = uncategorizedCourses
                });
            }

            // Отладочная информация
            if (!coursesByCategory.Any(c => c.Courses.Any()))
            {
                return NotFound("Не найдено ни одной категории или курса.");
            }

            return Ok(coursesByCategory);
        }

        // 3. Получение всех подписок пользователя
        [HttpGet("paid-subscribers")]
        public async Task<ActionResult<IEnumerable<PaidSubscriber>>> GetPaidSubscribers([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Неверный UserId.");
            }

            var subscriptions = await _context.PaidSubscribers
                .Where(ps => ps.UserId == userId)
                .ToListAsync();

            if (subscriptions == null || !subscriptions.Any())
            {
                return NotFound("Подписки не найдены.");
            }

            return Ok(subscriptions);
        }

        // 4. Получение курса по ID с проверкой подписки
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCourseById(int id, [FromQuery] int userId)
        {
            // Находим курс
            var course = await _context.Courses
                .Include(c => c.Category)
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.CourseName,
                    c.Description,
                    c.VideoUrl,
                    c.IsPaid,
                    CategoryName = c.Category.CategoryName
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound("Курс с таким ID не найден.");
            }

            // Если курс платный, проверяем подписку
            if (course.IsPaid)
            {
                var subscription = await _context.PaidSubscribers
                    .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.SubscriptionEndDate > DateTime.UtcNow);

                if (subscription == null)
                {
                    // Если подписки нет, возвращаем ограниченные данные
                    return Ok(new
                    {
                        course.Id,
                        course.CourseName,
                        Description = "Требуется подписка",
                        VideoUrl = (string)null,
                        course.IsPaid,
                        CategoryName = course.CategoryName
                    });
                }
            }

            // Если курс бесплатный или есть подписка, возвращаем полные данные
            return Ok(course);
        }
    }
}