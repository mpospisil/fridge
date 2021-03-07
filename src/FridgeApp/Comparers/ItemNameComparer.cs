using FridgeApp.ViewModels;
using System.Collections.Generic;

namespace FridgeApp.Comparers
{
	public class ItemNameComparer : IComparer<IItemViewModel>
	{
		public int Compare(IItemViewModel x, IItemViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}
}
