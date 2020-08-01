using LicenseProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicenseProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Project>Projects { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<MyUser> MyUser { get; set; }
        public DbSet<Task> Tasks { get; set; }
       // public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<GrantApplication> GrantApplications { get; set; }

       /* protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTask>()
                .HasKey(t => new { t.MyUserId, t.TaskID });

            modelBuilder.Entity<UserTask>()
                .HasOne(pt => pt.MyUser)
                .WithMany(p => p.UserTasks)
                .HasForeignKey(pt => pt.MyUserId);

            modelBuilder.Entity<UserTask>()
                .HasOne(pt => pt.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(pt => pt.TaskID);
        }*/
    }
}

