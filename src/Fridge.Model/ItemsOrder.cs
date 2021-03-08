using System;

namespace Fridge.Model
{
	[Flags]
	public enum ItemsOrder
	{
		NotSorted = 0,
		ByFridge = 1,
		ByName = 2,
		ByDate = 4
	}
}