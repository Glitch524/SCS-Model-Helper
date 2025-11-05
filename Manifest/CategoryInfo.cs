using SCS_Mod_Helper.Base;

namespace SCS_Mod_Helper.Manifest;

public class CategoryInfo(string category): BaseBinding {
	private bool mCheck = false;
	public bool Check {
		get => mCheck;
		set {
			mCheck = value;
			InvokeChange();
		}
	}

	private string mCategory = category;
    public string Category {
        get => mCategory;
        set {
            mCategory = value;
            InvokeChange();
        }
    }
}
