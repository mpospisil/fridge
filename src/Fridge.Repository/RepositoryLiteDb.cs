using Fridge.Model;
using FridgeApp.Services;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fridge.Repository
{
	public class RepositoryLiteDb : IFridgeDAL, IDisposable
	{
		// colection Names
		static readonly string UserCollection = "users";

		private bool disposedValue;
		private readonly LiteDatabase db;
		private readonly IFridgeLogger Logger;

		public RepositoryLiteDb(IFridgeLogger logger, string connectionString) 
		{
			this.Logger = logger;
			Logger.LogDebug($"RepositoryLiteDb.RepositoryLiteDb connectionString = '{connectionString}'");
			db = new LiteDatabase(connectionString);
		}

		public LiteDatabase Db => db;

		#region implementation IFridgeDAL
		public async Task<User> GetUserAsync()
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetUserAsync");
				var users = Db.GetCollection<User>(UserCollection);

				return users.FindAll().FirstOrDefault();
			});
		}

		public async Task CreateUserAsync(User newUser)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.CreateUserAsync");
				// Get a collection (or create, if doesn't exist)
				var users = Db.GetCollection<User>(UserCollection);

				var insertedVal = users.Insert(newUser);
			});
		}

		public Task<IEnumerable<Model.Fridge>> GetFridgesAsync(bool forceRefresh = false)
		{
			throw new NotImplementedException();
		}

		public Task<Model.Fridge> GetFridgeAsync(Guid fridgeId)
		{
			throw new NotImplementedException();
		}

		public Task AddFridge(Model.Fridge newFridgeData)
		{
			throw new NotImplementedException();
		}

		public Task UpdateFridge(Model.Fridge modifiedFridge)
		{
			throw new NotImplementedException();
		}

		public Task DeleteFridgeAsync(Guid fridgeId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ItemInFridge>> GetItemsAsync(bool forceRefresh = false)
		{
			throw new NotImplementedException();
		}

		public Task AddItemAsync(ItemInFridge newFridgeData)
		{
			throw new NotImplementedException();
		}

		public Task<ItemInFridge> GetItemAsync(Guid itemId)
		{
			throw new NotImplementedException();
		}

		public Task UpdateItemAsync(ItemInFridge modifiedItem)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(Guid itemInFridgeId)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Disposing
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (Db != null)
					{
						Db.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~RepositoryLiteDb()
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
		#endregion
	}
}
