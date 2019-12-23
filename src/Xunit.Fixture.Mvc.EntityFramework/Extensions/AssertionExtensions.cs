using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit.Fixture.Mvc.EntityFramework.Context;

namespace Xunit.Fixture.Mvc.EntityFramework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/> for building assertions against an Entity Framework Core database.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Adds an assertion against the configured DB context.
        /// This will run an Entity Framework Core query and assert against the results.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldSatisfyDatabaseQuery<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Action<IDatabaseAssertionContext<TEntity>> action)
            where TEntity : class =>
            fixture.ShouldHaveServiceWhich(p =>
            {
                var assertion = new DatabaseAssertionContext<TEntity>();
                action(assertion);

                var context = p.GetRequiredService<IEntityFrameworkFixtureContext>().Context;

                return assertion.RunAsync(context);
            });

        /// <summary>
        /// Adds an assertion that the response body is JSON and can be deserialized to the specified response type
        /// followed by running an Entity Framework Core query using the response as input.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldSatisfyDatabaseQueryFromJsonResponse<TResponse, TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Action<TResponse, IDatabaseAssertionContext<TEntity>> action)
            where TEntity : class =>
            fixture.ShouldReturnBody(JsonConvert.DeserializeObject<TResponse>, (p, r) =>
            {
                var assertion = new DatabaseAssertionContext<TEntity>();
                action(r, assertion);

                var context = p.GetRequiredService<IEntityFrameworkFixtureContext>().Context;

                return assertion.RunAsync(context);
            });

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type that matches the specified predicate exists in the database,
        /// also running the specified assertions against it.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldExistInDatabase<TEntity>(this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            params Action<TEntity>[] assertions)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnSingle(assertions));
        
        /// <summary>
        /// Adds an assertion to check that entities of the specified type that match the specified predicate exist in the database,
        /// also running the specified assertions against them.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldExistManyInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            params Action<ICollection<TEntity>>[] assertions)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnCollection(assertions));

        /// <summary>
        /// Adds an assertion to check that entities of the specified type exist in the database,
        /// also running the specified assertions against them.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldExistManyInDatabase<TEntity>(this IMvcFunctionalTestFixture fixture,
                                                                                   params Action<ICollection<TEntity>>[] assertions)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.ShouldReturnCollection(assertions));

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type exists with the specified identifier in the database,
        /// also running the specified assertions against it.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldExistInDatabase<TEntity>(this IMvcFunctionalTestFixture fixture,
            object id,
            params Action<TEntity>[] assertions)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Find(id).ShouldReturnSingle(assertions));

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type that matches the specified predicate does not exist in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldNotExistInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnEmptyCollection());

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type with the specified identifier does not exist in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldNotExistInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture, object id)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Find(id).ShouldReturnEmptyCollection());

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type that matches the specified predicate exists in the database
        /// and that it is equivalent to the specified object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity, TExpectation>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            TExpectation expected,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnEquivalent(expected, options));

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type that matches the specified predicate exists in the database
        /// and that it is equivalent to the specified object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            object expected)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnEquivalent(expected));

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type with the specified identifier exists in the database
        /// and that it is equivalent to the specified object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity, TExpectation>(
            this IMvcFunctionalTestFixture fixture,
            object id,
            TExpectation expected,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Find(id).ShouldReturnEquivalent(expected, options));

        /// <summary>
        /// Adds an assertion to check that an entity of the specified type with the specified identifier exists in the database
        /// and that it is equivalent to the specified object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            object id,
            object expected)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Find(id).ShouldReturnEquivalent(expected));

        /// <summary>
        /// Adds an assertion to check that a single entity of the specified type that is equivalent to the specified object exists in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity, TExpectation>(
            this IMvcFunctionalTestFixture fixture,
            TExpectation expected,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.ShouldReturnCollectionContainingEquivalent(expected, options));

        /// <summary>
        /// Adds an assertion to check that a single entity of the specified type that is equivalent to the specified object exists in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            object expected)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.ShouldReturnCollectionContainingEquivalent(expected));

        /// <summary>
        /// Adds an assertion to check that entities of the specified type that satisfy the specified predicate exist in the database and are equivalent to the specified collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity, TExpectation>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            ICollection<TExpectation> expected,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnCollectionContainingEquivalent(expected, options));

        /// <summary>
        /// Adds an assertion to check that entities of the specified type that satisfy the specified predicate exist in the database and are equivalent to the specified collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            Expression<Func<TEntity, bool>> predicate,
            ICollection<object> expected)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.Where(predicate).ShouldReturnCollectionContainingEquivalent(expected));

        /// <summary>
        /// Adds an assertion to check that entities of the specified type exist in the database and are equivalent to the specified collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity, TExpectation>(
            this IMvcFunctionalTestFixture fixture,
            ICollection<TExpectation> expected,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.ShouldReturnCollectionContainingEquivalent(expected, options));

        /// <summary>
        /// Adds an assertion to check that entities of the specified type exist in the database and are equivalent to the specified collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldBeEquivalentInDatabase<TEntity>(
            this IMvcFunctionalTestFixture fixture,
            ICollection<object> expected)
            where TEntity : class =>
            fixture.ShouldSatisfyDatabaseQuery<TEntity>(f => f.ShouldReturnCollectionContainingEquivalent(expected));

        private class DatabaseAssertionContext<TEntity> : IDatabaseAssertionContext<TEntity>
            where TEntity : class
        {
            private Func<DbSet<TEntity>, IQueryable<TEntity>> _dbSetConfigurator;

            private readonly ICollection<Func<IQueryable<TEntity>, IQueryable<TEntity>>> _predicates =
                new List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();

            private readonly List<Action<ICollection<TEntity>>> _assertions = new List<Action<ICollection<TEntity>>>();

            private object _id;

            public IDatabaseAssertionContext<TEntity> Having(Func<DbSet<TEntity>, IQueryable<TEntity>> configurator)
            {
                if (_dbSetConfigurator != null)
                {
                    throw new InvalidOperationException("This DB set is already configured");
                }

                _dbSetConfigurator = configurator;
                return this;
            }

            public IDatabaseAssertionContext<TEntity> Matching(Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate)
            {
                _predicates.Add(predicate);
                return this;
            }

            public IDatabaseAssertionContext<TEntity> Where(Expression<Func<TEntity, bool>> predicate) =>
                Matching(set => set.Where(predicate));

            public IDatabaseAssertionContext<TEntity> Find(object id)
            {
                _id = id;
                return this;
            }

            public IDatabaseAssertionContext<TEntity> ShouldReturnEmptyCollection() =>
                ShouldReturnCollection(x => x.Should().BeEmpty());

            public IDatabaseAssertionContext<TEntity> ShouldReturnSingle(params Action<TEntity>[] assertions)
            {
                if (!assertions.Any())
                {
                    return ShouldReturnCollection(x => x.Should().ContainSingle());
                }

                var collectionAssertions = assertions
                    .Select<Action<TEntity>, Action<ICollection<TEntity>>>(a => x => a(x.Should().ContainSingle().Which))
                    .ToArray();
                return ShouldReturnCollection(collectionAssertions);
            }

            public IDatabaseAssertionContext<TEntity> ShouldReturnEquivalent<TExpectation>(TExpectation expectation,
                Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null) =>
                ShouldReturnSingle(e =>
                {
                    if (options == null)
                    {
                        e.Should().BeEquivalentTo(expectation);
                    }
                    else
                    {
                        e.Should().BeEquivalentTo(expectation, options);
                    }
                });

            public IDatabaseAssertionContext<TEntity> ShouldReturnCollection(params Action<ICollection<TEntity>>[] assertions)
            {
                if (!assertions.Any())
                {
                    _assertions.Add(x => x.Should().NotBeEmpty());
                    return this;
                }

                _assertions.AddRange(assertions);
                return this;
            }

            public IDatabaseAssertionContext<TEntity> ShouldReturnEquivalentCollection<TExpectation>(ICollection<TExpectation> expectation,
                Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null) =>
                ShouldReturnCollection(set =>
                {
                    if (options == null)
                    {
                        set.Should().BeEquivalentTo(expectation);
                    }
                    else
                    {
                        set.Should().BeEquivalentTo(expectation, options);
                    }
                });

            public IDatabaseAssertionContext<TEntity> ShouldReturnCollectionContainingEquivalent<TExpectation>(TExpectation expectation,
                Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null) =>
                ShouldReturnCollection(set =>
                {
                    if (options == null)
                    {
                        set.Should().ContainEquivalentOf(expectation);
                    }
                    else
                    {
                        set.Should().ContainEquivalentOf(expectation, options);
                    }
                });

            public IDatabaseAssertionContext<TEntity> ShouldReturnCollectionContainingEquivalent<TExpectation>(ICollection<TExpectation> expectation,
                Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null) =>
                ShouldReturnCollection(expectation.Select<TExpectation, Action<ICollection<TEntity>>>(e =>
                    set =>
                    {
                        if (options == null)
                        {
                            set.Should().ContainEquivalentOf(e);
                        }
                        else
                        {
                            set.Should().ContainEquivalentOf(e, options);
                        }
                    }
                ).ToArray());

            public async Task RunAsync(DbContext context)
            {
                if (!_assertions.Any())
                {
                    throw new InvalidOperationException("No assertions added to this query");
                }

                var set = context.Set<TEntity>();

                ICollection<TEntity> entities;

                if (_id != null)
                {
                    if (_dbSetConfigurator != null)
                    {
                        throw new InvalidOperationException("Cannot configure a find query");
                    }

                    if (_predicates.Any())
                    {
                        throw new InvalidOperationException("Cannot filter a find query");
                    }

                    var entity = await set.FindAsync(_id);
                    entities = entity == null ? Array.Empty<TEntity>() : new[] {entity};
                }
                else
                {
                    var configuredSet = _dbSetConfigurator == null ? set : _dbSetConfigurator(set);
                    entities = await _predicates
                        .Aggregate(configuredSet, (current, predicate) => predicate(current))
                        .ToListAsync();
                }

                foreach (var assertion in _assertions)
                {
                    assertion(entities);
                }
            }
        }
    }
}