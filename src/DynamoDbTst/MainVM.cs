using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamoDbTst
{
	public class MainVM : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private AmazonDynamoDBClient client;
		private AmazonDynamoDBClient Client
		{
			get { return client; }
			set
			{
				client = value;
			}
		}

		public MainVM()
		{
			this.ConnectCommand = new CustomCommand(CanConnectDb, ConnectDb);
		}

		private bool CanConnectDb(object obj)
		{
			return Client == null;
		}

		private void ConnectDb(object arg)
		{
			try
			{
				Client = CreateDbClient(true);
				NotifyPropertyChanged("");
			}
			catch(Exception e)
			{
				Debug.Fail(e.Message);
			}
		}

		public ICommand ConnectCommand { get; }


		// So we know whether local DynamoDB is running
		private static readonly string Ip = "localhost";
		private static readonly int Port = 8000;
		private static readonly string EndpointUrl = "http://" + Ip + ":" + Port;
		private bool disposedValue;

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
				Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
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


		// This method is called by the Set accessor of each property.  
		// The CallerMemberName attribute that is applied to the optional propertyName  
		// parameter causes the property name of the caller to be substituted as an argument.  
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					if(Client != null)
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
		// ~MainVM()
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
