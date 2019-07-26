using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Fixture.Mvc.EntityFramework.Context;
using Xunit.Fixture.Mvc.Extensions;

namespace Xunit.Fixture.Mvc.EntityFramework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/> for configuring an Entity Framework Core database.
    /// </summary>
    public static class ArrangeExtensions
    {
        private const string DbContextType = nameof(DbContextType);

        /// <summary>
        /// Configures an Entity Framework Core database with the specified context type.
        /// This will create a bootstrap job that will migrate the database.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingDatabase<TDbContext>(this IMvcFunctionalTestFixture fixture)
            where TDbContext : DbContext =>
            fixture
                .WhenHavingStringProperty(DbContextType, 
                    x => true,
                    f => throw new ArgumentException("Only supports a single DbContext"))
                .HavingProperty(DbContextType, typeof(TDbContext).Name)
                .HavingServices(services => services.AddTransient<IEntityFrameworkFixtureContext, EntityFrameworkFixtureContext<TDbContext>>())
                .HavingBootstrap<IEntityFrameworkFixtureContext>(b => b.BootstrapAsync());

        /// <summary>
        /// Configures the fixture to delete the Entity Framework Core database before migrating it.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingCleanDatabase(this IMvcFunctionalTestFixture fixture) =>
            fixture.HavingServices(s => s.Configure<EntityFrameworkFixtureOptions>(o => o.CleanDatabase = true));
    }
}
