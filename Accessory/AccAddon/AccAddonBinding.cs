using Microsoft.Win32;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon;
class AccAddonBinding: INotifyPropertyChanged {
	private readonly ModProject ModelProject = Instances.ModelProject;
	private readonly AccessoryAddonData AddonItem = new();

	public void SaveHistory() => AddonItem.SaveHistory();

	public string ProjectLocation => ModelProject.ProjectLocation;

	public string DisplayName {
		get => AddonItem.DisplayName;
		set {
			AddonItem.DisplayName = value;
			InvokeChange();
			InvokeChange(nameof(CheckResVisibility));
		}
	}

	private string mCheckResResult = "";
	public string CheckResResult {
		get => mCheckResResult;
		set {
			mCheckResResult = value;
			InvokeChange();
		}
	}

	private bool mPopupCheckOpen = false;
	public bool PopupCheckOpen {
		get => mPopupCheckOpen;
		set {
			mPopupCheckOpen = value;
			InvokeChange();
		}
	}

	public Visibility CheckResVisibility => AddonItem.CheckResVisibility;

	public void CheckNameStringRes() {
		CheckResResult = AccessoryDataUtil.GetStringResResults(DisplayName);
		PopupCheckOpen = true;
	}

	public string ModelName {
		get => AddonItem.ModelName;
		set {
			AddonItem.ModelName = value; 
			InvokeChange();
		}
	}

	public string PartType {
		get => AddonItem.PartType;
		set {
			AddonItem.PartType = value;
			InvokeChange();
		}
	}

	public static List<PartTypeItem> PartTypes => AccessoryData.PartTypes;

	public long? Price {
		get => AddonItem.Price;
		set {
			AddonItem.Price = value;
			InvokeChange();
		}
	}

	public uint? UnlockLevel {
		get => AddonItem.UnlockLevel;
		set {
			AddonItem.UnlockLevel = value;
			InvokeChange();
		}
	}

	public string IconName {
		get => AddonItem.IconName;
		set {
			AddonItem.IconName = value;
			InvokeChange();
		}
	}

	public string ModelPath {
		get => AddonItem.ModelPath;
		set {
			AddonItem.ModelPath = value;
			InvokeChange();
		}
	}

	public string ModelPathUK {
		get => AddonItem.ModelPathUK;
		set {
			AddonItem.ModelPathUK = value;
			InvokeChange();
		}
	}

	public string CollPath {
		get => AddonItem.CollPath;
		set {
			AddonItem.CollPath = value;
			InvokeChange();
		}
	}

	private bool mUseCollPath = false;
	public bool UseCollPath {
		get => mUseCollPath;
		set {
			mUseCollPath = value;
			InvokeChange(nameof(UseCollPath));
		}
	}

	public string ModelType {
		get => AddonItem.ModelType;
		set {
			AddonItem.ModelType = value;
			if (CurrentModelType != null && value != CurrentModelType.Accessory) {
				CurrentModelType = null;
			}
			InvokeChange();
		}
	}

	//配件类型、look、variant列表以及菜单
	private ObservableCollection<ModelTypeInfo>? mModelTypes = null;
	public ObservableCollection<ModelTypeInfo> ModelTypes {
		get {
			mModelTypes ??= DefaultData.ModelTypes;
			return mModelTypes;
		}
	}

	private readonly ModelTypeInfo emptyAcc = new();

	private ModelTypeInfo? mCurrentModelType = null;
	public ModelTypeInfo? CurrentModelType {
		get => mCurrentModelType ?? emptyAcc;
		set {
			mCurrentModelType = value ?? emptyAcc;
			InvokeChange();
		}
	}

	public ObservableCollection<string> LookList => AddonItem.LookList;
	public ObservableCollection<string> VariantList => AddonItem.VariantList;
	public string Look {
		get => AddonItem.Look;
		set {
			AddonItem.Look = value;
			InvokeChange();
		}
	}
	public string Variant {
		get => AddonItem.Variant;
		set {
			AddonItem.Variant = value;
			InvokeChange();
		}
	}
	public uint HideIn {
		get => AddonItem.HideIn;
		set {
			AddonItem.HideIn = value;
			InvokeChange();
		}
	}

