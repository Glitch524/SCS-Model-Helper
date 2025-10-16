using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.AccAddon.Items;

public class OthersItem: INotifyPropertyChanged {

	public OthersItem(string othersName, string othersValue) {
		mOthersName = othersName;
		mOthersValue = othersValue;
		mOthersNameTip = "";
		SetData(othersName);
	}

	private void SetData(string othersName) {
		switch (othersName) {
			case AccDataIO.NameData:
				OthersNameTip = Util.GetString("MenuOthersData");
				IsData = true;
				IsArray = true;
				UseQuoteMark = false;
				CheckBoxEnabled = false;
				break;
			case AccDataIO.NameSuitableFor:
				OthersNameTip = Util.GetString("MenuOthersSuitableFor");
				IsArray = true;
				UseQuoteMark = true;
				CheckBoxEnabled = false;
				break;
			case AccDataIO.NameConflictWith:
				OthersNameTip = Util.GetString("MenuOthersConflictWith");
				IsArray = true;
				UseQuoteMark = true;
				CheckBoxEnabled = false;
				break;
			case AccDataIO.NameDefaults:
				OthersNameTip = Util.GetString("MenuOthersDefaults");
				IsArray = true;
				UseQuoteMark = true;
				CheckBoxEnabled = false;
				break;
			case AccDataIO.NameOverrides:
				OthersNameTip = Util.GetString("MenuOthersOverrides");
				IsArray = true;
				UseQuoteMark = true;
				CheckBoxEnabled = false;
				break;
			case AccDataIO.NameRequire:
				OthersNameTip = Util.GetString("MenuOtherRequire");
				IsArray = true;
				UseQuoteMark = true;
				CheckBoxEnabled = false;
				break;
			default:
				OthersNameTip = "";
				IsArray = false;
				UseQuoteMark = false;
				CheckBoxEnabled = true;
				break;
		}
	}

	public OthersItem() : this("", "") { }
	private string mOthersName;
	public string OthersName {
		get => mOthersName;
		set {
			mOthersName = value;
			InvokeChange(nameof(OthersName));

			SetData(mOthersName);
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

	public static void ReadLines(ObservableCollection<OthersItem> othersList, string lines, Action<string> data) {
		var lineList = lines.Trim().Split(DefaultData.LineSplit);
		try {
			foreach (string line in lineList) {
				if (line.Length == 0) 
					continue;
				OthersItem? o = LineParse(line);
				if (o != null) {
					othersList.Add(o);
					if (o.OthersName == AccDataIO.NameData)
						data.Invoke(o.OthersValue);
				}
			}
		} catch (Exception) {
			MessageBox.Show(Util.GetString("MessageLoadOtherskErr"));
		}
	}

	public static OthersItem? LineParse(string line) {
		var items = line.Trim().Split(DefaultData.ItemSplit);
		if (items.Length < 2)
			return null;
		return new OthersItem(items[0], items[1]);
	}

	public const int IndexOthersName = 0;
	public const int IndexOthersValue = 1;
	public static string GetHeaderLine() => string.Join(DefaultData.ItemSplit, [nameof(OthersName), nameof(OthersValue)]);

	public string ToLine() => $"{mOthersName}{DefaultData.ItemSplit}{mOthersValue}";


	public static string JoinOthers(ObservableCollection<OthersItem> othersList) => Util.Join(othersList, (t) => t.ToLine());

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
