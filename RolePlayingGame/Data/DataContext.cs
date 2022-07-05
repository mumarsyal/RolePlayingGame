using Microsoft.EntityFrameworkCore;
using RolePlayingGame.Models;

namespace RolePlayingGame.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Character> Characters { get; set; }
    }
}
