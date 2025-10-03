using System.Windows;

namespace SCS_Mod_Helper.Base {
	public class BaseWindow: Window {
		public string GetString(string key, params object[] args) {
			string res = FindResource(key).ToString()!;
			if (args.Length > 0) {
				res = string.Format(res, args);
			}
			return res;
		}
	}
}
