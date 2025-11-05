using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Accessory.AccHookup {
	public class SuiItem(string suiFilename): BaseBinding {

		public SuiItem():this("") {

		}

		private string mSuiFilename = suiFilename;
		public string SuiFilename {
			get => mSuiFilename;
			set {
				mSuiFilename = value;
				InvokeChange();
			}
		}

		public readonly ObservableCollection<AccessoryHookupData> HookupItems = [];

		public readonly ObservableCollection<PhysicsData> PhysicsItems = [];
	}
}
