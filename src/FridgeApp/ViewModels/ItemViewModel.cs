using FridgeApp.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IItemViewModel
	{
		string ItemId { get; }

		string PartitionId { get; }

		string Name { get; }

		string FridgeName { get; }

		int FridgeIndex { get; }

		string PartitionName { get; }

		int PartitionIndex { get; }
	}

	[QueryProperty(nameof(PartitionId), nameof(PartitionId))]
	[QueryProperty("FridgeId", "FridgeId")]
	public class ItemViewModel : BaseViewModel, IItemViewModel
	{
		string itemId;
		string partitionId;
		string fridgeId;
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
			ItemId = Guid.Empty.ToString();

			if (item != null)
			{
				SetPropertiesInVM(item);
			}

			SaveCommand = new Command(OnSave, ValidateSave);
			CancelCommand = new Command(OnCancel);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();
		}

		public string ItemId
		{
			get => itemId;
			set => SetProperty(ref itemId, value);
		}

		public string PartitionId
		{
			get => partitionId;
			set
			{
				SetProperty(ref partitionId, value);
			}
		}

		public string FridgeId
		{
			get => fridgeId;
			set => SetProperty(ref fridgeId, value);
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
		public Command SaveCommand { get; }
		public Command CancelCommand { get; }

		private void SetPropertiesInVM(Fridge.Model.ItemInFridge item)
		{
			this.itemId = item.ItemId.ToString();
			this.FridgeId = item.FridgeId.ToString();
			this.PartitionId = item.PartitionId.ToString();
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


		private async void OnCancel()
		{
			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		public async Task SaveData()
		{
			if(string.IsNullOrEmpty(itemId) || itemId.Equals(Guid.Empty.ToString()))
			{
				// new item
				var newItem = ItemFromVM();
				newItem.ItemId = Guid.NewGuid();
				newItem.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = newItem.TimeStamp });
				await FridgeDal.AddItemAsync(newItem);
			}
			else
			{
				// existing item
			}

			//if (fridgeGuid == Guid.Empty)
			//{
			//	// new fridge
			//	var newFridge = FridgeFromVM();
			//	newFridge.FridgeId = Guid.NewGuid();
			//	await FridgeDal.AddFridge(newFridge);
			//}
			//else
			//{
			//	// existing fridge
			//	var updatedFridge = FridgeFromVM();
			//	await FridgeDal.UpdateFridge(updatedFridge);
			//}
		}

		public Fridge.Model.ItemInFridge ItemFromVM()
		{
			var item = new Fridge.Model.ItemInFridge();
			item.FridgeId = Guid.Parse(FridgeId);
			item.PartitionId = Guid.Parse(PartitionId);
			item.ItemId = Guid.Parse(ItemId);
			item.Name = Name;
			item.TimeStamp = TimeStamp;

			return item;
		}

		private async void OnSave()
		{
			await SaveData();

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private bool ValidateSave()
		{
			return !String.IsNullOrWhiteSpace(Name);
		}
	}
}
