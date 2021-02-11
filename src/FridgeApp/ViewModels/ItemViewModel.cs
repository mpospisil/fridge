using FridgeApp.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IItemViewModel
	{
		string ItemId { get; }

		string PartitionId { get; set; }

		/// <summary>
		/// True if this item is currently in a fridge
		/// </summary>
		bool IsInFridge { get; set; }

		string Name { get; set; }

		string FridgeName { get; }

		int FridgeIndex { get; }

		string PartitionName { get; }

		int PartitionIndex { get; }

		/// <summary>
		/// Method set read data from repository and sets them into properties ov VM
		/// </summary>
		string ItemFromRepositoryId { get; set; }

		/// <summary>
		/// Remove item from fridge. 
		/// </summary>
		/// <param name="itemId">Id of the item which will be removed</param>
		/// <param name="removedItemIdentifier">The id which will be set to properties ItemId and PartitionId</param>
		/// <returns>Task</returns>
		Task RemoveItemFromFridge(Guid itemId, Guid removedItemIdentifier);
	}

	[QueryProperty(nameof(ItemId), nameof(ItemId))]
	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	[QueryProperty(nameof(PartitionId), nameof(PartitionId))]
	[QueryProperty(nameof(ItemFromRepositoryId), nameof(ItemFromRepositoryId))]
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
		private bool isInFridge;

		public ItemViewModel() : this(null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public ItemViewModel(IFridgeDAL fridgeDal, Fridge.Model.ItemInFridge item) : base(fridgeDal)
		{
			itemId = Guid.Empty.ToString();

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
			set
			{
				var previousPropValue = itemId;
				SetProperty(ref itemId, value);

				if (!string.IsNullOrEmpty(itemId) && !itemId.Equals(Guid.Empty.ToString()) && !previousPropValue.Equals(itemId))
				{
					LoadValuesFromRepository(Guid.Parse(itemId));
				}
			}
		}

		/// <summary>
		/// the method set of this property reads data from repository and sets them into properties ov VM
		/// </summary>
		public string ItemFromRepositoryId
		{
			get => ItemId;
			set
			{
				if (!string.IsNullOrEmpty(value) && !value.Equals(Guid.Empty.ToString()))
				{
					LoadValuesFromRepository(Guid.Parse(value));
				}
			}
		}

		private async void LoadValuesFromRepository(Guid itemId)
		{
			var itemData = await FridgeDal.GetItemAsync(itemId);
			SetPropertiesInVM(itemData);
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

		public bool IsInFridge
		{
			get => isInFridge;
			set => SetProperty(ref isInFridge, value);
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

		public async Task RemoveItemFromFridge(Guid itemId, Guid removedItemIdentifier)
		{
			var itemToRemove = await FridgeDal.GetItemAsync(itemId);

			// set FrigeId and PartitionId to value of RemovedItemsIdentifier for this fridge
			itemToRemove.FridgeId = removedItemIdentifier;
			itemToRemove.PartitionId = removedItemIdentifier;
			itemToRemove.IsInFridge = false;
			itemToRemove.TimeStamp = DateTime.UtcNow;
			itemToRemove.History.Add(new Fridge.Model.ItemChange() { TimeOfChange = itemToRemove.TimeStamp, TypeOfChange = Fridge.Model.ChangeTypes.Removed });
			await FridgeDal.UpdateItemAsync(itemToRemove);
		}

		private void SetPropertiesInVM(Fridge.Model.ItemInFridge item)
		{
			this.itemId = item.ItemId.ToString();
			this.FridgeId = item.FridgeId.ToString();
			this.PartitionId = item.PartitionId.ToString();
			this.Name = item.Name;
			this.TimeStamp = item.TimeStamp;
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
				newItem.IsInFridge = true;
				newItem.History.Add(new Fridge.Model.ItemChange() { TypeOfChange = Fridge.Model.ChangeTypes.Added, TimeOfChange = newItem.TimeStamp });
				await FridgeDal.AddItemAsync(newItem);
			}
			else
			{
				// existing item
				var updatedItem = ItemFromVM();
				await FridgeDal.UpdateItemAsync(updatedItem);
			}
		}

		public Fridge.Model.ItemInFridge ItemFromVM()
		{
			var item = new Fridge.Model.ItemInFridge();
			item.FridgeId = Guid.Parse(FridgeId);
			item.PartitionId = Guid.Parse(PartitionId);
			item.ItemId = Guid.Parse(ItemId);
			item.Name = Name;
			item.IsInFridge = IsInFridge;
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
