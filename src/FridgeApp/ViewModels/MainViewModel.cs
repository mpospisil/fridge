using FridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FridgeApp.ViewModels
{
	public interface IMainViewModel
	{
		void OnAppearing();
		ObservableCollection<ItemViewModel> Items { get; }
	}

	public class MainViewModel : BaseViewModel, IMainViewModel
	{
		
		public MainViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			this.Items = new ObservableCollection<ItemViewModel>();
		}

		public ObservableCollection<ItemViewModel> Items { get; private set; }

		public void OnAppearing()
		{
			IsBusy = true;
		}
	}
}
