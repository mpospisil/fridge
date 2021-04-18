using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamoDbTst
{
	public class MainVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public MainVM()
		{
			this.ConnectCommand = new CustomCommand(ConnectDb, CanConnectDb);
		}

		private void CanConnectDb(object obj)
		{
		}

		private bool ConnectDb(object arg)
		{
			return true;
		}

		public ICommand ConnectCommand { get; }

		// This method is called by the Set accessor of each property.  
		// The CallerMemberName attribute that is applied to the optional propertyName  
		// parameter causes the property name of the caller to be substituted as an argument.  
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
