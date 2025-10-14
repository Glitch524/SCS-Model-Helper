using Microsoft.Win32;
using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace SCS_Mod_Helper.Accessory;
public static class AccessoryDataUtil {
	public static void SetupStringResMenu(ContextMenu MenuStringRes, Action<MenuItem> action) {
		MenuStringRes.Placement = PlacementMode.Top;
		MenuStringRes.Items.Clear();

		var res = Instances.LocaleDict;
		MenuItem head = new() {
			Name = "head",
			Header = Util.GetString("MenuResTip"),
			IsEnabled = false
		};
		MenuStringRes.Items.Add(head);
		MenuItem separator = new() {
			Height = 3,
			Background = new SolidColorBrush(Colors.LightGray),
			IsEnabled = false
		};
		MenuStringRes.Items.Add(separator);
		if (res.Count == 0) {
			MenuItem empty = new() {
				Name = "empty",
				Header = Util.GetString("StatusEmpty"),
				IsEnabled = false
			};
			MenuStringRes.Items.Add(empty);
		} else {
			foreach (var pair in res) {
				MenuItem item = new() {
					Name = pair.Key,
					Header = pair.Key.Replace("_", "__")
				};
				StringBuilder tips = new();
				tips.AppendLine(Util.GetString("ResourceValueHeader"));
				for (int i = 0; i < pair.Value.Count; i++) {
					var value = pair.Value.ElementAt(i);
					tips.Append($"    {value}");
					if (i < pair.Value.Count - 1)
						tips.Append('\n');
				}
				item.ToolTip = tips.ToString();
				ToolTipService.SetInitialShowDelay(item, 500);
				action(item);
				MenuStringRes.Items.Add(item);
			}
		}
		MenuItem separator2 = new() {
			Height = 3,
			Background = new SolidColorBrush(Colors.LightGray),
			IsEnabled = false
		};
		MenuStringRes.Items.Add(separator2);
		MenuItem openLocalization = new() {
			Name = "openLocalization",
			Header = Util.GetString("MenuResOption")
		};
		action(openLocalization);
		MenuStringRes.Items.Add(openLocalization);
	}

	public static string GetStringResResults(string displayName) {
		var localeDict = Instances.LocaleDict;
		int localeCount = localeDict.First().Value.Count;
		bool hasKey = false;
		string[] values;
		if (localeCount == 0) {
			values = [displayName.Replace("@@", "")];
		} else {
			values = new string[localeCount];
			var split = displayName.Split("@@");
			for (int i = 0; i < split.Length; i++) {
				if (i % 2 == 1) {
					var list = localeDict.GetValueOrDefault(split[i]);
					if (list != null) {
						hasKey = true;
						for (int j = 0; j < values.Length; j++) {
							values[j] += list[j];
						}
						continue;
					}
				}
				if (split[i] != "") {
					for (int j = 0; j < values.Length; j++) {
						values[j] += split[i];
					}
				}
			}
		}
		string result;
		if (hasKey)
			result = string.Join('\n', values);
		else
			result = values[0];
		return result;
	}

	public static void OpenLocalization(Window window, Action setup) {
		var modLocalization = new ModLocalizationWindow() {
			Owner = window
		};
		modLocalization.ShowDialog();
		if (modLocalization.HasChanges)
			setup();
	}

	public static void ApplyStringRes(TextBox TextDisplayName, string resId) {

		var start = TextDisplayName.SelectionStart;
		TextDisplayName.SelectedText = "";
		var insert = $"@@{resId}@@";
		TextDisplayName.Text = TextDisplayName.Text.Insert(start, insert);
		start += insert.Length;
		TextDisplayName.SelectionStart = start;
		TextDisplayName.Focus();
	}

	public static void SetOtherItem(MenuItem item) {
		OthersItem o = (OthersItem)item.DataContext;
		var name = (string)item.Tag;
		o.OthersName = name;
		o.OthersNameTip = (string)item.Header;
	}

