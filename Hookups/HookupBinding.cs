using SCS_Mod_Helper.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SCS_Mod_Helper.Hookups;

public class HookupBinding: INotifyPropertyChanged {

	public HookupBinding() {
	}

	public string MProjectLocation = "";
	public string ProjectLocation {
		get {
			return MProjectLocation;
		}
		set {
			MProjectLocation = value;
			InvokeChange(nameof(ProjectLocationShow));
		}
	}
	public string ProjectLocationShow => Util.GetString("TextProjectLocation") + MProjectLocation;

	private bool mIsAnimHookup = true;
	public bool IsAnimHookup {
		get => mIsAnimHookup;
		set {
			mIsAnimHookup = value;
			InvokeChange(nameof(IsAnimHookup));

			InvokeChange(nameof(AnimVisibility));
			InvokeChange(nameof(StartClickable));
		}
	}

	private string mHookupName = "";
	public string HookupName {
		get => mHookupName;
		set {
			mHookupName = value;
			InvokeChange(nameof(HookupName));

			InvokeChange(nameof(StartClickable));
			InvokeChange(nameof(HookupNameForeground));
		}
	}

	public SolidColorBrush HookupNameForeground => new(HookupName.Length > 12 ? Colors.Red : Colors.Black);

	private string mModelLocation = "";
	public string ModelLocation {
		get => mModelLocation;
		set {
			mModelLocation = value;
			InvokeChange(nameof(ModelLocation));

			InvokeChange(nameof(StartClickable));
		}
	}

	private string mAnimationLocation = "";
	public string AnimationLocation {
		get => mAnimationLocation;
		set {
			mAnimationLocation = value;
			InvokeChange(nameof(AnimationLocation));

			InvokeChange(nameof(StartClickable));
		}
	}

	public Visibility AnimVisibility => IsAnimHookup ? Visibility.Visible : Visibility.Collapsed;

	public bool StartClickable {
		get => HookupName.Length > 0 && ModelLocation.Length > 0 && (!IsAnimHookup || AnimationLocation.Length > 0);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void InvokeChange(string name) {
		PropertyChanged?.Invoke(this, new(name));
	}
}
