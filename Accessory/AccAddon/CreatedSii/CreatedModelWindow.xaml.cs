using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon.CreatedSii;

/// <summary>
/// CleanSii.xaml 的交互逻辑
/// </summary>
public partial class CreatedModelWindow : BaseWindow
{
	private readonly CreatedModelBinding Binding = new();

	readonly string ProjectLocation = Instances.ProjectLocation;

	public ObservableCollection<CreatedModel> CreatedModelList {
		get => Binding.CreatedModelList; set => Binding.CreatedModelList = value;
	}
	public CreatedModel? CurrentModel {
		get => Binding.CurrentModel; set => Binding.CurrentModel = value;
	}

	public ObservableCollection<CreatedModelItem>? CurrentModelItems => Binding.CurrentModelItems;


	public CreatedModelWindow()
    {
        InitializeComponent();
		GridMain.DataContext = Binding;

		Binding.CurrentProjectLocation = ProjectLocation;

		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
		LoadCreatedSii();
	}

	private readonly DirectoryInfo defDir = new(Paths.DefTruckDir(Instances.ProjectLocation));
	readonly Dictionary<string, CreatedModel> ModelPair = [];
	private void LoadCreatedSii() {
		ModelPair.Clear();
		void NoSii() {
			MessageBox.Show(this, GetString("MessageLoadSiiErrNoSii"));
			Close();
		}
		if (!defDir.Exists) {
			NoSii();
			return;
		}
		LoadCreatedSii(defDir);
		if (CreatedModelList.Count == 0) {
			NoSii();
			return;
		}
		CurrentModel = CreatedModelList.First();
	}

	private void LoadCreatedSii(DirectoryInfo info) {
		foreach (var dir in info.GetDirectories()) {
			LoadCreatedSii(dir);
		}
		foreach (var file in info.GetFiles()) {
			if (file.Extension.Equals(".sii")) {
				var filePath = file.FullName;

				var pathShort = filePath.Replace(Paths.DefTruckDir(ProjectLocation), "");

				var modelName = "";
				var truckID = "";
				var modelType = "";
				var ingameName = "";
				var look = "";
				var variant = "";
				var deTruck = filePath.Replace(defDir.FullName + '\\', "");
				var split = deTruck.Split('\\');
				if (split.Length >= 4) {
					truckID = split[0];
					modelType = split[2];
					modelName = split[3][..^4];
				}
				CreatedModelItem c = new(filePath, pathShort, truckID, modelType, modelName, ingameName, look, variant);

				if (ModelPair.TryGetValue(modelName, out CreatedModel? createdModel)) {
					c.Parent = createdModel;
					createdModel.CreatedModelItems.Add(c);
				} else {
					CreatedModel cm = new(modelName);
					c.Parent = cm;
					cm.CreatedModelItems.Add(c);
					CreatedModelList.Add(cm);
					ModelPair.Add(modelName, cm);
				}
			}
		}
	}

	private void ButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonDelete) {
			var result = MessageBox.Show(this, GetString("MessageDeleteDoubleCheck"), GetString("ButtonDelete"), MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				foreach (var s in selected) {
					if (s.Check) {
						File.Delete(s.Path);
						var parent = s.Parent;
						if (parent != null) {
							parent.CreatedModelItems.Remove(s);
							if (parent.CreatedModelItems.Count == 0)
								CreatedModelList.Remove(parent);
						}
					}
				}
			}
			Binding.SelectAll = false;
		} else if (sender == ButtonOpen) {
			CreatedModelItem s = (CreatedModelItem)TableCreatedSii.SelectedItem;
			if (s == null) {
				MessageBox.Show(this, GetString("MessageOpenErrNoSelected"));
				return;
			}
			if (!File.Exists(s.Path)) {
				MessageBox.Show(this, GetString("MessageOpenErrNotExist"));
				return;
			}
			Process.Start("notepad.exe", s.Path);
		} else if (sender == ButtonClose) {
			DialogResult = true;
		}
	}

	private readonly ObservableCollection<CreatedModelItem> selected = [];

	private void CheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox)sender;
		CreatedModelItem createdSii = (CreatedModelItem)box.DataContext;
		if (createdSii.Check) {
			if (selected.Contains(createdSii))
				return;
			selected.Add(createdSii);
		} else
			selected.Remove(createdSii);
	}
}