	public ObservableCollection<OthersItem> OthersList => AddonItem.OthersList;

	public List<PhysicsToyData> PhysicsList => AddonItem.PhysicsList;

	public void LoadLooksAndVariants(string? path = null) {
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
		LookList.Clear();
		VariantList.Clear();

		AccDataIO.ReadLookAndVariant(path, LookList, VariantList);

		static void setValue(ObservableCollection<string> list, string oldValue, Action<string> set) {
			if (list.Count > 0) {
				if (list.Contains(oldValue)) {
					set(oldValue);
				} else
					set(list[0]);
			}
		}
		setValue(LookList, oldLook, (set) => Look = set);
		setValue(VariantList, oldVariant, (set) => Variant = set);
	}

	public void SetupLookAndVariantMenu(Action<MenuItem> actionLook, Action<MenuItem> actionVariant) {
		foreach (var look in LookList) {
			MenuItem item = new() {
				Name = look,
				Header = look.Replace("_", "__")
			};
			actionLook(item);
		}
		foreach (var variant in VariantList) {
			MenuItem item = new() {
				Name = variant,
				Header = variant.Replace("_", "__")
			};
			actionVariant(item);
		}
	}

	private bool mTruckExpandedETS2 = true;
	public bool TruckExpandedETS2 {
		get => mTruckExpandedETS2;
		set {
			mTruckExpandedETS2 = value;
			InvokeChange(nameof(TruckExpandedETS2));

			PopupAddTruckOpen = false;
		}
	}
	private bool mTruckExpandedATS = true;
	public bool TruckExpandedATS {
		get => mTruckExpandedATS;
		set {
			mTruckExpandedATS = value;
			InvokeChange(nameof(TruckExpandedATS));

			PopupAddTruckOpen = false;
		}
	}

	private bool mPopupAddTruckOpen = false;
	public bool PopupAddTruckOpen {
		get => mPopupAddTruckOpen;
		set {
			mPopupAddTruckOpen = value;
			InvokeChange(nameof(PopupAddTruckOpen));
		}
	}

	private bool mSelectAllETS2 = false;
	public bool SelectAllETS2 {
		get => mSelectAllETS2;
		set {
			mSelectAllETS2 = value;
			InvokeChange(nameof(SelectAllETS2));
			SelectAllTruck(TrucksETS2, value);
		}
	}

	private bool mSelectAllATS = false;
	public bool SelectAllATS {
		get => mSelectAllATS;
		set {
			mSelectAllATS = value;
			InvokeChange(nameof(SelectAllATS));
			SelectAllTruck(TrucksATS, value);
		}
	}

	public void SelectAllTruck(ObservableCollection<Truck> trucks, bool check) {
		foreach (Truck truck in trucks) {
			truck.Check = check;
			OverwriteEmpty(truck, check);
		}
	}

	public void OverwriteEmpty(Truck truck, bool check) {
		if (check) {
			if (truck.ModelType.Length == 0)
				truck.ModelType = ModelType;
			if (truck.Look.Length == 0)
				truck.Look = Look;
			if (truck.Variant.Length == 0)
				truck.Variant = Variant;
		}
	}

	public ObservableCollection<Truck> TrucksETS2 => AddonItem.TrucksETS2;
	public ObservableCollection<Truck> TrucksATS => AddonItem.TrucksATS;


	public void LoadTrucks() {
		ForeachList(AccAddonHistory.Default.TruckHistoryETS2, TrucksETS2);
		ForeachList(AccAddonHistory.Default.TruckHistoryATS, TrucksATS);
	}

	void ForeachList(string truckList, ObservableCollection<Truck> target) {
		target.Clear();
		if (truckList.Length == 0 || truckList.Equals("init") || !truckList.Contains(DefaultData.ItemSplit)) {
			foreach (Truck t in DefaultData.GetDefaultTrucks(target == TrucksETS2)) {
				target.Add(t);
			}
			return;
		}
		var lines = truckList.Split(DefaultData.LineSplit);
		try {
			foreach (string line in lines) {
				if (line.Length == 0)
					continue;
				Truck? t = Truck.LineParse(line);
				if (t != null)
					target.Add(t);
			}
		} catch (Exception) {
			MessageBox.Show(Util.GetString("MessageLoadTruckErr"));
			ForeachList("init", target);
		}
	}

