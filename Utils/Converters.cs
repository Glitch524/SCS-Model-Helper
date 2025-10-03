using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SCS_Mod_Helper.Utils {

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
	public class Float3Converter: IValueConverter {
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null && value is float[] f) {
				return string.Join(',', f);
			}
			return "0,0,0";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string text = (string)value;
			var split = text.Split(',');
			float[] f = new float[split.Length];
			for (int i = 0; i < split.Length; i++) {
				f[i] = float.Parse(split[i]);
			}
			return f;
		}
	}
	public class FloatConverter: IValueConverter {
		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) {
			if (value is float f) {
				return f.ToString("0.######");
			}
			return "";
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string text = (string)value;
			try {
				return float.Parse(text);
			} catch {
				return null;
			}
		}
	}
	public class UIntConverter: IValueConverter {
		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) {
			if (value is uint i) {
				return i.ToString();
			}
			return "";
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string text = (string)value;
			try {
				return uint.Parse(text);
			} catch {
				return null;
			}
		}
	}
	public class LongConverter: IValueConverter {
		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) {
			if (value is long i) {
				return i.ToString();
			}
			return "";
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string text = (string)value;
			try {
				return long.Parse(text);
			} catch {
				return null;
			}
		}
	}

	public class ValueEqualsConverter: IMultiValueConverter {
		public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length <2) 
				return false;
			return values[0].Equals(values[1]);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
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
