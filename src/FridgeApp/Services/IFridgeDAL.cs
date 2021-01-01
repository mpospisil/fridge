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

		void AddFridge(Fridge.Model.Fridge fridge);

		void DeleteFridge(Fridge.Model.Fridge fridge);
	}
}
