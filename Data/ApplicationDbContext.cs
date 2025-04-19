using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Models;

namespace sigma_backend.Data
{
    // Bridge between application and the PostgreSQL database. It holds DbSet properties that represent tables in the database.
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        // Represent tables in Db
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }
        public DbSet<UserFollower> UserFollowers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
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

            // Configure User Profile
            builder.Entity<UserProfile>()
                .HasKey(up => up.UserId); // UserId is the primary key in UserProfile

            builder.Entity<UserProfile>()
                .HasOne<User>()
                .WithOne(u => u.Profile) // User has one UserProfile
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserProfile

            // Configure Profile Picture
            builder.Entity<ProfilePicture>()
                .HasKey(pp => new { pp.UserId, pp.FileName });

            builder.Entity<ProfilePicture>()
                .HasOne<UserProfile>()
                .WithMany(pp => pp.ProfilePictures) // User has many pictures
                .HasForeignKey(pp => pp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for UserProfile -> Profile Pictures;

            // Configure followers
            builder.Entity<UserFollower>()
                .HasKey(f => new { f.FollowerId, f.FolloweeId });

            builder.Entity<UserFollower>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> followers;

            builder.Entity<UserFollower>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> followee;

            // Restrict duplicated follows
            builder.Entity<UserFollower>()
                .HasIndex(uf => new { uf.FollowerId, uf.FolloweeId })
                .IsUnique();

            // Configure Posts
            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts) // User has many Posts
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> Posts

            // Configure Post Images
            builder.Entity<PostImage>()
                .HasKey(pi => new { pi.PostId, pi.FileName });

            builder.Entity<PostImage>()
                .HasOne<Post>()
                .WithMany(p => p.Images) // Post has many images
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Post -> Post Images;

            // Configure Refresh Tokens
            builder.Entity<RefreshToken>()
                .HasKey(rt => new { rt.UserId, rt.DeviceId });

            builder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany() // User has many Refresh Tokens
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> RefreshToken
        }
    }
}
