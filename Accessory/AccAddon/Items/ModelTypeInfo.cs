using SCS_Mod_Helper.Utils;
using System.ComponentModel;

namespace SCS_Mod_Helper.Accessory.AccAddon.Items; 
public class ModelTypeInfo: INotifyPropertyChanged {

	public ModelTypeInfo(
		string accETS2,
		string accATS,
		string nameKey) {
		mAccessoryETS2 = accETS2;
		mAccessoryATS = accATS;
		NameKey = nameKey;
		if (nameKey == "")
			mName = "";
		else
			mName = Util.GetString(nameKey);
	}

	public ModelTypeInfo(
		string acc,
		bool? gameType = null) {
		mAccessoryETS2 = acc;
		mGameType = gameType;
		if (gameType == null) {
			mAccessoryETS2 = acc;
			mAccessoryATS = acc;
		} else if (gameType == true) {
			mAccessoryETS2 = acc;
		} else {
			mAccessoryATS = acc;
		}
		NameKey = $"Acc.{acc}";
		mName = Util.GetString($"Acc.{acc}");
	}

	public ModelTypeInfo() : this("", "", "") { }

	private string? mAccessoryETS2;
	public string? AccessoryETS2 {
		get => mAccessoryETS2;
		set {
			mAccessoryETS2 = value;
			InvokeChange(nameof(AccessoryETS2));
		}
	}

	private string? mAccessoryATS;
	public string? AccessoryATS {
		get => mAccessoryATS;
		set {
			mAccessoryATS = value;
			InvokeChange(nameof(AccessoryATS));
		}
	}
	public string Accessory {
		get {
			if (mGameType == null) {
				if (mAccessoryETS2 == mAccessoryATS)
					return mAccessoryETS2!;
				else
					return $"{mAccessoryETS2}/{mAccessoryATS}";
			} else if (mGameType == true)
				return mAccessoryETS2!;
			else
				return mAccessoryATS!;
		}
	}

	private string NameKey;

	private string mName;
	public string Name {
		get => mName;
		set {
			mName = value;
			InvokeChange(nameof(Name));
		}
	}
	public void RefreshName() {
		if (Accessory.Length == 0)
			return;
		Name = Util.GetString(NameKey);
	}
	private readonly bool? mGameType;
	public bool? GameType => mGameType;

	public bool ForETS2 => mGameType == null || mGameType == true;
	public bool ForATS => mGameType == null || mGameType == false;

	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
