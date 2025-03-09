using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTrackerAPI.Models
{
    [Table("courses")]
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("course_name")]
        public string CourseName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [StringLength(255)]
        [Column("video_url")]
        public string? VideoUrl { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("is_paid")]
        public bool IsPaid { get; set; }

        public CourseCategory Category { get; set; }
    }

    [Table("course_categories")]
    public class CourseCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("category_name")]
        public string CategoryName { get; set; }

        public List<Course> Courses { get; set; }
    }

    [Table("paid_subscribers")]
    public class PaidSubscriber
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Column("user_id")] 
        public int UserId { get; set; }

        [Required]
        [Column("subscription_start_date")]
        public DateTime SubscriptionStartDate { get; set; }

        [Required]
        [Column("subscription_end_date")]
        public DateTime SubscriptionEndDate { get; set; }

        public User User { get; set; }
    }
}
