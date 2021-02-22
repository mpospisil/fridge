using Fridge.Model;
using System;

namespace FridgeApp.ViewModels
{
	internal class PartitionDescriptor
	{
		internal int FridgeInx { get; set; }
		internal int PartitionInx { get; set; }
		internal Partition Partition { get; set; }
		internal Fridge.Model.Fridge Fridge { get; set; }
	}
}