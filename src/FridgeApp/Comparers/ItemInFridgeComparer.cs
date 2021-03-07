using FridgeApp.ViewModels;
using System.Collections.Generic;

namespace FridgeApp.Comparers
{
	class ItemInFridgeComparer : IComparer<IItemViewModel>
	{
		public int Compare(IItemViewModel x, IItemViewModel y)
		{
			if(x.FridgeIndex < y.FridgeIndex)
			{
				return -1;
			}
			else if(x.FridgeIndex > y.FridgeIndex)
			{
				return 1;
			}
			else
			{
				// fride indexes are same - compare indexes of partitions
				if (x.PartitionIndex < y.PartitionIndex)
				{
					return -1;
				}
				else if (x.PartitionIndex > y.PartitionIndex)
				{
					return 1;
				}
				else
				{
					// same fridge, same partition, compare names of items
					return string.Compare(x.Name, y.Name);
				}
			}
		}
	}
}