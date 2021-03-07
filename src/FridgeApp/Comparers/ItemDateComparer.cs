using FridgeApp.ViewModels;
using System;
using System.Collections.Generic;

namespace FridgeApp.Comparers
{
	public class ItemDateComparer : IComparer<IItemViewModel>
	{
		public int Compare(IItemViewModel x, IItemViewModel y)
		{
			return DateTime.Compare(x.AddToFridgeTime, y.AddToFridgeTime);
		}
	}
}