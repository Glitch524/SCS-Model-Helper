using Microsoft.Win32;
using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory;
public static class AccessoryDataUtil {
	private static readonly ObservableCollection<StringResItem> mStringResList = [];
	public static ObservableCollection<StringResItem> StringResList {
		get {
			if (mStringResList.Count == 0) 
				SetupStringResList();
			return mStringResList;
		}
	}

	private static void SetupStringResList() {
		StringResItem head, separator;
		StringResItem separator2, openLocalization;
		if (mStringResList.Count == 0) {
			head = StringResItem.Head();
			separator = StringResItem.Separator();
			separator2 = StringResItem.Separator();
			openLocalization = StringResItem.OpenLocalization();
		} else {
			head = mStringResList.First();
			separator = mStringResList[1];
			separator2 = mStringResList[^2];
			openLocalization = mStringResList.Last();
			mStringResList.Clear();
		}
		mStringResList.Add(head);
		mStringResList.Add(separator);
		var dict = Instances.LocaleDict;
		if (dict.Count == 0) {
			mStringResList.Add(StringResItem.Empty());
		} else {
			foreach (var pair in dict) {
				StringBuilder tips = new();
				tips.AppendLine(Util.GetString("ResourceValueHeader"));
				for (int i = 0; i < pair.Value.Count; i++) {
					var value = pair.Value.ElementAt(i);
					tips.Append($"    {value}");
					if (i < pair.Value.Count - 1)
						tips.Append('\n');
				}
				StringResItem item = new(pair.Key, pair.Key, tips.ToString());
				StringResList.Add(item);
			}
		}
		
		mStringResList.Add(separator2);
		mStringResList.Add(openLocalization);
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

	public static void OpenLocalization(Window window) {
		var modLocalization = new ModLocalizationWindow() {
			Owner = window
		};
		modLocalization.ShowDialog();
		if (modLocalization.HasChanges) {
			SetupStringResList();
		}
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
	public static string? ChooseIcon(Window window, out string? iconPath) {
		iconPath = null;
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
			siiIconLocation = siiIconLocation[..^4];
			iconPath = iconFile.FullName;
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

	public static BitmapSource? LoadModelIconByIconName(string iconName) {
		if (iconName.Length == 0)
			return null;
		var projectLocation = Instances.ProjectLocation;
		iconName = iconName.Replace('/', '\\');
		var iconPath = Paths.AccessoryDir(projectLocation, iconName);
		var iconFile = iconPath + ".tga";
		if (File.Exists(iconFile)) 
			return LoadModelIcon(iconFile);
		iconFile = iconPath + ".dds";
		if (File.Exists(iconFile))
			return LoadModelIcon(iconFile);
		iconFile = iconPath + ".tobj";
		if (File.Exists(iconFile)) {
			using StreamReader sr = new(iconFile);
			string? line;
			while ((line = sr.ReadLine()?.Trim()) != null) {
				int start = line.IndexOf("/mate");
				if (start != -1) {
					int end = line.IndexOf(".tga");
					if (end == -1)
						end = line.IndexOf(".dds");
					end += 4;
					iconFile = line[start..end];
					iconFile = iconFile.Replace('/', '\\');
					iconFile = projectLocation + iconFile;
					return LoadModelIcon(iconFile);
				}
			}
		}
		return null;
	}

	public static BitmapSource? LoadModelIcon(string? iconPath) {
		if (iconPath == null || !File.Exists(iconPath))
			return null;
		return Util.LoadPfimIcon(iconPath);
	}


	public static string? ChooseRope() => ChooseMaterial(Util.GetString("DialogTitleChooseRope"));
	public static string? ChoosePatch() => ChooseMaterial(Util.GetString("DialogTitleChoosePatch"));
	private static string? ChooseMaterial(string title) {
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
			var matLocation = CheckRopeMatExistence(iconFile);
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

	private static string? CheckRopeMatExistence(DirectoryInfo iconFile) {
		string projectLocation = Instances.ProjectLocation;

		var deExt = iconFile.FullName[..^4];
		var matPath = deExt + ".mat";
		var tobjPath = deExt + ".tobj";

		if (File.Exists(matPath) && File.Exists(tobjPath))
			return matPath;
		else {
			{
				using StreamWriter sw = new(tobjPath);
				sw.WriteLine($"map	2d	{iconFile.Name}");
				sw.WriteLine("addr	clamp_to_edge	clamp_to_edge");
			}
			tobjPath = new FileInfo(tobjPath).Name;
			{
				using StreamWriter sw = new(matPath);
				sw.WriteLine("material: \"eut2.dif.spec.shadow\"");
				sw.WriteLine("{");
				sw.WriteLine($"\ttexture: \"{tobjPath}\"");
				sw.WriteLine($"\ttexture_name: \"texture_base\"");
				sw.WriteLine("}");
			}
		}
		return matPath;

	}
}

public class StringResItem: BaseBinding {

	public StringResItem(string header, string tag, string? tooltip = null) {
		IsSeparator = false;
		mHeader = header;
		mTag = tag;
		mTooltip = tooltip;
	}

	public static StringResItem Head() => new(Util.GetString("MenuResTip"), "head") { IsEnabled = false };

	public static StringResItem Separator() {
		return new();
	}

	public static StringResItem Empty() {
		return new(Util.GetString("StatusEmpty"), "empty") { IsEnabled = false };
	}

	public static StringResItem OpenLocalization() {
		return new(Util.GetString("MenuResOption"), "openLocalization");
	}

	private StringResItem() {
		IsSeparator = true;
		mHeader = "";
		mTag = "";
	}

	private readonly bool IsSeparator;

	public Visibility TextBlockVisibility => IsSeparator ? Visibility.Collapsed : Visibility.Visible;
	public Visibility SeparatorVisibility => IsSeparator ? Visibility.Visible : Visibility.Collapsed;

	private bool mIsEnabled = true;
	public bool IsEnabled {
		get => mIsEnabled;
		set {
			mIsEnabled = value;
			InvokeChange();
		}
	}

	private string mHeader;

	public string Header {
		get => mHeader;
		set {
			mHeader = value;
			InvokeChange();
		}
	}

	private string mTag;
	public string Tag {
		get => mTag;
		set {
			mTag = value;
			InvokeChange();
		}
	}

	private string? mTooltip;

	public string? Tooltip {
		get => mTooltip;
		set {
			mTooltip = value;
			InvokeChange();
		}
	}
}
