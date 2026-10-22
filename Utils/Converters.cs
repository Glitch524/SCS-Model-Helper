using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCS_Mod_Helper.Utils {

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
		public static FloatConverter Instance { get; } = new();
		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) {
			if (value is float f) {
				return f.ToString("0.######");
			}
			return string.Empty;
		}
		public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string text && float.TryParse(text, out float result)) {
				return result;
			}
			return DependencyProperty.UnsetValue;
		}
	}

	public class UIntConverter: IValueConverter {
		public static UIntConverter Instance { get; } = new();
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
}
