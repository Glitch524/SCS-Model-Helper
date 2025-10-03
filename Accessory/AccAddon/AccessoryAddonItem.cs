using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon;
public class AccessoryAddonItem: AccessoryItem {
	private readonly ModProject ModelProject = Instances.ModelProject;
	public AccessoryAddonItem() : base(
		AccAddonHistory.Default.ModelName,
		AccAddonHistory.Default.DisplayName,
		AccAddonHistory.Default.Price,
		AccAddonHistory.Default.UnlockLevel,
		AccAddonHistory.Default.IconName,
		AccAddonHistory.Default.PartType ?? "unknown",
		AccAddonHistory.Default.CollisionPath,
		AccAddonHistory.Default.Look,
		AccAddonHistory.Default.Variant) {
		mModelType = AccAddonHistory.Default.ModelType;
		mModelPath = AccAddonHistory.Default.ModelPath;
		mModelPathUK = AccAddonHistory.Default.ModelPathUK;
		mHideIn = AccAddonHistory.Default.HideIn;
	}

	public void SaveHistory() {
		ModBasic.Default.ProjectLocation = ProjectLocation;
		ModBasic.Default.Save();
		AccAddonHistory.Default.ModelName = ModelName;
		AccAddonHistory.Default.DisplayName = DisplayName;
		try {
			AccAddonHistory.Default.Price = Price ?? 1;
			AccAddonHistory.Default.UnlockLevel = UnlockLevel ?? 0;
		} catch {
		}
		AccAddonHistory.Default.IconName = IconName;
		AccAddonHistory.Default.PartType = PartType;
		AccAddonHistory.Default.ModelPath = ModelPath;
		AccAddonHistory.Default.ModelPathUK = ModelPathUK;
		AccAddonHistory.Default.ModelType = ModelType;
		AccAddonHistory.Default.Look = Look;
		AccAddonHistory.Default.Variant = Variant;
		AccAddonHistory.Default.HideIn = HideIn;

		AccAddonHistory.Default.Others = OthersItem.JoinOthers(OthersList);

		AccAddonHistory.Default.TruckHistoryETS2 = Truck.JoinTruck(TrucksETS2);
		AccAddonHistory.Default.TruckHistoryATS = Truck.JoinTruck(TrucksATS);

		AccAddonHistory.Default.Save();
	}

	public string ProjectLocation => ModelProject.ProjectLocation;

	private string mModelPath;
	public string ModelPath {
		get => mModelPath;
		set {
			mModelPath = value;
			InvokeChange(nameof(ModelPath));
		}
	}

	private string mModelPathUK;
	public string ModelPathUK {
		get => mModelPathUK;
		set {
			mModelPathUK = value;
			InvokeChange(nameof(ModelPathUK));
		}
	}

	private string mModelType;
	public string ModelType {
		get => mModelType;
		set {
			mModelType = value;
			if (CurrentAcc != null && value != CurrentAcc.Accessory) {
				CurrentAcc = null;
			}
			InvokeChange(nameof(ModelType));
		}
	}

	protected string mHideIn;
	public string HideIn {
		get => mHideIn;
		set {
			mHideIn = value;
			InvokeChange(nameof(HideIn));
		}
	}

