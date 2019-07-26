namespace Xunit.Fixture.Mvc.EntityFramework.ConnectionStrings
{
    /// <summary>
    /// Structured representation of a database connection string.
    /// </summary>
    public interface IConnectionString
    {
        /// <summary>
        /// The database.
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();
    }
}