using FridgeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface ISectorViewModel
	{
		string Name { get; set; }
		DateTime TimeStamp { get; set; }

		Guid SectorId { get; set; }

		ObservableCollection<IItemViewModel> Items { get; }

		void SetPropertiesInVM(Fridge.Model.Sector sector);
		Fridge.Model.Sector SectorFromVM();
		Command LoadItemsCommand { get; }
	}

	public class SectorViewModel : BaseViewModel, ISectorViewModel
	{
		private Guid sectorId;
		private string name;
		private DateTime timeStamp;

		public SectorViewModel(IFridgeDAL fridgeDal, Fridge.Model.Sector sector) : base(fridgeDal)
		{
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			Items = new ObservableCollection<IItemViewModel>();
			if (sector != null)
			{
				SetPropertiesInVM(sector);
			}

			LoadItemsCommand.Execute(null);
		}

		public Guid SectorId
		{
			get => sectorId;
			set => SetProperty(ref sectorId, value);
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
		/// <param name="sector"></param>
		public void SetPropertiesInVM(Fridge.Model.Sector sector)
		{
			this.SectorId = sector.SectorId;
			this.Name = sector.Name;
			this.TimeStamp = sector.TimeStamp;
		}

		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			try
			{
				Items.Clear();
				var allItems = await FridgeDal.GetItemsAsync();
				var itemsInSector = allItems.Where(i => i.SectorId == this.SectorId);

				foreach (var item in itemsInSector)
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
		public Fridge.Model.Sector SectorFromVM()
		{
			var sectorDataFromVM = new Fridge.Model.Sector();
			sectorDataFromVM.SectorId = SectorId;
			sectorDataFromVM.Name = Name;
			sectorDataFromVM.TimeStamp = TimeStamp;

			return sectorDataFromVM;
		}
	}
}
