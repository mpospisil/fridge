using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;

namespace Fridge.DynamoDb
{
	public abstract class DynamoDbTableBase<T>
	{
		readonly string TableName;
		private Table dbTable;

		protected DynamoDbTableBase(AmazonDynamoDBClient dbClient, string tableName)
		{
			TableName = tableName;
			DbTable = Table.LoadTable(dbClient, TableName);
		}

		protected Table DbTable { get => dbTable; private set => dbTable = value; }

		public abstract void Add(T item);

		public abstract IEnumerable<T> GetItems();
	}
}
