using Microsoft.EntityFrameworkCore;
using RolePlayingGame.Models;

namespace RolePlayingGame.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>()
                .HasOne(c => c.User)
                .WithMany(u => u.Characters)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
