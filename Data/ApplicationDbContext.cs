using Microsoft.EntityFrameworkCore;
using sigma_backend.Entities;

namespace sigma_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        // Represent tables in Db
        public DbSet<User> Users { get; set; }
    }
}
