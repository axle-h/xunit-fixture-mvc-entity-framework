using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Fixture.Mvc.EntityFramework.ConnectionStrings
{
    /// <summary>
    /// A connection string consisting of a series of name-value pairs.
    /// </summary>
    /// <seealso cref="Xunit.Fixture.Mvc.EntityFramework.ConnectionStrings.IConnectionStringFactory" />
    public class NameValuePairConnectionStringFactory : IConnectionStringFactory
    {
        private readonly char _separator;
        private readonly char _equality;
        private readonly string _databaseKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValuePairConnectionStringFactory"/> class.
        /// </summary>
        /// <param name="separator">The character that separates key value pairs.</param>
        /// <param name="equality">The character that separates keys from values.</param>
        /// <param name="databaseKey">The key of the pair that contains the database name.</param>
        public NameValuePairConnectionStringFactory(
            char separator = ';',
            char equality = '=',
            string databaseKey = "Database")
        {
            _separator = separator;
            _equality = equality;
            _databaseKey = databaseKey;
        }

        /// <summary>
        /// Builds a new connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public IConnectionString Build(string connectionString) =>
            new NameValuePairConnectionString(connectionString, _separator, _equality, _databaseKey);

        private class NameValuePairConnectionString : IConnectionString
        {
            private readonly IDictionary<string, string> _keys;
            private readonly char _separator;
            private readonly char _equality;
            private readonly string _databaseKey;

            public NameValuePairConnectionString(string connectionString, char separator, char equality, string databaseKey)
            {
                _separator = separator;
                _equality = equality;
                _databaseKey = databaseKey;

                var tokens = connectionString.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                _keys = tokens.Select(t =>
                    {
                        var kvp = t.Split(new[] { equality }, 2, StringSplitOptions.RemoveEmptyEntries);

                        var key = kvp[0]?.Trim();

                        return kvp.Length == 2 ? (key: key, value: kvp[1]?.Trim()) : (key: key, value: null);
                    })
                    .Where(x => !string.IsNullOrEmpty(x.value))
                    .GroupBy(x => x.key)
                    .Select(x => x.First())
                    .ToDictionary(x => x.key, x => x.value);
            }

            public string Database
            {
                get => _keys.ContainsKey(_databaseKey) ? _keys[_databaseKey] : null;
                set => _keys[_databaseKey] = value;
            }

            public override string ToString() =>
                string.Join(_separator.ToString(),
                    _keys.Select(kvp => $"{kvp.Key}{_equality}{kvp.Value}"));
        }
    }
}