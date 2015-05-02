using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQL.LocalDB.Test;

namespace Tests
{
    [TestClass]
    public class SimpleExample
    {
        [TestMethod]
        public void LocalDbForSingleTest()
        {
            // The database will be recreated here if it already exists
            using (TempLocalDb db = new TempLocalDb("Test"))
            {
                // Create the schema in the database (This tool will work best if you are using some kind of database migration tool like FluentMigrator or DbUp)
                SchemaHelper.MigrateToCurrentSchema(db);

                // Call some code that interacts with the database
                DatabaseService dbService = new DatabaseService(db.ConnectionString);
                dbService.Insert("First", "Last");

                // Verify that the code worked correctly
                using (var conn = db.Open())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [Person] WHERE [FirstName] = @FirstName AND [LastName] = @LastName";
                    cmd.Parameters.AddWithValue("@FirstName", "First");
                    cmd.Parameters.AddWithValue("@LastName", "Last");

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Just verify that the record exists in the table
                        Assert.IsTrue(reader.Read());
                    }
                }

                // OR Call some other methods to verify the operation worked
                Assert.IsNotNull(dbService.FirstOrDefault("First", "Last"));
            }
        }
    }
}