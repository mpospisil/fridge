using FridgeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
		IMainViewModel viewModel;

		public ItemsPage()
		{
			InitializeComponent();
			BindingContext = viewModel = DependencyService.Resolve<IMainViewModel>();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Shell.SetTabBarIsVisible(this, true);
			viewModel.OnAppearing();
		}
	}
}