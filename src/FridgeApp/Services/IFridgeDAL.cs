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
		/// Get the current user or null if user doesn't exist
		/// </summary>
		/// <returns></returns>
		Task<Fridge.Model.User> GetUserAsync();

		Task CreateUserAsync(Fridge.Model.User newUser);

		Task<IEnumerable<Fridge.Model.Fridge>> GetFridgesAsync(bool forceRefresh = false);

		Task<Fridge.Model.Fridge> GetFridgeAsync(Guid fridgeId);

		Task AddFridge(Fridge.Model.Fridge newFridgeData);

		Task UpdateFridge(Fridge.Model.Fridge modifiedFridge);

		Task DeleteFridgeAsync(Guid fridgeId);

		Task<IEnumerable<Fridge.Model.ItemInFridge>> GetItemsAsync(bool forceRefresh = false);

		Task AddItemAsync(Fridge.Model.ItemInFridge newFridgeData);

		Task<Fridge.Model.ItemInFridge> GetItemAsync(Guid itemId);

		Task UpdateItemAsync(Fridge.Model.ItemInFridge modifiedItem);

		Task DeleteAsync(Guid itemInFridgeId);
	}
}
