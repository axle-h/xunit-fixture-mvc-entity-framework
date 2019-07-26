using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Fixture.Mvc.EntityFramework.Context;
using Xunit.Fixture.Mvc.Extensions;

namespace Xunit.Fixture.Mvc.EntityFramework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/> for adding data to an Entity Framework Core database.
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Runs the specified function against the configured DB Context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="contextFunction">The context function.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntities<TDbContext>(
            this IMvcFunctionalTestFixture fixture,
            Func<TDbContext, Task> contextFunction)
            where TDbContext : DbContext =>
            fixture.HavingBootstrap(p => contextFunction(p.GetRequiredService<TDbContext>()));

        /// <summary>
        /// Runs the specified function against the configured DB Context.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="contextFunction">The context function.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntities(
            this IMvcFunctionalTestFixture fixture,
            Func<DbContext, Task> contextFunction) =>
            fixture.HavingBootstrap(p => contextFunction(p.GetRequiredService<IEntityFrameworkFixtureContext>().Context));

        /// <summary>
        /// Adds the specified entity to the configured DB context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntity<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            TEntity entity)
            where TEntity : class =>
            fixture.HavingEntities(async c =>
            {
                await c.Set<TEntity>().AddAsync(entity);
                await c.SaveChangesAsync();
            });

        /// <summary>
        /// Adds the specified entity to the configured DB context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntity<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            out TEntity entity,
            Action<Faker, TEntity> configurator = null)
            where TEntity : class =>
            fixture.HavingModel(out entity, configurator)
                .HavingEntity(entity);

        /// <summary>
        /// Adds the specified entities to the configured DB context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntities<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            ICollection<TEntity> entities)
            where TEntity : class =>
            fixture.HavingEntities(async c =>
            {
                await c.Set<TEntity>().AddRangeAsync(entities);
                await c.SaveChangesAsync();
            });

        /// <summary>
        /// Adds the specified entities to the configured DB context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entities">The entities.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingEntities<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            out ICollection<TEntity> entities,
            Action<Faker, TEntity> configurator = null)
            where TEntity : class =>
            fixture.HavingModels(out entities, configurator)
                .HavingEntities(entities);
    }
}