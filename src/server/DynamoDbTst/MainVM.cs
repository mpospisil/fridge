using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Linq;

namespace DynamoDbTst
{
	public class MainVM : ViewModelBase, IDisposable
	{
		private bool disposedValue;

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

		ObservableCollection<TableVM> tables;
		public ObservableCollection<TableVM> Tables
		{
			get => tables;
			set
			{
				tables = value;
				NotifyPropertyChanged("Tables");
			}
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

				var tablesInDb = await Client.GetTablesAsync();
				if(tablesInDb != null)
				{
					var tableList = new ObservableCollection<TableVM>(tablesInDb.Select(t => new TableVM(t)));
					Tables = tableList;
				}

				NotifyPropertyChanged("");
			}
			catch(Exception e)
			{
				Debug.Fail(e.Message);
			}
		}

		public ICommand ConnectCommand { get; }

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
