﻿using FridgeApp.Services;
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
