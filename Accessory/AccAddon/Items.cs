using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.AccAddon; 

public class PartTypeItem(string partTypeValue, string partTypeDisplay) {
	private string mPartTypeValue = partTypeValue;
	public string PartTypeValue {
		get => mPartTypeValue; set => mPartTypeValue = value;
	}
	private string mPartTypeDisplay = partTypeDisplay;
	public string PartTypeDisplay {
		get => mPartTypeDisplay; set => mPartTypeDisplay = value;
	}
}

public class OthersItem: INotifyPropertyChanged {

	public OthersItem(string othersName, string othersValue, string othersNameTip = "") {
		mOthersName = othersName;
		mOthersValue = othersValue;
		if (othersNameTip.Length == 0) {
			switch (mOthersName) {
				case AccDataIO.NameData:
					othersNameTip = Util.GetString("MenuOthersData");
					IsData = true;
					IsArray = true;
					UseQuoteMark = false;
					CheckBoxEnabled = false;
					break;
				case AccDataIO.NameSuitableFor:
					othersNameTip = Util.GetString("MenuOthersSuitableFor");
					IsArray = true;
					UseQuoteMark = true;
					CheckBoxEnabled = false;
					break;
				case AccDataIO.NameConflictWith:
					othersNameTip = Util.GetString("MenuOthersConflictWith");
					IsArray = true;
					UseQuoteMark = true;
					CheckBoxEnabled = false;
					break;
				case AccDataIO.NameDefaults:
					othersNameTip = Util.GetString("MenuOthersDefaults");
					IsArray = true;
					UseQuoteMark = true;
					CheckBoxEnabled = false;
					break;
				case AccDataIO.NameOverrides:
					othersNameTip = Util.GetString("MenuOthersOverrides");
					IsArray = true;
					UseQuoteMark = true;
					CheckBoxEnabled = false;
					break;
				case AccDataIO.NameRequire:
					othersNameTip = Util.GetString("MenuOtherRequire");
					IsArray = true;
					UseQuoteMark = true;
					CheckBoxEnabled = false;
					break;
			}
		}
		mOthersNameTip = othersNameTip;
	}
	public OthersItem() : this("", "") { }
	private string mOthersName;
	public string OthersName {
		get => mOthersName;
		set {
			mOthersName = value;
			if (AccessoryItem.ArrayItems.Contains(value)) {
				IsArray = true;
				if (value == "data") {
					IsData = true;
					UseQuoteMark = false;
				} else {
					IsData = false;
					UseQuoteMark = true;
				}
				CheckBoxEnabled = false;
			} else {
				CheckBoxEnabled = true;
				IsData = false;
			}

			InvokeChange(nameof(OthersName));
		}
	}


	private bool mIsData = false;
	public bool IsData {
		get => mIsData;
		set {
			mIsData = value;
			InvokeChange(nameof(IsData));
		}
	}

	private string mOthersNameTip;
	public string OthersNameTip {
		get {
			if (mOthersNameTip.Length > 0) 
				return mOthersNameTip;
			return mOthersName;
		}
		set {
			mOthersNameTip = value;
			InvokeChange(nameof(OthersNameTip));
		}
	}

	private string mOthersValue;
	public string OthersValue {
		get => mOthersValue;
		set {
			mOthersValue = value;
			InvokeChange(nameof(OthersValue));
		}
	}

	private bool mIsArray;
	public bool IsArray {
		get => mIsArray;
		set {
			mIsArray = value;
			InvokeChange(nameof(IsArray));
		}
	}

	private bool mCheckBoxEnabled = true;
	public bool CheckBoxEnabled {
		get => mCheckBoxEnabled;
		set {
			mCheckBoxEnabled = value;
			InvokeChange(nameof(CheckBoxEnabled));
		}
	}

	private bool mUseQuoteMark;
	public bool UseQuoteMark {
		get => mUseQuoteMark;
		set {
			mUseQuoteMark = value;
			InvokeChange(nameof(UseQuoteMark));
		}
	}

	public static void ReadLines(ObservableCollection<OthersItem> othersList, string lines) {
		var lineList = lines.Trim().Split(DefaultData.LineSplit);
		try {
			foreach (string line in lineList) {
				if (line.Length == 0) continue;
				OthersItem? o = LineParse(line);
				if (o != null)
					othersList.Add(o);
			}
		} catch (Exception) {
			MessageBox.Show(Util.GetString("MessageLoadOtherskErr"));
		}
	}

	public static OthersItem? LineParse(string line) {
		var items = line.Trim().Split(DefaultData.ItemSplit);
		if (items.Length < 3)
			return null;
		return new OthersItem(items[0], items[1], items[2]);
	}

	public const int IndexOthersName = 0;
	public const int IndexOthersValue = 1;
	public static string GetHeaderLine() => string.Join(DefaultData.ItemSplit, [nameof(OthersName), nameof(OthersValue)]);

	public string ToLine() => $"{mOthersName}{DefaultData.ItemSplit}{mOthersValue}{DefaultData.ItemSplit}{mOthersNameTip}";


	public static string JoinOthers(ObservableCollection<OthersItem> othersList) {
		return Util.Join(othersList, (t) => true, (t) => t.ToLine());
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
