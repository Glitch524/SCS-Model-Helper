using Microsoft.Win32;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.AccAddon.Popup;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SCS_Mod_Helper.Accessory.AccAddon;
public class AccAddonBinding: BaseBinding, IListDataInterface {
	private readonly AccessoryAddonData mAddonItem = new();
	public AccessoryAddonData AddonItem => mAddonItem;

	public void SaveHistory() => AddonItem.SaveHistory();


	public AccAddonBinding() {
		ModelIcon = AccessoryDataUtil.LoadModelIconByIconName(AddonItem.IconName);

		UseCollPath = CollPath.Length > 0;

		UpdateOthersChecked();
	}

	public static string ProjectLocation => Instances.ProjectLocation;
	public static bool ProjectExist => Directory.Exists(ProjectLocation);

	public string DisplayName {
		get => AddonItem.DisplayName;
		set {
			AddonItem.DisplayName = value;
			CheckResResult = null;
			InvokeChange();
			InvokeChange(nameof(CheckResVisibility));
		}
	}
	private string? LocalizedDisplayName = null;

	private string? mCheckResResult = null;
	public string? CheckResResult {
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
		CheckResResult ??= StringResUtil.GetStringResResults(DisplayName, out LocalizedDisplayName);
		PopupCheckOpen = true;
	}



	public string ModelName {
		get => AddonItem.ModelName;
		set {
			AddonItem.ModelName = value; 
			InvokeChange();

			InvokeChange(nameof(NameOver12));
		}
	}

	public bool NameOver12 => ModelName.Length > 12;

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

