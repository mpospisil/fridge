using Fridge.Model;
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
	public class FridgeViewModelTest
	{
		[TestMethod]
		public void InitViewModelTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));
			var fridgeLogger = Substitute.For<IFridgeLogger>();

			var originalFrigeData = fridges[0];

			var fridgeVM = new FridgeViewModel(fridgeLogger, fridgeDal, originalFrigeData);

			var fridgeFromVM = fridgeVM.FridgeFromVM();

			var jsonOriginalFridge = originalFrigeData.ToJson();
			var jsonFridgeFromVM = fridgeFromVM.ToJson();
			Assert.AreEqual(jsonOriginalFridge, jsonFridgeFromVM);
		}

		[TestMethod]
		public void AddSectorTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var originalFrigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeLogger, fridgeDal, originalFrigeData);

			Assert.AreEqual(fridgeVM.Sectors.Count, 3);

			fridgeVM.AddSectorCommand.Execute(null);

			Assert.AreEqual(fridgeVM.Sectors.Count, 4);
		}

		[TestMethod]
		public void DeleteSectorTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var frigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeLogger, fridgeDal, frigeData);

			Assert.AreEqual(fridgeVM.Sectors.Count, 3);

			// delete the first sector
			fridgeVM.DeleteSectorCommand.Execute(fridgeVM.Sectors[0]);

			Assert.AreEqual(fridgeVM.Sectors.Count, 2);
			Assert.AreEqual(fridgeVM.Sectors[0].Name, MockFridgeDAL.Sector2Name);
		}

		[TestMethod]
		public void ItemsInFridgeTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			var fridgeLogger = Substitute.For<IFridgeLogger>();

			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			List<Fridge.Model.ItemInFridge> itemsInFridge = MockFridgeDAL.CreateMockItems();
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));
			fridgeDal.GetItemsAsync(false).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(itemsInFridge.AsEnumerable()));

			var frigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeLogger, fridgeDal, frigeData);

			Assert.IsTrue(fridgeVM.Name == MockFridgeDAL.Fridge1Name, $"Invalid name of the fridge '{fridgeVM.Name}' !='{MockFridgeDAL.Fridge1Name}'");

			Assert.IsTrue(fridgeVM.Sectors.Count == 3, "Expecting 3 sectors");

			var firstSector = fridgeVM.Sectors[0];
			Assert.IsTrue(firstSector.Name == MockFridgeDAL.Sector1Name, "Invalid name of the first sector");
			Assert.IsTrue(firstSector.SectorId == MockFridgeDAL.Sector1Id, "Invalid id of the first sector");
			Assert.IsTrue(firstSector.Items.Count == 3, "Expecting 3 items in the first sector");

			var firstItemInPart1 = firstSector.Items[0];
			Assert.IsTrue(firstItemInPart1.ItemId == MockFridgeDAL.Fr1Part1Item1Id.ToString(), "Invalid id of the item");
			Assert.IsTrue(firstItemInPart1.Name == MockFridgeDAL.Fr1Part1Item1Name, "Invalid name of the item");
			Assert.IsTrue(firstItemInPart1.IsInFridge == true, "The item should be in the fridge");
			Assert.IsTrue(firstItemInPart1.SectorId == MockFridgeDAL.Sector1Id.ToString(), "Incorrect sectorId");
			Assert.IsTrue(firstItemInPart1.FridgeId == MockFridgeDAL.Fridge1Id.ToString(), "Incorrect sectorId");

			var secondSector = fridgeVM.Sectors[1];
			Assert.IsTrue(secondSector.Name == MockFridgeDAL.Sector2Name, "Invalid name of the second sector");
			Assert.IsTrue(secondSector.SectorId == MockFridgeDAL.Sector2Id, "Invalid id of the second sector");
			Assert.IsTrue(secondSector.Items.Count == 1, "Expecting 1 items in the second sector");

			var thirdSector = fridgeVM.Sectors[2];
			Assert.IsTrue(thirdSector.Name == MockFridgeDAL.Sector3Name, "Invalid name of the third sector");
			Assert.IsTrue(thirdSector.SectorId == MockFridgeDAL.Sector3Id, "Invalid id of the third sector");
			Assert.IsTrue(thirdSector.Items.Count == 0, "Expecting no items in the third sector");
		}

		[TestMethod]
		public void CanAddItemTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));

			var originalFrigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeLogger, fridgeDal, originalFrigeData);

			Assert.AreEqual(fridgeVM.Sectors.Count, 3);

			fridgeVM.SelectedSector = null;

			var canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedSector);
			Assert.IsFalse(canAddItemRes, "When no sectors is selected the user can not add item");

			// select the first sector
			fridgeVM.SelectedSector = fridgeVM.Sectors[0];
			canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedSector);
			Assert.IsTrue(canAddItemRes, "When a sectors is selected the user can add item");
		}

		[TestMethod]
		public async Task AddFridgeTest()
		{
			var defaultUser = MockFridgeDAL.GetDefaultUser();

			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));
			fridgeDal.GetUserAsync().Returns(defaultUser);

			Assert.IsTrue(fridges.Count == 1, "Expecting 1 fridge");

			fridgeDal.When(x => x.AddFridge(Arg.Any<Fridge.Model.Fridge>())).Do(param1 =>
			{
				fridges.Add(param1.ArgAt<Fridge.Model.Fridge>(0));
			});

			// tested view model
			FridgeViewModel fridgeViewModel = new FridgeViewModel(fridgeLogger, fridgeDal);
			fridgeViewModel.FridgeId = Guid.Empty.ToString();
			Assert.IsTrue(FridgeApp.Resources.NewFridge.Equals(fridgeViewModel.Name), $"The name of the new fridge should be '{FridgeApp.Resources.NewFridge}'");
			Assert.IsTrue(fridgeViewModel.Sectors.Count == 3, "Expecting 3 sectors");

			const string newFridgeName = "My new fridge";
			fridgeViewModel.Name = newFridgeName;
			fridgeViewModel.AddSectorCommand.Execute(null);

			await fridgeViewModel.SaveData();

			Assert.IsTrue(fridges.Count == 2, "Expecting 2 fridges");
			var testedFridge = fridges.Last();

			Assert.IsTrue(testedFridge.OwnerId != Guid.Empty, "Owner should be set");
			Assert.IsTrue(testedFridge.Name == newFridgeName, $"Invalid name of the user. Expected value is '{newFridgeName}'");

			Assert.IsTrue(testedFridge.Sectors.Count == 4, "Expecting 4 sector");
		}
	}
}