	string? LoadedFilename = null;
	public void SaveDED() {
		PopupAddTruckOpen = false;
		SaveFileDialog saveFileDialog = new() {
			Title = Util.GetString("SaveDED"),
			AddExtension = true,
			DefaultDirectory = Paths.DefaultDEDDir(),
			InitialDirectory = GetDEDInitDir(),
			DefaultExt = "ded",
			Filter = Util.GetFilter("DialogFilterDED"),
		};
		if (LoadedFilename == null) {
			if (DisplayName.Length > 0 && ModelType.Length > 0 && ModelName.Length > 0)
				saveFileDialog.FileName = $"{DisplayName} {ModelType} {ModelName}";
		} else
			saveFileDialog.FileName = LoadedFilename;
		if (saveFileDialog.ShowDialog() == true) {
			SaveDedLocation(saveFileDialog.FileName);
			AccAppIO.SaveAccAddon(this, saveFileDialog.FileName);
			MessageBox.Show(Util.GetString("MessageSaveDED"));
		}
	}

	public void LoadDED() {
		PopupAddTruckOpen = false;
		OpenFileDialog openFileDialog = new() {
			Title = Util.GetString("LoadDED"),
			Multiselect = false,
			AddExtension = true,
			DefaultDirectory = Paths.DefaultDEDDir(),
			InitialDirectory = GetDEDInitDir(),
			DefaultExt = "ded",
			Filter = Util.GetFilter("DialogFilterDED"),
		};
		if (openFileDialog.ShowDialog() == true) {
			SaveDedLocation(openFileDialog.FileName);
			LoadedFilename = openFileDialog.SafeFileName;
			AccAppIO.LoadAccAddon(this, openFileDialog.FileName);
		}
	}

	private static string GetDEDInitDir() {
		var dedPath = AccAddonHistory.Default.DEDLocation;
		if (dedPath.Length == 0) {
			dedPath = Paths.DefaultDEDDir();
			Directory.CreateDirectory(dedPath);
		}
		return dedPath;
	}

	private static void SaveDedLocation(string filename) {
		var dir = new DirectoryInfo(filename).Parent!.FullName;
		if (dir == Paths.DefaultDEDDir()) {
			AccAddonHistory.Default.DEDLocation = "";
		} else {
			AccAddonHistory.Default.DEDLocation = dir;
		}
		AccAddonHistory.Default.Save();
	}

	private string mAddTruckID = string.Empty;
	public string AddTruckID {
		get => mAddTruckID;
		set {
			mAddTruckID = value;
			InvokeChange(nameof(AddTruckID));
		}
	}
	private string mAddTruckIngameName = string.Empty;
	public string AddTruckIngameName {
		get => mAddTruckIngameName;
		set {
			mAddTruckIngameName = value;
			InvokeChange(nameof(AddTruckIngameName));
		}
	}
	private string mAddTruckDescription = string.Empty;
	public string AddTruckDescription {
		get => mAddTruckDescription;
		set {
			mAddTruckDescription = value;
			InvokeChange(nameof(AddTruckDescription));
		}
	}

	public void AddNewTruck(bool isETS2) {
		try {
			ObservableCollection<Truck> Trucks = isETS2 ? TrucksETS2 : TrucksATS;
			if (AddTruckID.Length == 0 || AddTruckIngameName.Length == 0)
				throw new(Util.GetString("MessageAddErrNotFilled"));
			if (AddTruckID[0] <= 'm') {
				for (int i = 0; i < Trucks.Count; i++) {
					var cResult = string.Compare(AddTruckID, Trucks[i].TruckID);
					if (cResult == 0) {
						throw new(Util.GetString("MessageAddErrSameID"));
					} else if (cResult < 0) {
						Trucks.Insert(i, new(AddTruckID, AddTruckIngameName, AddTruckDescription, false));
						break;
					}
				}
			} else {
				for (int i = Trucks.Count - 1; i >= 0; i--) {
					var cResult = string.Compare(AddTruckID, Trucks[i].TruckID);
					if (cResult == 0) {
						throw new(Util.GetString("MessageAddErrSameID"));
					} else if (cResult > 0) {
						Trucks.Insert(i + 1, new(AddTruckID, AddTruckIngameName, AddTruckDescription, false));
						break;
					}
				}
			}
			if (isETS2)
				AccAddonHistory.Default.TruckHistoryETS2 = Truck.JoinTruck(Trucks);
			else
				AccAddonHistory.Default.TruckHistoryATS = Truck.JoinTruck(Trucks);
			AccAddonHistory.Default.Save();
		} catch (Exception ex) {
			MessageBox.Show(ex.Message, Util.GetString("MessageTitleErr"));
		}
	}



