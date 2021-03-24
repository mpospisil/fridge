using Fridge.Model;
using Fridge.Repository;
using FridgeApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UT_Fridge.Repository
{
	[TestClass]
	public class RepositoryTest
	{
		static readonly Guid FirstUserId = Guid.Parse("58AEDAE6-11BB-41BB-9F9D-4576BE93915E");
		static readonly string FirstUserName = "First User";
		static readonly string FirstUserEmail = "name@comapany.com";

		[TestMethod]
		public async Task CreateRepositoryTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge.db");
			try
			{
				using(var fridgeDal = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{
					{
						var firstUser = await fridgeDal.GetUserAsync();
						Assert.IsNull(firstUser);
					}

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
	}
}
