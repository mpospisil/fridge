using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeApp.Services
{
	public class MockFridgeDAL : IFridgeDAL
	{
		public static readonly Guid User1Id = Guid.Parse("B49518B9-92A8-4FDF-8395-A452EDBAECD2");
		public const string User1Name = "Tester";
		public const string User1Email = "tester@tester.com";

		public static readonly DateTime Date1 = new DateTime(2021, 1, 9);
		public static readonly DateTime Date2 = new DateTime(2021, 1, 8);
		public static readonly DateTime Date3 = new DateTime(2021, 1, 8);

		public static readonly Guid Fridge1Id = Guid.Parse("07860F5F-038F-4417-9190-E139EF9FE961");
		public static readonly Guid Fridge1RemovedItemsId = Guid.Parse("0CF37477-E479-4EA7-B825-4B1A25A3E1D9");
		public static readonly Guid Partition1Id = Guid.Parse("4257558B-BC0A-43A8-A5A3-CB36B46D0DFC");
		public static readonly Guid Partition2Id = Guid.Parse("B01D35D0-D6FC-4A30-80B9-6EA841FBF85B");
		public static readonly Guid Partition3Id = Guid.Parse("7F79DE2A-13F8-4C20-932F-CFF6BD877D0C");
		public const string Fridge1Name = "Fridge 1";
		public const string Partition1Name = "Upper partition";
		public const string Partition2Name = "Middle partition";
		public const string Partition3Name = "Lower partition";

		public static readonly Guid Fr1Part1Item1Id = Guid.Parse("59C7EDD9-E451-4D2C-852E-63A101241CBA");
		public const string Fr1Part1Item1Name = "Pork meat";

		public static readonly Guid Fr1Part1Item2Id = Guid.Parse("12E8F688-E710-4DE4-90A7-1D407D4975FC");
		public const string Fr1Part1Item2Name = "Trout";

		public static readonly Guid Fr1Part1Item3Id = Guid.Parse("1F7BB9D0-295E-4F3A-A268-DA35CD1DF5F6");
		public const string Fr1Part1Item3Name = "Goulash";

		public static readonly Guid Fr1Part2Item1Id = Guid.Parse("33C431C6-F838-4964-9585-A73039741D3D");
		public const string Fr1Part2Item1Name = "Bread";

		private readonly List<Fridge.Model.Fridge> fridges;
		private readonly List<Fridge.Model.ItemInFridge> items;
		private Fridge.Model.User user;

		public MockFridgeDAL(bool createEmpty = false)
		{
			if (createEmpty)
			{
				fridges = new List<Fridge.Model.Fridge>();
				items = new List<Fridge.Model.ItemInFridge>();
			}
			else
			{
				fridges = CreateMockFridges();
				items = CreateMockItems();
			}
		}

		public static List<Fridge.Model.ItemInFridge> CreateMockItems()
		{
			var items = new List<Fridge.Model.ItemInFridge>();

			var item1 = new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item1Id,
				Name = Fr1Part1Item1Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				IsInFridge = true,
				TimeStamp = Date1,
			};

			item1.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = Date1 });
			items.Add(item1);

			var item2 = new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item2Id,
				Name = Fr1Part1Item2Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				IsInFridge = true,
				TimeStamp = Date2
			};

			item2.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = Date2 });
			items.Add(item2);

			var item3 = new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part1Item3Id,
				Name = Fr1Part1Item3Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition1Id,
				IsInFridge = true,
				TimeStamp = Date1
			};

			item3.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = Date1 });
			items.Add(item3);

			var item4 = new Fridge.Model.ItemInFridge()
			{
				ItemId = Fr1Part2Item1Id,
				Name = Fr1Part2Item1Name,
				FridgeId = Fridge1Id,
				PartitionId = Partition2Id,
				IsInFridge = true,
				TimeStamp = Date3
			};

			item4.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = Date3 });
			items.Add(item4);

			return items;
		}

		public static List<Fridge.Model.Fridge> CreateMockFridges()
		{
			var fridges = new List<Fridge.Model.Fridge>();
			var fridge1 = new Fridge.Model.Fridge() { Name = Fridge1Name, FridgeId = Fridge1Id, RemovedItemsIdentifier = Fridge1RemovedItemsId, OwnerId = User1Id };
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
			if (fridgeId == Guid.Empty)
			{
				return;
			}

			int index = fridges.FindIndex((item) => item.FridgeId == fridgeId);
			if (index < -1)
			{
				throw new Exception($"Fridge id = '{fridgeId}' doesn't exist");
			}

			fridges.RemoveAt(index);

			await Task.CompletedTask;
		}

		public async Task<IEnumerable<Fridge.Model.ItemInFridge>> GetItemsAsync(bool forceRefresh = false)
		{
			return await Task.FromResult(items);
		}

		public async Task AddItemAsync(Fridge.Model.ItemInFridge newItemInFridge)
		{
			items.Add(newItemInFridge);
			await Task.CompletedTask;
		}

		public async Task<Fridge.Model.ItemInFridge> GetItemAsync(Guid itemId)
		{
			return await Task.FromResult(items.First(item => item.ItemId == itemId));
		}

		public async Task UpdateItemAsync(Fridge.Model.ItemInFridge modifiedItem)
		{
			int index = items.FindIndex((item) => item.ItemId == modifiedItem.ItemId);
			items.RemoveAt(index);
			items.Insert(index, modifiedItem);
			await Task.CompletedTask;
		}

		public async Task DeleteAsync(Guid itemInFridgeId)
		{
			if (itemInFridgeId == Guid.Empty)
			{
				return;
			}

			int index = items.FindIndex((item) => item.ItemId == itemInFridgeId);
			if (index < -1)
			{
				throw new Exception($"Item id = '{itemInFridgeId}' doesn't exist");
			}

			items.RemoveAt(index);

			await Task.CompletedTask;
		}

		public async Task<Fridge.Model.User> GetUserAsync()
		{
			return await Task.FromResult(user);
		}

		public static Fridge.Model.User GetDefaultUser()
		{
			var newUser = new Fridge.Model.User()
			{
				UserId = User1Id,
				Name = User1Name,
				Email = User1Email,
			};

			var myFridge = new Fridge.Model.FridgeForUser()
			{
				FridgeId = Fridge1Id,
				MyPermission = Fridge.Model.UserPermissionTypes.Owner,
				OwnerId = User1Id
			};

			newUser.MyFridges.Add(myFridge);
			return newUser;
		}

		public async Task CreateUserAsync(Fridge.Model.User newUser)
		{
			this.user = newUser;
			await Task.CompletedTask;
		}
	}
}
