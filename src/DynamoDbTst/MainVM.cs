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
		private bool disposedValue;
		public event PropertyChangedEventHandler PropertyChanged;

		private FridgeDynamoClient client;
		private FridgeDynamoClient Client
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

		private bool CanConnectDb(object arg)
		{
			return Client == null;
		}

		private async void ConnectDb(object arg)
		{
			try
			{
				var newClient = new FridgeDynamoClient();
				await newClient.ConnectAsync(true);

				Client = newClient;
				NotifyPropertyChanged("");
			}
			catch(Exception e)
			{
				Debug.Fail(e.Message);
			}
		}

		public ICommand ConnectCommand { get; }



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
