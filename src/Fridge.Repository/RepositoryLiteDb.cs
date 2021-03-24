using FridgeApp.Services;
using LiteDB;
using System;

namespace Fridge.Repository
{
	public class RepositoryLiteDb : IDisposable
	{
		private bool disposedValue;
		private readonly LiteDatabase Db;
		private readonly IFridgeLogger Logger;

		public RepositoryLiteDb(IFridgeLogger logger, string connectionString)
		{
			this.Logger = logger;
			Logger.LogDebug($"RepositoryLiteDb.RepositoryLiteDb connectionString = '{connectionString}'");
			Db = new LiteDatabase(connectionString);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(Db != null)
					{
						Db.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~RepositoryLiteDb()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
