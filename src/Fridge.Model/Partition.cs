using System;
using System.Collections.Generic;

namespace Fridge.Model
{
	/// <summary>
	/// A partition in a fridge
	/// </summary>
	public class Partition
	{
		public Partition()
		{
			PartitionId = Guid.NewGuid(); ;
			TimeStamp = DateTime.UtcNow;
		}

		/// <summary>
		/// The unique identifier of the partition in the fridge
		/// </summary>
		public Guid PartitionId { get; set; }

		/// <summary>
		/// User friendly name of a partition in a fridge
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Time when the partition in the fridge  was modified
		/// </summary>
		public DateTime TimeStamp { get; set; }
	}
}
