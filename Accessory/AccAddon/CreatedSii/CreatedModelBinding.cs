using SCS_Mod_Helper.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.Accessory.AccAddon.CreatedSii {
	internal class CreatedModelBinding: INotifyPropertyChanged {
		private string mCurrentProjectLocation = "";
		public string CurrentProjectLocation {
			get => mCurrentProjectLocation;
			set {
				mCurrentProjectLocation = Util.GetString("TextProjectLocation") + value;
				InvokeChange();
			}
		}

		private bool mSelectAll = false;
		public bool SelectAll {
			get => mSelectAll;
			set {
				mSelectAll = value;
				if (CurrentModelItems != null) {
					foreach (var item in CurrentModelItems) {
						item.Check = value;
					}
				}
				InvokeChange();
			}
		}

		private ObservableCollection<CreatedModel> mCreatedModelList = [];
		public ObservableCollection<CreatedModel> CreatedModelList {
			get => mCreatedModelList;
			set {
				mCreatedModelList = value;
				InvokeChange();
			}
		}

		private CreatedModel? mCurrentModel = null;
		public CreatedModel? CurrentModel {
			get => mCurrentModel;
			set {
				mCurrentModel = value;
				InvokeChange();

				InvokeChange(nameof(CurrentModelItems));
			}
		}

		public ObservableCollection<CreatedModelItem>? CurrentModelItems => CurrentModel?.CreatedModelItems;

		public event PropertyChangedEventHandler? PropertyChanged;
		private void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
	}
}
