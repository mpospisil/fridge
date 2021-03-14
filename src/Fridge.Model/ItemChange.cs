using System;

namespace Fridge.Model
{
	public enum ChangeTypes
	{
		Added,
		Removed,
	}


	/// <summary>
	/// Represents the change of the item in the fridge
	/// </summary>
	public class ItemChange
	{
		public ItemChange()
		{
			TypeOfChange = ChangeTypes.Added;
			TimeOfChange = DateTime.UtcNow;
		}

		public ChangeTypes TypeOfChange { get; set; }

		public DateTime TimeOfChange { get; set; }
	}
}
