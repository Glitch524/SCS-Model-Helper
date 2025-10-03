using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Manifest;

namespace SCS_Mod_Helper.Utils {
	public static class Instances {
		private static ModProject? modelProject;
		public static ModProject ModelProject {
			get {
				modelProject ??= new ModProject();
				return modelProject;
			}
		}

		public static string ProjectLocation {
			get => ModelProject.ProjectLocation; set => ModelProject.ProjectLocation = value;
		}

		private static string projectLocationOfDict = "";

		private static Dictionary<string, List<string>>? mLocaleDict = null;
		public static Dictionary<string, List<string>> LocaleDict {
			get {
				if (mLocaleDict == null || ModelProject.ProjectLocation != projectLocationOfDict) {
					var projectLocation = ModelProject.ProjectLocation;
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
