using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbTst
{
	public class TableVM : ViewModelBase
	{
		public TableVM(string tableName)
		{
			this.TableName = tableName;
		}

		private string tableName;

		public string TableName
		{
			get => tableName;
			set
			{
				tableName = value;
				NotifyPropertyChanged();
			}
		}
	}
}
