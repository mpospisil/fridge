using FridgeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;

namespace FridgeApp.ViewModels
{
	public interface IFridgeViewModel
	{
		string Name { get; set; }
		string FridgeId { get; set; }
	}

	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	public class FridgeViewModel : BaseViewModel, IFridgeViewModel
	{
		#region Private fields
		private string name;
		private Guid fridgeGuid;
		private DateTime timeStamp; 
		#endregion

		#region Consructors
		public FridgeViewModel() : this(null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			Partitions = new ObservableCollection<IPartitionViewModel>();
			if (fridge != null)
			{
				SetPropertiesInVM(fridge);
			}

			SaveCommand = new Command(OnSave, ValidateSave);
			CancelCommand = new Command(OnCancel);
			AddPartitionCommand = new Command(OnAddPartition);
			DeletePartitionCommand = new Command(OnDeletePartition);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();
		}
		#endregion

		#region Properties

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		public DateTime TimeStamp
		{
			get => timeStamp;
			set => SetProperty(ref timeStamp, value);
		}

		public ObservableCollection<IPartitionViewModel> Partitions { get; }

		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public string FridgeId
		{
			get => fridgeGuid.ToString();
			set
			{
				Guid newId = Guid.Parse(value);

				if (newId != Guid.Empty)
				{
					// existing fridge
					SetProperty(ref fridgeGuid, newId);
					LoadItemId(newId);
				}
				else
				{
					// create temporary fridge
					fridgeGuid = Guid.Empty;
					Name = Resources.NewFridge;
				}
			}
		}

		#endregion
		private async void LoadItemId(Guid fridgeId)
		{
			try
			{
				var item = await this.FridgeDal.GetFridgeAsync(fridgeId);
				SetPropertiesInVM(item);
			}
			catch (Exception)
			{
				Debug.WriteLine("Failed to Load Item");
			}
		}

		public Command SaveCommand { get; }
		public Command CancelCommand { get; }
		public Command AddPartitionCommand { get; }
		public Command DeletePartitionCommand { get; }

		private void OnAddPartition()
		{
			try
			{
				var newPartition = new Fridge.Model.Partition();
				newPartition.Name = Resources.NewPartition;
				Partitions.Add(new PartitionViewModel(FridgeDal, newPartition));
			}
			catch(Exception e)
			{
				Debug.WriteLine($"Failed to add partition {e.Message}");
			}
		}

		private void OnDeletePartition(object obj)
		{
			try
			{
				IPartitionViewModel partitionToDelete = obj as IPartitionViewModel;
				var partitionIndexToDelete = Partitions.IndexOf(partitionToDelete);
				Partitions.RemoveAt(partitionIndexToDelete);
			}
			catch(Exception e)
			{
				Debug.WriteLine($"Failed to delete partition {e.Message}");
			}
		}

		private async void OnCancel()
		{
			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private async void OnSave()
		{
			if(fridgeGuid == Guid.Empty)
			{
				// new fridge
				var newFridge = FridgeFromVM();
				newFridge.FridgeId = Guid.NewGuid();
				await FridgeDal.AddFridge(newFridge);
			}
			else
			{
				// existing fridge
				var updatedFridge = FridgeFromVM();
				await FridgeDal.UpdateFridge(updatedFridge);
			}

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private void SetPropertiesInVM(Fridge.Model.Fridge fridge)
		{
			this.fridgeGuid = fridge.FridgeId;
			this.Name = fridge.Name;
			this.TimeStamp = fridge.TimeStamp;

			Partitions.Clear();

			foreach (var partition in fridge.Partitions)
			{
				Partitions.Add(new PartitionViewModel(FridgeDal, partition));
			}
		}

		private Fridge.Model.Fridge FridgeFromVM()
		{
			var fridgeDataFromVM = new Fridge.Model.Fridge();
			fridgeDataFromVM.FridgeId = fridgeGuid;
			fridgeDataFromVM.Name = Name;
			fridgeDataFromVM.TimeStamp = TimeStamp;

			foreach(var partitionVM in Partitions)
			{
				fridgeDataFromVM.Partitions.Add(partitionVM.PartitionFromVM());
			}

			return fridgeDataFromVM;
		}

		private bool ValidateSave()
		{
			return !String.IsNullOrWhiteSpace(Name);
		}
	}
}
