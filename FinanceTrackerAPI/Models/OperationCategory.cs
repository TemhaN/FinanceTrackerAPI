using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("operation_categories")] // Указываем точное имя таблицы
    public class OperationCategory
    {
        public int Id { get; set; }
        [Column("category_name")]
        public string CategoryName { get; set; }
    }
}
