using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Utils {

	static partial class TextControl {
		public static void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

		[GeneratedRegex("[^0-9]+")]
		private static partial Regex RegexNumber();
		public static void FloatOnly(object sender, TextCompositionEventArgs e) {
			var currentText = ((TextBox)sender).Text;
			string input = e.Text;
			if (input.Length == 0) {
				e.Handled = false;
				return;
			}
			var match = RegexFloat().IsMatch(e.Text);
			if (match) {
				e.Handled = true;
				return;
			}
			if (input[0] == '.') {
				if (currentText.Length == 0 || currentText.Contains('.')) {
					e.Handled = true;
					return;
				}
			}
			e.Handled = false;
		}

		[GeneratedRegex("[^0-9.-]+")]
		private static partial Regex RegexFloat();
	}

	static class Util {
		public static Window? MainWindow = null;
		public static string GetString(string key, params object[] args) {
			string res;
			try {
				res = Application.Current.FindResource(key).ToString()!;
			} catch (NullReferenceException) {
				//"Object reference not set to an instance of an object." 如果字典的值为空就会出现这个报错
				res = "";
			}
			if (args.Length > 0) {
				res = string.Format(res, args);
			}
			return res;
		}

		public static string GetFilter(string key) {
			var specificFilter = GetString(key);
			var allFilter = GetString("FilterAllFiles");
			return $"{specificFilter}|{allFilter}";
		}

		public static bool IsEmpty(params string[] values) {
			foreach (var v in values) {
				if (v.Length == 0)
					return true;
			}
			return false;
		}


		public static void OpenFile(string filename) {
			Process pro = new() {
				StartInfo = new(filename) { UseShellExecute = true }
			};
			pro.Start();
		}

		public static BitmapImage LoadIcon(string filename) {
			var image = new BitmapImage();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
			image.UriSource = new(filename);
			image.EndInit();
			return image;
		}

		public static string Join<T>(ObservableCollection<T> trucks, Func<T, bool> condition, Func<T, string> toLine) {
			StringBuilder sb = new();
			foreach (var truck in trucks) {
				if (condition(truck)) {
					if (sb.Length > 0)
						sb.Append(DefaultData.LineSplit);
					sb.Append(toLine(truck));
				}
			}
			return sb.ToString();
		}
	}
}
