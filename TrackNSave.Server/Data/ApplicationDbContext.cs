using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
