using FridgeApp.Services;
using System;

namespace FridgeApp.ViewModels
{
	public interface IItemViewModel
	{
		string ItemId { get; }

		string Name { get; }

		string FridgeName { get; }

		int FridgeIndex { get; }

		string PartitionName { get; }

		int PartitionIndex { get; }
	}

	public class ItemViewModel : BaseViewModel, IItemViewModel
	{
		string itemId;
		string name;
		string fridgeName;
		int fridgeIndex;
		string partitionName;
		int partitionIndex;
		private DateTime timeStamp;

		public ItemViewModel() : this(null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal, Fridge.Model.ItemInFridge item) : base(fridgeDal)
		{
			if (item != null)
			{
				SetPropertiesInVM(item);
			}
		}

		public string ItemId
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
		public DateTime TimeStamp
		{
			get => timeStamp;
			set => SetProperty(ref timeStamp, value);
		}


		private void SetPropertiesInVM(Fridge.Model.ItemInFridge item)
		{
			this.itemId = item.ItemId.ToString();
			this.Name = item.Name;
			this.TimeStamp = item.TimeStamp;


		}

		public Fridge.Model.ItemInFridge FridgeFromVM()
		{
			var fridgeDataFromVM = new Fridge.Model.ItemInFridge();
			//fridgeDataFromVM.FridgeId = fridgeGuid;
			//fridgeDataFromVM.Name = Name;
			//fridgeDataFromVM.TimeStamp = TimeStamp;

			//foreach (var partitionVM in Partitions)
			//{
			//	fridgeDataFromVM.Partitions.Add(partitionVM.PartitionFromVM());
			//}

			return fridgeDataFromVM;
		}

	}
}
