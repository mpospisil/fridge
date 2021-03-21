using System;
using System.Collections.Generic;

namespace Fridge.Model
{
	/// <summary>
	/// Contains data about a fridge
	/// </summary>
	public class Fridge
	{
		public Fridge()
		{
			Sectors = new List<Sector>();
			TimeStamp = DateTime.UtcNow;
			FridgeId = Guid.NewGuid();
			RemovedItemsIdentifier = Guid.NewGuid();
		}

		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public Guid FridgeId { get; set; }

		/// <summary>
		/// Id of the owner of this fridge
		/// </summary>
		public Guid OwnerId { get; set; }

		/// <summary>
		/// The unique identifier of the items which were removed from this fridge
		/// </summary>
		public Guid RemovedItemsIdentifier { get; set; }

		/// <summary>
		/// User friendly name of the fridge
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Time when fridge was modified
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Sectors in the fridge
		/// </summary>
		public List<Sector> Sectors { get; set; }
	}
}
