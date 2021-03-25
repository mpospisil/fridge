using Fridge.Model;
using Fridge.Repository;
using FridgeApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace UT_Fridge.Repository
{
	[TestClass]
	public class RepositoryTest
	{
		static readonly Guid FirstUserId = Guid.Parse("58AEDAE6-11BB-41BB-9F9D-4576BE93915E");
		static readonly string FirstUserName = "First User";
		static readonly string FirstUserEmail = "name@comapany.com";

		static readonly Guid Fridge1Id = Guid.Parse("281A6F64-3C4D-42C7-99D1-588DE7B4D7E2");
		static readonly string Fridge1Name = "Fridge 1";
		static readonly Guid Fridge1Sector1Id = Guid.Parse("0FBD7E4C-0F74-4891-B663-CC7F52DA260A");
		static readonly string Fridge1Sector1Name = "Fridge 1-1";

		static readonly string Fridge1NameChanged = "Updated Fridge 1";

		static readonly Guid Fridge2Id = Guid.Parse("22FADBC4-29F6-4830-A678-E439CB8B3345");
		static readonly string Fridge2Name = "Fridge 2";
		static readonly Guid Fridge2Sector1Id = Guid.Parse("A7038CD7-58FD-4233-B86B-50A0FA557614");
		static readonly string Fridge2Sector1Name = "Fridge 2-1";

		static readonly Guid Item1Id = Guid.Parse("824FAA96-128A-4841-9AE2-2E21E5E72810");
		static readonly string Item1Name = "Bread";

		static readonly string Item1NameChanged = "Wheat bread";

		static readonly Guid Item2Id = Guid.Parse("C64F0A24-4BC2-4390-B00E-CF3D70E65DDA");
		static readonly string Item2Name = "Meat";

		[TestMethod]
		public async Task UserTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge-user-tst.db");

			if (File.Exists(tempDbFileName))
			{
				File.Delete(tempDbFileName);
			}

			try
			{
				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					{
						var firstUser = await fridgeDal.GetUserAsync();
						Assert.IsNull(firstUser);
					}

					{
						await CreateFirstUser(fridgeDal);

						var firstUser = await fridgeDal.GetUserAsync();
						Assert.IsNotNull(firstUser);
						Assert.IsTrue(firstUser.UserId == FirstUserId);
						Assert.IsTrue(firstUser.Name == FirstUserName);
						Assert.IsTrue(firstUser.Email == FirstUserEmail);
					}
				}

				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					{
						var firstUser = await fridgeDal.GetUserAsync();
						Assert.IsNotNull(firstUser);
						Assert.IsTrue(firstUser.UserId == FirstUserId);
						Assert.IsTrue(firstUser.Name == FirstUserName);
						Assert.IsTrue(firstUser.Email == FirstUserEmail);
					}
				}
			}
			finally
			{
				try
				{
					if (File.Exists(tempDbFileName))
					{
						File.Delete(tempDbFileName);
					}
				}
				catch { }
			}
		}

		[TestMethod]
		public async Task FridgeTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge-fridge-tst.db");

			if (File.Exists(tempDbFileName))
			{
				File.Delete(tempDbFileName);
			}

			try
			{
				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					// test - no fridge in the empty database
					await CreateFirstUser(fridgeDal);


					var fridgesStep1 = (await fridgeDal.GetFridgesAsync(false)).ToList();
					Assert.IsTrue(fridgesStep1.Any() == false, "Expect no fridge");

					// add fridge 1
					await CreateFridge1(fridgeDal);

					var fridgesStep2 = (await fridgeDal.GetFridgesAsync(true)).ToList();
					Assert.IsTrue(fridgesStep2.Count == 1, "Expect one fridge");

					var fridgeStep2 = fridgesStep2[0];
					Assert.IsTrue(fridgeStep2.FridgeId == Fridge1Id);
					Assert.IsTrue(fridgeStep2.Name == Fridge1Name);

					// add the second fridge
					await CreateFridge2(fridgeDal);

					var fridgesStep3 = (await fridgeDal.GetFridgesAsync(true)).ToList();
					Assert.IsTrue(fridgesStep3.Count == 2, "Expect two fridge");

					var fridgeStep3 = fridgesStep3.Where(f => f.FridgeId == Fridge2Id).FirstOrDefault();
					Assert.IsNotNull(fridgeStep3);
					Assert.IsTrue(fridgeStep3.FridgeId == Fridge2Id);
					Assert.IsTrue(fridgeStep3.Name == Fridge2Name);

					// rename the first fridge
					fridgeStep2.Name = Fridge1NameChanged;
					await fridgeDal.UpdateFridgeAsync(fridgeStep2);

					// get fridge by id
					var updatedFridge = await fridgeDal.GetFridgeAsync(Fridge1Id);
					Assert.IsNotNull(updatedFridge);
					Assert.IsTrue(updatedFridge.FridgeId == Fridge1Id);
					Assert.IsTrue(updatedFridge.Name == Fridge1NameChanged);

					// try to delete fridge 1
					await fridgeDal.DeleteFridgeAsync(Fridge1Id);

					var fridgesStep4 = (await fridgeDal.GetFridgesAsync(true)).ToList();
					Assert.IsTrue(fridgesStep4.Count == 1, "Expect one fridge");

					var fridgeStep4 = fridgesStep4[0];
					Assert.IsTrue(fridgeStep4.FridgeId == Fridge2Id);
					Assert.IsTrue(fridgeStep4.Name == Fridge2Name);
				}

				// try to reopen the database
				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					var fridgesStep4 = (await fridgeDal.GetFridgesAsync(true)).ToList();
					Assert.IsTrue(fridgesStep4.Count == 1, "Expect one fridge");

					var fridgeStep4 = fridgesStep4[0];
					Assert.IsTrue(fridgeStep4.FridgeId == Fridge2Id);
					Assert.IsTrue(fridgeStep4.Name == Fridge2Name);
				}
			}
			finally
			{
				try
				{
					if (File.Exists(tempDbFileName))
					{
						File.Delete(tempDbFileName);
					}
				}
				catch { }
			}
		}

		[TestMethod]
		public async Task FridgeItemTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge-items-tst.db");

			if (File.Exists(tempDbFileName))
			{
				File.Delete(tempDbFileName);
			}

			try
			{
				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					// test - no fridge in the empty database
					await CreateFirstUser(fridgeDal);

					// add fridge 1
					await CreateFridge1(fridgeDal);

					// add the second fridge
					await CreateFridge2(fridgeDal);

					var itemsStep1 = (await fridgeDal.GetItemsAsync(true)).ToList();
					Assert.IsTrue(itemsStep1.Count == 0, "Expect no item");

					var itemInFridge1 = new ItemInFridge()
					{
						ItemId = Item1Id,
						Name = Item1Name,
						FridgeId = Fridge1Id,
						SectorId = Fridge1Sector1Id,
					};

					await fridgeDal.AddItemAsync(itemInFridge1);

					var itemsStep2 = (await fridgeDal.GetItemsAsync(true)).ToList();
					Assert.IsTrue(itemsStep2.Count == 1, "Expect one item");

					var itemInFridge2 = new ItemInFridge()
					{
						ItemId = Item2Id,
						Name = Item2Name,
						FridgeId = Fridge1Id,
						SectorId = Fridge1Sector1Id,
					};

					await fridgeDal.AddItemAsync(itemInFridge2);
					var itemsStep3 = (await fridgeDal.GetItemsAsync(true)).ToList();
					Assert.IsTrue(itemsStep3.Count == 2, "Expect two items");

					var itemFromFridge = await fridgeDal.GetItemAsync(Item2Id);
					Assert.IsNotNull(itemFromFridge);
					Assert.IsTrue(itemFromFridge.ItemId == Item2Id);
					Assert.IsTrue(itemFromFridge.Name == Item2Name);

					// rename the first fridge
					var itemToUpdate = await fridgeDal.GetItemAsync(Item1Id);
					itemToUpdate.Name = Item1NameChanged;

					await fridgeDal.UpdateItemAsync(itemToUpdate);

					// get fridge by id
					var updatedItem = await fridgeDal.GetItemAsync(Item1Id);
					Assert.IsNotNull(updatedItem);
					Assert.IsTrue(updatedItem.ItemId == Item1Id);
					Assert.IsTrue(updatedItem.Name == Item1NameChanged);
				}

				using (var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					// get fridge by id
					var updatedItem = await fridgeDal.GetItemAsync(Item1Id);
					Assert.IsNotNull(updatedItem);
					Assert.IsTrue(updatedItem.ItemId == Item1Id);
					Assert.IsTrue(updatedItem.Name == Item1NameChanged);

					// delete the first fridge
					await fridgeDal.DeleteItemAsync(Item1Id);

					var itemsStep2 = (await fridgeDal.GetItemsAsync(true)).ToList();
					Assert.IsTrue(itemsStep2.Count == 1, "Expect one item");

					//
					var itemFromFridge = await fridgeDal.GetItemAsync(Item2Id);
					Assert.IsNotNull(itemFromFridge);
					Assert.IsTrue(itemFromFridge.ItemId == Item2Id);
					Assert.IsTrue(itemFromFridge.Name == Item2Name);
				}
			}
			finally
			{
				try
				{
					if (File.Exists(tempDbFileName))
					{
						File.Delete(tempDbFileName);
					}
				}
				catch { }
			}
		}


		private static async Task CreateFirstUser(RepositoryLiteDb fridgeDal)
		{
			User user1 = new User()
			{
				UserId = FirstUserId,
				Name = FirstUserName,
				Email = FirstUserEmail
			};

			await fridgeDal.CreateUserAsync(user1);
			var firstUser = await fridgeDal.GetUserAsync();
			Assert.IsNotNull(firstUser);
			Assert.IsTrue(firstUser.UserId == FirstUserId);
			Assert.IsTrue(firstUser.Name == FirstUserName);
			Assert.IsTrue(firstUser.Email == FirstUserEmail);
		}

		private static async Task CreateFridge1(RepositoryLiteDb fridgeDal)
		{
			Fridge.Model.Fridge fridge = new Fridge.Model.Fridge()
			{
				FridgeId = Fridge1Id,
				Name = Fridge1Name,
				OwnerId = FirstUserId
			};

			fridge.Sectors.Add(new Sector() { SectorId = Fridge1Sector1Id, Name = Fridge1Sector1Name });

			await fridgeDal.AddFridgeAsync(fridge);

			var firstFridge = await fridgeDal.GetFridgeAsync(fridge.FridgeId);
			Assert.IsNotNull(firstFridge);
			Assert.IsTrue(firstFridge.FridgeId == Fridge1Id);
			Assert.IsTrue(firstFridge.Name == Fridge1Name);
		}

		private static async Task CreateFridge2(RepositoryLiteDb fridgeDal)
		{
			Fridge.Model.Fridge fridge = new Fridge.Model.Fridge()
			{
				FridgeId = Fridge2Id,
				Name = Fridge2Name,
				OwnerId = FirstUserId
			};

			fridge.Sectors.Add(new Sector() { SectorId = Fridge2Sector1Id, Name = Fridge2Sector1Name });

			await fridgeDal.AddFridgeAsync(fridge);

			var firstFridge = await fridgeDal.GetFridgeAsync(fridge.FridgeId);
			Assert.IsNotNull(firstFridge);
			Assert.IsTrue(firstFridge.FridgeId == Fridge2Id);
			Assert.IsTrue(firstFridge.Name == Fridge2Name);
		}
	}
}
