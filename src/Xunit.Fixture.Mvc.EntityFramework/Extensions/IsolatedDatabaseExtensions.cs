using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Fixture.Mvc.EntityFramework.ConnectionStrings;
using Xunit.Fixture.Mvc.EntityFramework.Context;
using Xunit.Fixture.Mvc.Infrastructure;

namespace Xunit.Fixture.Mvc.EntityFramework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/> for configuring an Entity Framework Core database.
    /// </summary>
    public static class IsolatedDatabaseExtensions
    {
        /// <summary>
        /// Configures Entity Framework Core to use an isolated database for this test.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="connectionStringFactory">The connection string factory.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingIsolatedDatabase<TDbContext>(
            this IMvcFunctionalTestFixture fixture,
            IConnectionStringFactory connectionStringFactory)
            where TDbContext : DbContext =>
            fixture
                .HavingServices(services => services
                    .Configure<EntityFrameworkFixtureOptions>(o => o.IsolatedDatabase = true)
                    .AddSingleton(connectionStringFactory)
                    .AddSingleton<IsolatedDbContextFactory<TDbContext>>()
                    .AddTransient(p => p.GetRequiredService<IsolatedDbContextFactory<TDbContext>>().Build())
                );

        /// <summary>
        /// Configures Entity Framework Core to use an isolated database for this test.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingIsolatedDatabase<TDbContext>(this IMvcFunctionalTestFixture fixture)
            where TDbContext : DbContext =>
            fixture.HavingIsolatedDatabase<TDbContext>(new NameValuePairConnectionStringFactory());
        
        internal static RelationalOptionsExtension GetRelationalOptions<TDbContext>(this DbContextOptions<TDbContext> options)
            where TDbContext : DbContext =>
            options.Extensions
                .OfType<RelationalOptionsExtension>()
                .FirstOrDefault() ??
            throw new ArgumentException(
                "Cannot find suitable relational extension");

        private class IsolatedDbContextFactory<TDbContext>
            where TDbContext : DbContext
        {
            private readonly Func<TDbContext> _factory;

            public IsolatedDbContextFactory(ITestFixtureContext fixtureContext,
                DbContextOptions<TDbContext> options,
                IConnectionStringFactory connectionStringFactory,
                ILogger<IsolatedDbContextFactory<TDbContext>> logger)
            {
                var relationalOptions = options.GetRelationalOptions();

                var updateRelationalExtension = typeof(IDbContextOptionsBuilderInfrastructure).GetMethod(
                                                        nameof(IDbContextOptionsBuilderInfrastructure.AddOrUpdateExtension))
                                                    ?.MakeGenericMethod(relationalOptions.GetType())
                                                ?? throw new Exception("Cannot find add or update extension method");

                var constructor =
                    typeof(TDbContext).GetConstructor(new[] {typeof(DbContextOptions<TDbContext>)})
                    ?? typeof(TDbContext).GetConstructor(new[] {typeof(DbContextOptions)})
                    ?? throw new ArgumentException(
                        $"Cannot find a suitable constructor on type {typeof(TDbContext)}");

                // Must be built outside the context or else each resolution will resolve a different DB
                var fallbackDatabase = Guid.NewGuid().ToString();

                _factory = () =>
                {
                    var connectionString = connectionStringFactory.Build(relationalOptions.ConnectionString);

                    var testName = fixtureContext.TestOutput?.GetCurrentTestName();
                    connectionString.Database = string.IsNullOrEmpty(testName)
                        ? fallbackDatabase
                        : new Guid(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(testName))).ToString();

                    logger.LogInformation($"using isolated database: {connectionString.Database}");

                    var builder = new DbContextOptionsBuilder<TDbContext>(options);
                    updateRelationalExtension.Invoke(builder, new object[]
                    {
                        relationalOptions.WithConnectionString(connectionString.ToString())
                    });
                    return constructor.Invoke(new object[] {builder.Options}) as TDbContext;
                };
            }

            public TDbContext Build() => _factory();
        }
    }
}
