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
		Task<IEnumerable<Fridge.Model.Fridge>> GetFridgesAsync(bool forceRefresh = false);

		Task<Fridge.Model.Fridge> GetFridgeAsync(Guid fridgeId);

		Task AddFridge(Fridge.Model.Fridge newFridgeData);

		Task UpdateFridge(Fridge.Model.Fridge modifiedFridge);

		Task DeleteFridgeAsync(Guid fridgeId);
	}
}
