using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Utils;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using SCS_Mod_Helper.Manifest;

namespace SCS_Mod_Helper.Accessory.PhysicsToy;

class PhysicsToyViewModel: INotifyPropertyChanged {
	private readonly ModProject ModelProject = Instances.ModelProject;
	public string ProjectLocation => ModelProject.ProjectLocation;

	private bool mAllVisible = false;
	public bool AllVisible {
		get => mAllVisible;
		set {
			mAllVisible = value;
			InvokeChange(nameof(AngularVisibility));
			InvokeChange(nameof(RopeDataVisibility));
		}
	}

	private bool mChooseMode = false;
	public bool ChooseMode {
		get => mChooseMode;
		set {
			mChooseMode = value;
			InvokeChange(nameof(ChooseMode));
		}
	}

	private SuiItem? mCurrentSuiItem;
	public SuiItem? CurrentSuiItem {
		get => mCurrentSuiItem;
		set {
			mCurrentSuiItem = value;
			InvokeChange(nameof(CurrentSuiItem));

			InvokeChange(nameof(PhysicsItems));
			if (PhysicsItems != null && PhysicsItems.Count > 0)
				CurrentPhysicsItem = PhysicsItems.First();
		}
	}

	public ObservableCollection<PhysicsToyData> PhysicsItems => 
		CurrentSuiItem != null ? CurrentSuiItem.PhysicsItems : AccAppIO.PhysicsItems;

	public delegate void CurrentPhysicsCallback(PhysicsToyData? physics);
	public CurrentPhysicsCallback? physicsCallback = null;

	private PhysicsToyData? mCurrentPhysicsItem;
	public PhysicsToyData? CurrentPhysicsItem {
		get => mCurrentPhysicsItem;
		set {
			mCurrentPhysicsItem = value;
			InvokeChange(nameof(CurrentPhysicsItem));
			physicsCallback?.Invoke(value);
		}
	}

	public void PhysicsListAdd(PhysicsToyData physics) {
		PhysicsItems.Add(physics);
		CurrentPhysicsItem = physics;
	}

	public string ModelPath {
		get => CurrentPhysicsItem?.ModelPath ?? "";
		set {
			if (CurrentPhysicsItem != null) {
				CurrentPhysicsItem.ModelPath = value;
				InvokeChange(nameof(ModelPath));
			}
		}
	}

	public string CollPath {
		get => CurrentPhysicsItem?.CollPath ?? "";
		set {
			if (CurrentPhysicsItem != null) {
				CurrentPhysicsItem.CollPath = value;
				InvokeChange(nameof(CollPath));
			}
		}
	}
	public string ToyType {
		get => CurrentPhysicsItem?.ToyType ?? "";
		set {
			if (CurrentPhysicsItem != null) {
				CurrentPhysicsItem.ToyType = value;
				InvokeChange(nameof(ToyType));
			}
		}
	}

	public string RopeMaterial {
		get => CurrentPhysicsItem?.RopeMaterial ?? "";
		set {
			if (CurrentPhysicsItem != null) {
				CurrentPhysicsItem.RopeMaterial = value;
				InvokeChange(nameof(RopeMaterial));
			}
		}
	}
	public Visibility AngularVisibility =>
		AllVisible || ToyType == "TT_joint" || ToyType == "TT_joint_free" ? Visibility.Visible : Visibility.Collapsed;
	public Visibility RopeDataVisibility =>
		AllVisible || ToyType == "TT_rope" || ToyType == "TT_double_rope" ? Visibility.Visible : Visibility.Hidden;

	public static void SavePhysicsList() => AccAppIO.SavePhysicsList();

	//模型、碰撞体
	public void ChooseModel(bool isColl) {
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
		
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
