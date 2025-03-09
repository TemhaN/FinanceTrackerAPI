using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.Models;

public class FinanceContext : DbContext
{
    public FinanceContext(DbContextOptions<FinanceContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<InstantExpense> InstantExpenses { get; set; }
    public DbSet<OperationCategory> OperationCategories { get; set; }
    public DbSet<FinancialOperation> FinancialOperations { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<UserParameter> UserParameters { get; set; } // Переименовано с UserCurrencies
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseCategory> CourseCategories { get; set; }
    public DbSet<PaidSubscriber> PaidSubscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Expense>().HasKey(e => e.Id);
        modelBuilder.Entity<Income>().HasKey(i => i.Id);
        modelBuilder.Entity<FinancialOperation>().HasKey(f => f.Id);
        modelBuilder.Entity<Currency>().HasKey(c => c.Id);
        modelBuilder.Entity<UserParameter>().HasKey(up => up.Id); // Ключи для UserParameter

        // Определяем связи
        modelBuilder.Entity<UserParameter>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserParameters) // Предполагается, что у User есть коллекция UserParameters
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserParameter>()
            .HasOne(up => up.Currency)
            .WithMany(c => c.UserParameters) // Предполагается, что у Currency есть коллекция UserParameters
            .HasForeignKey(up => up.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Category)
            .WithMany(cc => cc.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PaidSubscriber>()
            .HasOne(ps => ps.User)
            .WithMany()
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}