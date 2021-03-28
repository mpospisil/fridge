using Fridge.Model;
using FridgeApp.Services;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fridge.Repository
{
	public class RepositoryLiteDb : IFridgeDAL, IDisposable
	{
		// colection Names
		static readonly string UserCollection = "users";
		static readonly string FridgeCollection = "fridges";
		static readonly string ItemCollection = "items";
		static readonly string RemovedItemCollection = "removeditems";

		private bool disposedValue;
		private LiteDatabase db;
		private readonly IFridgeLogger Logger;

		static RepositoryLiteDb()
		{
			BsonMapper.Global.Entity<ItemInFridge>().Id(oid => oid.ItemId);
			BsonMapper.Global.Entity<Fridge.Model.Fridge>().Id(oid => oid.FridgeId);
			BsonMapper.Global.Entity<Fridge.Model.User>().Id(oid => oid.UserId);
		}

		public RepositoryLiteDb(IFridgeLogger logger)
		{
			this.Logger = logger;
		}

		public LiteDatabase Db
		{
			get => db;
			set => db = value;
		}

		internal string ConnectionString {get; private set;}

	#region implementation IFridgeDAL
	public void OpenRepository(string connectionString)
		{
			Logger.LogDebug($"RepositoryLiteDb.OpenRepository connectionString = '{connectionString}'");
			Db = new LiteDatabase(connectionString);
			ConnectionString = connectionString;
		}

		public void ResetRepository()
		{
			if(Db == null)
			{
				return;
			}

			Logger.LogDebug("RepositoryLiteDb.ResetRepository");

			if (string.IsNullOrEmpty(ConnectionString))
			{
				Logger.LogError("RepositoryLiteDb.ResetRepository missin the connection string", new Exception());
				return;
			}

			Db.Dispose();

			Logger.LogDebug($"RepositoryLiteDb.ResetRepository deleting the file '{ConnectionString}'");
			File.Delete(ConnectionString);

			OpenRepository(ConnectionString);
		}

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
				Db.BeginTrans();
				var users = Db.GetCollection<User>(UserCollection);
				var insertedVal = users.Insert(newUser);
				users.EnsureIndex(u => u.UserId);
				Db.Commit();
			});
		}

		public async Task<IEnumerable<Model.Fridge>> GetFridgesAsync(bool forceRefresh = false)
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetFridgesAsync");
				// Get a collection (or create, if doesn't exist)
				var fridges = Db.GetCollection<Model.Fridge>(FridgeCollection);
				var res = fridges.FindAll();
				return res;
			});
		}

		public async Task<Model.Fridge> GetFridgeAsync(Guid fridgeId)
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetFridgeAsync fridgeId = '{fridgeId}'");

				// Get a collection (or create, if doesn't exist)
				var fridges = Db.GetCollection<Model.Fridge>(FridgeCollection);
				var res = fridges.Query().Where(f => f.FridgeId == fridgeId).FirstOrDefault();
				return res;
			});
		}

		public async Task AddFridgeAsync(Model.Fridge newFridgeData)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.AddFridge  fridgeId = {newFridgeData.FridgeId.ToString()}, fridgeName = '{newFridgeData.Name}'");
				Db.BeginTrans();
				// Get a collection (or create, if doesn't exist)
				var fridges = Db.GetCollection<Model.Fridge>(FridgeCollection);
				var insertedVal = fridges.Insert(newFridgeData);
				fridges.EnsureIndex(f => f.FridgeId);
				Db.Commit();
			});
		}

		public async Task UpdateFridgeAsync(Model.Fridge modifiedFridge)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.UpdateFridgeAsync  fridgeId = {modifiedFridge.FridgeId.ToString()}, fridgeName = '{modifiedFridge.Name}'");
				Db.BeginTrans();
				// Get a collection (or create, if doesn't exist)
				Db.BeginTrans();
				var fridges = Db.GetCollection<Model.Fridge>(FridgeCollection);
				var res = fridges.Update(modifiedFridge);
				Db.Commit();
			});
		}

		public async Task DeleteFridgeAsync(Guid fridgeId)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.DeleteFridgeAsync  fridgeId = {fridgeId}");
				Db.BeginTrans();
				// Get a collection (or create, if doesn't exist)
				var fridges = Db.GetCollection<Model.Fridge>(FridgeCollection);
				//var foundFridge = fridges.FindOne(f => f.FridgeId == fridgeId);
				fridges.Delete(fridgeId);
				Db.Commit();
			});
		}

		public async Task<IEnumerable<ItemInFridge>> GetItemsAsync(bool forceRefresh = false)
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetItemsAsync");
				// Get a collection (or create, if doesn't exist)
				var items = Db.GetCollection<Model.ItemInFridge>(ItemCollection);
				var res = items.FindAll();
				return res;
			});
		}

		public async Task AddItemAsync(ItemInFridge newItem)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.AddItemAsync  newItem = {newItem.ItemId.ToString()}, itemName = '{newItem.Name}'");
				// Get a collection (or create, if doesn't exist)
				Db.BeginTrans();
				var items = Db.GetCollection<Model.ItemInFridge>(ItemCollection);
				var insertedVal = items.Insert(newItem);
				items.EnsureIndex(i => i.ItemId);
				Db.Commit();
			});
		}

		public async Task<ItemInFridge> GetItemAsync(Guid itemId)
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetItemAsync itemId = '{itemId}'");

				// Get a collection (or create, if doesn't exist)
				var items = Db.GetCollection<Model.ItemInFridge>(ItemCollection);
				var res = items.Query().Where(i => i.ItemId == itemId).FirstOrDefault();
				return res;
			});
		}

		public async Task UpdateItemAsync(ItemInFridge modifiedItem)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.UpdateItemAsync itemId = {modifiedItem.ItemId.ToString()}, itemName = '{modifiedItem.Name}'");
				Db.BeginTrans();
				// Get a collection (or create, if doesn't exist)
				var items = Db.GetCollection<Model.ItemInFridge>(ItemCollection);
				var res = items.Update(modifiedItem);
				Db.Commit();
			});
		}

		public async Task DeleteItemAsync(Guid itemInFridgeId)
		{
			await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.DeleteItemAsync  itemInFridgeId = {itemInFridgeId}");
				Db.BeginTrans();
				// Get a collection (or create, if doesn't exist)
				var items = Db.GetCollection<Model.ItemInFridge>(ItemCollection);
				items.Delete(itemInFridgeId);
				Db.Commit();
			});
		}

		public async Task<bool> RemoveItemAsync(Guid removingItemId)
		{
			var itemToRemove = await GetItemAsync(removingItemId);
			if (itemToRemove == null)
			{
				return await Task.FromResult(false);
			}

			var fridgeOfItem = await GetFridgeAsync(itemToRemove.FridgeId);
			await DeleteItemAsync(removingItemId);

			itemToRemove.FridgeId = fridgeOfItem.RemovedItemsIdentifier;
			itemToRemove.SectorId = fridgeOfItem.RemovedItemsIdentifier;
			itemToRemove.IsInFridge = false;
			itemToRemove.TimeStamp = DateTime.UtcNow;
			itemToRemove.History.Add(new Fridge.Model.ItemChange() { TimeOfChange = itemToRemove.TimeStamp, TypeOfChange = Fridge.Model.ChangeTypes.Removed });

			var removedItems = Db.GetCollection<Model.ItemInFridge>(RemovedItemCollection);

			removedItems.Insert(itemToRemove);
			return await Task.FromResult(true);
		}

		public async Task<IEnumerable<ItemInFridge>> GetRemovedItemsAsync(bool forceRefresh = false)
		{
			return await Task.Run(() =>
			{
				Logger.LogDebug($"RepositoryLiteDb.GetRemovedItemsAsync");
				// Get a collection (or create, if doesn't exist)
				var removedItems = Db.GetCollection<Model.ItemInFridge>(RemovedItemCollection);
				var res = removedItems.FindAll();
				return res;
			});
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
