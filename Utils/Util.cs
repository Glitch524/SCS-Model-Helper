using Def_Writer.Windows.ModelSii;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace Def_Writer.Utils {

	static class Util {
		public static Window? MainWindow = null;
		public static string GetString(string key, params object[] args) {
			string res;
			var md = Application.Current.Resources.MergedDictionaries;
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


		public static void OpenFile(string filename) {
			Process pro = new() {
				StartInfo = new(filename) { UseShellExecute = true }
			};
			pro.Start();
		}


		public static string Join(ObservableCollection<Truck> trucks, Func<Truck, bool> condition, Func<Truck, string> toLine) {
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