			InvokeChange(nameof(IconNameClearVisibility));
		}
	}
	public Visibility IconNameClearVisibility => IconName.Length > 0 ? Visibility.Visible : Visibility.Collapsed;


	public BitmapSource? ModelIcon {
		get => AddonItem.ModelIcon;
		set {
			AddonItem.ModelIcon = value;
			InvokeChange();
		}
	}

	public string ModelPath {
		get => AddonItem.ModelPath;
		set {
			AddonItem.ModelPath = value;
			InvokeChange();

			InvokeChange(nameof(ModelPathClearVisibility));
		}
	}
	public Visibility ModelPathClearVisibility => ModelPath.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

	public string ModelPathUK {
		get => AddonItem.ModelPathUK;
		set {
			AddonItem.ModelPathUK = value;
			InvokeChange();
			InvokeChange(nameof(ModelPathUKClearVisibility));
		}
	}
	public Visibility ModelPathUKClearVisibility => ModelPathUK.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

	private bool? mExteriorPath = null;
	public bool EnableExterior {
		get {
			mExteriorPath ??= ExtModelPath.Length > 0 || ExtModelPathUK.Length > 0;
			return (bool)mExteriorPath;
		}
		set {
			mExteriorPath = value;
			InvokeChange();
		}
	}

	public string ExtModelPath {
		get => AddonItem.ExtModelPath;
		set {
		AddonItem.ExtModelPath = value;
			InvokeChange();

			InvokeChange(nameof(EnableExterior));
			InvokeChange(nameof(ExtModelPathClearVisibility));
		}
	}
	public Visibility ExtModelPathClearVisibility => ExtModelPath.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

	public string ExtModelPathUK {
		get => AddonItem.ExtModelPathUK;
		set {
			AddonItem.ExtModelPathUK = value;
			InvokeChange();

			InvokeChange(nameof(EnableExterior));
			InvokeChange(nameof(ExtModelPathUKClearVisibility));
		}
	}
	public Visibility ExtModelPathUKClearVisibility => ExtModelPathUK.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

	public string CollPath {
		get => AddonItem.CollPath;
		set {
			AddonItem.CollPath = value;
			InvokeChange();

			UseCollPath = value.Length > 0;

			InvokeChange(nameof(CollPathClearVisibility));
		}
	}
	public Visibility CollPathClearVisibility => CollPath.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

	private bool mUseCollPath = false;
	public bool UseCollPath {
		get => mUseCollPath;
		set {
			mUseCollPath = value;
			InvokeChange();
		}
	}

	public string ModelType {
		get => AddonItem.ModelType;
		set {
			AddonItem.ModelType = value;
			if (CurrentModelType != null && value != CurrentModelType.Accessory) {
				mCurrentModelType = null;
			}
			InvokeChange();

			bool? flag = null;
			if (value == $"{DefaultData.TypeFlagL}/{DefaultData.TypeFlagFL}") {
				flag = true;
			} else if (value == $"{DefaultData.TypeFlagR}/{DefaultData.TypeFlagFR}") {
				flag = false;
			}
			if (flag != null) {
				var result = MessageBox.Show(Util.GetString("MessageModelTypeFlag"), Util.GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					if (flag == true) {
						ModelPath = "/vehicle/truck/upgrade/flag/flag_left.pmd";
						CollPath = "/vehicle/truck/upgrade/flag/flag_left.pmc";
					} else if (flag == false) {
						ModelPath = "/vehicle/truck/upgrade/flag/flag_right.pmd";
						CollPath = "/vehicle/truck/upgrade/flag/flag_right.pmc";
					}
				}
			}
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

	public ModelTypeInfo? mCurrentModelType = null;
	public ModelTypeInfo CurrentModelType {
		get {
			if (mCurrentModelType == null) {
				foreach (var type in ModelTypes) {
					if (ModelType == type.Accessory) {
						mCurrentModelType = type;
						return mCurrentModelType;
					}
				}
				return emptyAcc;
			}
			return mCurrentModelType;
		}
		set {
			mCurrentModelType = value ?? emptyAcc;
			InvokeChange();
		}
	}

	public static ObservableCollection<StringResItem> StringResList => StringResUtil.StringResList;

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

	public List<PhysicsData> PhysicsList => AddonItem.PhysicsList;

	public void LoadLooksAndVariants(string? path = null) {
		try {
			string oldLook = Look, oldVariant = Variant;
			if (path == null) {
				if (ModelPath.Length > 0)
					path = ModelPath;
				else if (ModelPathUK.Length > 0)
					path = ModelPathUK;
				else
					return;
				path = path.Replace('/', '\\');
				if (path.Length < 4)
					return;
				path = path[..^4] + ".pit";
				path = ProjectLocation + path;
			} else if (path.EndsWith(".pim")) {
				if (path.Length < 4)
					return;
				path = path[..^4] + ".pit";
			}
			Application.Current.Dispatcher.Invoke(() => {
				LookList.Clear();
				VariantList.Clear();
			}, DispatcherPriority.DataBind);

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
		} catch (Exception ex) {
			MessageBox.Show(Util.GetString("MessageLoadDEDErrFail") + "\n" + ex.Message);
		}
	}



	private bool othersChecked = false;
	public bool OthersChecked {
		get => othersChecked;
		set {
			othersChecked = value;
			InvokeChange();
		}
	}
	private void UpdateOthersChecked() {
		OthersChecked = HideIn != 0
			|| ElectricType != "vehicle"
			|| Data.Count > 0
			|| SuitableFor.Count > 0
			|| ConflictWith.Count > 0
			|| Defaults.Count > 0
			|| Overrides.Count > 0
			|| Require.Count > 0;
	}

	public uint HideIn {
		get => AddonItem.HideIn;
		set {
			AddonItem.HideIn = value;
			InvokeChange();
		}
	}

	public HideInUC? HideInUC = null;
	public string ElectricType {
		get => AddonItem.ElectricType;
		set {
			AddonItem.ElectricType = value;
			InvokeChange();
		}
	}

	public ObservableCollection<string> Data => AddonItem.Data;
	public ObservableCollection<string> SuitableFor => AddonItem.SuitableFor;
	public ObservableCollection<string> ConflictWith => AddonItem.ConflictWith;
	public ObservableCollection<string> Defaults => AddonItem.Defaults;
	public ObservableCollection<string> Overrides => AddonItem.Overrides;
	public ObservableCollection<string> Require => AddonItem.Require;

	private string mNewData = "";
	public string NewData {
		get {
			return mNewData;
		}
		set {
			mNewData = value;
			InvokeChange();
		}
	}
	private string mNewSuitableFor = "";
	public string NewSuitableFor {
		get => mNewSuitableFor;
		set {
			mNewSuitableFor = value;
			InvokeChange();
		}
	}
	private string mNewConflictWith = "";
	public string NewConflictWith {
		get => mNewConflictWith;
		set {
			mNewConflictWith = value;
			InvokeChange();
		}
	}
	private string mNewDefaults = "";
	public string NewDefaults {
		get => mNewDefaults;
		set {
			mNewDefaults = value; 
			InvokeChange();
		}
	}
	private string mNewOverrides = "";
	public string NewOverrides {
		get => mNewOverrides;
		set {
			mNewOverrides = value;
			InvokeChange();
		}
	}
	private string mNewRequire = "";
	public string NewRequire {
		get => mNewRequire;
		set {
			mNewRequire = value;
			InvokeChange();
		}
	}
	public void AddNewData(string? data = null) {
		AddListItem(Data, data ?? NewData);
		NewData = string.Empty;
		InvokeChange(nameof(DataListContent));
	}

	public void AddNewSuitableFor() {
		AddListItem(SuitableFor, NewSuitableFor);
		NewSuitableFor = string.Empty;
		InvokeChange(nameof(SuitableForListContent));
	}

	public void AddNewConflictWith() {
		AddListItem(ConflictWith, NewConflictWith);
		NewConflictWith = string.Empty;
		InvokeChange(nameof(ConflictWithListContent));
	}

	public void AddNewDefaults() {
		AddListItem(Defaults, NewDefaults);
		NewDefaults = string.Empty;
		InvokeChange(nameof(DefaultsListContent));
	}

	public void AddNewOverrides() {
		AddListItem(Overrides, NewOverrides);
		NewOverrides = string.Empty;
		InvokeChange(nameof(OverridesListContent));
	}

	public void AddNewRequire() {
		AddListItem(Require, NewRequire);
		NewRequire = string.Empty;
		InvokeChange(nameof(RequireListContent));
	}

	public static void AddListItem(ObservableCollection<string> list,string newItem) {
		if (newItem.Length == 0 || list.Contains(newItem))
			return;
		list.Add(newItem);
	}
	public string DataListContent => AddonItem.DataListContent;
	public string SuitableForListContent => AddonItem.SuitableForListContent;
	public string ConflictWithListContent => AddonItem.ConflictWithListContent;
	public string DefaultsListContent => AddonItem.DefaultsListContent;
	public string OverridesListContent => AddonItem.OverridesListContent;
	public string RequireListContent => AddonItem.RequireListContent;

	public string? OpeningList {
		get => AddonItem.OpeningList;
		set {
			AddonItem.OpeningList = value;
			InvokeChange(nameof(PopupCollection));
		}
	}

	public ObservableCollection<string>? PopupCollection => AddonItem.PopupCollection;

	public void DeleteItem(string item) {
		PopupCollection?.Remove(item);
		switch (OpeningList) {
			case "TextData":
				InvokeChange(nameof(DataListContent));
				break;
			case "TextSuitableFor":
				InvokeChange(nameof(SuitableForListContent));
				break;
			case "TextConflictWith":
				InvokeChange(nameof(ConflictWithListContent));
				break;
			case "TextDefaults":
				InvokeChange(nameof(DefaultsListContent));
				break;
			case "TextOverrides":
				InvokeChange(nameof(OverridesListContent));
				break;
			case "TextRequire":
				InvokeChange(nameof(RequireListContent));
				break;
		}
	}

	public void UpdateAllContent() {
		InvokeChange(nameof(DataListContent));
		InvokeChange(nameof(SuitableForListContent));
		InvokeChange(nameof(ConflictWithListContent));
		InvokeChange(nameof(DefaultsListContent));
		InvokeChange(nameof(OverridesListContent));
		InvokeChange(nameof(RequireListContent));
	}

	public ListDataUC? ListDataUC = null;


	private bool mTruckExpandedETS2 = true;
	public bool TruckExpandedETS2 {
		get => mTruckExpandedETS2;
		set {
			mTruckExpandedETS2 = value;
			InvokeChange();

			PopupAddTruckOpen = false;
		}
	}
	private bool mTruckExpandedATS = true;
	public bool TruckExpandedATS {
		get => mTruckExpandedATS;
		set {
			mTruckExpandedATS = value;
			InvokeChange();

			PopupAddTruckOpen = false;
		}
	}

	private bool mPopupAddTruckOpen = false;
	public bool PopupAddTruckOpen {
		get => mPopupAddTruckOpen;
		set {
			mPopupAddTruckOpen = value;
			InvokeChange();
		}
	}

	public bool? SelectAllETS2 {
		get {
			if (SelectedCountETS2 == 0)
				return false;
			else if (SelectedCountETS2 == TrucksETS2.Count)
				return true;
			else
				return null;
		}
		set {
			if (value == true)
				SelectedCountETS2 = TrucksETS2.Count;
			else if (value == false)
				SelectedCountETS2 = 0;
			InvokeChange();
			if (value != null)
				SelectAllTruck(TrucksETS2, (bool)value);
		}
	}
	private int mSelectedCountETS2 = 0;
	public int SelectedCountETS2 {
		get => mSelectedCountETS2;
		set {
			mSelectedCountETS2 = value;

			InvokeChange(nameof(SelectAllETS2));
		}
	}

	public bool? SelectAllATS {
		get {
			if (SelectedCountATS == 0)
				return false;
			else if (SelectedCountATS == TrucksATS.Count)
				return true;
			else
				return null;
		}
		set {
			if (value == true)
				SelectedCountATS = TrucksATS.Count;
			else if (value == false)
				SelectedCountATS = 0;
			InvokeChange();
			if (value != null)
				SelectAllTruck(TrucksATS, (bool)value);
		}
	}

	private int mSelectedCountATS = 0;
	public int SelectedCountATS {
		get => mSelectedCountATS;
		set {
			mSelectedCountATS = value;

			InvokeChange(nameof(SelectAllATS));
		}
	}

	public void SetSelected(Truck truck) {
		bool check = truck.Check;
		if (truck.IsETS2) {
			if (check)
				SelectedCountETS2++;
			else
				SelectedCountETS2--;
		} else {
			if (check)
				SelectedCountATS++;
			else
				SelectedCountATS--;
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
		SelectedCountETS2 = AccAppIO.LoadTruckList(true, TrucksETS2);
		SelectedCountATS = AccAppIO.LoadTruckList(false, TrucksATS);
	}

	public void ReinitTruckList() {
		AccAppIO.ClearTruckList();
		TrucksETS2.Clear();
		TrucksATS.Clear();
		LoadTrucks();
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
			if (LocalizedDisplayName == null) {
				StringResUtil.GetStringResResults(DisplayName, out LocalizedDisplayName);
			}
			saveFileDialog.FileName = $"{LocalizedDisplayName} {ModelType} {ModelName}";
		} else
			saveFileDialog.FileName = LoadedFilename;
		if (saveFileDialog.ShowDialog() == true) {
			SaveDedLocation(saveFileDialog.FileName);
			AccAppIO.SaveAccAddon(this, saveFileDialog.FileName);
			MessageBox.Show(Util.GetString("MessageSaveDED"));
		}
	}

	public void LoadDED() {
		try {
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
				UpdateOthersChecked();
			}
		} catch (Exception ex) {
			MessageBox.Show(Util.GetString("MessageLoadDEDErrFail") + "\n" + ex.Message);
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

	public AddTruckUC? AddTruckUC;

	public void AddNewTruck(Truck newTruck) {
		try {
			bool isETS2 = newTruck.IsETS2;
			ObservableCollection<Truck> Trucks = isETS2 ? TrucksETS2 : TrucksATS;
			if (newTruck.TruckID[0] <= 'm') {
				for (int i = 0; i < Trucks.Count; i++) {
					var cTruck = Trucks[i];
					var cResult = newTruck.CompareTo(cTruck);
					if (cResult == 0) {
						throw new(Util.GetString("MessageAddErrSameID"));
					} else if (cResult < 0) {
						Trucks.Insert(i, newTruck);
						break;
					} else if (i == Trucks.Count - 1)
						Trucks.Add(newTruck);
				}
			} else {
				for (int i = Trucks.Count - 1; i >= 0; i--) {
					var cTruck = Trucks[i];
					var cResult = newTruck.TruckID.CompareTo(cTruck.TruckID);
					if (cResult == 0) {
						throw new(Util.GetString("MessageAddErrSameID"));
					} else if (cResult > 0) {
						Trucks.Insert(i + 1, newTruck);
						break;
					} else if (i == 0)
						Trucks.Insert(0, newTruck);
				}
				Trucks.Insert(0, newTruck);
			}
			AccAppIO.SaveTruckList(isETS2, Trucks);
		} catch (Exception ex) {
			MessageBox.Show(ex.Message, Util.GetString("MessageTitleErr"));
		}
	}

	public static void DeleteTruck(bool ets2, ObservableCollection<Truck> Trucks, DataGrid Table) {
		var changed = false;
		while (Table.SelectedIndex != -1) {
			changed = true;
			var selected = Trucks[Table.SelectedIndex];
			Trucks.Remove(selected);
		}
		Table.UnselectAll();
		if (changed) 
			AccAppIO.SaveTruckList(ets2, Trucks);
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
		var icon = AccessoryDataUtil.ChooseIcon(window, out string? iconPath);
		if (icon != null) {
			ModelIcon = AccessoryDataUtil.LoadModelIcon(iconPath);
			IconName = icon;
		}
	}

	public void LoadIcon() {
		if (IconName.Length == 0)
			return;
		var iconPath = IconName;
		if (iconPath.Contains('/'))
			iconPath = iconPath.Replace("/", "\\");
		iconPath = Paths.AccessoryIconDir(ProjectLocation, iconPath);
		var fullIconPath = iconPath + ".tga";
		if (!File.Exists(fullIconPath)) {
			fullIconPath = iconPath + ".dds";
			if (!File.Exists(fullIconPath))
				return;
		}
		ModelIcon = AccessoryDataUtil.LoadModelIcon(fullIconPath);
	}

	public const int MODEL = 0;
	public const int MODEL_UK = 1;
	public const int EXT_MODEL = 2;
	public const int EXT_MODEL_UK = 3;
	public const int MODEL_COLL = 4;
	//模型、模型UK、碰撞体
	public void ChooseModel(Window window, int type) {
		if (ProjectLocation.Length == 0)
			throw new(Util.GetString("MessageProjectLocationFirst"));
		string title, fileter, defaultExt;
		if (type == MODEL || type == EXT_MODEL) {
			title = Util.GetString("DialogTitleChooseModel");
			fileter = Util.GetFilter("DialogFilterChooseModel");
			defaultExt = "pmd";
		} else if (type == MODEL_UK || type == EXT_MODEL_UK) {
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
				if (s[i] == info.AccessoryETS2 || s[i] == info.AccessoryATS)
					modelType = info.Accessory;
			}
		}
		if (type == MODEL || type == EXT_MODEL) {
			modelPath = inProjectPath.Replace('\\', '/');
			modelPath = modelPath[..^4];
			modelName = modelPath[(modelPath.LastIndexOf('/') + 1)..];
			if (File.Exists(path[..^4] + "_uk.pim") || File.Exists(path[..^4] + "_uk.pmd"))
				modelPathUK = modelPath + "_uk.pmd";
			if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
				collPath = modelPath + ".pmc";
			modelPath += ".pmd";
		} else if (type == MODEL_UK || type == EXT_MODEL_UK) {
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


			if (ModelName == modelName)
				modelName = null;
			if (ModelType == modelType)
				modelType = null;
		if (type == MODEL) {
			ModelPath = modelPath!;
			if (ModelPathUK == modelPathUK)
				modelPathUK = null;

			if (CollPath == collPath)
				collPath = null;
			showAR = modelName != null || modelType != null || modelPathUK != null || collPath != null;
		} else if (type == EXT_MODEL) {
			ExtModelPath = modelPath!;
			if (ExtModelPathUK == modelPathUK)
				modelPathUK = null;

			if (CollPath == collPath)
			collPath = null;
			showAR = modelName != null || modelType != null || modelPathUK != null || collPath != null;
		} else if (type == MODEL_UK) {
			ModelPathUK = modelPathUK!;
			if (ModelPath == modelPath)
				modelPath = null;

			if (CollPath == collPath)
				collPath = null;
			showAR = modelName != null || modelType != null || modelPath != null || collPath != null;
		} else if (type == EXT_MODEL_UK) {
			ExtModelPathUK = modelPathUK!;
			if (ExtModelPath == modelPath)
				modelPath = null;

			if (CollPath == collPath)
				collPath = null;
			showAR = modelName != null || modelType != null || modelPath != null || collPath != null;
		} else if (type == MODEL_COLL) {
			CollPath = collPath!;
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
				if (arWindow.CheckModelType) {
					mCurrentModelType = null;
					ModelType = modelType!;
				}
				if (arWindow.CheckModelPath) {
					if (type == EXT_MODEL || type == EXT_MODEL_UK)
						ExtModelPath = modelPath!;
					else
						ModelPath = modelPath!;
				}
				if (arWindow.CheckModelPathUK) {
					if (type == EXT_MODEL || type == EXT_MODEL_UK)
						ExtModelPathUK = modelPathUK!;
					else
						ModelPathUK = modelPathUK!;
				}
				if (arWindow.CheckCollPath)
					CollPath = collPath!;
			}
		}
	}
}
