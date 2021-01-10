using FridgeApp.Services;
using System;

namespace FridgeApp.ViewModels
{
	public interface IItemViewModel
	{
		Guid ItemId { get; }

		string Name { get; }

		string FridgeName { get; }

		int FridgeIndex { get; }

		string PartitionName { get; }

		int PartitionIndex { get; }
	}

	public class ItemViewModel : BaseViewModel, IItemViewModel
	{
		Guid itemId;
		string name;
		string fridgeName;
		int fridgeIndex;
		string partitionName;
		int partitionIndex;

		public ItemViewModel() : this(null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal, Fridge.Model.ItemInFridge item) : base(fridgeDal)
		{
		}

		public Guid ItemId
		{
			get => itemId;
			set => SetProperty(ref itemId, value);
		}

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		public string FridgeName
		{
			get => fridgeName;
			set => SetProperty(ref fridgeName, value);
		}

		public int FridgeIndex
		{
			get => fridgeIndex;
			set => SetProperty(ref fridgeIndex, value);
		}

		public string PartitionName
		{
			get => partitionName;
			set => SetProperty(ref partitionName, value);
		}

		public int PartitionIndex
		{
			get => partitionIndex;
			set => SetProperty(ref partitionIndex, value);
		}
	}
}
