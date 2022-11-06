using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess
{
    public class DictionaryContext : DbContext
    {
        public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new Exception("Please set DB_CONNECTION_STRING EnvVar!"));

    }
}
