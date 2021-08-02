using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL.LocalDB.Test;

namespace Tests
{
    public static class SchemaHelper
    {
        public static void MigrateToCurrentSchema(LocalDb db)
        {
            using (var conn = db.Open())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE [dbo].[Person](
	[PersonID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_PersonID] PRIMARY KEY CLUSTERED 
(
	[PersonID] ASC
) ON [PRIMARY],
) ON [PRIMARY]";
                cmd.ExecuteNonQuery();
            }
        }
    }
}