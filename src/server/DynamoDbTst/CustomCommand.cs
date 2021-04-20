using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamoDbTst
{


	public class CustomCommand : ICommand
	{
		private Func<object, Task> ExecuteFuncAsync;
		private Func<object, bool> CanExecuteFunc;
		private bool IsExecuting { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public CustomCommand(Func<object, bool> canExecuteFunc, Func<object, Task> executeFunc)
		{
			ExecuteFuncAsync = executeFunc;
			CanExecuteFunc = canExecuteFunc;
		}

		public bool CanExecute(object parameter)
		{
			if(IsExecuting)
			{
				return false;
			}

			return CanExecuteFunc(parameter);
		}

		public async void Execute(object parameter)
		{

			IsExecuting = true;
			try
			{
				RaiseCanExecuteChanged(); // Not necessary if Execute is not called locally
				await ExecuteFuncAsync(parameter);
			}
			finally
			{
				IsExecuting = false;

				RaiseCanExecuteChanged();
			}
			
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}