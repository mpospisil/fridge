using Fridge.Model;
using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UT_FridgeApp
{
	[TestClass]
	public class ItemViewModelTest
	{
		[TestMethod]
		public async Task AddItemTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			List<Fridge.Model.ItemInFridge> allItems = new List<Fridge.Model.ItemInFridge>();
			ItemInFridge addedItem = null;

			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(allItems));

			fridgeDal.When(x => x.AddItemAsync(Arg.Any<ItemInFridge>())).Do(param1 =>
			{
				addedItem = param1.ArgAt<ItemInFridge>(0);
			});

			var firstFridgeData = fridges[0];
			Assert.AreEqual(firstFridgeData.Partitions.Count, 3);
			Assert.IsTrue(firstFridgeData.FridgeId == MockFridgeDAL.Fridge1Id);

			var firstPartition = firstFridgeData.Partitions[0];
			Assert.IsTrue(firstPartition.PartitionId == MockFridgeDAL.Partition1Id);

			var newItem = new ItemInFridge();
			var newItemVM = new ItemViewModel(fridgeDal);
			newItemVM.FridgeId = firstFridgeData.FridgeId.ToString();
			newItemVM.PartitionId = firstPartition.PartitionId.ToString();
			newItemVM.TimeStamp = MockFridgeDAL.Date1;

			var canSaveItem = newItemVM.SaveCommand.CanExecute(null);
			Assert.IsFalse(canSaveItem, "Can not save item if the name is empty");

			const string NewItemName = "Meatloaf";
			newItemVM.Name = NewItemName;

			canSaveItem = newItemVM.SaveCommand.CanExecute(null);
			Assert.IsTrue(canSaveItem, "Can save item if the name is not null");

			await newItemVM.SaveData();

			Assert.IsTrue(addedItem != null);
			Assert.IsTrue(addedItem.Name.Equals(NewItemName));
			Assert.IsTrue(addedItem.FridgeId == MockFridgeDAL.Fridge1Id);
			Assert.IsTrue(addedItem.PartitionId == MockFridgeDAL.Partition1Id);
			Assert.IsTrue(addedItem.TimeStamp == MockFridgeDAL.Date1);
			Assert.IsTrue(addedItem.IsInFridge == true);

			Assert.IsTrue(addedItem?.History.Count == 1);

			var history1 = addedItem.History[0];
			Assert.IsTrue(history1.TypeOfChange == ChangeTypes.Added);
			Assert.IsTrue(history1.TimeOfChange == MockFridgeDAL.Date1);
		}

		[TestMethod]
		public void InitItemFromRepositoryTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			var troutItem = itemsInFridge[1];
			Assert.IsTrue(troutItem.ItemId == MockFridgeDAL.Fr1Part1Item2Id);
			fridgeDal.GetItemAsync(MockFridgeDAL.Fr1Part1Item2Id).Returns(troutItem);

			var itemVM = new ItemViewModel(fridgeDal);

			// it will initialize view model
			itemVM.ItemFromRepositoryId = troutItem.ItemId.ToString(); // id of the Trout

			Assert.IsTrue(itemVM.ItemId.Equals(troutItem.ItemId.ToString()));
			Assert.IsTrue(itemVM.FridgeId.Equals(troutItem.FridgeId.ToString()));
			Assert.IsTrue(itemVM.PartitionId.Equals(troutItem.PartitionId.ToString()));
			Assert.IsTrue(itemVM.Name == troutItem.Name);
			Assert.IsTrue(itemVM.IsInFridge == troutItem.IsInFridge);
			Assert.IsTrue(itemVM.TimeStamp == troutItem.TimeStamp);
		}

		[TestMethod]
		public async Task RemoveItemTest()
		{
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			ItemInFridge removedItem = null;

			fridgeDal.When(x => x.UpdateItemAsync(Arg.Any<ItemInFridge>())).Do(param1 =>
			{
				removedItem = param1.ArgAt<ItemInFridge>(0);
			});

			var firstFridge = fridges[0];

			var troutItem = itemsInFridge[1];
			Assert.IsTrue(troutItem.ItemId == MockFridgeDAL.Fr1Part1Item2Id);
			fridgeDal.GetItemAsync(MockFridgeDAL.Fr1Part1Item2Id).Returns(troutItem);
			Assert.IsTrue(troutItem.TimeStamp == MockFridgeDAL.Date2);
			Assert.IsTrue(troutItem.History.Count == 1);
			Assert.IsTrue(troutItem.IsInFridge);

			var itemVM = new ItemViewModel(fridgeDal);

			// it will initialize view model
			itemVM.ItemFromRepositoryId = troutItem.ItemId.ToString(); // id of the Trout

			Assert.IsTrue(removedItem == null);
			await itemVM.RemoveItemFromFridge(troutItem.ItemId, firstFridge.RemovedItemsIdentifier);

			Assert.IsTrue(removedItem != null);
			Assert.IsTrue(removedItem.ItemId == MockFridgeDAL.Fr1Part1Item2Id);
			Assert.IsTrue(removedItem.Name == MockFridgeDAL.Fr1Part1Item2Name);
			Assert.IsTrue(removedItem.FridgeId == firstFridge.RemovedItemsIdentifier);
			Assert.IsTrue(removedItem.PartitionId == firstFridge.RemovedItemsIdentifier);
			Assert.IsFalse(removedItem.IsInFridge);
			Assert.IsFalse(removedItem.TimeStamp == MockFridgeDAL.Date2);

			Assert.IsTrue(troutItem.History.Count == 2);
			var histItemRemove = troutItem.History[1];
			Assert.IsTrue(histItemRemove.TypeOfChange == ChangeTypes.Removed);
			Assert.IsTrue(histItemRemove.TimeOfChange == removedItem.TimeStamp);
		}
	}
}

