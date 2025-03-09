using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("user_parameters")]
    public class UserParameter
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("currency_id")]
        public int CurrencyId { get; set; }

        [Column("max_monthly_spending")]
        public decimal? MaxMonthlySpending { get; set; }

        [Column("max_entertainment_spending")]
        public decimal? MaxEntertainmentSpending { get; set; }

        [Column("max_savings_goal")]
        public decimal? MaxSavingsGoal { get; set; }

        public User User { get; set; }
        public Currency Currency { get; set; }
    }
}
