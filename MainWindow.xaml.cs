using Def_Writer.Locale;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Def_Writer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow: BaseWindow {
	private readonly ModelInfo ModelInfo = new();

	public string ProjectLocation {
		get => ModelInfo.ProjectLocation;
		set {
			ModelInfo.ProjectLocation = value;
			SetupStringResMenu();
		}
	}
	public string DisplayName {
		get => ModelInfo.DisplayName; set => ModelInfo.DisplayName = value;
	}
	public string Price {
		get => ModelInfo.Price; set => ModelInfo.Price = value;
	}
	public string UnlockLevel {
		get => ModelInfo.UnlockLevel; set => ModelInfo.UnlockLevel = value;
	}
	public string ModelPath {
		get => ModelInfo.ModelPath; set => ModelInfo.ModelPath = value;
	}
	public string ModelPathUK {
		get => ModelInfo.ModelPathUK; set => ModelInfo.ModelPathUK = value;
	}
	public string ModelName {
		get => ModelInfo.ModelName; set => ModelInfo.ModelName = value;
	}
	public string IconName {
		get => ModelInfo.IconName; set => ModelInfo.IconName = value;
	}
	public string ModelType {
		get => ModelInfo.ModelType; set => ModelInfo.ModelType = value;
	}

	public string Look {
		get => ModelInfo.Look; set => ModelInfo.Look = value;
	}
	public string Variant {
		get => ModelInfo.Variant; set => ModelInfo.Variant = value;
	}
	private bool DeleteUnchecked {
		get => ModelInfo.DeleteUnchecked; set => ModelInfo.DeleteUnchecked = value;
	}

	readonly ObservableCollection<Truck> trucksETS2 = [];
	readonly ObservableCollection<Truck> trucksATS = [];
	public readonly ObservableCollection<AccessoryInfo> accessoryType = [];
	public ObservableCollection<string> lookList = [];
	public ObservableCollection<string> variantList = [];

	private readonly ContextMenu MenuModelType, MenuLook, MenuVariant, MenuStringRes;

	public MainWindow() {
		InitializeComponent();
		Utils.MainWindow = this;
		Utils.InitLanguage();

		TextProjectLocation.DataContext = ModelInfo;
		TextDisplayName.DataContext = ModelInfo;
		TextPrice.DataContext = ModelInfo;
		TextUnlockLevel.DataContext = ModelInfo;

		TextModelPath.DataContext = ModelInfo;
		TextModelPathUK.DataContext = ModelInfo;
		TextModelType.DataContext = ModelInfo;
		TextAccName.DataContext = ModelInfo;
		TextLook.DataContext = ModelInfo;
		TextVariant.DataContext = ModelInfo;
		TextModelName.DataContext = ModelInfo;
		TextIconName.DataContext = ModelInfo;
		CheckDeleteUnchecked.DataContext = ModelInfo;

		MenuModelType = (ContextMenu)Resources["MenuModelType"];
		MenuLook = (ContextMenu)Resources["MenuLook"];
		MenuVariant = (ContextMenu)Resources["MenuVariant"];
		MenuStringRes = (ContextMenu)Resources["MenuStringRes"];
		SetupModelTypeMenu();

		accessoryType = DefaultData.Accessories;
		TextModelType.ItemsSource = accessoryType;
		TextLook.ItemsSource = lookList;
		TextVariant.ItemsSource = variantList;
		LoadLooksAndVariants();

		TableTrucksETS2.ItemsSource = trucksETS2;
		TableTrucksATS.ItemsSource = trucksATS;
		LoadTrucks();

		SetupStringResMenu();
		Closing += SaveOnClosing;
	}

	private void LoadLooksAndVariants(string? path = null) {
		string oldLook = Look, oldVariant = Variant;
		if (path == null) {
			if (ModelPath.Length > 0)
				path = ModelPath;
			else if (ModelPathUK.Length > 0)
				path = ModelPathUK;
			else
				return;
			path = path.Replace('/', '\\');
			path = path[..^4] + ".pit";
			path = ProjectLocation + path;
		} else if (path.EndsWith(".pim")) {
			path = path[..^4] + ".pit";
		}
		lookList.Clear();
		variantList.Clear();

		if (File.Exists(path)) {
			using StreamReader sr = new(path);
			string? line;
			do {
				line = sr.ReadLine();
			} while (line == null || line.Trim().Length == 0);
			line = line.Trim();
			if (line.StartsWith('#')) {
				do {
					line = line.Trim();
					static void ReadNames(StreamReader sr, ObservableCollection<string> list) {
						string? line;
						while ((line = sr.ReadLine()) != null) {
							line = line.Trim();
							if (line.StartsWith('#')) {
								line = line[1..].Trim();
								if (line.Length > 0)
									list.Add(line);
								else
									break;
							} else {
								break;
							}
						}
					}
					if (line.Contains("Look Names:", StringComparison.OrdinalIgnoreCase))
						ReadNames(sr, lookList);
					else if (line.Contains("Variant Names:", StringComparison.OrdinalIgnoreCase))
						ReadNames(sr, variantList);
				} while ((line = sr.ReadLine()) != null && line.Contains('#'));
			} else {
				int variantCount = -1;
				int variantFound = 0;
				while ((line = sr.ReadLine()?.Trim()) != null) {
					if (line.StartsWith("VariantCount:", StringComparison.OrdinalIgnoreCase)) {
						variantCount = int.Parse(line["VariantCount:".Length..].Trim());
					} else if (line.StartsWith("Look {", StringComparison.OrdinalIgnoreCase)) {
						ReadNamesAlt(sr, lookList);
					} else if (line.StartsWith("Variant {", StringComparison.OrdinalIgnoreCase)) {
						if (ReadNamesAlt(sr, variantList)) {
							variantFound++;
							if (variantCount > 0 && variantFound >= variantCount)
								break;
						}
					}
				}
			}
		}
		SetupLookAndVariantMenu();
		static void setValue(ObservableCollection<string> list, string oldValue, Action<string> set) {
			if (list.Count > 0) {
				if (list.Contains(oldValue)) {
					set(oldValue);
				} else
					set(list[0]);
			}
		}
		setValue(lookList, oldLook, (set) => Look = set);
		setValue(variantList, oldVariant, (set) => Variant = set);
	}


	private static bool ReadNamesAlt(StreamReader sr, ObservableCollection<string> list) {
		string? line;
		if ((line = sr.ReadLine()?.Trim()) != null && line.Contains("Name:", StringComparison.OrdinalIgnoreCase)) {
			int start = line.IndexOf('\"') + 1;
			int end = line.LastIndexOf('\"');
			var look = line[start..end];
			list.Add(look);
			return true;
		}
		return false;
	}

	private void SetupModelTypeMenu() {
		MenuModelType.Items.Clear();
		foreach(var acc in accessoryType) {
			MenuItem item = new() {
				Name = acc.Accessory,
				Header = $"{acc.Accessory.Replace("_", "__")}→{acc.Name}"
			};
			item.Click += OnMenuClicked;
			MenuModelType.Items.Add(item);
		}
		//MenuItem menuitembyname = LogicalTreeHelper.FindLogicalNode(menu, "ModifyGroupMenuItem") as MenuItem;
	}

	private void SetupLookAndVariantMenu() {
		MenuLook.Items.Clear();
		foreach (var look in lookList) {
			MenuItem item = new() {
				Name = look,
				Header = look.Replace("_", "__")
			};
			item.Click += OnMenuClicked;
			MenuLook.Items.Add(item);
		}
		MenuVariant.Items.Clear();
		foreach (var variant in variantList) {
			MenuItem item = new() {
				Name = variant,
				Header = variant.Replace("_", "__")
			};
			item.Click += OnMenuClicked;
			MenuVariant.Items.Add(item);
		}
	}
	
	private void SetupStringResMenu() {//ProjectLocation改变的时候都运行一次（除非前后的值一样）
		var res = LocaleUtil.GetStringRes(ProjectLocation);//value的value是重复次数，重复次数最多而且大于3的大概就是通用语言
		MenuStringRes.Items.Clear();
		MenuItem head = new() {
			Name = "head",
			Header = GetString("MenuResTip"),
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
				Header = GetString("StatusEmpty"),
				IsEnabled = false
			};
			MenuStringRes.Items.Add(empty);
		} else {
			foreach (var pair in res) {
				MenuItem item = new() {
					Name = pair.Key,
					Header = pair.Key.Replace("_", "__")
				};
				item.Click += OnMenuClicked;
				StringBuilder tips = new();
				tips.AppendLine(GetString("ResourceValueHeader"));
				for (int i = 0; i < pair.Value.Count; i++) {
					var value = pair.Value.ElementAt(i);
					tips.Append($"    {value.Key}");
					if (i < pair.Value.Count - 1)
						tips.Append('\n');
				}
				foreach (var value in pair.Value) {
				}
				item.ToolTip = tips.ToString();
				ToolTipService.SetInitialShowDelay(item, 500);
				
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
			Header = GetString("MenuResOption")
		};
		openLocalization.Click += OnMenuClicked;
		MenuStringRes.Items.Add(openLocalization);
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		MenuStringRes.Placement = PlacementMode.Top;
	}


	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent;
		if (cm == MenuModelType) {
			Truck truck = (Truck)item.DataContext;
			truck.ModelType = item.Name;
		} else if (cm == MenuLook) {
			Truck truck = (Truck)item.DataContext;
			truck.Look = item.Name;
		} else if (cm == MenuVariant) {
			Truck truck = (Truck)item.DataContext;
			truck.Variant = item.Name;
		} else if (cm == MenuStringRes) {
			if (item.Name.Equals("openLocalization")) {
				OpenLocalization();
			} else {
				DisplayName = $"@@{item.Name}@@";
			}
		}
		//TextBox caller = cm.PlacementTarget;
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) {
		MenuStringRes.IsOpen = true;
	}

	private void LoadTrucks() {
		trucksETS2.Clear();
		trucksATS.Clear();

		ForeachList(true, History.Default.TruckHistoryETS2, trucksETS2);
		ForeachList(false, History.Default.TruckHistoryATS, trucksATS);
	}

	void ForeachList(bool isEts2, string truckList, ObservableCollection<Truck> target) {
		truckList = "init";
		if (truckList.Length == 0 || truckList.Equals("init") || !truckList.Contains(DefaultData.itemSplit)) {
			foreach (Truck t in DefaultData.GetDefaultTrucks(isEts2)) {
				target.Add(t);
			}
			if (isEts2) {
				History.Default.TruckHistoryETS2 = Utils.JoinTruck(target);
			} else {
				History.Default.TruckHistoryATS = Utils.JoinTruck(target);
			}
			History.Default.Save();
			return;
		}
		var lines = truckList.Split(DefaultData.lineSplit);
		try {
			foreach (string line in lines) {
				if (line.Length == 0)
					continue;
				Truck? t = Truck.LineParse(accessoryType, line);
				if (t != null)
					target.Add(t);
			}
		} catch (Exception) {
			MessageBox.Show(GetString("MessageLoadTruckErr"));
			ForeachList(isEts2, "init", target);
		}
	}

	private void SaveOnClosing(object? sender, CancelEventArgs e) {
		History.Default.ProjectLocation = ProjectLocation;
		History.Default.DisplayName = DisplayName;
		try {
			History.Default.Price = int.Parse(Price);
			History.Default.UnlockLevel = int.Parse(UnlockLevel);
		} catch {
		}
		History.Default.Icon = IconName;
		History.Default.ModelType = ModelType;
		History.Default.Look = Look;
		History.Default.Variant = Variant;
		History.Default.ModelName = ModelName;
		History.Default.ModelPath = ModelPath;
		History.Default.ModelPathUK = ModelPathUK;

		History.Default.TruckHistoryETS2 = Utils.JoinTruck(trucksETS2);
		History.Default.TruckHistoryATS = Utils.JoinTruck(trucksATS);

		History.Default.Save();
	}

	private void ChooseProjectLocation(object sender, RoutedEventArgs e) {
		var folderDialog = new OpenFolderDialog {
			Multiselect = false
		};
		var history = History.Default.ProjectLocation;
		if (history.Length > 0) {
			if (Directory.Exists(history)) {
				folderDialog.InitialDirectory = history;
			} else {
				try {
					while (history.Length > 0) {
						history = new DirectoryInfo(history).Parent!.FullName;
						if (Directory.Exists(history)) {
							folderDialog.InitialDirectory = history;
							break;
						}
					}
				} catch (Exception) {}
			}
		}
		if (folderDialog.ShowDialog() == true) {
			History.Default.ProjectLocation = folderDialog.FolderName;
			History.Default.Save();
			ProjectLocation = folderDialog.FolderName;
			CheckIfNeedPrepare(folderDialog.FolderName);
			var result = MessageBox.Show(this, GetString("MessageCleanupAsk"), "注意", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				DisplayName = "";
				IconName = "";
				Price = "1";
				UnlockLevel = "0";
				ModelPath = "";
				ModelPathUK = "";
				ModelName = "";
				ModelType = "";
				Look = "";
				Variant = "";
				static void ClearList(ObservableCollection<Truck> trucks) {
					foreach (Truck truck in trucks) {
						truck.Check = false;
						truck.Look = "";
						truck.ModelType = "";
						truck.Variant = "";
					}
				}
				ClearList(trucksETS2);
				ClearList(trucksATS);
			}
		}
	}

	private void CheckIfNeedPrepare(string location) {
		if (Directory.Exists(location)) {
			if(!File.Exists(Utils.ManifestFile(location))) {
				var result = MessageBox.Show(this, GetString("MessageNoManifest"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					ProjectPreparation prepare = new(location) {
						Owner = this
					};
					prepare.ShowDialog();
				}
			}
		}
	}

	private void ClearButtonVisibility(object sender, TextChangedEventArgs e) {
		if (sender is TextBox box) {
			Button clear;
			switch (box.Name) {
				case "TextIconName":
					clear = ButtonIconNameClear;
					break;
				case "TextModelPath":
					clear = ButtonChooseModelClear;
					break;
				case "TextModelPathUK":
					clear = ButtonChooseModelUKClear;
					break;
				default:
					return;
			}
			clear.Visibility = box.Text.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender is Button clear) {
			switch (clear.Name) {
				case "ButtonIconNameClear":
					IconName = "";
					break;
				case "ButtonChooseModelClear":
					ModelPath = "";
					break;
				case "ButtonChooseModelUKClear":
					ModelPathUK = "";
					break;
				default:
					return;
			}
		}
	}

	private void ButtonChooseIcon(object sender, RoutedEventArgs e) {
		if (ProjectLocation.Length == 0) {
			MessageBox.Show(this, GetString("MessageProjectLocationFirst"));
			return;
		}
		var pathAcc = Utils.AccessoryDir(ProjectLocation);
		var fileDialog = new OpenFileDialog {
			Multiselect = false,
			DefaultDirectory = pathAcc,
			DefaultExt = "tga",
			Title = GetString("DialogTitleChooseIcon"),
			Filter = GetString("DialogFilterChooseIcon")
		};
		if (!fileDialog.InitialDirectory.StartsWith(pathAcc))
			fileDialog.InitialDirectory = pathAcc;
		if (!Directory.Exists(pathAcc)) {
			if (Directory.CreateDirectory(pathAcc) == null) {
				MessageBox.Show(this, GetString("MessageCreateAccessoryFail"));
				return;
			}
		}
		if (fileDialog.ShowDialog() == true) {
			var path = fileDialog.FileName;
			if (path.StartsWith(ProjectLocation) && path.Length > ProjectLocation.Length) {
				var iconFile = new DirectoryInfo(path);

				string pathCheck = path;
				pathCheck = pathCheck.Replace(ProjectLocation, "");
				if (pathCheck.Split(' ', '(', ')').Length > 1) {
					MessageBox.Show(this, GetString("MessageIconInvalidChar"));
					return;
				}

				var siiIconLocation = CheckIconFileExistence(pathAcc, iconFile);
				if (siiIconLocation == null)
					return;
				siiIconLocation = siiIconLocation.Replace(pathAcc, "");
				siiIconLocation = siiIconLocation.Replace('\\', '/');
				siiIconLocation = siiIconLocation[1..^5];
				IconName = siiIconLocation;
			} else
				MessageBox.Show(this, GetString("MessageIconOutsideProject"));
		}
	}


	/// <summary>
	/// 检查图标文件是否有对应的tobj和mat。
	/// 会从图标所在位置以及Accessory文件夹检查
	/// </summary>
	private string? CheckIconFileExistence(string pathAcc, DirectoryInfo iconFile) {//如果图标不在accessory文件夹内，就只能选择accessory文件夹生成
		var iconParent = iconFile.Parent!.FullName;

		var deExt = iconFile.Name[..^4];
		var matName = deExt + ".mat";
		var tobjName = deExt + ".tobj";

		var iconLocationExist = File.Exists(iconParent + '\\' + matName) && File.Exists(iconParent + '\\' + tobjName);
		var accExist = File.Exists(pathAcc + '\\' + matName) && File.Exists(pathAcc + '\\' + tobjName);

		string siiIconLocation;
		if (iconLocationExist)
			siiIconLocation = iconParent + '\\' + tobjName;
		else if (accExist)
			siiIconLocation = pathAcc + '\\' + tobjName;
		else {
			var locationEqual = String.Equals(iconParent, pathAcc);

			var dialog = new CreateIconFiles(!locationEqual) {
				Owner = this
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
			siiIconLocation = genLocation + '\\' + tobjName;
			{
				using StreamWriter sw = new(genLocation + '\\' + matName);
				sw.WriteLine("material: \"ui\"");
				sw.WriteLine("{");
				sw.WriteLine($"\ttexture: \"{tobjName}\"");
				sw.WriteLine($"\ttexture_name: \"texture\"");
				sw.WriteLine("}");
			}
			{
				string iconLocation = iconFile.FullName.Replace(ProjectLocation, "");
				iconLocation = iconLocation.Replace('\\', '/');
				using StreamWriter sw = new(siiIconLocation);
				sw.WriteLine("map 2d");
				sw.WriteLine($"    {iconLocation}");
				sw.WriteLine("addr");
				sw.WriteLine("    clamp_to_edge");
				sw.WriteLine("    clamp_to_edge");
				sw.WriteLine("nocompress");
			}
		}
		return siiIconLocation;
	}

	private void ButtonChooseModelClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			var clickUK = button == ButtonChooseModelUK;
			if (ProjectLocation.Length == 0) {
				MessageBox.Show(this, GetString("MessageProjectLocationFirst"));
				return;
			}
			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				DefaultDirectory = ProjectLocation,
				DefaultExt = "pmd",
				Title = GetString(clickUK ? "DialogTitleChooseModel" : "DialogTitleChooseModelUK"),
				Filter = GetString("DialogFilterChooseModel"),
				InitialDirectory = GetInitialPath()
			};
			if (fileDialog.ShowDialog() != true)
				return;
			string path = fileDialog.FileName;
			if (path.StartsWith(ProjectLocation) && path.Length > ProjectLocation.Length) {
				if (path.EndsWith(".pim") || path.EndsWith(".pmd")) {
					History.Default.ModelLocation = new DirectoryInfo(fileDialog.FileName).Parent!.FullName;
					History.Default.Save();
					LoadLooksAndVariants(path);//修改模型路径后，look和variant都不同，需要重新读取
					string inProjectPath = path.Replace(ProjectLocation, "");
					{
						var s = inProjectPath.Split('\\');
						for (int i = s.Length - 2; i > 0; i--) {
							foreach (AccessoryInfo info in accessoryType) {//根据路径猜测模型的类型
								if (info.Accessory.Equals(s[i]))
									ModelType = info.Accessory;
							}
						}
					}
					string?
						modelPath = null,
						modelName = null,
						modelPathUK = null;
					if (clickUK) {
						modelPathUK = inProjectPath.Replace('\\', '/');
						modelPathUK = modelPathUK[..^4];
						if (modelPathUK.EndsWith("_uk")) {
							modelName = modelPathUK[(modelPathUK.LastIndexOf('/') + 1)..^3];
							if (File.Exists(path[..^7] + ".pim") || File.Exists(path[..^7] + ".pmd"))
								modelPath = modelPathUK[..^3] + ".pmd";
						}
						modelPathUK += ".pmd";
					} else {
						modelPath = inProjectPath.Replace('\\', '/');
						modelPath = modelPath[..^4];
						modelName = modelPath[(modelPath.LastIndexOf('/') + 1)..];
						if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
							modelPathUK = modelPath + "_uk.pmd";
						modelPath += ".pmd";
					}
					if (modelPath != null)
						ModelPath = modelPath;
					if (modelPathUK != null)
						ModelPathUK = modelPathUK;
					if (modelName != null)
						ModelName = modelName;
				} else {
					MessageBox.Show(this, GetString("MessageInvalidExt"));
				}
			} else
				MessageBox.Show(this, GetString("MessageModelOutsideProject"));
		}
	}

	private string GetInitialPath() {
		string? currentPath = null;
		if (ModelPath.Length > 0) {
			currentPath = ModelPath;
		} else if (ModelPathUK.Length > 0) {
			currentPath = ModelPathUK;
		}
		if (currentPath != null) {
			currentPath = currentPath.Replace('/', '\\');
			currentPath = ProjectLocation + currentPath;
			while(currentPath.StartsWith(ProjectLocation) && currentPath.Length > ProjectLocation.Length) {
				var parent = new DirectoryInfo(currentPath).Parent;
				if (parent != null && parent.Exists) {
					return parent.FullName;
				}
				currentPath = parent!.FullName;
			}
		}
		var historyModel = History.Default.ModelLocation;
		if (!string.IsNullOrEmpty(historyModel) && historyModel.StartsWith(ProjectLocation)) {
			if (Directory.Exists(historyModel)) {
				return historyModel;
			}
		}
		return ProjectLocation + @"\vehicle";
	}

	private void ButtonStartClick(object sender, RoutedEventArgs e) {
		if (!ExpanderETS2.IsExpanded && !ExpanderATS.IsExpanded) {
			MessageBox.Show(this, GetString("MessageCreateSiiNoExpanded"));
			return;
		}
		if (ProjectLocation.Length == 0 ||
			DisplayName.Length == 0 ||
			Price.Length == 0 ||
			UnlockLevel.Length == 0 ||
			ModelPath.Length == 0 ||
			ModelName.Length == 0 ||
			IconName.Length == 0) {
			MessageBox.Show(this, GetStringMultiLine("MessageCreateSiiNotFilled", "MessageCreateSiiNotFilled2"));
			return;
		}
		try {
			if (int.Parse(Price) == 0) {
				MessageBox.Show(this, GetString("ModelPriceErrNot0"));
				return;
			}
			int.Parse(UnlockLevel);
		} catch {
			MessageBox.Show(this, GetString("ModelPriceErrNoNumber"));
			return;
		}

		var created = 0;
		if (ExpanderETS2.IsExpanded)
			created += CreateSii(true);
		if (ExpanderATS.IsExpanded)
			created += CreateSii(false);
		MessageBox.Show(this, GetString(created == 0 ? "MessageCreateSiiZero" : "MessageCreateSiiResult"));
	}

	private int CreateSii(bool isETS2) {
		var numberCreated = 0;
		foreach (Truck truck in isETS2 ? trucksETS2 : trucksATS) {
			String siiFile = Utils.SiiFile(ProjectLocation, truck.TruckID, truck.ModelType, ModelName);
			if (!truck.Check) {
				if (DeleteUnchecked) {
					if (File.Exists(siiFile))
						File.Delete(siiFile);
				}
				continue;
			} else if (truck.ModelType.Length == 0 || truck.Look.Length == 0 || truck.Variant.Length == 0)
				continue;
			DirectoryInfo sii = new(siiFile);
			if (!sii.Parent!.Exists)
				sii.Parent!.Create();
			using StreamWriter sw = new(siiFile);
			sw.WriteLine("SiiNunit");
			sw.WriteLine("{");
			sw.WriteLine($"\taccessory_addon_int_data : {ModelName}.{truck.TruckID}.{truck.ModelType}");
			sw.WriteLine("\t{");
			sw.WriteLine($"\t\tname: \"{DisplayName}\"");
			sw.WriteLine($"\t\tprice: {Price}");
			sw.WriteLine($"\t\tunlock: {UnlockLevel}");
			sw.WriteLine($"\t\ticon: \"{IconName}\"");
			sw.WriteLine($"\t\texterior_model: \"{ModelPath}\"");
			sw.WriteLine($"\t\tinterior_model: \"{ModelPath}\"");
			if (isETS2 && ModelPathUK.Length > 0) {
				sw.WriteLine($"\t\texterior_model_uk: \"{ModelPathUK}\"");
				sw.WriteLine($"\t\tinterior_model_uk: \"{ModelPathUK}\"");
			}
			sw.WriteLine($"\t\tlook: {truck.Look}");
			sw.WriteLine($"\t\tvariant: {truck.Variant}");
			sw.WriteLine("\t}");
			sw.WriteLine("}");
			numberCreated++;
		}
		return numberCreated;
	}

	private void ButtonPrepareClick(object sender, RoutedEventArgs e) {
		ProjectPreparation pp = new(ProjectLocation) {
			Owner = this
		};
		pp.ShowDialog();
	}

	string? LoadedFilename = null;
	private void ButtonSaveClick(object sender, RoutedEventArgs e) {
		SaveFileDialog saveFileDialog = new() {
			Title = GetString("SaveDED"),
			AddExtension = true,
			DefaultExt = "ded",
			Filter = GetString("DialogFilterDED"),
			InitialDirectory = History.Default.DEDLocation,
		};
		if (LoadedFilename == null) {
			if (DisplayName.Length > 0 && ModelType.Length > 0 && ModelName.Length > 0)
				saveFileDialog.FileName = $"{DisplayName} {ModelType} {ModelName}";
		} else
			saveFileDialog.FileName = LoadedFilename;
		if (saveFileDialog.ShowDialog() == true) {
			History.Default.DEDLocation = new DirectoryInfo(saveFileDialog.FileName).Parent!.FullName;
			using StreamWriter sw = new(saveFileDialog.FileName);
			sw.WriteLine("Def Writer");
			sw.WriteLine("{");
			sw.WriteLine($"\tProjectLocation |=|{ProjectLocation}");
			sw.WriteLine($"\tDisplayName |=|{DisplayName}");
			sw.WriteLine($"\tPrice |=|{Price}");
			sw.WriteLine($"\tUnlockLevel |=|{UnlockLevel}");
			sw.WriteLine($"\tModelPath |=|{ModelPath}");
			sw.WriteLine($"\tModelPathUK |=|{ModelPathUK}");
			sw.WriteLine($"\tModelName |=|{ModelName}");
			sw.WriteLine($"\tIconName |=|{IconName}");
			sw.WriteLine($"\tModelType |=|{ModelType}");
			sw.WriteLine($"\tLook |=|{TextLook.Text}");
			sw.WriteLine($"\tVariant |=|{TextVariant.Text}");
			sw.WriteLine($"\tSelectedHeader |=|{Truck.DEDHeader()}");
			sw.WriteLine($"\tSelectedETS2 |=|{Utils.JoinDED(trucksETS2)}");
			sw.WriteLine($"\tSelectedATS |=|{Utils.JoinDED(trucksATS)}");
			sw.WriteLine("}");
			MessageBox.Show(this, GetString("MessageSaveDED"));
		}
	}

	private void ButtonLoadClick(object sender, RoutedEventArgs e) {
		OpenFileDialog openFileDialog = new() {
			Title = GetString("LoadDED"),
			Multiselect = false,
			AddExtension = true,
			InitialDirectory = History.Default.DEDLocation,
			DefaultExt = "ded",
			Filter = GetString("DialogFilterDED"),
		};
		if (openFileDialog.ShowDialog() == true) {
			try {
				History.Default.DEDLocation = new DirectoryInfo(openFileDialog.FileName).Parent!.FullName;
				LoadedFilename = openFileDialog.SafeFileName;
				using StreamReader sr = new(openFileDialog.FileName);
				string? line = sr.ReadLine();
				if (line == null || !line.Equals("Def Writer"))
					throw new(GetString("MessageLoadDEDErrNotDED"));
				DisplayName = "";
				IconName = "";
				Price = "1";
				UnlockLevel = "0";
				ModelPath = "";
				ModelPathUK = "";
				ModelName = "";
				ModelType = "";
				Look = "";
				Variant = "";
				SelectAllETS2.IsChecked = false;
				SelectAllATS.IsChecked = false;
				List<string> dedHeaders = [];
				while ((line = sr.ReadLine()) != null) {
					if (line.Contains("|=|")) {
						var read = line.Split("|=|");
						if (read.Length > 2)
							read[1] = String.Join("|=|", read, 1, read.Length - 1);
						switch (read[0].Trim().ToLower()) {
							case "projectlocation":
								ProjectLocation = read[1];
								break;
							case "displayname":
								DisplayName = read[1];
								break;
							case "price":
								Price = read[1];
								break;
							case "unlocklevel":
								UnlockLevel = read[1];
								break;
							case "modelpath":
								ModelPath = read[1];
								break;
							case "modelpathuk":
								ModelPathUK = read[1];
								break;
							case "modelname":
								ModelName = read[1];
								break;
							case "iconname":
								IconName = read[1];
								break;
							case "modeltype":
								ModelType = read[1];
								break;
							case "look":
								Look = read[1];
								break;
							case "variant":
								Variant = read[1];
								break;
							case "selectedheader":
								dedHeaders.AddRange(read[1].Split(DefaultData.itemSplit));
								break;
							case "selectedets2":
								SetSelected(dedHeaders, read[1], trucksETS2);
								break;
							case "selectedats":
								SetSelected(dedHeaders, read[1], trucksATS);
								break;
						}
					}
				}
				LoadLooksAndVariants();
			} catch (Exception ex) {
				MessageBox.Show(this, GetString("MessageLoadDEDErrFail") + "\n" + ex.Message);
			}
		}
	}

	private void ButtonCleanSiiClick(object sender, RoutedEventArgs e) {
		CleanSii cleanSii = new(ProjectLocation) {
			Owner = this
		};
		cleanSii.ShowDialog();
	}
		
	private static void SetSelected(List<string> dedHeaders, string read, ObservableCollection<Truck> truckList) {
		List<string> selected = [..read.Split(DefaultData.lineSplit)];
		foreach (Truck truck in truckList) {
			truck.Check = false;
			truck.ModelType = "";
			truck.Look = "";
			truck.Variant = "";
			foreach (string line in selected) {
				var s = line.Trim().Split(DefaultData.itemSplit);
				if (truck.TruckID.Equals(s[dedHeaders.IndexOf(nameof(truck.TruckID))])) {
					truck.Check = true;
					GetData(dedHeaders, nameof(truck.ModelType), s, (v) => truck.ModelType = v);
					GetData(dedHeaders, nameof(truck.Look), s, (v) => truck.Look = v);
					GetData(dedHeaders, nameof(truck.Variant), s, (v) => truck.Variant = v);
					selected.Remove(line);
					break;
				}
			}
		}
	}

	private static void GetData(List<string> dedHeaders, string name, string[] s, Action<string> setter) {
		var index = dedHeaders.IndexOf(name);
		if (index == -1)
			return;
		setter(s[index]);
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) {
		e.Handled = RegexNumber().IsMatch(e.Text);
	}

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();

	private void ButtonTruckInitialize(object sender, RoutedEventArgs e) {
		History.Default.TruckHistoryETS2 = "init";
		History.Default.TruckHistoryATS = "init";
		History.Default.Save();
		LoadTrucks();
	}

	private void ButtonAddTruckClick(object sender, RoutedEventArgs e) {
		var isETS2 = sender == ButtonAddTruckETS2;
		NewTruckWindow addTruck = new(isETS2, isETS2 ? trucksETS2 : trucksATS) {
			Owner = this
		};
		addTruck.ShowDialog();
	}

	private void ButtonDeleteTruckClick(object sender, RoutedEventArgs e) {
		if (TableTrucksETS2.SelectedIndex == -1 && TableTrucksATS.SelectedIndex == -1)
			return;
		var changed = false;
		if (sender == ButtonDeleteTruckETS2) {
			while (TableTrucksETS2.SelectedIndex != -1) {
				changed = true;
				var selected = trucksETS2[TableTrucksETS2.SelectedIndex];
				trucksETS2.Remove(selected);
			}
			TableTrucksETS2.UnselectAll();
			if (changed)
				History.Default.TruckHistoryETS2 = Utils.JoinTruck(trucksETS2);
		} else if (sender == ButtonDeleteTruckATS) {
			while (TableTrucksATS.SelectedIndex != -1) {
				changed = true;
				var selected = trucksATS[TableTrucksATS.SelectedIndex];
				trucksATS.Remove(selected);
			}
			TableTrucksATS.UnselectAll();
			if (changed)
				History.Default.TruckHistoryETS2 = Utils.JoinTruck(trucksATS);
		}
		History.Default.Save();
	}

	private void SelectAllChecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox box) {
			foreach (Truck truck in box == SelectAllETS2 ? trucksETS2 : trucksATS) {
				truck.Check = box.IsChecked == true;
				if (truck.Check) {
					if (truck.ModelType.Length == 0)
						truck.ModelType = ModelType;
					if (truck.Look.Length == 0)
						truck.Look = Look;
					if (truck.Variant.Length == 0)
						truck.Variant = Variant;
				}
			}
		}
	}

	private void ButtonCoverValue(object sender, RoutedEventArgs e) {
		bool setter(Truck truck) {
			if (sender == ButtonCoverModelType) {
				truck.ModelType = TextModelType.Text;
			} else if (sender == ButtonCoverLook) {
				truck.Look = TextLook.Text;
			} else if (sender == ButtonCoverVariant) {
				truck.Variant = TextVariant.Text;
			}
			return true;
		}
		if (ExpanderETS2.IsExpanded) {
			foreach (Truck t in trucksETS2) {
				if (t.Check)
					setter(t);
			}
		}
		if (ExpanderATS.IsExpanded) {
			foreach (Truck t in trucksATS) {
				if (t.Check)
					setter(t);
			}
		}
	}

	private void ModelTypeChanged(object sender, RoutedEventArgs e) {
		var combo = (ComboBox)sender;
		if (combo == TextModelType) {
			if (combo.SelectedValue == null) {
				TextAccName.Name = "";
			} else if (combo.SelectedValue is AccessoryInfo info)
				TextAccName.DataContext = info;
		}
	}

	private void TruckCheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox) sender;
		Truck selected = (Truck) box.DataContext;
		if (selected.Check) {
			if (selected.ModelType.Length == 0)
				selected.ModelType = ModelType;
			if (selected.Look.Length == 0)
				selected.Look = Look;
			if (selected.Variant.Length == 0)
				selected.Variant = Variant;
		}
	}

	private void ButtonAddonHookupClick(object sender, RoutedEventArgs e) {

    }

	private void ComboLanguage_Checked(object sender, RoutedEventArgs e) {
		Utils.SwitchLanguage(ComboLanguage.IsChecked == true ? Utils.LANG_ZH_CN : Utils.LANG_EN_US);

	}

	private void ButtonHookupClick(object sender, RoutedEventArgs e) {
		var createHookup = new CreateHookup(ProjectLocation) {
			Owner = this
		};
		createHookup.ShowDialog();
	}

	private void ButtonLocalizationClick(object sender, RoutedEventArgs e) {
		OpenLocalization();
	}

	private void OpenLocalization() {
		var modLocalization = new ModLocalization(ProjectLocation) {
			Owner = this
		};
		modLocalization.ShowDialog();
	}
}

