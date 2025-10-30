using SCS_Mod_Helper.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.AccAddon;

/// <summary>
/// AutoFillSelectionWindow.xaml 的交互逻辑
/// </summary>
public partial class AutoFillSelectionWindow: BaseWindow {
	private readonly AutoFillBinding binding;
	public bool CheckModelName {
		get => binding.CheckModelName; set => binding.CheckModelName = value;
	}
	public bool CheckModelType {
		get => binding.CheckModelType; set => binding.CheckModelType = value;
	}
	public bool CheckModelPath {
		get => binding.CheckModelPath; set => binding.CheckModelPath = value;
	}
	public bool CheckModelPathUK {
		get => binding.CheckModelPathUK; set => binding.CheckModelPathUK = value;
	}
	public bool CheckCollPath {
		get => binding.CheckCollPath; set => binding.CheckCollPath = value;
	}
	public AutoFillSelectionWindow(
		int type,
		string? modelName,
		string? modelType,
		string? modelPath,
		string? modelPathUK,
		string? collPath) {
		InitializeComponent();
		binding = new(type, modelName, modelType, modelPath, modelPathUK, collPath);
		int selectCount = 0;
		if (modelName != null)
			selectCount++;
		if (modelType != null)
			selectCount++;
		if (modelPath != null)
			selectCount++;
		if (modelPathUK != null)
			selectCount++;
		if (collPath != null)
			selectCount++;
		binding.SelectCount = selectCount;
		StackMain.DataContext = binding;
	}

	private void ResultClick(object sender, RoutedEventArgs e) {
		DialogResult = true;
		Close();
	}

	private void Window_Closing(object sender, CancelEventArgs e) {
		AccAddonHistory.Default.AutoFillSelectAll = binding.SelectAll;
		AccAddonHistory.Default.Save();
	}
}

public class AutoFillBinding(
	int chooseType, 
	string? modelName,
	string? modelType,
	string? modelPath,
	string? modelPathUK,
	string? collPath): INotifyPropertyChanged {

	private int mchooseType = chooseType;
	public int ChooseType {
		get => mchooseType;
		set {
			mchooseType = value;

			InvokeChange(nameof(VisibleModelName));
		}
	}

	private bool mSelectAll = AccAddonHistory.Default.AutoFillSelectAll;
	public bool SelectAll {
		get => mSelectAll;
		set {
			mSelectAll = value;
			InvokeChange();

			CheckModelName = value;
			CheckModelType = value;
			CheckModelPath = value;
			CheckModelPathUK = value;
			CheckCollPath = value;
		}
	}

	private int mSelectCount = 0;
	public int SelectCount {
		get => mSelectCount;
		set {
			mSelectCount = value; 
			InvokeChange();
		}
	}

	private bool mCheckModelName = !string.IsNullOrEmpty(modelName);
	public bool CheckModelName {
		get => mCheckModelName;
		set {
			mCheckModelName = value;
			InvokeChange();
		}
	}

	private string? mTextModelName = modelName;
	public string? TextModelName {
		get => mTextModelName;
		set {
			mTextModelName = value;
			InvokeChange();

			InvokeChange(nameof(VisibleModelName));
		}
	}
	public Visibility VisibleModelName {
		get {
			if (string.IsNullOrEmpty(TextModelName))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
	}


	private bool mCheckModelType = !string.IsNullOrEmpty(modelType);
	public bool CheckModelType {
		get => mCheckModelType;
		set {
			mCheckModelType = value; 
			InvokeChange();
		}
	}

	private string? mTextModelType = modelType;
	public string? TextModelType {
		get => mTextModelType;
		set {
			mTextModelType = value;
			InvokeChange();

			InvokeChange(nameof(VisibleModelType));
		}
	}
	public Visibility VisibleModelType {
		get {
			if (string.IsNullOrEmpty(TextModelType))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
	}


	private bool mCheckModelPath = !string.IsNullOrEmpty(modelPath);
	public bool CheckModelPath {
		get => mCheckModelPath;
		set {
			mCheckModelPath = value;
			InvokeChange();
		}
	}

	private string? mTextModelPath = modelPath;
	public string? TextModelPath {
		get => mTextModelPath;
		set {
			mTextModelPath = value;
			InvokeChange();

			InvokeChange(nameof(VisibleModelPath));
		}
	}

	public Visibility VisibleModelPath {
		get {
			if (ChooseType == AccAddonBinding.MODEL || string.IsNullOrEmpty(TextModelPath))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
	}


	private bool mCheckModelPathUK = !string.IsNullOrEmpty(modelPathUK);
	public bool CheckModelPathUK {
		get => mCheckModelPathUK;
		set {
			mCheckModelPathUK = value;
			InvokeChange();
		}
	}

	private string? mTextModelPathUK = modelPathUK;
	public string? TextModelPathUK {
		get => mTextModelPathUK;
		set {
			mTextModelPathUK = value;
			InvokeChange();

			InvokeChange(nameof(VisibleModelPathUK));
		}
	}

	public Visibility VisibleModelPathUK {
		get {
			if (ChooseType == AccAddonBinding.MODEL_UK || string.IsNullOrEmpty(TextModelPathUK))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
	}


	private bool mCheckCollPath = !string.IsNullOrEmpty(collPath);
	public bool CheckCollPath {
		get => mCheckCollPath;
		set {
			mCheckCollPath = value;
			InvokeChange();
		}
	}

	private string? mTextCollPath = collPath;
	public string? TextCollPath {
		get => mTextCollPath;
		set {
			mTextCollPath = value;
			InvokeChange();

			InvokeChange(nameof(VisibleCollPath));
		}
	}

	public Visibility VisibleCollPath {
		get {
			if (ChooseType == AccAddonBinding.MODEL_COLL || string.IsNullOrEmpty(TextCollPath))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
}