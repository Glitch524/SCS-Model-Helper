using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Utils;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory.AccHookup;

class AccHookupBinding: INotifyPropertyChanged {

	public AccHookupBinding() { }
	public static string ProjectLocation => Instances.ProjectLocation;

	private string mStorageName = "";
	public string StorageName {
		get => mStorageName;
		set {
			mStorageName = value;
			InvokeChange(nameof(StorageName));
		}
	}

	public static ObservableCollection<StringResItem> StringResList => AccessoryDataUtil.StringResList;

	public readonly ObservableCollection<SuiItem> mSuiItems = [];
	public ObservableCollection<SuiItem> SuiItems => mSuiItems;

	private SuiItem? mCurrentSuiItem;
	public SuiItem? CurrentSuiItem {
		get => mCurrentSuiItem;
		set {
			mCurrentSuiItem = value;
			InvokeChange(nameof(CurrentSuiItem));

			InvokeChange(nameof(Hookups));
			if (Hookups != null && Hookups.Count > 0)
				CurrentHookupItem = Hookups.First();

			SuiChanged?.Invoke(value);
		}
	}

	public delegate void OnSuiItemChanged(SuiItem? suiItem);
	public OnSuiItemChanged? SuiChanged = null;

	public ObservableCollection<AccessoryHookupData>? Hookups => CurrentSuiItem?.HookupItems;

	private AccessoryHookupData? mCurrentHookupItem;
	public AccessoryHookupData? CurrentHookupItem {
		get => mCurrentHookupItem;
		set {
			mCurrentHookupItem = value;
			if (value != null && ModelIcon == null) {
				value.ModelIcon = AccessoryDataUtil.LoadModelIconByIconName(value.IconName);
			}
			InvokeChange(nameof(CurrentHookupItem));
		}
	}
	public string ModelName {
		get => CurrentHookupItem?.ModelName ?? "";
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.ModelName = value;
				InvokeChange(nameof(ModelName));
			}
		}
	}
	public string DisplayName {
		get => CurrentHookupItem?.DisplayName ?? "";
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.DisplayName = value;
				InvokeChange(nameof(DisplayName));
			}
		}
	}


	private string mCheckResResult = "";
	public string CheckResResult {
		get => mCheckResResult;
		set {
			mCheckResResult = value;
			InvokeChange(nameof(CheckResResult));
		}
	}

	private bool mPopupCheckOpen = false;
	public bool PopupCheckOpen {
		get => mPopupCheckOpen;
		set {
			mPopupCheckOpen = value;
			InvokeChange(nameof(PopupCheckOpen));
		}
	}

	public void CheckNameStringRes() {
		CheckResResult = AccessoryDataUtil.GetStringResResults(DisplayName);
		PopupCheckOpen = true;
	}

	public string IconName {
		get => CurrentHookupItem?.IconName ?? "";
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.IconName = value;
				InvokeChange(nameof(IconName));
			}
		}
	}

	public BitmapSource? ModelIcon {
		get => CurrentHookupItem?.ModelIcon;
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.ModelIcon = value;
				InvokeChange(nameof(ModelIcon));
			}
		}
	}


	public static List<PartTypeItem> PartTypes => AccessoryData.PartTypes;

	public string ModelPath {
		get => CurrentHookupItem?.ModelPath ?? "";
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.ModelPath = value;
				InvokeChange(nameof(ModelPath));
			}
		}
	}
	public string CollPath {
		get => CurrentHookupItem?.CollPath ?? "";
		set {
			if (CurrentHookupItem != null) {
				CurrentHookupItem.CollPath = value;
				InvokeChange(nameof(CollPath));
			}
		}
	}

	public ObservableCollection<OthersItem>? OthersList => CurrentHookupItem?.OthersList;

	public void LoadFiles() => AccDataIO.LoadAddonHookup(this);
	public void SaveFiles() => AccDataIO.SaveAddonHookup(this);

	//模型、碰撞体
	public void ChooseModel(bool isColl) {
		try {
			if (ProjectLocation.Length == 0)
				throw new(Util.GetString("MessageProjectLocationFirst"));
			string title, fileter, defaultExt;
			if (isColl) {
				title = Util.GetString("DialogTitleChooseColl");
				fileter = Util.GetFilter("DialogFilterChooseColl");
				defaultExt = "pmc";
			} else {
				title = Util.GetString("DialogTitleChooseModel");
				fileter = Util.GetFilter("DialogFilterChooseModel");
				defaultExt = "pmd";
			}
			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				DefaultDirectory = ProjectLocation,
				DefaultExt = defaultExt,
				Title = title,
				Filter = fileter,
				InitialDirectory = AccessoryDataUtil.GetInitialPath(ModelPath, CollPath)
			};
			if (fileDialog.ShowDialog() != true)
				return;
			string path = fileDialog.FileName;
			if (!path.StartsWith(ProjectLocation) || path.Length == ProjectLocation.Length)
				throw new(Util.GetString("MessageModelOutsideProject"));
			if (isColl) {
				if (!path.EndsWith(".pic") && !path.EndsWith(".pmc"))
					throw new(Util.GetString("MessageInvalidExtColl"));
			} else if (!path.EndsWith(".pim") && !path.EndsWith(".pmd")) {
				throw new(Util.GetString("MessageInvalidExt"));
			}
			AccAddonHistory.Default.ChooseModelHistory = new DirectoryInfo(fileDialog.FileName).Parent!.FullName;
			AccAddonHistory.Default.Save();
			string inProjectPath = path.Replace(ProjectLocation, "");
			string?
				modelPath = null,
				modelColl = null;
			if (isColl) {
				modelColl = inProjectPath.Replace('\\', '/');
				modelColl = modelColl[..^4];
				if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
					modelPath = modelColl + ".pmd";
				modelColl += ".pmc";
			} else {
				modelPath = inProjectPath.Replace('\\', '/');
				modelPath = modelPath[..^4];
				if (File.Exists(path[..^4] + ".pic") || File.Exists(path[..^4] + ".pmc"))
					modelColl = modelPath + ".pmc";
				modelPath += ".pmd";
			}

			if (modelPath != null)
				ModelPath = modelPath;
			if (modelColl != null)
				CollPath = modelColl;
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

	public void AddPhysToOthersList(PhysicsData? physics) {
		if (CurrentHookupItem == null || physics == null)
			return;
		var physName = physics.PhysicsName;
		physName += ".phys_data";
		var othersList = CurrentHookupItem.OthersList;
		othersList.Add(new("data", physName));
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
