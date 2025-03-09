using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerAPI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; }

        public string? Username { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? CurrentBalance { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string SessionToken { get; set; }
        public int CurrencyId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string ImageUrl { get; set; }
    }



}
