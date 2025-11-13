using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Accessory.AccAddon.CreatedSii; 
public class CreatedModel(string modelName): BaseBinding {
	private string modelName = modelName;
	public string ModelName {
		get => modelName;
		set {
			modelName = value;
			InvokeChange();
		}
	}

	private ObservableCollection<CreatedModelItem> createdModelItems = [];
	public ObservableCollection<CreatedModelItem> CreatedModelItems {
		get => createdModelItems;
		set {
			createdModelItems = value;
			InvokeChange();
		}
	}

	private int selectCount = 0;
	public int SelectCount {
		get => selectCount;
		set {
			selectCount = value;
			InvokeChange(nameof(SelectAll));
		}
	}

	public void UpdateCount() {
		selectCount = 0;
	}

	public bool? SelectAll {
		get {
			if (SelectCount == CreatedModelItems.Count)
				return true;
			else if (SelectCount == 0)
				return false;
			return null;
		}
		set {
			foreach (var item in CreatedModelItems) {
				item.Check = value == true;
			}
			SelectCount = value == true ? createdModelItems.Count : 0;
		}
	}
}

public class CreatedModelItem: BaseBinding {

	public CreatedModelItem(string path, string pathShort) {
		this.path = path;
		this.pathShort = pathShort;
		truckID = Util.GetString("UnknownFile");
		modelType = "";
		modelName = Util.GetString("Unknown");
	}

	public CreatedModelItem(string path, string pathShort, string truckID, string modelType, string modelName) {
		this.path = path;
		this.pathShort = pathShort;
		this.truckID = truckID;
		this.modelType = modelType;
		this.modelName = modelName;
	}

	public bool MCheck = false;
	public bool Check {
		get => MCheck;
		set {
			MCheck = value;
			InvokeChange();
		}
	}

	public string path;
	public string Path {
		get => path;
		set {
			path = value;
			InvokeChange();
		}
	}
	public string pathShort;
	public string PathShort {
		get => pathShort;
		set {
			pathShort = value;
			InvokeChange();
		}
	}

	public string truckID;
	public string TruckID {
		get => truckID;
		set {
			truckID = value;
			InvokeChange();
		}
	}

	public string modelType;
	public string ModelType {
		get => modelType;
		set {
			modelType = value;
			InvokeChange();
		}
	}
	public string modelName;
	public string ModelName {
		get => modelName;
		set {
			modelName = value;
			InvokeChange();
		}
	}

	public string ingameName = "";
	public string IngameName {
		get => ingameName;
		set {
			ingameName = value;
			InvokeChange();
		}
	}

	public string look = "";
	public string Look {
		get => look;
		set {
			look = value;
			InvokeChange();
		}
	}

	public string variant = "";

	public string Variant {
		get => variant;
		set {
			variant = value;
			InvokeChange();
		}
	}

	public void ReadModelDetail() {
		if (IngameName != string.Empty)
			return;
		Task.Run(() => {
			AccDataIO.ReadAccAddon(this);
			if (Look == string.Empty)
				Look = "default";
			if (Variant == string.Empty)
				Variant = "default";
		});
	}
}