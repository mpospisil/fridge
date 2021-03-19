using Fridge.Model;
using FridgeApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UT_FridgeApp
{
	[TestClass]
	public class FridgeModelTest
	{
		[TestMethod]
		public void CopyFridgeTest()
		{
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			var fridge = fridges[0];

			var deepCopy = fridge.DeepCopy();
			Assert.IsTrue(fridge.FridgeId == deepCopy.FridgeId);
			Assert.IsTrue(fridge.OwnerId == deepCopy.OwnerId);
			Assert.IsTrue(fridge.RemovedItemsIdentifier == deepCopy.RemovedItemsIdentifier);
			Assert.IsTrue(fridge.Name == deepCopy.Name);
			Assert.IsTrue(fridge.TimeStamp == deepCopy.TimeStamp);
			Assert.IsTrue(fridge.Partitions.Count == deepCopy.Partitions.Count);

			Assert.IsTrue(fridge.Partitions[0].PartitionId == deepCopy.Partitions[0].PartitionId);
			Assert.IsTrue(fridge.Partitions[0].Name == deepCopy.Partitions[0].Name);
			Assert.IsTrue(fridge.Partitions[0].TimeStamp == deepCopy.Partitions[0].TimeStamp);
		}
	}
}
