using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCS_Mod_Helper.Manifest;

public class CategoryInfo(string category): INotifyPropertyChanged {
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

	public event PropertyChangedEventHandler? PropertyChanged;
    private void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
    }
