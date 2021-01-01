using FridgeApp.Services;
using System.Threading.Tasks;

namespace FridgeApp.ViewModels
{
	public class FridgeViewModel : BaseViewModel
	{
		private string name;
		private string description;

		private Fridge.Model.Fridge Fridge { get; set; }
		public FridgeViewModel(IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			this.Fridge = fridge;
			Name = fridge?.Name;
			Description = string.Empty;
		}
		

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		public string Description
		{
			get => description;
			set => SetProperty(ref description, value);
		}
	}
}
