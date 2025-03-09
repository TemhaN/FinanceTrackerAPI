using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("user_sessions")]
    public class UserSession
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("session_token")]
        public string SessionToken { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("LastAccessedAt")]
        public DateTime LastAccessedAt { get; set; }

        [Column("ExpiresAt")]
        public DateTime ExpiresAt { get; set; }
    }
}
