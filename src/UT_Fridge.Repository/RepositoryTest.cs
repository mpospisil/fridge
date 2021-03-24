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
		static readonly string Fridge1NameChanged = "Updated Fridge 1";
		static readonly Guid Fridge2Id = Guid.Parse("22FADBC4-29F6-4830-A678-E439CB8B3345");
		static readonly string Fridge2Name = "Fridge 2";

		[TestMethod]
		public async Task CreateUserTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge-user-tst.db");

			if(File.Exists(tempDbFileName))
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

					var fridgesStep2= (await  fridgeDal.GetFridgesAsync(true)).ToList();
					Assert.IsTrue(fridgesStep2.Count == 1, "Expect one fridge");

					var fridgeStep2 = fridgesStep2[0];
					Assert.IsTrue(fridgeStep2.FridgeId == Fridge1Id);
					Assert.IsTrue(fridgeStep2.Name ==  Fridge1Name);

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

			await fridgeDal.AddFridgeAsync(fridge);

			var firstFridge = await fridgeDal.GetFridgeAsync(fridge.FridgeId);
			Assert.IsNotNull(firstFridge);
			Assert.IsTrue(firstFridge.FridgeId == Fridge2Id);
			Assert.IsTrue(firstFridge.Name == Fridge2Name);
		}
	}
}
