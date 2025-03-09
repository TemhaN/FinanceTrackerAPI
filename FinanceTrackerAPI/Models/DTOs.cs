using FinanceTrackerAPI.Models;

namespace FinanceTrackerAPI.DTOs
{
    public class UpdateUserRequest
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int CurrencyId { get; set; }
        public string ImageUrl { get; set; }
    }


    public class ExpenseDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public int RepeatCount { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public OperationCategory? Category { get; set; }
    }

    public class FinancialOperationCreateDto
    {
        public int UserId { get; set; }
        public string OperationName { get; set; }
        public string Description { get; set; }
        public string OperationType { get; set; }
        public int CategoryId { get; set; }
        public OperationCategory? Category { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal Amount { get; set; }
    }
    public class IncomeCreateDto
    {
        public int UserId { get; set; }
        public int OperationCategoryId { get; set; }
        public OperationCategory? Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public int RepeatCount { get; set; }
        public string Description { get; set; }
    }
    public class InstantExpenseCreateDto
    {
        public int UserId { get; set; }
        public string ShopName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class ExpenseWithCategoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public int RepeatCount { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class IncomeWithCategoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public int RepeatCount { get; set; }
        public string Description { get; set; }
        public int OperationCategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class FinancialOperationWithCategoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OperationName { get; set; }
        public string Description { get; set; }
        public string OperationType { get; set; }
        public int CategoryId { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal Amount { get; set; }
        public string CategoryName { get; set; }
    }

    public class UserParameterDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }
        public decimal? MaxMonthlySpending { get; set; }
        public decimal? MaxEntertainmentSpending { get; set; }
        public decimal? MaxSavingsGoal { get; set; }
    }

    public class UpdateUserParameterRequest
    {
        public int? CurrencyId { get; set; }
        public decimal? MaxMonthlySpending { get; set; }
        public decimal? MaxEntertainmentSpending { get; set; }
        public decimal? MaxSavingsGoal { get; set; }
    }
}
