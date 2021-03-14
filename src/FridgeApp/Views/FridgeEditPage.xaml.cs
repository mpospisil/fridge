using FridgeApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FridgeEditPage : ContentPage
	{
		public FridgeEditPage()
		{
			InitializeComponent();
			BindingContext = DependencyService.Resolve<IFridgeViewModel>();
		}
	}
}