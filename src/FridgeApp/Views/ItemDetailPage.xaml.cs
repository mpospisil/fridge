using FridgeApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace FridgeApp.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		public ItemDetailPage()
		{
			InitializeComponent();
			BindingContext = new ItemDetailViewModel();
		}
	}
}