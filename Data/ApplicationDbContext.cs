using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Models;

namespace sigma_backend.Data
{
    // Bridge between application and the PostgreSQL database. It holds DbSet properties that represent tables in the database.
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        // Represent tables in Db
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealFile> MealFiles { get; set; }
        public DbSet<FoodNutrition> FoodNutrition { get; set; }
        public DbSet<CustomFood> CustomFoods { get; set; }
        public DbSet<CustomDish> CustomDishes { get; set; }
        public DbSet<CustomDishIngredient> CustomDishIngredients { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create roles
            List<IdentityRole> roles = new List<IdentityRole>{
                new IdentityRole{
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole{
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
