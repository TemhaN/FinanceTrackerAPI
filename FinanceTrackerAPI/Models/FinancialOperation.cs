using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("financial_operations")]
    public class FinancialOperation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("operation_name")]
        public string OperationName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("operation_type")]
        public string OperationType { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("operation_date")]
        public DateTime OperationDate { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }
        public OperationCategory Category { get; set; }
    }
}
