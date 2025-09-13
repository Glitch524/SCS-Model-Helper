using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Def_Writer {

	public class OptionWarningConverter: IMultiValueConverter {

		public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length == 0)
				return null;
			if (values[0] is not string s)
				return null;
			bool check;
			if (values.Length > 1 || values[1] is bool)
				check = (bool)values[1];
			else
				check = true;
			if (check) {
				if (s.Length == 0) {
					SolidColorBrush brush = new(Colors.Yellow);
					if (parameter != null) {
						string param = (string)parameter;
						if (param.Equals("red", StringComparison.OrdinalIgnoreCase))
							brush = new(Colors.Red);
					}
					return brush;
				} else
					return new SolidColorBrush();
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BooleanVisibilityConverter: IValueConverter {
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is bool b) {
				if (b)
					return Visibility.Visible;
				else
					return Visibility.Hidden;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class MultiNullCheckConverter: IMultiValueConverter {
		public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values[0] is not string)
				return false;
			string[] option = ((string)parameter).Split(",");
			bool and = option[0].Equals("and", StringComparison.OrdinalIgnoreCase);
			bool nonull = option.Length > 1 && option[1].Equals("Nonull", StringComparison.OrdinalIgnoreCase);
			if (nonull) {
				if (and) {
					foreach (var value in values) {
						if (string.IsNullOrEmpty((string)value))
							return false;
					}
					return true;
				} else {
					foreach (var value in values) {
						if (!string.IsNullOrEmpty((string)value))
							return true;
					}
					return false;
				}
			} else {
				if (and) {
					foreach (var value in values) {
						if (!string.IsNullOrEmpty((string)value))
							return false;
					}
					return true;
				} else {
					foreach (var value in values) {
						if (string.IsNullOrEmpty((string)value))
							return true;
					}
					return false;
				}
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
