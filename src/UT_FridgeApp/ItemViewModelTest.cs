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
	public class ItemViewModelTest
	{
		[TestMethod]
		public void AddItemTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));
			fridgeDal.GetItemsAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.ItemInFridge>>(new List<Fridge.Model.ItemInFridge>()));

			var firstFridgeData = fridges[0];
			Assert.AreEqual(firstFridgeData.Partitions.Count, 3);
			Assert.IsTrue(firstFridgeData.FridgeId == MockFridgeDAL.Fridge1Id);

			var firstPartition = firstFridgeData.Partitions[0];
			Assert.IsTrue(firstPartition.PartitionId == MockFridgeDAL.Partition1Id);

			var newItem = new ItemInFridge();
			var newItemVM = new ItemViewModel(fridgeDal);
			newItemVM.FridgeId = firstFridgeData.FridgeId.ToString();
			newItemVM.PartitionId = firstPartition.PartitionId.ToString();

			var canSaveItem = newItemVM.SaveCommand.CanExecute(null);
			Assert.IsFalse(canSaveItem, "Can not save item if the name is empty");

			newItemVM.Name = "Pork meat";
		}
	}
}
