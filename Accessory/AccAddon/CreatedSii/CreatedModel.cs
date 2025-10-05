using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.Accessory.AccAddon.CreatedSii {
	public class CreatedModel(string modelName): INotifyPropertyChanged {
		private string mModelName = modelName;
		public string ModelName {
			get => mModelName;
			set {
				mModelName = value;
				InvokeChange();
			}
		}

		private ObservableCollection<CreatedModelItem> mCreatedModelItems = [];
		public ObservableCollection<CreatedModelItem> CreatedModelItems {
			get => mCreatedModelItems;
			set {
				mCreatedModelItems = value;
				InvokeChange();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
	}
}
