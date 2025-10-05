using SCS_Mod_Helper.Utils;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using SCS_Mod_Helper.Base;

namespace SCS_Mod_Helper.Hookups;
/// <summary>
/// CreateHookup.xaml 的交互逻辑
/// </summary>
public partial class HookupsWindow: BaseWindow {
	private readonly HookupBinding HookupInfo = new();

	public string ProjectLocation {
		get => HookupInfo.ProjectLocation; set => HookupInfo.ProjectLocation = value;
	}
	private bool IsAnimHookup {
		get => HookupInfo.IsAnimHookup; set => HookupInfo.IsAnimHookup = value;
	}
	public string HookupName {
		get => HookupInfo.HookupName; set => HookupInfo.HookupName = value;
	}
	public string ModelLocation {
		get => HookupInfo.ModelLocation; set => HookupInfo.ModelLocation = value;
	}
	public string AnimationLocation {
		get => HookupInfo.AnimationLocation; set => HookupInfo.AnimationLocation = value;
	}

	public HookupsWindow() {
		InitializeComponent();
		ProjectLocation = Instances.ModelProject.ProjectLocation;
		PanelMain.DataContext = HookupInfo;
	}

	private void ButtonChooseFile(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			var ChooseModel = button == ButtonChooseModel;

			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				InitialDirectory = Paths.IntDecorsDir(ProjectLocation),
				DefaultExt = "pmd",
				Title = GetString(ChooseModel ? "DialogTitleModel" : "DialogTitleAnim"),
				Filter = Util.GetFilter(ChooseModel ? "DialogFilterModel" : "DialogFilterAnim")
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
			if (HookupName.Length == 0 || ModelLocation.Length == 0 || (IsAnimHookup && AnimationLocation.Length == 0)) {
				MessageBox.Show(this, GetString("MessageCreateErrNotFilled"));
				return;
			}
			var hookupBase = Paths.HookupFile(ProjectLocation,HookupName);
			using StreamWriter sw = new(hookupBase);
			sw.WriteLine("SiiNunit");
			sw.WriteLine("{");
			sw.WriteLine($"\t{(IsAnimHookup ? "animated_hookup" : "compass_hookup")} : {HookupName}");
			sw.WriteLine("\t{");
			sw.WriteLine($"\t\tmodel: \"{ModelLocation}\"");
			if (IsAnimHookup)
				sw.WriteLine($"\t\tanimation: \"{AnimationLocation}\"");
			sw.WriteLine("\t}");
			sw.WriteLine("}");

			MessageBox.Show(this, GetString("MessageCreateSuccess"));
		} else if (sender == ButtonCancel)
			Close();
	}
}