	private bool mDeleteUnchecked;
	public bool DeleteUnchecked {
		get => mDeleteUnchecked;
		set {
			mDeleteUnchecked = value;
			InvokeChange();
		}
	}

	public void StartCreateSii() {
		try {
			PopupAddTruckOpen = false;
			if (!TruckExpandedETS2 && !TruckExpandedATS)
				throw new(Util.GetString("MessageCreateSiiNoExpanded"));
			if (ProjectLocation.Length == 0 ||
				DisplayName.Length == 0 ||
				Price == null ||
				UnlockLevel == null ||
				ModelPath.Length == 0 ||
				ModelName.Length == 0 ||
				IconName.Length == 0) {
				throw new(Util.GetString("MessageCreateSiiNotFilled"));
			}
			try {
				if (Price == 0)
					throw new(Util.GetString("ModelPriceErrNot0"));
			} catch {
				throw new(Util.GetString("ModelPriceErrNoNumber"));
			}
			var created = AccDataIO.CreateAccAddonSii(this);
			MessageBox.Show(Util.GetString(created == 0 ? "MessageCreateSiiZero" : "MessageCreateSiiResult"));
		} catch (Exception ex) {
			MessageBox.Show(ex.Message);
		}
	}

	public void ChooseIcon(Window window) {
		var icon = AccessoryDataUtil.ChooseIcon(window);
		if (icon != null)
			IconName = icon;
	}

