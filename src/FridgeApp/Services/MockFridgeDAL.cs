using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FridgeApp.Services
{
	public class MockFridgeDAL : IFridgeDAL
	{
		public static DateTime date1 = new DateTime(2021, 1, 9);
		public static DateTime date2 = new DateTime(2021, 1, 8);
		public static DateTime date3 = new DateTime(2021, 1, 8);

		public static readonly Guid Fridge1Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public static readonly Guid Partition1Id = Guid.Parse("4257558B-BC0A-43A8-A5A3-CB36B46D0DFC");
		public static readonly Guid Partition2Id = Guid.Parse("B01D35D0-D6FC-4A30-80B9-6EA841FBF85B");
		public static readonly Guid Partition3Id = Guid.Parse("7F79DE2A-13F8-4C20-932F-CFF6BD877D0C");
		public const string Fridge1Name = "Fridge 1";
		public const string Partition1Name = "Upper partition";
		public const string Partition2Name = "Middle partition";
		public const string Partition3Name = "Lower partition";

		public static readonly Guid Fr1Part1Item1Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public const string Fr1Part1Item1Name = "Pork meat";

		public static readonly Guid Fr1Part1Item2Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public const string Fr1Part1Item2Name = "Trout";

		public static readonly Guid Fr1Part1Item3Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public const string Fr1Part1Item3Name = "Goulash";

		public static readonly Guid Fr1Part2Item1Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public const string Fr1Part2Item1Name = "Bread";

		private readonly List<Fridge.Model.Fridge> fridges;

		private readonly List<Fridge.Model.ItemInFridge> items;

		public MockFridgeDAL()
		{
			fridges = CreateMockFridges();
			items = CreateMockItems();
		}

		public static List<Fridge.Model.ItemInFridge> CreateMockItems()
		{
			var items = new List<Fridge.Model.ItemInFridge>();

			items.Add(new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item1Id,
				Name = Fr1Part1Item1Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				TimeStamp = date1
			});

			items.Add(new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item2Id,
				Name = Fr1Part1Item2Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				TimeStamp = date1
			});

			items.Add(new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item2Id,
				Name = Fr1Part1Item2Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				TimeStamp = date1
			});

			items.Add(new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item3Id,
				Name = Fr1Part1Item3Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				TimeStamp = date1
			});

			items.Add(new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part2Item1Id,
				Name = Fr1Part2Item1Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition2Id,
				TimeStamp = date2
			});

			return items;
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

		public async Task AddFridge(Fridge.Model.Fridge newFridge)
		{
			fridges.Add(newFridge);
			await Task.CompletedTask;
		}

		public async Task UpdateFridge(Fridge.Model.Fridge fridge)
		{
			int index = fridges.FindIndex((item) => item.FridgeId == fridge.FridgeId);
			fridges.RemoveAt(index);
			fridges.Insert(index, fridge);
			await Task.CompletedTask;
		}

		public async Task DeleteFridgeAsync(Guid fridgeId)
		{
			if(fridgeId == Guid.Empty)
			{
				return;
			}

			int index = fridges.FindIndex((item) => item.FridgeId == fridgeId);
			if(index < -1)
			{
				throw new Exception($"Fridge id = '{fridgeId}' doesn't exist");
			}

			fridges.RemoveAt(index);

			await Task.CompletedTask;
		}

		public Task<IEnumerable<Fridge.Model.ItemInFridge>> GetItemsAsync(bool forceRefresh = false)
		{
			throw new NotImplementedException();
		}

		public Task<Fridge.Model.ItemInFridge> AddAsync(Fridge.Model.ItemInFridge newFridgeData)
		{
			throw new NotImplementedException();
		}

		public Task<Fridge.Model.ItemInFridge> TakeOutAsync(Guid itemInFridgeId)
		{
			throw new NotImplementedException();
		}

		public Task<Fridge.Model.ItemInFridge> DeleteAsync(Guid itemInFridgeId)
		{
			throw new NotImplementedException();
		}
	}
}
