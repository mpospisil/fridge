using Fridge.Model;
using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace UT_FridgeApp
{
	[TestClass]
	public class FridgeViewModelTest
	{
		[TestMethod]
		public void InitViewModelTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var originalFrigeData = fridges[0];

			var fridgeVM = new FridgeViewModel(fridgeDal, originalFrigeData);

			var fridgeFromVM = fridgeVM.FridgeFromVM();

			var jsonOriginalFridge = originalFrigeData.ToJson();
			var jsonFridgeFromVM = fridgeFromVM.ToJson();
			Assert.AreEqual(jsonOriginalFridge, jsonFridgeFromVM);
		}

		[TestMethod]
		public void AddPartitionTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var originalFrigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeDal, originalFrigeData);

			Assert.AreEqual(fridgeVM.Partitions.Count, 3);

			fridgeVM.AddPartitionCommand.Execute(null);

			Assert.AreEqual(fridgeVM.Partitions.Count, 4);
		}

		[TestMethod]
		public void DeletePartitionTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var frigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeDal, frigeData);

			Assert.AreEqual(fridgeVM.Partitions.Count, 3);

			// delete the first partition
			fridgeVM.DeletePartitionCommand.Execute(fridgeVM.Partitions[0]);

			Assert.AreEqual(fridgeVM.Partitions.Count, 2);
			Assert.AreEqual(fridgeVM.Partitions[0].Name, MockFridgeDAL.Partition2Name);
		}

		[TestMethod]
		public void ItemsInFridgeTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			var frigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeDal, frigeData);

			Assert.IsTrue(fridgeVM.Name == MockFridgeDAL.Fridge1Name, $"Invalid name of the fridge '{fridgeVM.Name}' !='{MockFridgeDAL.Fridge1Name}'");

			Assert.IsTrue(fridgeVM.Partitions.Count == 3, "Expecting 3 partitions");

			var firstPartition = fridgeVM.Partitions[0];
			Assert.IsTrue(firstPartition.Name == MockFridgeDAL.Partition1Name, "Invalid name of the first partition");
			Assert.IsTrue(firstPartition.PartitionId == MockFridgeDAL.Partition1Id, "Invalid id of the first partition");
			Assert.IsTrue(firstPartition.Items.Count == 3, "Expecting 3 items in the first partition");

			var firstItemInPart1 = firstPartition.Items[0];
			Assert.IsTrue(firstItemInPart1.ItemId == MockFridgeDAL.Fr1Part1Item1Id.ToString(), "Invalid id of the item");
			Assert.IsTrue(firstItemInPart1.Name == MockFridgeDAL.Fr1Part1Item1Name, "Invalid name of the item");
			Assert.IsTrue(firstItemInPart1.IsInFridge == true, "The item should be in the fridge");
			Assert.IsTrue(firstItemInPart1.PartitionId == MockFridgeDAL.Partition1Id.ToString(), "Incorrect partitionId");
			Assert.IsTrue(firstItemInPart1.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Incorrect partitionId");

			var secondPartition = fridgeVM.Partitions[1];
			Assert.IsTrue(secondPartition.Name == MockFridgeDAL.Partition2Name, "Invalid name of the second partition");
			Assert.IsTrue(secondPartition.PartitionId == MockFridgeDAL.Partition2Id, "Invalid id of the second partition");
			Assert.IsTrue(secondPartition.Items.Count == 1, "Expecting 1 items in the second partition");

			var thirdPartition = fridgeVM.Partitions[2];
			Assert.IsTrue(thirdPartition.Name == MockFridgeDAL.Partition3Name, "Invalid name of the third partition");
			Assert.IsTrue(thirdPartition.PartitionId == MockFridgeDAL.Partition3Id, "Invalid id of the third partition");
			Assert.IsTrue(thirdPartition.Items.Count == 0, "Expecting no items in the third partition");
		}

			[TestMethod]
		public void CanAddItemTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var originalFrigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeDal, originalFrigeData);

			Assert.AreEqual(fridgeVM.Partitions.Count, 3);

			fridgeVM.SelectedPartition = null;

			var canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedPartition);
			Assert.IsFalse(canAddItemRes, "When no partitions is selected the user can not add item");
			
			// select the first partition
			fridgeVM.SelectedPartition = fridgeVM.Partitions[0];
			canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedPartition);
			Assert.IsTrue(canAddItemRes, "When a partitions is selected the user can add item");
		}
	}
}
