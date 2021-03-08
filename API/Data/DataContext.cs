using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        
        // We need to do something with our Likes. With our photos for example we dont necessarily do something that's why no DbSet
        // We give this a table name of Likes. This will be a "join" table where we can run queries against
        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        /* Give the entities some configuration. We need to override a method inside the DbContext */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // If we don't do this we can sometimes get an error with the migration
            base.OnModelCreating(builder);

            // Work on our UserLike Entity here. We didn't configure a primary key, so we configure one ourselves.
            // The k is gonna represent the primary key for this particular table
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.LikedUserId});
            
            // Configure the relationships
            // One to Many: A SourceUser can like many other users
            // If we delete a user, we're gonna delete the related entities
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // The other side of the relationship
            // A LikedUser can have many LikedByUsers
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}