using Amazon.DynamoDBv2;
using Fridge.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fridge.DynamoDb
{
	public class UsersTable : DynamoDbTableBase<IdentityUser>
	{
		protected UsersTable(AmazonDynamoDBClient dbClient, string tableName) : base(dbClient, tableName)
		{
		}

		public override void Add(IdentityUser item)
		{
			//DbTable.
		}

		public override IEnumerable<IdentityUser> GetItems()
		{
			return null;
		}
	}
}
