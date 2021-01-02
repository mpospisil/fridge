using FridgeApp.Services;
using System;

namespace FridgeApp.ViewModels
{
	public interface IPartitionViewModel
	{
		string Name { get; set; }
		DateTime TimeStamp { get; set; }

		Guid PartitionId { get; set; }

		void SetPropertiesInVM(Fridge.Model.Partition partition);
		Fridge.Model.Partition FridgeFromVM();
	}

	public class PartitionViewModel : BaseViewModel, IPartitionViewModel
	{
		private Guid partitionId;
		private string name;
		private DateTime timeStamp;

		public PartitionViewModel(IFridgeDAL fridgeDal, Fridge.Model.Partition partition) : base(fridgeDal)
		{
			if (partition != null)
			{
				SetPropertiesInVM(partition);
			}
		}

		public Guid PartitionId
		{
			get => partitionId;
			set => SetProperty(ref partitionId, value);
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

		public void SetPropertiesInVM(Fridge.Model.Partition partition)
		{
			this.PartitionId = partition.PartitionId;
			this.Name = partition.Name;
			this.TimeStamp = partition.TimeStamp;
		}

		public Fridge.Model.Partition FridgeFromVM()
		{
			var partitionDataFromVM = new Fridge.Model.Partition();
			partitionDataFromVM.PartitionId = PartitionId;
			partitionDataFromVM.Name = Name;
			partitionDataFromVM.TimeStamp = TimeStamp;

			return partitionDataFromVM;
		}
	}
}
