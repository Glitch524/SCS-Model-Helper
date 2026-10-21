using SCS_Mod_Helper.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCS_Mod_Helper.Base;
public class BaseBinding: INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));

	public static string GetString(string key, params object[] param) => Util.GetString(key, param);
}