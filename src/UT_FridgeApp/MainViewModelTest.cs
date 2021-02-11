using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT_FridgeApp
{
	[TestClass]
	public class MainViewModelTest
	{
		[TestMethod]
		public void LoadItemsCommandTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.ItemInFridge> items = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(items.AsEnumerable()));

			// tested view model
			var mainViewModel = new MainViewModel(fridgeDal);

			Assert.IsFalse(mainViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(mainViewModel.Items.Count == 0, "InitiallyExpecting no items");

			mainViewModel.OnAppearing();

			Assert.IsTrue(mainViewModel.IsBusy, "Expecting IsBusy == true");

			mainViewModel.LoadItemsCommand.Execute(null);

			Assert.IsFalse(mainViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(mainViewModel.Items.Count == 4, "Expecting 4 items");
		}

		[TestMethod]
		public void LoadFridgesCommandTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			// tested view model
			var mainViewModel = new MainViewModel(fridgeDal);
			Assert.IsFalse(mainViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(mainViewModel.Fridges.Count == 0, "InitiallyExpecting no items");

			mainViewModel.LoadFridgesCommand.Execute(null);

			Assert.IsFalse(mainViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(mainViewModel.Fridges.Count == 1, "Expecting 1 fridge");

			IFridgeViewModel firstFridgeVM = mainViewModel.Fridges[0];

			Assert.IsTrue(firstFridgeVM.Name == MockFridgeDAL.Fridge1Name, $"Invalid name of the fridge '{firstFridgeVM.Name}' !='{MockFridgeDAL.Fridge1Name}'");

			Assert.IsTrue(firstFridgeVM.Partitions.Count == 3, "Expecting 3 partitions");

			var firstPartition = firstFridgeVM.Partitions[0];
			Assert.IsTrue(firstPartition.Name == MockFridgeDAL.Partition1Name, "Invalid name of the first partition");
			Assert.IsTrue(firstPartition.PartitionId == MockFridgeDAL.Partition1Id, "Invalid id of the first partition");
			Assert.IsTrue(firstPartition.Items.Count == 3, "Expecting 3 items in the first partition");

			var firstItemUnTheParition = firstPartition.Items[0];
			Assert.IsTrue(firstItemUnTheParition.ItemId == MockFridgeDAL.Fr1Part1Item1Id.ToString(), "Invalid id of the item");
			Assert.IsTrue(firstItemUnTheParition.Name == MockFridgeDAL.Fr1Part1Item1Name, "Invalid name of the item");
			Assert.IsTrue(firstItemUnTheParition.IsInFridge == true, "The item should be in the fridge");
			Assert.IsTrue(firstItemUnTheParition.PartitionId == MockFridgeDAL.Partition1Id.ToString(), "Incorrect partitionId");
			Assert.IsTrue(firstItemUnTheParition.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Incorrect partitionId");

			var secondPartition = firstFridgeVM.Partitions[1];
			Assert.IsTrue(secondPartition.Name == MockFridgeDAL.Partition2Name, "Invalid name of the second partition");
			Assert.IsTrue(secondPartition.PartitionId == MockFridgeDAL.Partition2Id, "Invalid id of the second partition");
			Assert.IsTrue(secondPartition.Items.Count == 1, "Expecting 1 items in the second partition");

			var thirdPartition = firstFridgeVM.Partitions[2];
			Assert.IsTrue(thirdPartition.Name == MockFridgeDAL.Partition3Name, "Invalid name of the third partition");
			Assert.IsTrue(thirdPartition.PartitionId == MockFridgeDAL.Partition3Id, "Invalid id of the third partition");
			Assert.IsTrue(thirdPartition.Items.Count == 0, "Expecting no items in the third partition");
		}
	}
}
