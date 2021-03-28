using FridgeApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IItemViewModel
	{
		string ItemId { get; }

		string FridgeId { get; }

		string SectorId { get; set; }

		/// <summary>
		/// True if this item is currently in a fridge
		/// </summary>
		bool IsInFridge { get; set; }

		string Name { get; set; }

		string FridgeName { get; }

		int FridgeIndex { get; }

		string SectorName { get; }

		int SectorIndex { get; }

		/// <summary>
		/// Method set read data from repository and sets them into properties ov VM
		/// </summary>
		string ItemFromRepositoryId { get; set; }

		bool IsSelected { get; set; }

		bool IsVisible { get; set; }

		/// <summary>
		/// The date when item was added to the fridge
		/// </summary>
		DateTime AddToFridgeTime { get; set; }

		/// <summary>
		/// Remove item from fridge. 
		/// </summary>
		/// <param name="itemId">Id of the item which will be removed</param>
		/// <param name="removedItemIdentifier">The id which will be set to properties ItemId and SectorId</param>
		/// <returns>Task</returns>
		Task RemoveItemFromFridge(Guid itemId, Guid removedItemIdentifier);
	}

	[QueryProperty(nameof(ItemId), nameof(ItemId))]
	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	[QueryProperty(nameof(SectorId), nameof(SectorId))]
	[QueryProperty(nameof(ItemFromRepositoryId), nameof(ItemFromRepositoryId))]
	public class ItemViewModel : BaseViewModel, IItemViewModel
	{
		private readonly IFridgeLogger Logger;
		string itemId;
		string sectorId;
		string fridgeId;
		string name;
		string fridgeName;
		int fridgeIndex;
		string sectorName;
		int sectorIndex;
		private DateTime timeStamp;
		private bool isInFridge;
		DateTime addToFridgeTime;
		bool isSelected;
		bool isVisible;

		public ItemViewModel(IFridgeLogger logger) : this(logger, null)
		{
		}

		public ItemViewModel(IFridgeLogger logger, IFridgeDAL fridgeDal) : this(logger, fridgeDal, null)
		{
		}

		public ItemViewModel(IFridgeLogger logger, IFridgeDAL fridgeDal, Fridge.Model.ItemInFridge item) : base(fridgeDal)
		{
			Logger = logger;
			itemId = Guid.Empty.ToString();

			if (item != null)
			{
				Logger.LogDebug($"ItemViewModel.ItemViewModel Name = {item.Name}  {item.ItemId}");
				SetPropertiesInVM(item);
			}
			else
			{
				Logger.LogDebug("ItemViewModel.ItemViewModel  - new item");
				TimeStamp = DateTime.UtcNow;
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

		public string SectorId
		{
			get => sectorId;
			set
			{
				SetProperty(ref sectorId, value);
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

		public string SectorName
		{
			get => sectorName;
			set => SetProperty(ref sectorName, value);
		}

		public int SectorIndex
		{
			get => sectorIndex;
			set => SetProperty(ref sectorIndex, value);
		}
		public DateTime TimeStamp
		{
			get => timeStamp;
			set => SetProperty(ref timeStamp, value);
		}

		/// <summary>
		/// The date when item was added to the fridge
		/// </summary>
		public DateTime AddToFridgeTime
		{
			get => addToFridgeTime;
			set => SetProperty(ref addToFridgeTime, value);
		}

		/// <summary>
		/// True if the item is selected in a view
		/// </summary>
		public bool IsSelected
		{
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}

		public bool IsVisible
		{
			get => isVisible;
			set => SetProperty(ref isVisible, value);
		}

		public Command SaveCommand { get; }
		public Command CancelCommand { get; }

		public async Task RemoveItemFromFridge(Guid itemId, Guid removedItemIdentifier)
		{
			await FridgeDal.RemoveItemAsync(itemId);
		}

		private void SetPropertiesInVM(Fridge.Model.ItemInFridge item)
		{
			this.itemId = item.ItemId.ToString();
			this.IsInFridge = item.IsInFridge;
			this.FridgeId = item.FridgeId.ToString();
			this.SectorId = item.SectorId.ToString();
			this.Name = item.Name;
			this.TimeStamp = item.TimeStamp;

			if (item?.History?.Any() == true)
			{
				AddToFridgeTime = item.History.First().TimeOfChange;
			}
		}

		private async void OnCancel()
		{
			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		public async Task SaveData()
		{
			if (string.IsNullOrEmpty(itemId) || itemId.Equals(Guid.Empty.ToString()))
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
			item.SectorId = Guid.Parse(SectorId);
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
