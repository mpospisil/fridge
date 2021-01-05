using FridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridgeApp.ViewModels
{
	public interface IMainViewModel
	{

	}

	public class MainViewModel : BaseViewModel, IMainViewModel
	{
		public MainViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{

		}


	}
}
