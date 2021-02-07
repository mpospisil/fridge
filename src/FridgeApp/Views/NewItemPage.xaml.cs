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
	public partial class NewItemPage : ContentPage
	{
		public NewItemPage()
		{
			InitializeComponent();
			BindingContext = DependencyService.Resolve<IItemViewModel>();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}