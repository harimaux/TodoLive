using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoLive.Models;

namespace TodoLive.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Todos> TodosDB { get; set; }
        public DbSet<TaskPriority> TaskPriorityDB { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}