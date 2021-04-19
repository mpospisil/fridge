using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace DynamoDbTst
{
	public class FridgeDynamoClient : IDisposable
	{
		private bool disposedValue;
		private AmazonDynamoDBClient Client { get; set; }

		const string UserTableName = "Users2";

		public FridgeDynamoClient()
		{
		}

		public async Task ConnectAsync(bool useLocalDb)
		{
			if(Client != null)
			{
				throw new Exception("Client exists");
			}

			Client = CreateDbClient(useLocalDb);

			var isUserTable = await CheckingTableExistence_async(UserTableName);
			if(!isUserTable)
			{
				await CreateExampleTable(UserTableName);
			}

			var response = await Client.ListTablesAsync();
		}

		// So we know whether local DynamoDB is running
		private static readonly string Ip = "localhost";
		private static readonly int Port = 8000;
		private static readonly string EndpointUrl = "http://" + Ip + ":" + Port;

		private static bool IsPortInUse()
		{
			bool isAvailable = true;

			// Evaluate current system TCP connections. This is the same information provided
			// by the netstat command line application, just in .Net strongly-typed object
			// form.  We will look through the list, and if our port we would like to use
			// in our TcpClient is occupied, we will set isAvailable to false.
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

			foreach (IPEndPoint endpoint in tcpConnInfoArray)
			{
				if (endpoint.Port == Port)
				{
					isAvailable = false;
					break;
				}
			}

			return isAvailable;
		}

		public static AmazonDynamoDBClient CreateDbClient(bool useDynamoDbLocal)
		{
			if (useDynamoDbLocal)
			{
				// First, check to see whether anyone is listening on the DynamoDB local port
				// (by default, this is port 8000, so if you are using a different port, modify this accordingly)
				var portUsed = IsPortInUse();

				if (portUsed)
				{
					throw new Exception("The local version of DynamoDB is NOT running.");
				}

				// DynamoDB-Local is running, so create a client
				AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
				ddbConfig.ServiceURL = EndpointUrl;

				var client = new AmazonDynamoDBClient(ddbConfig);
				return client;

			}
			else
			{
				return new AmazonDynamoDBClient();
			}
		}


		public async Task<bool> CheckingTableExistence_async(string tblNm)
		{
			var response = await Client.ListTablesAsync();
			return response.TableNames.Contains(tblNm);
		}

		public async Task<List<string>> GetTablesAsync()
		{
			var response = await Client.ListTablesAsync();
			return response.TableNames;
		}

		public async Task<bool> CreateTable_async(string tableName,
				List<AttributeDefinition> tableAttributes,
				List<KeySchemaElement> tableKeySchema,
				ProvisionedThroughput provisionedThroughput)
		{
			bool response = true;

			// Build the 'CreateTableRequest' structure for the new table
			var request = new CreateTableRequest
			{
				TableName = tableName,
				AttributeDefinitions = tableAttributes,
				KeySchema = tableKeySchema,
				// Provisioned-throughput settings are always required,
				// although the local test version of DynamoDB ignores them.
				ProvisionedThroughput = provisionedThroughput
			};


			var makeTbl = await Client.CreateTableAsync(request);


			return response;
		}

		public async Task<TableDescription> GetTableDescription(string tableName)
		{
			TableDescription result = null;

			// If the table exists, get its description.

			var response = await Client.DescribeTableAsync(tableName);
			result = response.Table;


			return result;
		}

		//private async Task CreateUsersTable()
		//{

		//}

		private async Task CreateExampleTable(string tableName)
		{
			Console.WriteLine("\n*** Creating table ***");
			var request = new CreateTableRequest
			{
				AttributeDefinitions = new List<AttributeDefinition>()
						{
								new AttributeDefinition
								{
										AttributeName = "Id",
										AttributeType = "N"
								},
								new AttributeDefinition
								{
										AttributeName = "ReplyDateTime",
										AttributeType = "N"
								}
						},
				KeySchema = new List<KeySchemaElement>
						{
								new KeySchemaElement
								{
										AttributeName = "Id",
										KeyType = "HASH" //Partition key
                },
								new KeySchemaElement
								{
										AttributeName = "ReplyDateTime",
										KeyType = "RANGE" //Sort key
                }
						},
				ProvisionedThroughput = new ProvisionedThroughput
				{
					ReadCapacityUnits = 5,
					WriteCapacityUnits = 6
				},
				TableName = tableName
			};

			var response = await Client.CreateTableAsync(request);

			var tableDescription = response.TableDescription;
			Console.WriteLine("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
								tableDescription.TableStatus,
								tableDescription.TableName,
								tableDescription.ProvisionedThroughput.ReadCapacityUnits,
								tableDescription.ProvisionedThroughput.WriteCapacityUnits);

			string status = tableDescription.TableStatus;
			Console.WriteLine(tableName + " - " + status);

			await WaitUntilTableReady(tableName);
		}


		private async Task WaitUntilTableReady(string tableName)
		{
			string status = null;
			// Let us wait until table is created. Call DescribeTable.
			do
			{
				System.Threading.Thread.Sleep(5000); // Wait 5 seconds.

					var res = await Client.DescribeTableAsync(new DescribeTableRequest
					{
						TableName = tableName
					});
				status = res.Table.TableStatus;


			} while (status != "ACTIVE");
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (Client != null)
					{
						Client.Dispose();
						Client = null;
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~DbClient()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
