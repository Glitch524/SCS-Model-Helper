using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.Accessory.AccHookup;

public class AccessoryHookupItem(string modelName): AccessoryItem(modelName, "", null, null, "", "unknown", "", "default", "default") {
	public AccessoryHookupItem() : this("") {
	}

	private string mModelPath = "";
	public string ModelPath {
		get => mModelPath;
		set {
			mModelPath = value;
			InvokeChange(nameof(ModelPath));

			LoadLooksAndVariants();
		}
	}

	public ObservableCollection<string>? mLookList = null;
	public ObservableCollection<string>? mVariantList = null;
	public ObservableCollection<string> LookList {
		get {
			if (mLookList == null)
				LoadLooksAndVariants();
			return mLookList!;
		}
	}
	public ObservableCollection<string> VariantList {
		get {
			if (mVariantList == null)
				LoadLooksAndVariants();
			return mVariantList!;
		}
	}

	public void LoadLooksAndVariants() {
		string oldLook = Look, oldVariant = Variant;
		if (mLookList == null)
			mLookList = [];
		else
			mLookList.Clear();
		if (mVariantList == null)
			mVariantList = [];
		else
			mVariantList.Clear();
		if (mModelPath.Length == 0)
			return;
		var path = mModelPath;

		path = path.Replace('/', '\\');
		path = path[..^4] + ".pit";
		path = Instances.ProjectLocation + path;

		static void setValue(ObservableCollection<string> list, string oldValue, Action<string> set) {
			if (list.Count > 0) {
				if (list.Contains(oldValue)) {
					set(oldValue);
				} else
					set(list[0]);
			}
		}
		AccDataIO.ReadLookAndVariant(path, mLookList, mVariantList);
		setValue(mLookList, oldLook, (set) => Look = set);
		setValue(mVariantList, oldVariant, (set) => Variant = set);
	}

	private readonly ObservableCollection<OthersItem> mOthersList = [];
	public ObservableCollection<OthersItem> OthersList => mOthersList;
}