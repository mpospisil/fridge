using Fridge.Model;

namespace FridgeApp.ViewModels
{
	internal class SectorDescriptor
	{
		internal int FridgeInx { get; set; }
		internal int SectorInx { get; set; }
		internal Sector Sector { get; set; }
		internal Fridge.Model.Fridge Fridge { get; set; }
	}
}