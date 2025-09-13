using System.Text;
using System.Windows;

namespace Def_Writer {
	public class BaseWindow: Window {
		protected string GetString(string key, params object[] args) {
			string res = FindResource(key).ToString()!;
			if (args.Length > 0) {
				res = string.Format(res, args);
			}
			return res;
		}
		protected string GetStringMultiLine(params string[] key) {
			StringBuilder sb = new();
			for (int i = 0; i < key.Length; i++) {
				if (i > 0)
					sb.Append('\n');
				sb.Append(FindResource(key[i]));
			}
			return sb.ToString();
		}
	}
}
