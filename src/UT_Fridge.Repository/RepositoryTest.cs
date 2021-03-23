using Fridge.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UT_Fridge.Repository
{
	[TestClass]
	public class RepositoryTest
	{
		[TestMethod]
		public void CreateRepositoryTest()
		{
			var tmpdir = System.IO.Path.GetTempPath();
			var tempDbFileName = Path.Combine(tmpdir, "fridge.db");
			try
			{
				using(var repos = new RepositoryLiteDb(tempDbFileName))
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
