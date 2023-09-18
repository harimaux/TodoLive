using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoLive.Models
{
    public class TaskPriority
    {
        [Key]
        public int Id { get; set; }
        public string? PriorityName { get; set; }
        public byte[]? PriorityImage { get; set; }
        public string? PriorityLevel { get; set; }
        public string? PriorityVisual { get; set; }
        public string? HTMLcode { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public bool? IsSet { get; set; }
    }
}
