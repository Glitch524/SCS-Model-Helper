using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Def_Writer;

/// <summary>
/// CleanSii.xaml 的交互逻辑
/// </summary>
public partial class CleanSii : BaseWindow
{

	string ProjectLocation;
    public CleanSii(string projectLocation)
    {
        InitializeComponent();
		ProjectLocation = projectLocation;
		TextProjectLocation.Text = GetString("TextProjectLocation") + projectLocation;
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
		LoadCreatedSii();
	}

	readonly Dictionary<string, ObservableCollection<CreatedSii>> ExistingSIIs = [];
	private void LoadCreatedSii() {
		var defDir = new DirectoryInfo(Utils.DefTruckDir(ProjectLocation));
		void NoSii() {
			MessageBox.Show(this, GetString("MessageLoadSiiErrNoSii"));
			Close();
		}
		if (!defDir.Exists) {
			NoSii();
			return;
		}
		LoadCreatedSii(defDir);
		if (ExistingSIIs.Count == 0) {
			NoSii();
			return;
		}
		TableModelName.ItemsSource = ExistingSIIs.Keys.ToList();
		if (ExistingSIIs.ContainsKey(GetString("Unknown"))) {
			TableModelName.SelectedItem = GetString("Unknown");
		} else {
			var kv = ExistingSIIs.First();
			TableModelName.SelectedItem = kv.Key;
		}
	}

	private void LoadCreatedSii(DirectoryInfo info) {
		foreach (var file in info.GetFiles()) {
			if (file.Extension.Equals(".sii")) {
				GetFileDetail(file.FullName);
			}
		}
		foreach (var dir in info.GetDirectories()) {
			LoadCreatedSii(dir);
		}
	}

	private void GetFileDetail(string file) {
		var pathShort = file.Replace(Utils.DefTruckDir(ProjectLocation), "");
		var modelName = "";
		var truckID = "";
		var modelType = "";
		var ingameName = "";
		var look = "";
		var variant = "";
		try {
			using StreamReader sr = new(file);
			string? line = sr.ReadLine();
			if (line == null || !line.Trim().Equals("SiiNunit")) {
				AddSii(GetString("ModelTypeUnknown"), new CreatedSii(file, pathShort));
				return;
			}
			while ((line = sr.ReadLine()) != null) {
				line = line.Trim();
				if (line.StartsWith("accessory_")) {
					line = line[(line.IndexOf(':') + 1)..].Trim();
					var firstDot = line.IndexOf('.');
					var lastDot = line.LastIndexOf('.');
					modelName = line[..firstDot];
					truckID = line[(firstDot + 1)..lastDot];
					modelType = line[(lastDot + 1)..];
				} else if (line.StartsWith("name:")) {
					var first = line.IndexOf('"');
					var last = line.LastIndexOf('"');
					ingameName = line[(first + 1)..last];
				} else if (line.StartsWith("look:")) {
					look = line.Replace("look:", "").Trim();
				} else if (line.StartsWith("variant:")) {
					variant = line.Replace("variant:", "").Trim();
				}
			}
		} catch (Exception ex) {
			MessageBox.Show(this, GetString("MessageLoadSiiErr") + '\n' + ex.Message);
		}
		AddSii(modelName, new CreatedSii(file, pathShort, truckID, modelType, modelName, ingameName, look, variant));
	}

	private void AddSii(string modelName, CreatedSii c) {
		if (ExistingSIIs.TryGetValue(modelName, out ObservableCollection<CreatedSii>? value)) {
			value.Add(c);
		} else {
			ExistingSIIs.Add(modelName, [c]);
		}
	}

	private void ModelNameSelectionChanged(object sender, SelectionChangedEventArgs e) {
		if (sender == TableModelName) {
			string? selected = TableModelName.SelectedItem?.ToString();
			if (selected == null) {
				var kv = ExistingSIIs.First();
				TableModelName.SelectedItem = kv.Key;
			} else
				TableCreatedSii.ItemsSource = ExistingSIIs[selected];
		}
	}