	private bool mTruckExpandedETS2 = true;
	public bool TruckExpandedETS2 {
		get => mTruckExpandedETS2;
		set {
			mTruckExpandedETS2 = value;
			InvokeChange(nameof(TruckExpandedETS2));
		}
	}
	private bool mTruckExpandedATS = true;
	public bool TruckExpandedATS {
		get => mTruckExpandedATS;
		set {
			mTruckExpandedATS = value;
			InvokeChange(nameof(TruckExpandedATS));
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

	public List<PhysicsToyData> PhysicsList = [];

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


	private bool mDeleteUnchecked;
	public bool DeleteUnchecked {
		get => mDeleteUnchecked;
		set {
			mDeleteUnchecked = value;
			InvokeChange(nameof(DeleteUnchecked));
		}
	}

	private List<PartTypeItem>? mPartTypes = null;
	public List<PartTypeItem> PartTypes {
		get {
			mPartTypes ??= [
			new("unknown", Util.GetString("PartTypeUnknown")),
			new("aftermarket", Util.GetString("PartTypeAftermarket")),
			new("factory", Util.GetString("PartTypeFactory")),
			new("licensed", Util.GetString("PartTypeLicensed"))];
			return mPartTypes;
		}
	}
		

	//配件类型、look、variant列表以及菜单
	private ObservableCollection<AccessoryInfo>? mAccessoryType = null;
	public ObservableCollection<AccessoryInfo> AccessoryType {
		get {
			mAccessoryType ??= DefaultData.Accessories;
			return mAccessoryType;
		}
	}

	private readonly AccessoryInfo emptyAcc = new();

	private AccessoryInfo? mCurrentAcc = null;
	public AccessoryInfo? CurrentAcc {
		get => mCurrentAcc ?? emptyAcc;
		set {
			mCurrentAcc = value ?? emptyAcc;
			InvokeChange(nameof(CurrentAcc));
		}
	}

	private readonly ObservableCollection<string> mLookList = [];
	private readonly ObservableCollection<string> mVariantList = [];
	private readonly ObservableCollection<OthersItem> mOthersList = [];
	public ObservableCollection<string> LookList => mLookList;
	public ObservableCollection<string> VariantList => mVariantList;
	public ObservableCollection<OthersItem> OthersList => mOthersList;
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

	public void StartCreateSii() {
		try {
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
	public void ChooseModel(int type) {
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
		for (int i = s.Length - 2; i > 0; i--) {
			foreach (AccessoryInfo info in AccessoryType) {//根据路径猜测模型的类型
				if (info.Accessory.Equals(s[i]))
					ModelType = info.Accessory;
			}
		}
		string?
			modelPath = null,
			modelName = null,
			modelPathUK = null,
			modelColl = null;
		if (type == MODEL) {
			modelPath = inProjectPath.Replace('\\', '/');
			modelPath = modelPath[..^4];
			modelName = modelPath[(modelPath.LastIndexOf('/') + 1)..];
			if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
				modelPathUK = modelPath + "_uk.pmd";
			if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
				modelColl = modelPath + ".pmc";
			modelPath += ".pmd";
		} else if (type == MODEL_UK) {
			modelPathUK = inProjectPath.Replace('\\', '/');
			modelPathUK = modelPathUK[..^4];
			if (modelPathUK.EndsWith("_uk")) {
				modelName = modelPathUK[(modelPathUK.LastIndexOf('/') + 1)..^3];
				if (File.Exists(path[..^7] + ".pim") || File.Exists(path[..^7] + ".pmd"))
					modelPath = modelPathUK[..^3] + ".pmd";
				if (File.Exists(path[..^7] + ".pic") || File.Exists(path[..^7] + ".pmc"))
					modelColl = modelPathUK[..^3] + ".pmc";
			}
			modelPathUK += ".pmd";
		} else if (type == MODEL_COLL) {
			modelColl = inProjectPath.Replace('\\', '/');
			modelColl = modelColl[..^4];
			modelName = modelColl[(modelColl.LastIndexOf('/') + 1)..];
			if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
				modelPathUK = modelPath + "_uk.pmd";
			if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
				modelPath = modelColl + ".pmd";
			modelColl += ".pmc";
		}
		if (modelPath != null)
			ModelPath = modelPath;
		if (modelPathUK != null)
			ModelPathUK = modelPathUK;
		if (modelColl != null)
			CollPath = modelColl;
		if (modelName != null)
			ModelName = modelName;
	}

	//卡车列表
	private readonly ObservableCollection<Truck> mTrucksETS2 = [];
	private readonly ObservableCollection<Truck> mTrucksATS = [];
	public ObservableCollection<Truck> TrucksETS2 => mTrucksETS2;
	public ObservableCollection<Truck> TrucksATS => mTrucksATS;
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
		SaveFileDialog saveFileDialog = new() {
			Title = Util.GetString("SaveDED"),
			AddExtension = true,
			DefaultDirectory = Paths.DefaultDEDDir(),
			DefaultExt = "ded",
			Filter = Util.GetFilter("DialogFilterDED"),
			InitialDirectory = AccAddonHistory.Default.DEDLocation,
		};
		if (LoadedFilename == null) {
			if (DisplayName.Length > 0 && ModelType.Length > 0 && ModelName.Length > 0)
				saveFileDialog.FileName = $"{DisplayName} {ModelType} {ModelName}";
		} else
			saveFileDialog.FileName = LoadedFilename;
		if (saveFileDialog.ShowDialog() == true) {
			AccAddonHistory.Default.DEDLocation = new DirectoryInfo(saveFileDialog.FileName).Parent!.FullName;
			AccAppIO.SaveAccAddon(this, saveFileDialog.FileName);
			MessageBox.Show(Util.GetString("MessageSaveDED"));
		}
	}

	public void LoadDED() {
		OpenFileDialog openFileDialog = new() {
			Title = Util.GetString("LoadDED"),
			Multiselect = false,
			AddExtension = true,
			InitialDirectory = AccAddonHistory.Default.DEDLocation,
			DefaultExt = "ded",
			Filter = Util.GetFilter("DialogFilterDED"),
		};
		if (openFileDialog.ShowDialog() == true) {
			AccAddonHistory.Default.DEDLocation = new DirectoryInfo(openFileDialog.FileName).Parent!.FullName;
			LoadedFilename = openFileDialog.SafeFileName;
			AccAppIO.LoadAccAddon(this, openFileDialog.FileName);
		}
	}
}
