using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Entities;

namespace sigma_backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        // Represent tables in Db
        public DbSet<User> Users { get; set; }

        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     base.OnModelCreating(builder);

        //     builder.Entity<User>()
        //         .Property(u => u.Username).HasMaxLength(32); // Limit username length

        //     builder.HasDefaultSchema("sigmadb");
        // }
    }
}
