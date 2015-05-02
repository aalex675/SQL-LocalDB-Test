using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        public TempLocalDb(string databaseName)
            : base(databaseName)
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