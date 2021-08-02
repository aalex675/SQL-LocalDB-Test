﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQL.LocalDB.Test;

namespace Tests
{
    [TestClass]
    public class Shared_Database
    {
        private static TempLocalDb database;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            database = new TempLocalDb("Test");

            // Create schema for tests
            // If you want to run tests against your database, you should execute your migrations or creation script here
            SchemaHelper.MigrateToCurrentSchema(database);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void Database_Exists()
        {
            Assert.IsTrue(database.CheckExists());
        }

        [TestMethod]
        public void Insert_Works()
        {
            string firstName = "First-" + Guid.NewGuid().ToString();
            string lastName = "Last-" + Guid.NewGuid().ToString();

            DatabaseService db = new DatabaseService(database.ConnectionString);

            // With a shared database, you can use transaction scopes and not commit the changes so that unit tests don't interfere with each other.
            using (TransactionScope trans = new TransactionScope())
            {
                db.Insert(firstName, lastName);

                Assert.IsNotNull(db.FirstOrDefault(firstName, lastName));
            }

            Assert.IsNull(db.FirstOrDefault(firstName, lastName));
        }

        [TestMethod]
        public void Delete()
        {
            string firstName = "First-" + Guid.NewGuid().ToString();
            string lastName = "Last-" + Guid.NewGuid().ToString();

            DatabaseService db = new DatabaseService(database.ConnectionString);

            // With a shared database, you can use transaction scopes and not commit the changes so that unit tests don't interfere with each other.
            using (TransactionScope trans = new TransactionScope())
            {
                db.Insert(firstName, lastName);

                var person = db.FirstOrDefault(firstName, lastName);
                Assert.IsNotNull(person);

                db.Delete(person.PersonID);

                Assert.IsNull(db.FirstOrDefault(firstName, lastName));
            }
        }
    }
}