	private void ButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonDelete) {
			var result = MessageBox.Show(this, GetStringMultiLine("MessageDeleteDoubleCheck", "MessageDeleteDoubleCheck2"), GetString("ButtonDelete"), MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				foreach (var createdSii in selected) {
					if (createdSii.Check) {
						File.Delete(createdSii.Path);
						var modelName = createdSii.ModelName;
						ExistingSIIs[modelName].Remove(createdSii);
						if (ExistingSIIs[modelName].Count == 0) {
							ExistingSIIs.Remove(modelName);
						}
					}
				}
				TableModelName.ItemsSource = ExistingSIIs.Keys.ToList();
				selected.Clear();
			}
			CheckBoxSelectAll.IsChecked = false;
		} else if (sender == ButtonOpen) {
			CreatedSii s = (CreatedSii)TableCreatedSii.SelectedItem;
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

	private void CheckBoxSelectAllChecked(object sender, RoutedEventArgs e) {
		if (sender == CheckBoxSelectAll) {
			foreach (CreatedSii createdSii in TableCreatedSii.ItemsSource) {
				createdSii.Check = CheckBoxSelectAll.IsChecked == true;
			}
		}
	}

	private readonly ObservableCollection<CreatedSii> selected = [];

	private void CheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox)sender;
		CreatedSii createdSii = (CreatedSii)box.DataContext;
		if (createdSii.Check) {
			if (selected.Contains(createdSii))
				return;
			selected.Add(createdSii);
		} else
			selected.Remove(createdSii);
	}
}

public class CreatedSii: INotifyPropertyChanged {

	public CreatedSii(string path, string pathShort) {
		MPath = path;
		MPathShort = pathShort;
		MTruckID = Utils.GetString("UnknownFile");
		MModelType = "";
		ModelName = Utils.GetString("Unknown");
		MIngameName = "";
		MLook = "";
		MVariant = "";
	}

	public CreatedSii(string path, string pathShort, string truckID, string modelType, string modelName, string ingameName, string look, string variant) {
		MPath = path;
		MPathShort = pathShort;
		MTruckID = truckID;
		MModelType = modelType;
		ModelName = modelName;
		MIngameName = ingameName;
		MLook = look;
		MVariant = variant;
	}

	public bool MCheck = false;
	public bool Check {
		get {
			return MCheck;
		}
		set {
			MCheck = value;
			InvokeChange(nameof(Check));
		}
	}

	public string MPath;
	public string Path {
		get {
			return MPath;
		}
		set {
			MPath = value;
			InvokeChange(nameof(Path));
		}
	}
	public string MPathShort;
	public string PathShort {
		get {
			return MPathShort;
		}
		set {
			MPathShort = value;
			InvokeChange(nameof(PathShort));
		}
	}

	public string MTruckID;
	public string TruckID {
		get {
			return MTruckID;
		}
		set {
			MTruckID = value;
			InvokeChange(nameof(TruckID));
		}
	}

	public string MModelType;
	public string ModelType {
		get {
			return MModelType;
		}
		set {
			MModelType = value;
			InvokeChange(nameof(ModelType));
		}
	}
	public string ModelName;

	public string MIngameName;
	public string IngameName {
		get {
			return MIngameName;
		}
		set {
			MIngameName = value;
			InvokeChange(nameof(IngameName));
		}
	}

	public string MLook;
	public string Look {
		get {
			return MLook;
		}
		set {
			MLook = value;
			InvokeChange(nameof(Look));
		}
	}

	public string MVariant;

	public string Variant {
		get {
			return MVariant;
		}
		set {
			MVariant = value;
			InvokeChange(nameof(Variant));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void InvokeChange(string name) {
		PropertyChanged?.Invoke(this, new(name));
	}
}
