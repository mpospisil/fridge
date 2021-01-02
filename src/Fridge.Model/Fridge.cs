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
			Partitions = new List<Partition>();
			TimeStamp = DateTime.UtcNow;
			FridgeId = Guid.NewGuid();
		}

		public void CopyFrom(Fridge source)
		{
			this.TimeStamp = DateTime.UtcNow;
			this.Name = source.Name;
			this.FridgeId = source.FridgeId;
		}

		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public Guid FridgeId { get; set; }

		/// <summary>
		/// User friendly name of the fridge
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Time when fridge was modified
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Partitions in the fridge
		/// </summary>
		public List<Partition> Partitions { get; set; }
	}
}
