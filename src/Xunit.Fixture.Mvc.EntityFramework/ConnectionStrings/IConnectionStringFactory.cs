namespace Xunit.Fixture.Mvc.EntityFramework.ConnectionStrings
{
    /// <summary>
    /// Factory for creating <see cref="IConnectionString"/> instances.
    /// </summary>
    public interface IConnectionStringFactory
    {
        /// <summary>
        /// Builds a new connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        IConnectionString Build(string connectionString);
    }
}