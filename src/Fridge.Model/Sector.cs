using System;

namespace Fridge.Model
{
	/// <summary>
	/// A sector in a fridge
	/// </summary>
	public class Sector
	{
		public Sector()
		{
			SectorId = Guid.NewGuid(); ;
			TimeStamp = DateTime.UtcNow;
		}

		/// <summary>
		/// The unique identifier of the sector in the fridge
		/// </summary>
		public Guid SectorId { get; set; }

		/// <summary>
		/// User friendly name of a sector in a fridge
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Time when the sector in the fridge  was modified
		/// </summary>
		public DateTime TimeStamp { get; set; }
	}
}
