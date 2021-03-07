using Fridge.Model;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace FridgeApp.Converters
{
	public class ItemsOrderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ItemsOrder itemsOrder = (ItemsOrder)value;
			switch (itemsOrder)
			{
				case ItemsOrder.ByDate:
					return Resources.SortByDate;

				case ItemsOrder.ByName:
					return Resources.SortByName;

				case ItemsOrder.ByFridge:
					return Resources.SortByFridge;

				case ItemsOrder.NotSorted:
					return Resources.NotSorted;
			}

			return "Unknown";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
