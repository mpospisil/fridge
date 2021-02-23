﻿using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace UT_FridgeApp
{
	[TestClass]
	public class ItemsViewModelTest
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
			var itemsViewModel = new ItemsViewModel(fridgeDal);

			Assert.IsFalse(itemsViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(itemsViewModel.Items.Count == 0, "InitiallyExpecting no items");

			itemsViewModel.OnAppearing();

			Assert.IsTrue(itemsViewModel.IsBusy, "Expecting IsBusy == true");

			itemsViewModel.LoadItemsCommand.Execute(null);

			Assert.IsFalse(itemsViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(itemsViewModel.Items.Count == 4, "Expecting 4 items");

			{
				// the first item in the fridge 1 , partition 1
				var itemInFridge = itemsViewModel.Items[0];
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
				var itemInFridge = itemsViewModel.Items[2];
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
				var itemInFridge = itemsViewModel.Items[3];
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
	}
}