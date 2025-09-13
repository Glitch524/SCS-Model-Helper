using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Def_Writer;
/// <summary>
/// CreateHookup.xaml 的交互逻辑
/// </summary>
public partial class CreateHookup: BaseWindow {
	private readonly HookupInfo HookupInfo = new();

	public string ProjectLocation {
		get {
			return HookupInfo.ProjectLocation;
		}
		set {
			HookupInfo.ProjectLocation = value;
		}
	}

	public string HookupName {
		get {
			return HookupInfo.HookupName;
		}
		set {
			HookupInfo.HookupName = value;
		}
	}
	public string ModelLocation {
		get {
			return HookupInfo.ModelLocation;
		}
		set {
			HookupInfo.ModelLocation = value;
		}
	}
	public string AnimationLocation {
		get {
			return HookupInfo.AnimationLocation;
		}
		set {
			HookupInfo.AnimationLocation = value;
		}
	}

	public CreateHookup(string projectLocation) {
		InitializeComponent();
		ProjectLocation = projectLocation;
		TextProjectLocation.DataContext = HookupInfo;
		TextName.DataContext = HookupInfo;
		TextModel.DataContext = HookupInfo;
		TextAnimation.DataContext = HookupInfo;
	}

	private void ButtonChooseFile(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			var ChooseModel = button == ButtonChooseModel;

			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				InitialDirectory = Utils.IntDecorsDir(ProjectLocation),
				DefaultExt = "pmd",
				Title = GetString(ChooseModel ? "DialogTitleModel" : "DialogTitleAnim"),
				Filter = GetString(ChooseModel ? "DialogFilterModel" : "DialogFilterAnim")
			};
			if (fileDialog.ShowDialog() != true)
				return;
			try {
				string path = fileDialog.FileName;
				if (!path.StartsWith(ProjectLocation) || path.Length <= ProjectLocation.Length)
					throw new(GetString("MessageFileErrOutside"));
				if (ChooseModel) {
					if (!path.EndsWith(".pim") && !path.EndsWith(".pmd"))
						throw new(GetString("MessageFileErrNotModel"));
				} else if (!path.EndsWith(".pia") && !path.EndsWith(".pma"))
					throw new(GetString("MessageFileErrNotAnim"));
				var pmdExist = false;
				var pmaExist = false;
				path = path[..^4];
				if ((ChooseModel || ModelLocation.Length == 0) && (File.Exists(path + ".pim") || File.Exists(path + ".pmd")))
					pmdExist = true;
				if (File.Exists(path + ".pia") || File.Exists(path + ".pma"))
					pmaExist = true;
				path = path.Replace(ProjectLocation, "");
				path = path.Replace('\\', '/');
				if (pmdExist)
					ModelLocation = path + ".pmd";
				if (pmaExist)
					AnimationLocation = path + ".pma";
				if (ChooseModel || pmdExist || HookupName.Length == 0) {
					HookupName = fileDialog.SafeFileName;
					HookupName = HookupName[..HookupName.LastIndexOf('.')];
				}
			} catch (Exception ex) {
				MessageBox.Show(this, ex.Message);
			}
		}
	}

	private void ButtonResult(object sender, RoutedEventArgs e) {
		if (sender == ButtonOK) {
			if (HookupName.Length == 0 || ModelLocation.Length == 0 || AnimationLocation.Length == 0) {
				MessageBox.Show(this, GetString("MessageCreateErrNotFilled"));
				return;
			}
			var hookupBase = Utils.HookupFile(ProjectLocation,HookupName);
			using StreamWriter sw = new(hookupBase);
			sw.WriteLine("SiiNunit");
			sw.WriteLine("{");
			sw.WriteLine($"\tanimated_hookup : {HookupName}");
			sw.WriteLine("\t{");
			sw.WriteLine($"\t\tmodel: \"{ModelLocation}\"");
			sw.WriteLine($"\t\tanimation: \"{AnimationLocation}\"");
			sw.WriteLine("\t}");
			sw.WriteLine("}");

			MessageBox.Show(this, GetString("MessageCreateSuccess"));
		} else if (sender == ButtonCancel)
			Close();
	}
}

public class HookupInfo: INotifyPropertyChanged {

	public HookupInfo() {
	}

	public string MProjectLocation = "";
	public string ProjectLocation {
		get {
			return MProjectLocation;
		}
		set {
			MProjectLocation = value;
			InvokeChange(nameof(ProjectLocationShow));
		}
	}
	public string ProjectLocationShow {
		get {
			return Utils.GetString("TextProjectLocation") + MProjectLocation;
		}
	}

	public string MHookupName = "";
	public string HookupName {
		get {
			return MHookupName;
		}
		set {
			MHookupName = value;
			InvokeChange(nameof(HookupName));
		}
	}

	public string MModelLocation = "";
	public string ModelLocation {
		get {
			return MModelLocation;
		}
		set {
			MModelLocation = value;
			InvokeChange(nameof(ModelLocation));
		}
	}

	public string MAnimationLocation = "";
	public string AnimationLocation {
		get {
			return MAnimationLocation;
		}
		set {
			MAnimationLocation = value;
			InvokeChange(nameof(AnimationLocation));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void InvokeChange(string name) {
		PropertyChanged?.Invoke(this, new(name));
	}
}

