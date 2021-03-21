using System;
using System.Collections.Generic;

namespace Fridge.Model
{
	/// <summary>
	/// Item in the fridge
	/// </summary>
	public class ItemInFridge
	{
		public ItemInFridge()
		{
			TimeStamp = DateTime.UtcNow;
			History = new List<ItemChange>();
			IsInFridge = false;
		}

		/// <summary>
		/// The unique identifier of the item in the fridge
		/// </summary>
		public Guid ItemId { get; set; }

		/// <summary>
		/// The identifier of the fridhe where item is stored
		/// </summary>
		public Guid FridgeId { get; set; }

		/// <summary>
		/// The identifier of the sector where item is stored
		/// </summary>
		public Guid SectorId { get; set; }

		/// <summary>
		/// True if this item is currently in a fridge
		/// </summary>
		public bool IsInFridge { get; set; }

		/// <summary>
		/// Time when the sector in the fridge  was modified
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Name of the item
		/// </summary>
		public string Name { get; set; }

		public List<ItemChange> History { get; set; }
	}
}
