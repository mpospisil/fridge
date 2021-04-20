using Fridge.Logger;
using Serilog;
using System.Windows;

namespace DynamoDbTst
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MainVM vm;
		public MainWindow()
		{
			this.vm = new MainVM(App.Logger);
			DataContext = vm;
			InitializeComponent();
		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			if(vm != null)
			{
				vm.Dispose();
				vm = null;
			}
		}
	}
}
