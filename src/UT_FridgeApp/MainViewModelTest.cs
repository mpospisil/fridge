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
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

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

			{
				// the first item in the fridge 1 , partition 1
				var itemInFridge = mainViewModel.Items[0];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.PartitionId == MockFridgeDAL.Partition1Id.ToString(), "Invalid Partition Id");
				Assert.IsTrue(itemInFridge.PartitionName == MockFridgeDAL.Partition1Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.PartitionIndex == 0, "Invalid index of the partition");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part1Item1Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part1Item1Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date1, "Invalid time of adding the item into the fridge");
			}

			{
				// the third item in the fridge 1, partition 1
				var itemInFridge = mainViewModel.Items[2];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.PartitionId == MockFridgeDAL.Partition1Id.ToString(), "Invalid Partition Id");
				Assert.IsTrue(itemInFridge.PartitionName == MockFridgeDAL.Partition1Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.PartitionIndex == 0, "Invalid index of the partition");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part1Item3Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part1Item3Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date1, "Invalid time of adding the item into the fridge");
			}

			{
				// the third item in the fridge 1, partition 2
				var itemInFridge = mainViewModel.Items[3];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.PartitionId == MockFridgeDAL.Partition2Id.ToString(), "Invalid Partition Id");
				Assert.IsTrue(itemInFridge.PartitionName == MockFridgeDAL.Partition2Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.PartitionIndex == 1, "Invalid index of the partition");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part2Item1Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part2Item1Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date3, "Invalid time of adding the item into the fridge");
			}
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

			// tested main view model
			var mainViewModel = new MainViewModel(fridgeDal);
			Assert.IsFalse(mainViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(mainViewModel.Fridges.Count == 0, "InitiallyExpecting no items");

			mainViewModel.LoadFridgesCommand.Execute(null);

			Assert.IsFalse(mainViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(mainViewModel.Fridges.Count == 1, "Expecting 1 fridge");
		}
	}
}
