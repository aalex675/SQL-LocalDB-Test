using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SQL.LocalDB.Test
{
    /// <summary>
    /// Represents a local database using SQL LocalDB that must be manually created/deleted.
    /// </summary>
    public class LocalDb
    {
        /// <summary>
        /// The name of the database represented by this instance of <see cref="LocalDb"/>.
        /// </summary>
        private string databaseName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDb"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the SQL LocalDB database.</param>
        public LocalDb(string databaseName)
        {
            this.databaseName = databaseName;
        }

        /// <summary>
        /// Gets the connection string to the SQL LocalDB database.
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                return this.BuildConnectionString(this.databaseName);
            }
        }

        /// <summary>
        /// Opens a connection to the SQL LocalDB database.
        /// </summary>
        /// <returns>An open connection to the SQL LocalDB database.</returns>
        public virtual SqlConnection Open()
        {
            SqlConnection conn = new SqlConnection(this.ConnectionString);
            conn.Open();

            return conn;
        }

        /// <summary>
        /// Opens a connection to the SQL LocalDB master database.
        /// </summary>
        /// <returns>An open connection to the SQL LocalDB master database.</returns>
        public virtual SqlConnection OpenMaster()
        {
            SqlConnection conn = new SqlConnection(this.BuildConnectionString(null));
            conn.Open();

            return conn;
        }

        /// <summary>
        /// Checks whether the SQL LocalDB database exists or not.
        /// </summary>
        /// <returns>True if the database exists, otherwise false.</returns>
        public virtual bool CheckExists()
        {
            using (var conn = this.OpenMaster())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    string.Format(
                        @"
IF EXISTS(SELECT * FROM sys.databases WHERE name='{0}')
BEGIN
    SELECT 1
END
ELSE
BEGIN
    SELECT 0
END",
                        this.databaseName);

                int result = (int)cmd.ExecuteScalar();
                bool exists = result == 1;

                return exists;
            }
        }

        /// <summary>
        /// Executes a create database command to create the SQL LocalDB database.
        /// </summary>
        public virtual void CreateDatabase()
        {
            var sw = Stopwatch.StartNew();

            using (var conn = this.OpenMaster())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = string.Format(
                    @"
DECLARE @FILENAME as varchar(255)
DECLARE @LOGFILENAME as varchar(255)

SET @FILENAME = CONVERT(VARCHAR(255), SERVERPROPERTY('instancedefaultdatapath')) + '{0}';
SET @LOGFILENAME = CONVERT(VARCHAR(255), SERVERPROPERTY('instancedefaultdatapath')) + '{0}.log';

EXEC ('CREATE DATABASE [{0}]
        ON PRIMARY (NAME = [{0}], FILENAME = ''' + @FILENAME + ''' )
        LOG ON (NAME = [Log], FILENAME = ''' + @LOGFILENAME + ''' )
            ')",
                    this.databaseName);
                
                cmd.ExecuteNonQuery();
            }

            sw.Stop();
            Console.WriteLine("CreateDatabase completed in {0} ms.", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Executes a drop database command to delete the SQL LocalDB database.
        /// </summary>
        public virtual void DeleteDatabase()
        {
            var sw = Stopwatch.StartNew();
            
            using (var conn = this.OpenMaster())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = string.Format(
                    @"
IF EXISTS(SELECT * FROM sys.databases WHERE name='{0}')
BEGIN
    ALTER DATABASE [{0}]
    SET SINGLE_USER
    WITH ROLLBACK IMMEDIATE
    DROP DATABASE [{0}]
END",
                    this.databaseName);

                cmd.ExecuteNonQuery();
            }

            sw.Stop();
            Console.WriteLine("DeleteDatabase completed in {0} ms.", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Builds a connection string for the current database, or if a null or empty string is passed in, builds a connection string for the master database.
        /// </summary>
        /// <param name="databaseName">The name of the database to connect to. If null or empty string is provided, the connection string will connect to the master database.</param>
        /// <returns>A connection string</returns>
        protected virtual string BuildConnectionString(string databaseName)
        {
            var sb = new SqlConnectionStringBuilder();
            sb.DataSource = @"(localdb)\v11.0";

            if (string.IsNullOrWhiteSpace(databaseName) == false)
            {
                sb.InitialCatalog = databaseName;
            }

            return sb.ConnectionString;
        }
    }
}