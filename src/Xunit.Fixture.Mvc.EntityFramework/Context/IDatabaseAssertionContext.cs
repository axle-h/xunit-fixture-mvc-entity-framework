using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;

namespace Xunit.Fixture.Mvc.EntityFramework.Context
{
    /// <summary>
    /// A context for running an Entity Framework Core query and asserting against the results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IDatabaseAssertionContext<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Configures the DB set.
        /// This can only be called once.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> Having(Func<DbSet<TEntity>, IQueryable<TEntity>> configurator);

        /// <summary>
        /// Adds the specified filter predicate to the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> Matching(Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate);

        /// <summary>
        /// Adds the specified where predicate to the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Configures the query to return the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> Find(object id);

        /// <summary>
        /// Adds an assertion that an empty collection of entities should be returned.
        /// </summary>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnEmptyCollection();

        /// <summary>
        /// Adds an assertion that a single entity should be returned that is satisfied
        /// by the specified assertions.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnSingle(params Action<TEntity>[] assertions);

        /// <summary>
        /// Adds an assertion that a single entity should be returned that is equivalent
        /// to the specified model.
        /// </summary>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnEquivalent<TExpectation>(TExpectation expectation,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null);

        /// <summary>
        /// Adds an assertion that a collection of entities should be returned that
        /// are satisfied by the specified assertions.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnCollection(params Action<ICollection<TEntity>>[] assertions);

        /// <summary>
        /// Adds an assertion that a collection of entities should be returned that
        /// are equivalent to the specified models.
        /// </summary>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnEquivalentCollection<TExpectation>(ICollection<TExpectation> expectation,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null);

        /// <summary>
        /// Adds an assertion that a collection of entities should be returned that
        /// contains a least one entity that is equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnCollectionContainingEquivalent<TExpectation>(TExpectation expectation,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null);

        /// <summary>
        /// Adds an assertion that a collection of entities should be returned that
        /// contains a least one entity that is equivalent to each of the specified models.
        /// </summary>
        /// <typeparam name="TExpectation">The type of the expectation.</typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        IDatabaseAssertionContext<TEntity> ShouldReturnCollectionContainingEquivalent<TExpectation>(ICollection<TExpectation> expectation,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> options = null);
    }
}