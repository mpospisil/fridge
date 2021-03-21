using Fridge.Model;
using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UT_FridgeApp
{
	[TestClass]
	public class ItemsViewModelTest
	{
		[TestMethod]
		public async Task LoadItemsCommandTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> items = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(items.AsEnumerable()));

			// tested view model
			var itemsViewModel = new ItemsViewModel(fridgeDal);
			itemsViewModel.SortMethod = Fridge.Model.ItemsOrder.NotSorted;

			Assert.IsFalse(itemsViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(itemsViewModel.Items.Count == 0, "InitiallyExpecting no items");

			itemsViewModel.OnAppearing();

			Assert.IsTrue(itemsViewModel.IsBusy, "Expecting IsBusy == true");

			await itemsViewModel.ExecuteLoadItemsCommand();

			Assert.IsFalse(itemsViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(itemsViewModel.Items.Count == 4, "Expecting 4 items");

			{
				// the first item in the fridge 1 , sector 1
				var itemInFridge = itemsViewModel.Items[0];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.SectorId == MockFridgeDAL.Sector1Id.ToString(), "Invalid Sector Id");
				Assert.IsTrue(itemInFridge.SectorName == MockFridgeDAL.Sector1Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.SectorIndex == 0, "Invalid index of the sector");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part1Item1Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part1Item1Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date1, "Invalid time of adding the item into the fridge");
			}

			{
				// the third item in the fridge 1, sector 1
				var itemInFridge = itemsViewModel.Items[2];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.SectorId == MockFridgeDAL.Sector1Id.ToString(), "Invalid Sector Id");
				Assert.IsTrue(itemInFridge.SectorName == MockFridgeDAL.Sector1Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.SectorIndex == 0, "Invalid index of the sector");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part1Item3Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part1Item3Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date1, "Invalid time of adding the item into the fridge");
			}

			{
				// the third item in the fridge 1, sector 2
				var itemInFridge = itemsViewModel.Items[3];
				Assert.IsTrue(itemInFridge.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Invalid Id of the fridge");
				Assert.IsTrue(itemInFridge.FridgeName == MockFridgeDAL.Fridge1Name, "Invalid name of the fridge");
				Assert.IsTrue(itemInFridge.FridgeIndex == 0, "Invalid index of fridge");

				Assert.IsTrue(itemInFridge.SectorId == MockFridgeDAL.Sector2Id.ToString(), "Invalid Sector Id");
				Assert.IsTrue(itemInFridge.SectorName == MockFridgeDAL.Sector2Name, "Invalid Fridge Id");
				Assert.IsTrue(itemInFridge.SectorIndex == 1, "Invalid index of the sector");

				Assert.IsTrue(itemInFridge.Name == MockFridgeDAL.Fr1Part2Item1Name, "Invalid name of the item");
				Assert.IsTrue(itemInFridge.ItemId == MockFridgeDAL.Fr1Part2Item1Id.ToString(), "Invalid id of the item");
				Assert.IsTrue(itemInFridge.AddToFridgeTime == MockFridgeDAL.Date3, "Invalid time of adding the item into the fridge");
			}
		}

		[TestMethod]
		[Timeout(2000)]
		public void FilterItemsTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> items = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(items.AsEnumerable()));

			using (ManualResetEvent mre = new ManualResetEvent(false))
			{
				EventHandler handler = (s, e) =>
				{
					mre.Set();
				};

				// tested view model
				var itemsViewModel = new ItemsViewModel(fridgeDal);


				itemsViewModel.ItemFilterEvent += handler;
				itemsViewModel.SortMethod = Fridge.Model.ItemsOrder.NotSorted;

				itemsViewModel.LoadItemsCommand.Execute(null);

				Assert.IsTrue(itemsViewModel.Items.Count == 4, "Expecting 4 items");

				foreach (var item in itemsViewModel.Items)
				{
					Assert.IsFalse(item.IsVisible, $"Item '{item.Name}' should NOT be visible");
				}

				itemsViewModel.OnAppearing();
				mre.WaitOne();

				foreach (var item in itemsViewModel.Items)
				{
					Assert.IsTrue(item.IsVisible, $"Item '{item.Name}' should be visible");
				}

				{
					mre.Reset();
					itemsViewModel.Query = "Go";  // try to find goulash
					mre.WaitOne();

					var item0 = itemsViewModel.Items[0];
					Assert.IsFalse(item0.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item0.Name == MockFridgeDAL.Fr1Part1Item1Name);

					var item2 = itemsViewModel.Items[2];
					Assert.IsTrue(item2.IsVisible, $"Item '{item0.Name}' should be visible");
					Assert.IsTrue(item2.Name == MockFridgeDAL.Fr1Part1Item3Name);
				}

				{
					mre.Reset();
					itemsViewModel.Query = "go";  // try to find goulash
					mre.WaitOne();

					var item0 = itemsViewModel.Items[0];
					Assert.IsFalse(item0.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item0.Name == MockFridgeDAL.Fr1Part1Item1Name);

					var item2 = itemsViewModel.Items[2];
					Assert.IsTrue(item2.IsVisible, $"Item '{item0.Name}' should be visible");
					Assert.IsTrue(item2.Name == MockFridgeDAL.Fr1Part1Item3Name);
				}

				{
					mre.Reset();
					itemsViewModel.Query = "t";  // try to find 'Trout' and 'Pork meat'
					mre.WaitOne();

					var item0 = itemsViewModel.Items[0];
					Assert.IsTrue(item0.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item0.Name == MockFridgeDAL.Fr1Part1Item1Name);

					var item1 = itemsViewModel.Items[1];
					Assert.IsTrue(item1.IsVisible, $"Item '{item0.Name}' should be visible");
					Assert.IsTrue(item1.Name == MockFridgeDAL.Fr1Part1Item2Name);

					var item2 = itemsViewModel.Items[2];
					Assert.IsFalse(item2.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item2.Name == MockFridgeDAL.Fr1Part1Item3Name);
				}

				{
					mre.Reset();
					itemsViewModel.Query = "T";  // try to find 'Trout' and 'Pork meat'
					mre.WaitOne();

					var item0 = itemsViewModel.Items[0];
					Assert.IsTrue(item0.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item0.Name == MockFridgeDAL.Fr1Part1Item1Name);

					var item1 = itemsViewModel.Items[1];
					Assert.IsTrue(item1.IsVisible, $"Item '{item0.Name}' should be visible");
					Assert.IsTrue(item1.Name == MockFridgeDAL.Fr1Part1Item2Name);

					var item2 = itemsViewModel.Items[2];
					Assert.IsFalse(item2.IsVisible, $"Item '{item0.Name}' should NOT be visible");
					Assert.IsTrue(item2.Name == MockFridgeDAL.Fr1Part1Item3Name);
				}

				itemsViewModel.ItemFilterEvent -= handler;
			}
		}

		[TestMethod]
		[Timeout(2000)]
		public async Task SortItemsTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> items = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(items.AsEnumerable()));

			using (ManualResetEvent mre = new ManualResetEvent(false))
			{
				EventHandler handler = (s, e) =>
				{
					mre.Set();
				};

				// tested view model
				var itemsViewModel = new ItemsViewModel(fridgeDal);

				itemsViewModel.SortMethod = Fridge.Model.ItemsOrder.NotSorted;

				Assert.IsFalse(itemsViewModel.IsBusy, "Initially IsBusy == false");
				Assert.IsTrue(itemsViewModel.Items.Count == 0, "InitiallyExpecting no items");

				itemsViewModel.OnAppearing();

				Assert.IsTrue(itemsViewModel.IsBusy, "Expecting IsBusy == true");

				await itemsViewModel.ExecuteLoadItemsCommand();

				{
					// unsorted items
					var item = itemsViewModel.Items[0];
					Assert.IsTrue(item.Name == MockFridgeDAL.Fr1Part1Item1Name, $"The first item should be '{MockFridgeDAL.Fr1Part1Item1Name}'");
				}

				await itemsViewModel.SetSortedItems(ItemsOrder.ByFridge, itemsViewModel.Items);

				{
					// sorted be fridge
					var item = itemsViewModel.Items[0];
					Assert.IsTrue(item.Name == MockFridgeDAL.Fr1Part1Item3Name, $"The first item should be '{MockFridgeDAL.Fr1Part1Item3Name}'");
				}

				await itemsViewModel.SetSortedItems(ItemsOrder.ByDate, itemsViewModel.Items);

				{
					// sorted be fridge
					var item = itemsViewModel.Items[0];
					Assert.IsTrue(item.Name == MockFridgeDAL.Fr1Part1Item2Name, $"The first item should be '{MockFridgeDAL.Fr1Part1Item2Name}'");
				}

				await itemsViewModel.SetSortedItems(ItemsOrder.ByName, itemsViewModel.Items);

				{
					// sorted be fridge
					var item = itemsViewModel.Items[0];
					Assert.IsTrue(item.Name == MockFridgeDAL.Fr1Part2Item1Name, $"The first item should be '{MockFridgeDAL.Fr1Part2Item1Name}'");
				}

				itemsViewModel.ItemFilterEvent -= handler;
			}
		}
	}
}
