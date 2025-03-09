using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("expenses")]
    public class Expense
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("frequency")]
        public string Frequency { get; set; }

        [Column("repeat_count")]
        public int RepeatCount { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        // Навигационное свойство
        [ForeignKey("CategoryId")]
        public OperationCategory Category { get; set; }
    }
}
