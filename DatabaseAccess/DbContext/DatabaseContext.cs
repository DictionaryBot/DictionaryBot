using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.DbContext
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();
        public DbSet<Guild> Guilds => Set<Guild>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new Exception("Please set DB_CONNECTION_STRING EnvVar!"), new MariaDbServerVersion(new Version(12, 1, 2)));
    }
}
