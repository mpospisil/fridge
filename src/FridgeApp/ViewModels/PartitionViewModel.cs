using FridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using System.Diagnostics;

namespace FridgeApp.ViewModels
{
	public interface IPartitionViewModel
	{
		string Name { get; set; }
		DateTime TimeStamp { get; set; }

		Guid PartitionId { get; set; }

		void SetPropertiesInVM(Fridge.Model.Partition partition);
		Fridge.Model.Partition PartitionFromVM();
	}

	public class PartitionViewModel : BaseViewModel, IPartitionViewModel
	{
		private Guid partitionId;
		private string name;
		private DateTime timeStamp;

		public PartitionViewModel(IFridgeDAL fridgeDal, Fridge.Model.Partition partition) : base(fridgeDal)
		{
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			Items = new ObservableCollection<IItemViewModel>();
			if (partition != null)
			{
				SetPropertiesInVM(partition);
			}

			LoadItemsCommand.Execute(null);
		}

		public Guid PartitionId
		{
			get => partitionId;
			set => SetProperty(ref partitionId, value);
		}

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

		public Command LoadItemsCommand { get; }

		public ObservableCollection<IItemViewModel> Items { get; }

		/// <summary>
		/// Write data to the view model
		/// </summary>
		/// <param name="partition"></param>
		public void SetPropertiesInVM(Fridge.Model.Partition partition)
		{
			this.PartitionId = partition.PartitionId;
			this.Name = partition.Name;
			this.TimeStamp = partition.TimeStamp;
		}

		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			try
			{
				Items.Clear();
				var allItems = await FridgeDal.GetItemsAsync();
				var itemsInPartition = allItems.Where(i => i.PartitionId == this.PartitionId);

				foreach (var item in itemsInPartition)
				{
					var itemVM = new ItemViewModel(FridgeDal, item);
					Items.Add(itemVM);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		/// <summary>
		/// Read data from the view model
		/// </summary>
		/// <returns></returns>
		public Fridge.Model.Partition PartitionFromVM()
		{
			var partitionDataFromVM = new Fridge.Model.Partition();
			partitionDataFromVM.PartitionId = PartitionId;
			partitionDataFromVM.Name = Name;
			partitionDataFromVM.TimeStamp = TimeStamp;

			return partitionDataFromVM;
		}
	}
}
