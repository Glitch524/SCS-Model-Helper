﻿using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Utils;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.Physics;

class PhysicsBinding: INotifyPropertyChanged {
	public static string ProjectLocation => Instances.ProjectLocation;

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

	public ObservableCollection<PhysicsData> PhysicsItems => 
		CurrentSuiItem != null ? CurrentSuiItem.PhysicsItems : AccAppIO.PhysicsItems;

	public delegate void CurrentPhysicsCallback(PhysicsData? physics);
	public CurrentPhysicsCallback? physicsCallback = null;

	private PhysicsData? mCurrentPhysicsItem;
	public PhysicsData? CurrentPhysicsItem {
		get => mCurrentPhysicsItem;
		set {
			mCurrentPhysicsItem = value;
			InvokeChange(nameof(CurrentPhysicsItem));
			physicsCallback?.Invoke(value);
			InvokeChange(nameof(CurrentToyData));
			InvokeChange(nameof(CurrentPatchData));

			InvokeChange(nameof(PhysicsType));
			InvokeChange(nameof(ToyVisibility));
			InvokeChange(nameof(PatchVisibility));
		}
	}

	public const string PhysicsTypeToy = "physics_toy_data";
	public const string PhysicsTypePatch = "physics_patch_data";

	public string PhysicsType {
		get {
			if (CurrentPhysicsItem is PhysicsToyData)
				return PhysicsTypeToy;
			else if (CurrentPhysicsItem is PhysicsPatchData)
				return PhysicsTypePatch;
			return "";
		}
	}

	public Visibility ToyVisibility => CurrentPhysicsItem is PhysicsToyData ? Visibility.Visible : Visibility.Hidden;
	public Visibility PatchVisibility => CurrentPhysicsItem is PhysicsPatchData ? Visibility.Visible : Visibility.Hidden;

	public PhysicsToyData? CurrentToyData { 
		get => CurrentPhysicsItem is PhysicsToyData toyData ? toyData : null;
		set => CurrentPhysicsItem = value;
	}

	public void PhysicsListAdd(PhysicsData physics) {
		PhysicsItems.Add(physics);
		CurrentPhysicsItem = physics;
	}

	public string ModelPath {
		get => CurrentToyData?.ModelPath ?? "";
		set {
			if (CurrentToyData != null) {
				CurrentToyData.ModelPath = value;
				InvokeChange(nameof(ModelPath));
			}
		}
	}

	public string CollPath {
		get => CurrentToyData?.CollPath ?? "";
		set {
			if (CurrentToyData != null) {
				CurrentToyData.CollPath = value;
				InvokeChange(nameof(CollPath));
			}
		}
	}
	public string ToyType {
		get => CurrentToyData?.ToyType ?? "";
		set {
			if (CurrentToyData != null) {
				CurrentToyData.ToyType = value;
				InvokeChange(nameof(ToyType));
			}
		}
	}

	public string RopeMaterial {
		get => CurrentToyData?.RopeMaterial ?? "";
		set {
			if (CurrentToyData != null) {
				CurrentToyData.RopeMaterial = value;
				InvokeChange(nameof(RopeMaterial));
			}
		}
	}

	public PhysicsPatchData? CurrentPatchData {
		get => CurrentPhysicsItem is PhysicsPatchData patchData ? patchData : null;
		set => CurrentPhysicsItem = value;
	}

	public string PatchMaterial {
		get => CurrentPatchData?.Material ?? "";
		set {
			if (CurrentPatchData != null) {
				CurrentPatchData.Material = value;
				InvokeChange(nameof(PatchMaterial));
			}
		}
	}
	public string[] AeroModelTypes { get; } = [
		PhysicsPatchData.ATFaceOneSide,
		PhysicsPatchData.ATFaceTwoSide,
		PhysicsPatchData.ATFaceTwoSideLiftDrag,
		PhysicsPatchData.ATOneSide,
		PhysicsPatchData.ATPoint,
		PhysicsPatchData.ATTwoSide,
		PhysicsPatchData.ATTwoSideLiftDrag
		];


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
