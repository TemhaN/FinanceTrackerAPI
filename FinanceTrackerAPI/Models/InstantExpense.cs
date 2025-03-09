using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("instant_expenses")]
    public class InstantExpense
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("shop_name")]
        public string ShopName { get; set; }

        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