	public const int MODEL = 0;
	public const int MODEL_UK = 1;
	public const int MODEL_COLL = 2;
	//模型、模型UK、碰撞体
	public void ChooseModel(Window window, int type) {
		if (ProjectLocation.Length == 0)
			throw new(Util.GetString("MessageProjectLocationFirst"));
		string title, fileter, defaultExt;
		if (type == MODEL) {
			title = Util.GetString("DialogTitleChooseModel");
			fileter = Util.GetFilter("DialogFilterChooseModel");
			defaultExt = "pmd";
		} else if (type == MODEL_UK) {
			title = Util.GetString("DialogTitleChooseModelUK");
			fileter = Util.GetFilter("DialogFilterChooseModel");
			defaultExt = "pmd";
		} else if (type == MODEL_COLL) {
			title = Util.GetString("DialogTitleChooseColl");
			fileter = Util.GetFilter("DialogFilterChooseColl");
			defaultExt = "pmc";
		} else
			return;
		var fileDialog = new OpenFileDialog {
			Multiselect = false,
			DefaultDirectory = ProjectLocation,
			DefaultExt = defaultExt,
			Title = title,
			Filter = fileter,
			InitialDirectory = AccessoryDataUtil.GetInitialPath([ModelPath, ModelPathUK, CollPath])
		};
		if (fileDialog.ShowDialog() != true)
			return;
		string path = fileDialog.FileName;
		if (!path.StartsWith(ProjectLocation) || path.Length == ProjectLocation.Length)
			throw new(Util.GetString("MessageModelOutsideProject"));
		if (type == MODEL_COLL) {
			if (!path.EndsWith(".pic") && !path.EndsWith(".pmc"))
				throw new(Util.GetString("MessageInvalidExtColl"));
		} else if (!path.EndsWith(".pim") && !path.EndsWith(".pmd")) {
			throw new(Util.GetString("MessageInvalidExt"));
		}
		AccAddonHistory.Default.ChooseModelHistory = new DirectoryInfo(fileDialog.FileName).Parent!.FullName;
		AccAddonHistory.Default.Save();
		LoadLooksAndVariants(path);//修改模型路径后，look和variant都不同，需要重新读取
		string inProjectPath = path.Replace(ProjectLocation, "");
		var s = inProjectPath.Split('\\');

		string?
			modelName = null,
			modelType = null,
			modelPath = null,
			modelPathUK = null,
			collPath = null;

		for (int i = s.Length - 2; i > 0; i--) {
			foreach (ModelTypeInfo info in ModelTypes) {//根据路径猜测模型的类型
				if (info.Accessory.Equals(s[i]))
					modelType = info.Accessory;
			}
		}
		if (type == MODEL) {
			modelPath = inProjectPath.Replace('\\', '/');
			modelPath = modelPath[..^4];
			modelName = modelPath[(modelPath.LastIndexOf('/') + 1)..];
			if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
				modelPathUK = modelPath + "_uk.pmd";
			if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
				collPath = modelPath + ".pmc";
			modelPath += ".pmd";
		} else if (type == MODEL_UK) {
			modelPathUK = inProjectPath.Replace('\\', '/');
			modelPathUK = modelPathUK[..^4];
			if (modelPathUK.EndsWith("_uk")) {
				modelName = modelPathUK[(modelPathUK.LastIndexOf('/') + 1)..^3];
				if (File.Exists(path[..^7] + ".pim") || File.Exists(path[..^7] + ".pmd"))
					modelPath = modelPathUK[..^3] + ".pmd";
				if (File.Exists(path[..^7] + ".pic") || File.Exists(path[..^7] + ".pmc"))
					collPath = modelPathUK[..^3] + ".pmc";
			}
			modelPathUK += ".pmd";
		} else if (type == MODEL_COLL) {
			collPath = inProjectPath.Replace('\\', '/');
			collPath = collPath[..^4];
			modelName = collPath[(collPath.LastIndexOf('/') + 1)..];
			if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
				modelPathUK = modelPath + "_uk.pmd";
			if (File.Exists(path[..^4] + ".pim") || File.Exists(path[..^4] + ".pmd"))
				modelPath = collPath + ".pmd";
			collPath += ".pmc";
		}
		bool showAR;
		if (type == MODEL) {
			ModelPath = modelPath!;
			if (ModelName == modelName)
				modelName = null;
			if (ModelType == modelType)
				modelType = null;
			if (ModelPathUK == modelPathUK)
				modelPathUK = null;
			if (CollPath == collPath)
				collPath = null;
			showAR = modelName != null || modelType != null || modelPathUK != null || collPath != null;
		} else if (type == MODEL_UK) {
			ModelPathUK = modelPathUK!;
			if (ModelName == modelName)
				modelName = null;
			if (ModelType == modelType)
				modelType = null;
			if (ModelPath == modelPath)
				modelPath = null;
			if (CollPath == collPath)
				collPath = null;
			showAR = modelName != null || modelType != null || modelPath != null || collPath != null;
		} else if (type == MODEL_COLL) {
			CollPath = collPath!;
			if (ModelName == modelName)
				modelName = null;
			if (ModelType == modelType)
				modelType = null;
			if (ModelPath == modelPath)
				modelPath = null;
			if (ModelPathUK == modelPathUK)
				modelPathUK = null;
			showAR = modelName != null || modelType != null || modelPath != null || modelPathUK != null;
		} else
			return;
		if (showAR) {
			AutoFillSelectionWindow arWindow = new(type, modelName, modelType, modelPath, modelPathUK, collPath) { Owner = window };
			var result = arWindow.ShowDialog();
			if (result == true) {
				if (arWindow.CheckModelName)
					ModelName = modelName!;
				if (arWindow.CheckModelType)
					ModelType = modelType!;
				if (arWindow.CheckModelPath)
					ModelPath = modelPath!;
				if (arWindow.CheckModelPathUK)
					ModelPathUK = modelPathUK!;
				if (arWindow.CheckCollPath)
					CollPath = collPath!;
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
}
