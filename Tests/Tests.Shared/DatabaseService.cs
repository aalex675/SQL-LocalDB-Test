using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Tests
{
    public class DatabaseService
    {
        private string connectionString;

        public DatabaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Person FirstOrDefault(string firstName, string lastName)
        {
            using (var conn = this.Open())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT [PersonID], [FirstName], [LastName] FROM [dbo].[Person] WHERE [FirstName] = @FirstName AND [LastName] = @LastName";
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);

                List<Person> values = new List<Person>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string firstName2 = reader.GetString(1);
                        string lastName2 = reader.GetString(2);

                        Person p = new Person() { PersonID = id, FirstName = firstName2, LastName = lastName2 };

                        return p;
                    }
                }

                return null;
            }
        }

        public List<Person> GetAllPeople()
        {
            using (var conn = this.Open())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT [PersonID], [FirstName], [LastName] FROM [dbo].[Person]";
                List<Person> values = new List<Person>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        values.Add(new Person() { PersonID = id, FirstName = firstName, LastName = lastName });
                    }
                }

                return values;
            }
        }

        public void Insert(string firstName, string lastName)
        {
            using (var conn = this.Open())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [dbo].[Person] ( [FirstName], [LastName] ) VALUES ( @FirstName, @LastName )";
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int personID)
        {
            using (var conn = this.Open())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM [dbo].[Person] WHERE [PersonID] = @ID";
                cmd.Parameters.AddWithValue("@ID", personID);
                cmd.ExecuteNonQuery();
            }
        }

        private SqlConnection Open()
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}