using SCS_Mod_Helper.Accessory.PhysicsToy;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SCS_Mod_Helper.Accessory.AccHookup {
	public class SuiItem(string suiFilename): INotifyPropertyChanged {

		public SuiItem():this("") {

		}

		private string mSuiFilename = suiFilename;
		public string SuiFilename {
			get => mSuiFilename;
			set {
				mSuiFilename = value;
				InvokeChange(nameof(SuiFilename));
			}
		}

		public readonly ObservableCollection<AccessoryHookupData> HookupItems = [];

		public readonly ObservableCollection<PhysicsToyData> PhysicsItems = [];

		public event PropertyChangedEventHandler? PropertyChanged;
		private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
	}


}
