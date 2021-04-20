using Fridge.DynamoDb;
using FridgeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamoDbTst
{
	public class MainVM : ViewModelBase, IDisposable
	{
		private bool disposedValue;
		private IFridgeLogger Logger {get; set;}

		private FridgeDynamoClient client;
		private FridgeDynamoClient Client
		{
			get { return client; }
			set
			{
				client = value;
			}
		}

		public MainVM(IFridgeLogger logger)
		{
			this.Logger = logger;
			this.ConnectCommand = new CustomCommand(CanConnectDb, ConnectDbAsync);
			this.CreateTableCommand = new CustomCommand(CanCreateTable, CreateTableAsync);
			this.DeleteTableCommand = new CustomCommand(CanDeleteTable, DeleteTableAsync);

		}

		private async Task DeleteTableAsync(object obj)
		{
			Debug.Assert(obj != null);
			try
			{
				string tableName = obj.ToString();
				Logger.LogInformation($"MainVM.DeleteTableAsync '{tableName}'");

				await Client.DeleteTableAsync(tableName);

				await RefreshTableView();
			}
			catch(Exception e)
			{
				Logger.LogError($"MainVM.DeleteTableAsync : failed", e);
				ReportError(e.Message);
			}
		}

		private bool CanDeleteTable(object arg)
		{
			return SelectedTable != null;
		}

		private async Task CreateTableAsync(object obj)
		{
			Debug.Assert(obj != null);
			try
			{
				string tableName = obj.ToString();
				Logger.LogInformation($"MainVM.CreateTableAsync '{tableName}'");

				if (tableName.Equals(DynDbConstants.UserTableName))
				{
					await Client.CreateUsersTable();
				}

				await RefreshTableView();
			}
			catch(Exception e)
			{
				Logger.LogError($"MainVM.CreateTableAsync : failed", e);
				ReportError(e.Message);
			}
		}

		private bool CanCreateTable(object arg)
		{
			if(arg == null || Tables == null || Client == null)
			{
				return false;
			}

			var tableName = arg.ToString();
			if(string.IsNullOrEmpty(tableName))
			{
				return false;
			}

			var existingTable = Tables.FirstOrDefault(t => t.TableName.Equals(tableName));
			if(existingTable == null)
			{
				return true;
			}

			return false;
		}

		public ICommand ConnectCommand { get; }
		public ICommand CreateTableCommand { get; }
		public ICommand DeleteTableCommand { get; }

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

		TableVM selectedTable;
		public TableVM SelectedTable
		{
			get => selectedTable;
			set
			{
				selectedTable = value;
				NotifyPropertyChanged("SelectedTable");
			}
		}

		public static void ReportError(string message)
		{
			System.Windows.MessageBox.Show(App.Current.MainWindow, message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
		}

		private bool CanConnectDb(object arg)
		{
			return Client == null;
		}

		private async Task ConnectDbAsync(object arg)
		{
			Logger.LogInformation("MainVM.ConnectDbAsync");
			try
			{
				var newClient = new FridgeDynamoClient(Logger);
				await newClient.ConnectAsync(true);

				Client = newClient;

				await RefreshTableView();
			}
			catch (Exception e)
			{
				Logger.LogError($"MainVM.ConnectDbAsync : failed", e);
				ReportError(e.Message);
			}
		}

		private async Task RefreshTableView()
		{
			var tablesInDb = await Client.GetTablesAsync();
			if (tablesInDb != null)
			{
				var tableList = new ObservableCollection<TableVM>(tablesInDb.Select(t => new TableVM(t)));
				Tables = tableList;
			}

			NotifyPropertyChanged("");
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
