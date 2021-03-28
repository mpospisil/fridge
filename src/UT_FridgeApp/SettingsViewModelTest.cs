using FridgeApp.Services;
using FridgeApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace UT_FridgeApp
{
	[TestClass]
	public class SettingsViewModelTest
	{

		[TestMethod]
		public void InitViewModelTest()
		{
			// create mock
			var fridgeDal = Substitute.For<IFridgeDAL>();
			List<Fridge.Model.Fridge> fridges = MockFridgeDAL.CreateMockFridges();
			fridgeDal.GetFridgesAsync(true).Returns(TestTools.ToTask<IEnumerable<Fridge.Model.Fridge>>(fridges.AsEnumerable()));
			var fridgeLogger = Substitute.For<IFridgeLogger>();

			// tested view model
			var settingsViewModel = new SettingsViewModel(fridgeDal, fridgeLogger);

			Assert.IsFalse(settingsViewModel.IsBusy, "Initially IsBusy == false");
			Assert.IsTrue(settingsViewModel.Fridges.Count == 0, "InitiallyExpecting no fridge");

			settingsViewModel.OnAppearing();

			Assert.IsTrue(settingsViewModel.IsBusy, "Expecting IsBusy == true");

			settingsViewModel.LoadFridgesCommand.Execute(null);

			Assert.IsFalse(settingsViewModel.IsBusy, "IsBusy should equal to 'false'");
			Assert.IsTrue(settingsViewModel.Fridges.Count == 1, "Expecting 1 fridge");

			// get the middle sector
			var fridgeVM = settingsViewModel.Fridges[0];
			Assert.IsTrue(fridgeVM.Name.Equals(MockFridgeDAL.Fridge1Name), "Incorrect fridge name");
			Assert.IsTrue(fridgeVM.FridgeId.Equals(MockFridgeDAL.Fridge1Id.ToString()), "Incorrect fridge ID");
		}
	}
}
