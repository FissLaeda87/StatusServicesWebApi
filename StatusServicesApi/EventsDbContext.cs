using Microsoft.EntityFrameworkCore;

namespace StatusServicesApi
{
    public class EventsDbContext : DbContext
    {
        public EventsDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Event> Events { get; set; } // Указываем имя свойства Events

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=events.db");
        }
    }
}
