using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoLive.Models
{
    public class Todos
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string? Title { get; set; }
        [Required]
        [MaxLength(2000)]
        public string? Content { get; set; }
        public string? ContentExtra { get; set; }
        [Required]
        public string? Priority { get; set; }
        [Required]
        [MaxLength(60)]
        public string? FromRequested { get; set; }
        public string? State { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime DateEdited { get; set; }
        public DateTime DateCompleted { get; set; }
        [Required]
        public DateTime DateDue { get; set; }

        [ForeignKey("Owner")]
        public string? OwnerId { get; set; }
        public virtual IdentityUser? Owner { get; set; }

    }
}
