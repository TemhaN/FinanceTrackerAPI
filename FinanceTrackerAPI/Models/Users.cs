using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("current_balance")]
        public decimal CurrentBalance { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }
        public ICollection<UserParameter> UserParameters { get; set; } = new List<UserParameter>();

    }


}
