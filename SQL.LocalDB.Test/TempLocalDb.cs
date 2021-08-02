using System;

namespace SQL.LocalDB.Test
{
    /// <summary>
    /// Represents a local database using SQL LocalDB that is automatically (re)created each time an instance of this class is created and is deleted when the instance is disposed.
    /// </summary>
    public class TempLocalDb : LocalDb, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempLocalDb"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the SQL LocalDB database.</param>
        /// <param name="dataSource">The SQL Server instance to connect to</param>
        public TempLocalDb(string databaseName, string dataSource = @"(localdb)\MSSQLLocalDB")
            : base(databaseName, dataSource)
        {
            this.DeleteDatabase();
            this.CreateDatabase();
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void Dispose()
        {
            this.DeleteDatabase();
        }
    }
}