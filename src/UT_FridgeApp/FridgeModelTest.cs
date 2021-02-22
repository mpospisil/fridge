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
			var firdge = fridges[0];

			var deepCopy = firdge.DeepCopy();
			Assert.IsTrue(firdge.FridgeId == deepCopy.FridgeId);
			Assert.IsTrue(firdge.RemovedItemsIdentifier == deepCopy.RemovedItemsIdentifier);
			Assert.IsTrue(firdge.Name == deepCopy.Name);
			Assert.IsTrue(firdge.TimeStamp == deepCopy.TimeStamp);
			Assert.IsTrue(firdge.Partitions.Count == deepCopy.Partitions.Count);

			Assert.IsTrue(firdge.Partitions[0].PartitionId == deepCopy.Partitions[0].PartitionId);
			Assert.IsTrue(firdge.Partitions[0].Name == deepCopy.Partitions[0].Name);
			Assert.IsTrue(firdge.Partitions[0].TimeStamp == deepCopy.Partitions[0].TimeStamp);
		}
	}
}
