using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FridgeApp.Services
{
	public class MockFridgeDAL : IFridgeDAL
	{
		public static readonly Guid Fridge1Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public static readonly Guid Partition1Id = Guid.Parse("4257558B-BC0A-43A8-A5A3-CB36B46D0DFC");
		public static readonly Guid Partition2Id = Guid.Parse("B01D35D0-D6FC-4A30-80B9-6EA841FBF85B");
		public static readonly Guid Partition3Id = Guid.Parse("7F79DE2A-13F8-4C20-932F-CFF6BD877D0C");
		public const string Fridge1Name = "Fridge 1";
		public const string Partition1Name = "Upper partition";
		public const string Partition2Name = "Middle partition";
		public const string Partition3Name = "Lower partition";

		private readonly List<Fridge.Model.Fridge> fridges;

		public MockFridgeDAL()
		{
			fridges = CreateMockFridges();
		}

		public static List<Fridge.Model.Fridge> CreateMockFridges()
		{
			var fridges = new List<Fridge.Model.Fridge>();
			var fridge1 = new Fridge.Model.Fridge() { Name = Fridge1Name, FridgeId = Fridge1Id };
			var partition1 = new Fridge.Model.Partition() { Name = Partition1Name, PartitionId = Partition1Id };
			var partition2 = new Fridge.Model.Partition() { Name = Partition2Name, PartitionId = Partition2Id };
			var partition3 = new Fridge.Model.Partition() { Name = Partition3Name, PartitionId = Partition3Id };
			fridge1.Partitions.Add(partition1);
			fridge1.Partitions.Add(partition2);
			fridge1.Partitions.Add(partition3);
			fridges.Add(fridge1);

			return fridges;
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