	public static string GetInitialPath(params string[] paths) {
		string? currentPath = null;
		foreach (var path in paths) {
			if (path.Length > 0) {
				currentPath = path;
				break;
			}
		}
		var projectLocation = Instances.ProjectLocation;
		if (currentPath != null) {
			currentPath = currentPath.Replace('/', '\\');
			currentPath = projectLocation + currentPath;
			while (currentPath.StartsWith(projectLocation) && currentPath.Length > projectLocation.Length) {
				var parent = new DirectoryInfo(currentPath).Parent;
				if (parent != null && parent.Exists) {
					return parent.FullName;
				}
				currentPath = parent!.FullName;
			}
		}
		var historyModel = AccAddonHistory.Default.ChooseModelHistory;
		if (!string.IsNullOrEmpty(historyModel) && historyModel.StartsWith(projectLocation)) {
			if (Directory.Exists(historyModel)) {
				return historyModel;
			}
		}
		return Path.Combine(projectLocation, "vehicle");
	}

	//图标
	public static string? ChooseIcon(Window window) {
		try {
			string projectLocation = Instances.ProjectLocation;
			if (projectLocation.Length == 0)
				throw new(Util.GetString("MessageProjectLocationFirst"));
			var pathAcc = Paths.AccessoryDir(projectLocation);
			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				DefaultDirectory = pathAcc,
				DefaultExt = "tga",
				Title = Util.GetString("DialogTitleChooseIcon"),
				Filter = Util.GetFilter("DialogFilterChooseIcon")
			};
			if (!fileDialog.InitialDirectory.StartsWith(pathAcc))
				fileDialog.InitialDirectory = pathAcc;
			if (!Directory.Exists(pathAcc) && Directory.CreateDirectory(pathAcc) == null)//确保accessory文件夹存在
				throw new(Util.GetString("MessageCreateAccessoryFail"));
			if (fileDialog.ShowDialog() != true)
				return null;
			var path = fileDialog.FileName;
			if (!path.StartsWith(projectLocation) || path.Length == projectLocation.Length) //必须在项目内
				throw new(Util.GetString("MessageIconOutsideProject"));
			var iconFile = new DirectoryInfo(path);
			string pathCheck = path;
			pathCheck = pathCheck.Replace(projectLocation, "");
			if (pathCheck.Split(' ', '(', ')').Length > 1) //路径或文件名不能有空格或括号
				throw new(Util.GetString("MessageIconInvalidChar"));
			var siiIconLocation = CheckIconFileExistence(window, pathAcc, iconFile);
			if (siiIconLocation == null)
				return null;
			siiIconLocation = siiIconLocation.Replace(pathAcc, "");
			siiIconLocation = siiIconLocation.Replace('\\', '/');
			siiIconLocation = siiIconLocation[1..^4];
			return siiIconLocation;
		} catch (Exception ex) {
			MessageBox.Show(window, ex.Message);
		}
		return null;
	}

	/// <summary>
	/// 检查图标文件是否有对应的tobj和mat。
	/// 会从图标所在位置以及Accessory文件夹检查
	/// 输出：mat文件的绝对路径
	/// </summary>
	public static string? CheckIconFileExistence(Window window, string pathAcc, DirectoryInfo iconFile) {//如果图标不在accessory文件夹内，就只能选择accessory文件夹生成
		string projectLocation = Instances.ProjectLocation;

		var iconParent = iconFile.Parent!.FullName;

		var deExt = iconFile.Name[..^4];
		var matName = deExt + ".mat";
		var tobjName = deExt + ".tobj";

		var iconLocationExist = File.Exists(iconParent + '\\' + matName) && File.Exists(iconParent + '\\' + tobjName);
		var accExist = File.Exists(pathAcc + '\\' + matName) && File.Exists(pathAcc + '\\' + tobjName);

		string siiIconLocation;
		if (iconLocationExist)
			siiIconLocation = iconParent + '\\' + matName;
		else if (accExist)
			siiIconLocation = pathAcc + '\\' + matName;
		else {
			var accessoryLocation = Paths.AccessoryDir(projectLocation);
			var notInAcc = !iconFile.FullName.StartsWith(accessoryLocation);
			var atAcc = string.Equals(iconParent, pathAcc);

			var dialog = new IconDefWIndow(notInAcc, atAcc) {
				Owner = window
			};

			string genLocation;
			var result = dialog.ShowDialog();
			if (result == true) {
				if (dialog.CreateOnAccessory)
					genLocation = pathAcc;
				else
					genLocation = iconParent;
			} else
				return null;
			{
				string iconLocation = iconFile.FullName.Replace(projectLocation, "");
				iconLocation = iconLocation.Replace('\\', '/');
				using StreamWriter sw = new(genLocation + '\\' + tobjName);
				sw.WriteLine("map 2d");
				sw.WriteLine($"    {iconLocation}");
				sw.WriteLine("addr");
				sw.WriteLine("    clamp_to_edge");
				sw.WriteLine("    clamp_to_edge");
				sw.WriteLine("nocompress");
			}
			siiIconLocation = genLocation + '\\' + matName;
			{
				using StreamWriter sw = new(genLocation + '\\' + matName);
				sw.WriteLine("material: \"ui\"");
				sw.WriteLine("{");
				sw.WriteLine($"\ttexture: \"{tobjName}\"");
				sw.WriteLine($"\ttexture_name: \"texture\"");
				sw.WriteLine("}");
			}
		}
		return siiIconLocation;
	}

	public static string? ChooseRope() => ChooseMaterial(MatTypeRope, Util.GetString("DialogTitleChooseRope"));
	public static string? ChoosePatch() => ChooseMaterial(MatTypePatch, Util.GetString("DialogTitleChoosePatch"));

	private const int MatTypeRope = 1;
	private const int MatTypePatch = 2;
	private static string? ChooseMaterial(int type, string title) {
		string projectLocation = Instances.ProjectLocation;
		if (projectLocation.Length == 0)
			throw new(Util.GetString("MessageProjectLocationFirst"));
		var fileDialog = new OpenFileDialog {
			Multiselect = false,
			DefaultDirectory = projectLocation,
			DefaultExt = "mat",
			Title = title,
			Filter = Util.GetFilter("DialogFilterChooseMaterial"),
		};
		if (!fileDialog.InitialDirectory.StartsWith(projectLocation))
			fileDialog.InitialDirectory = projectLocation;
		if (fileDialog.ShowDialog() != true)
			return null;
		string path = fileDialog.FileName;
		if (!path.StartsWith(projectLocation) || path.Length == projectLocation.Length)
			throw new(Util.GetString("MessageModelOutsideProject"));
		if (path.EndsWith(".tga") || path.EndsWith(".dds")) {
			var iconFile = new DirectoryInfo(path);
			string pathCheck = path;
			pathCheck = pathCheck.Replace(projectLocation, "");
			if (pathCheck.Split(' ', '(', ')').Length > 1) //路径或文件名不能有空格或括号
				throw new(Util.GetString("MessageIconInvalidChar"));
			var matLocation = CheckRopeMatExistence(type, iconFile);
			if (matLocation == null)
				return null;
			matLocation = matLocation.Replace(projectLocation, "");
			return matLocation.Replace('\\', '/');
		} else if (path.EndsWith(".mat")) {
			string inProjectPath = path.Replace(projectLocation, "");
			return inProjectPath.Replace('\\', '/');
		} else
			throw new(Util.GetString("MessageErrorChooseMaterial"));
	}

	private static string? CheckRopeMatExistence(int type, DirectoryInfo iconFile) {
		string projectLocation = Instances.ProjectLocation;

		var deExt = iconFile.FullName[..^4];
		var matPath = deExt + ".mat";
		var tobjPath = deExt + ".tobj";

		if (File.Exists(matPath) && File.Exists(tobjPath))
			return matPath;
		else {
			{
				string iconLocation = iconFile.FullName.Replace(projectLocation, "");
				iconLocation = iconLocation.Replace('\\', '/');
				using StreamWriter sw = new(tobjPath);
				sw.WriteLine("map 2d");
				sw.WriteLine($"    {iconLocation}");
				sw.WriteLine("addr");
				sw.WriteLine("    clamp_to_edge");
				sw.WriteLine("    clamp_to_edge");
			}
			tobjPath = new FileInfo(tobjPath).Name;
			{
				using StreamWriter sw = new(matPath);
				if (type == MatTypeRope)
					sw.WriteLine("material: \"eut2.dif.spec.shadow\"");
				else
					sw.WriteLine("material: \"ui\"");
				sw.WriteLine("{");
				sw.WriteLine($"\ttexture: \"{tobjPath}\"");
				if (type == MatTypeRope)
					sw.WriteLine($"\ttexture_name: \"texture_base\"");
				else
					sw.WriteLine($"\ttexture_name: \"texture\"");
				sw.WriteLine("}");
			}
		}
		return matPath;

	}
}
