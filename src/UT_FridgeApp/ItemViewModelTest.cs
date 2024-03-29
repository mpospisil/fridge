﻿using Fridge.Model;
using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
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
			var fridgeLogger = Substitute.For<IFridgeLogger>();
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
			Assert.AreEqual(firstFridgeData.Sectors.Count, 3);
			Assert.IsTrue(firstFridgeData.FridgeId == MockFridgeDAL.Fridge1Id);

			var firstSector = firstFridgeData.Sectors[0];
			Assert.IsTrue(firstSector.SectorId == MockFridgeDAL.Sector1Id);

			var newItem = new ItemInFridge();
			var newItemVM = new ItemViewModel(fridgeLogger ,fridgeDal);
			newItemVM.FridgeId = firstFridgeData.FridgeId.ToString();
			newItemVM.SectorId = firstSector.SectorId.ToString();
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
			Assert.IsTrue(addedItem.SectorId == MockFridgeDAL.Sector1Id);
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
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			var troutItem = itemsInFridge[1];
			Assert.IsTrue(troutItem.ItemId == MockFridgeDAL.Fr1Part1Item2Id);
			fridgeDal.GetItemAsync(MockFridgeDAL.Fr1Part1Item2Id).Returns(troutItem);

			var itemVM = new ItemViewModel(fridgeLogger, fridgeDal);

			// it will initialize view model
			itemVM.ItemFromRepositoryId = troutItem.ItemId.ToString(); // id of the Trout

			Assert.IsTrue(itemVM.ItemId.Equals(troutItem.ItemId.ToString()));
			Assert.IsTrue(itemVM.FridgeId.Equals(troutItem.FridgeId.ToString()));
			Assert.IsTrue(itemVM.SectorId.Equals(troutItem.SectorId.ToString()));
			Assert.IsTrue(itemVM.Name == troutItem.Name);
			Assert.IsTrue(itemVM.IsInFridge == troutItem.IsInFridge);
			Assert.IsTrue(itemVM.TimeStamp == troutItem.TimeStamp);
		}

		[TestMethod]
		public async Task RemoveItemTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			Guid removedItemId = Guid.Empty;

			fridgeDal.When(x => x.RemoveItemAsync(Arg.Any<Guid>())).Do(param1 =>
			{
				removedItemId = param1.ArgAt<Guid>(0);
			});

			var firstFridge = fridges[0];

			var troutItem = itemsInFridge[1];
			Assert.IsTrue(troutItem.ItemId == MockFridgeDAL.Fr1Part1Item2Id);
			fridgeDal.GetItemAsync(MockFridgeDAL.Fr1Part1Item2Id).Returns(troutItem);
			Assert.IsTrue(troutItem.TimeStamp == MockFridgeDAL.Date2);
			Assert.IsTrue(troutItem.History.Count == 1);
			Assert.IsTrue(troutItem.IsInFridge);

			var itemVM = new ItemViewModel(fridgeLogger, fridgeDal);

			// it will initialize view model
			itemVM.ItemFromRepositoryId = troutItem.ItemId.ToString(); // id of the Trout

			Assert.IsTrue(removedItemId == Guid.Empty);

			await itemVM.RemoveItemFromFridge(troutItem.ItemId);

			Assert.IsTrue(removedItemId == troutItem.ItemId);
		}
	}
}

