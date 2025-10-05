using SCS_Mod_Helper.Utils;
using System.ComponentModel;

namespace SCS_Mod_Helper.Accessory.AccAddon.CreatedSii {
	public class CreatedModelItem: INotifyPropertyChanged {

		public CreatedModelItem(string path, string pathShort) {
			MPath = path;
			MPathShort = pathShort;
			MTruckID = Util.GetString("UnknownFile");
			MModelType = "";
			ModelName = Util.GetString("Unknown");
			MIngameName = "";
			MLook = "";
			MVariant = "";
		}

		public CreatedModelItem(string path, string pathShort, string truckID, string modelType, string modelName, string ingameName, string look, string variant) {
			MPath = path;
			MPathShort = pathShort;
			MTruckID = truckID;
			MModelType = modelType;
			ModelName = modelName;
			MIngameName = ingameName;
			MLook = look;
			MVariant = variant;
		}

		public CreatedModel? Parent = null;

		public bool MCheck = false;
		public bool Check {
			get => MCheck;
			set {
				MCheck = value;
				InvokeChange(nameof(Check));
			}
		}

		public string MPath;
		public string Path {
			get => MPath;
			set {
				MPath = value;
				InvokeChange(nameof(Path));
			}
		}
		public string MPathShort;
		public string PathShort {
			get => MPathShort;
			set {
				MPathShort = value;
				InvokeChange(nameof(PathShort));
			}
		}

		public string MTruckID;
		public string TruckID {
			get => MTruckID;
			set {
				MTruckID = value;
				InvokeChange(nameof(TruckID));
			}
		}

		public string MModelType;
		public string ModelType {
			get => MModelType;
			set {
				MModelType = value;
				InvokeChange(nameof(ModelType));
			}
		}
		public string ModelName;

		public string MIngameName;
		public string IngameName {
			get => MIngameName;
			set {
				MIngameName = value;
				InvokeChange(nameof(IngameName));
			}
		}

		public string MLook;
		public string Look {
			get => MLook;
			set {
				MLook = value;
				InvokeChange(nameof(Look));
			}
		}

		public string MVariant;

		public string Variant {
			get => MVariant;
			set {
				MVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
	}
}
