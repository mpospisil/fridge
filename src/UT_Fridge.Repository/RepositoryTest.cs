using Fridge.Repository;
using FridgeApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace UT_Fridge.Repository
{
	[TestClass]
	public class RepositoryTest
	{
		[TestMethod]
		public void CreateRepositoryTest()
		{
			var fridgeLogger = Substitute.For<IFridgeLogger>();
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge.db");
			try
			{
				using(var repos = new RepositoryLiteDb(fridgeLogger, tempDbFileName))
				{

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
