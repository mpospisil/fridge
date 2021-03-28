using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FridgeApp.Services
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFridgeDAL
	{
		/// <summary>
		/// Attach to the database <paramref name="connectionString"/>
		/// </summary>
		/// <param name="connectionString">Connection string for attaching to the database</param>
		void OpenRepository(string connectionString);

		/// <summary>
		/// Resets all data in the repository
		/// </summary>
		void ResetRepository();

		/// <summary>
		/// Get the current user or null if user doesn't exist
		/// </summary>
		/// <returns></returns>
		Task<Fridge.Model.User> GetUserAsync();

		Task CreateUserAsync(Fridge.Model.User newUser);

		Task<IEnumerable<Fridge.Model.Fridge>> GetFridgesAsync(bool forceRefresh = false);

		Task<Fridge.Model.Fridge> GetFridgeAsync(Guid fridgeId);

		Task AddFridgeAsync(Fridge.Model.Fridge newFridgeData);

		Task UpdateFridgeAsync(Fridge.Model.Fridge modifiedFridge);

		Task DeleteFridgeAsync(Guid fridgeId);

		Task<IEnumerable<Fridge.Model.ItemInFridge>> GetItemsAsync(bool forceRefresh = false);

		Task AddItemAsync(Fridge.Model.ItemInFridge newItemInFridge);

		Task<Fridge.Model.ItemInFridge> GetItemAsync(Guid itemId);

		Task UpdateItemAsync(Fridge.Model.ItemInFridge modifiedItem);

		/// <summary>
		/// Remove item from fridge
		/// </summary>
		/// <param name="removingItemId">Id of the item to remove</param>
		/// <returns>Returns true if item was removed</returns>
		Task<bool> RemoveItemAsync(Guid removingItemId);

		Task DeleteItemAsync(Guid itemInFridgeId);

		Task<IEnumerable<Fridge.Model.ItemInFridge>> GetRemovedItemsAsync(bool forceRefresh = false);
	}
}
