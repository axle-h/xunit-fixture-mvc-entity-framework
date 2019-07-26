using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit.Fixture.Mvc.EntityFramework.Extensions;

namespace Xunit.Fixture.Mvc.EntityFramework.Context
{
    internal static class EntityFrameworkFixtureContext
    {
        /// <summary>
        /// This is here simply to ensure we have a single copy of
        /// migration tasks by connection string across all DB context types.
        /// </summary>
        public static readonly IDictionary<string, Task> MigrationTasks = new Dictionary<string, Task>();
    }

    internal class EntityFrameworkFixtureContext<TDbContext> : IEntityFrameworkFixtureContext
        where TDbContext : DbContext
    {
        private readonly ILogger<EntityFrameworkFixtureContext<TDbContext>> _logger;
        private readonly TDbContext _context;
        private readonly EntityFrameworkFixtureOptions _options;
        private readonly string _connectionString;

        public EntityFrameworkFixtureContext(TDbContext context,
            IOptions<EntityFrameworkFixtureOptions> options,
            DbContextOptions<TDbContext> dbOptions,
            ILogger<EntityFrameworkFixtureContext<TDbContext>> logger)
        {
            _context = context;
            _logger = logger;
            _options = options.Value;
            _connectionString = dbOptions.GetRelationalOptions().ConnectionString;
        }

        public DbContext Context => _context;

        public async Task BootstrapAsync()
        {
            if (_options.IsolatedDatabase)
            {
                await MigrateDatabasePerTestAsync();
            }
            else
            {
                await MigrateSingleDatabaseAsync();
            }
        }

        private Task MigrateSingleDatabaseAsync()
        {
            if (EntityFrameworkFixtureContext.MigrationTasks.TryGetValue(_connectionString, out var task))
            {
                return task;
            }

            lock (EntityFrameworkFixtureContext.MigrationTasks)
            {
                // Check again, just in case another thread took the lock before us.
                // We really don't want this to be run twice.
                if (EntityFrameworkFixtureContext.MigrationTasks.TryGetValue(_connectionString, out task))
                {
                    return task;
                }

                task = MigrateAsync();
                EntityFrameworkFixtureContext.MigrationTasks.Add(_connectionString, task);
            }

            return task;

            async Task MigrateAsync()
            {
                _logger.LogInformation("Migrating database for connection string: " + _connectionString);

                if (_options.CleanDatabase)
                {
                    _logger.LogInformation("Deleting database");
                    await _context.Database.EnsureDeletedAsync();
                }

                await _context.Database.MigrateAsync();
            }
        }

        private async Task MigrateDatabasePerTestAsync()
        {
            _logger.LogInformation("Migrating database for connection string: " + _connectionString);
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();
        }
    }
}
