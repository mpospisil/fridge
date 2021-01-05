﻿using FridgeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		IMainViewModel viewModel;

		public MainPage()
		{
			InitializeComponent();
			BindingContext = viewModel = DependencyService.Resolve<IMainViewModel>();
		}


	}
}