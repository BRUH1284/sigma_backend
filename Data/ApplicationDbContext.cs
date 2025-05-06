using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Models;

namespace sigma_backend.Data
{
    // Bridge between application and the PostgreSQL database. It holds DbSet properties that represent tables in the database.
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // Represent tables in Db
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }
        public DbSet<UserFollower> UserFollowers { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealFile> MealFiles { get; set; }
        public DbSet<FoodNutrition> FoodNutrition { get; set; }
        public DbSet<CustomFood> CustomFoods { get; set; }
        public DbSet<CustomDish> CustomDishes { get; set; }
        public DbSet<CustomDishIngredient> CustomDishIngredients { get; set; }
        public DbSet<DataVersion> DataVersions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Uuid support
            builder.HasPostgresExtension("uuid-ossp");

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

            // Configure friend requests
            builder.Entity<FriendRequest>()
                .HasKey(r => new { r.SenderId, r.ReceiverId });

            builder.Entity<FriendRequest>()
                .HasOne(r => r.Sender)
                .WithMany(s => s.FriendRequests)
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> friend requests as sender

            builder.Entity<FriendRequest>()
                .HasOne(r => r.Receiver)
                .WithMany(r => r.ReceivedFriendRequests)
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> friend requests as receiver

            // Configure friendships
            builder.Entity<Friendship>()
                .HasKey(f => new { f.UserId, f.FriendId });

            builder.Entity<Friendship>()
                .HasOne(r => r.User)
                .WithMany(s => s.Friendships) // User has many friends
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> friendship as user

            builder.Entity<Friendship>()
                .HasOne(r => r.Friend)
                .WithMany()
                .HasForeignKey(r => r.FriendId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> friendship as friend

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
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> Refresh Token

            // Configure Activities
            builder.Entity<Activity>()
                .HasKey(a => a.Code);

            builder.Entity<Activity>()
                .Property(a => a.Code)
                .ValueGeneratedNever(); // Disable auto-increment            

            // Configure User Activities

            // Use uuid so it is safe to expose them to user
            builder.Entity<UserActivity>()
                .Property(ua => ua.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Activities) // User has many User Activities
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> User Activities

            // Configure Data Versions
            builder.Entity<DataVersion>()
                .HasKey(dv => dv.DataResource);

            // Configure Messages
            builder.Entity<Message>().HasKey(m => m.Id);
            builder.Entity<Message>()
                .Property(m => m.Id)
                .HasMaxLength(36);

            builder.Entity<Message>()
                .HasIndex(m => m.SenderUsername)
                .HasDatabaseName("IX_Messages_SenderUsername");

            builder.Entity<Message>()
                .HasIndex(m => m.ReceiverUsername)
                .HasDatabaseName("IX_Messages_ReceiverUsername");
        }
    }
}
