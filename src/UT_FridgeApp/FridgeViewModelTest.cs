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

			var originalFrigeData = fridges[0];
			var fridgeVM = new FridgeViewModel(fridgeDal, originalFrigeData);

			Assert.AreEqual(fridgeVM.Partitions.Count, 3);

			// delete the first partition
			fridgeVM.DeletePartitionCommand.Execute(fridgeVM.Partitions[0]);

			Assert.AreEqual(fridgeVM.Partitions.Count, 2);
			Assert.AreEqual(fridgeVM.Partitions[0].Name, MockFridgeDAL.Partition2Name);
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

			var canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedPartition);
			Assert.IsFalse(canAddItemRes, "When no partitions is selected the user can not add item");
			
			// select the first partition
			fridgeVM.SelectedPartition = fridgeVM.Partitions[0];
			canAddItemRes = fridgeVM.AddItemCommand.CanExecute(fridgeVM.SelectedPartition);
			Assert.IsTrue(canAddItemRes, "When a partitions is selected the user can add item");
		}
	}
}