public class ModelInfo: INotifyPropertyChanged {
	public ModelInfo() {
		MProjectLocation = History.Default.ProjectLocation;
		MDisplayName = History.Default.DisplayName;
		MPrice = History.Default.Price.ToString();
		MUnlockLevel = History.Default.UnlockLevel.ToString();
		MModelPath = History.Default.ModelPath;
		MModelPathUK = History.Default.ModelPathUK;
		MModelName = History.Default.ModelName;
		MIconName = History.Default.Icon;
		MModelType = History.Default.ModelType;
		MLook = History.Default.Look;
		MVariant = History.Default.Variant;
		MDeleteUnchecked = History.Default.DeleteUnchecked;
	}

	public string MProjectLocation;
	public string ProjectLocation {
		get {
			return MProjectLocation;
		}
		set {
			MProjectLocation = value;
			InvokeChange(nameof(ProjectLocation));
		}
	}

	public bool ProjectExist {
		get {
			return Directory.Exists(ProjectLocation);
		}
	}

	private string MDisplayName;
	public string DisplayName {
		get {
			return MDisplayName;
		}
		set {
			MDisplayName = value;
			InvokeChange(nameof(DisplayName));
		}
	}

	private string MPrice;
	public string Price {
		get {
			return MPrice;
		}
		set {
			MPrice = value;
			InvokeChange(nameof(Price));
		}
	}

