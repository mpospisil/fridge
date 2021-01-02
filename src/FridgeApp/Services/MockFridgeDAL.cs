using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FridgeApp.Services
{
	public class MockFridgeDAL : IFridgeDAL
	{
		private readonly List<Fridge.Model.Fridge> fridges;

		public MockFridgeDAL()
		{
			fridges = new List<Fridge.Model.Fridge>();
			var fridge1 = new Fridge.Model.Fridge() { Name = "Fridge 1", FridgeId = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961") };
			var partition1 = new Fridge.Model.Partition() { Name = "Partition 1", PartitionId = Guid.Parse("4257558B-BC0A-43A8-A5A3-CB36B46D0DFC") };
			var partition2 = new Fridge.Model.Partition() { Name = "Partition 2", PartitionId = Guid.Parse("B01D35D0-D6FC-4A30-80B9-6EA841FBF85B") };
			fridge1.Partitions.Add(partition1);
			fridge1.Partitions.Add(partition2);
			fridges.Add(fridge1);
		}
		 

		public async Task<IEnumerable<Fridge.Model.Fridge>> GetFridgesAsync(bool forceRefresh = false)
		{
			return await Task.FromResult(fridges);
		}

		public async Task<Fridge.Model.Fridge> GetFridgeAsync(Guid fridgeId)
		{
			return await Task.FromResult(fridges.First(f => f.FridgeId == fridgeId));
		}

		public void AddFridge(Fridge.Model.Fridge fridge)
		{
			fridges.Add(fridge);
		}
		public void DeleteFridge(Fridge.Model.Fridge fridge)
		{
			int index = fridges.FindIndex((item) => item.FridgeId == fridge.FridgeId);
			if(index < -1)
			{
				throw new Exception($"Fridge id = '{fridge.FridgeId}' doesn't exist");
			}

			fridges.RemoveAt(index);
		}
	}
}
