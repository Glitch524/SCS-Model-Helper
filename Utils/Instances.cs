using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Main;
using SCS_Mod_Helper.Manifest;

namespace SCS_Mod_Helper.Utils {
	public static class Instances {
		private static BasicInfo? mBasicInfo;
		public static BasicInfo BasicInfo {
			get {
				mBasicInfo ??= new();
				return mBasicInfo;
			}
		}

		public static string ProjectLocation {
			get => BasicInfo.ProjectLocation; set => BasicInfo.ProjectLocation = value;
		}

		private static string projectLocationOfDict = "";

		private static Dictionary<string, List<string>>? mLocaleDict = null;
		public static Dictionary<string, List<string>> LocaleDict {
			get {
				if (mLocaleDict == null || ProjectLocation != projectLocationOfDict) {
					var projectLocation = ProjectLocation;
					if (mLocaleDict == null)
						mLocaleDict = [];
					else
						mLocaleDict.Clear();
					projectLocationOfDict = projectLocation;
					AccDataIO.CheckLocaleDict(mLocaleDict);
				}
				return mLocaleDict;
			}
		}

		public static void LocaleDictReset(string projectLocation) {
			projectLocationOfDict = projectLocation;
			if (mLocaleDict == null) {
				mLocaleDict = [];
			} else
				mLocaleDict.Clear();
		}

		public static void LocaleDictAdd(string key, string value) {
			if (mLocaleDict!.TryGetValue(key, out List<string>? list)) {
				if (!list.Contains(value))
					list.Add(value);
			} else {
				list = [value];
				mLocaleDict.Add(key, list);
			}
		}
	}

}