	private string MUnlockLevel;
	public string UnlockLevel {
		get {
			return MUnlockLevel;
		}
		set {
			MUnlockLevel = value;
			InvokeChange(nameof(UnlockLevel));
		}
	}

	private string MModelPath;
	public string ModelPath {
		get {
			return MModelPath;
		}
		set {
			MModelPath = value;
			InvokeChange(nameof(ModelPath));
		}
	}

	private string MModelPathUK;
	public string ModelPathUK {
		get {
			return MModelPathUK;
		}
		set {
			MModelPathUK = value;
			InvokeChange(nameof(ModelPathUK));
		}
	}

	private string MModelName;
	public string ModelName {
		get {
			return MModelName;
		}
		set {
			MModelName = value;
			InvokeChange(nameof(ModelName));
		}
	}

	private string MIconName;
	public string IconName {
		get {
			return MIconName;
		}
		set {
			MIconName = value;
			InvokeChange(nameof(IconName));
		}
	}

	private string MModelType;
	public string ModelType {
		get {
			return MModelType;
		}
		set {
			MModelType = value;
			InvokeChange(nameof(ModelType));
		}
	}

	private string MLook;
	public string Look {
		get {
			return MLook;
		}
		set {
			MLook = value;
			InvokeChange(nameof(Look));
		}
	}

	private string MVariant;
	public string Variant {
		get {
			return MVariant;
		}
		set {
			MVariant = value;
			InvokeChange(nameof(Variant));
		}
	}

	private bool MDeleteUnchecked;
	public bool DeleteUnchecked {
		get {
			return MDeleteUnchecked;
		}
		set {
			MDeleteUnchecked = value;
			InvokeChange(nameof(DeleteUnchecked));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void InvokeChange(string name) {
		PropertyChanged?.Invoke(this, new(name));
	}
}
public class ProjectExistenceConverter: IValueConverter {
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		var v = (string)value;
		if (Directory.Exists(v))
			return new SolidColorBrush(Colors.Black);
		else
			return new SolidColorBrush(Colors.Red);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}
