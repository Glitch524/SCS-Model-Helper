using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCS_Mod_Helper.Accessory.Physics; 
public class PhysicsData: INotifyPropertyChanged {

	protected PhysicsData(string physicsName) {
		mPhysicsName = physicsName;
	}

	private string mPhysicsName;

	public string PhysicsName {
		get => mPhysicsName;
		set {
			mPhysicsName = value;
			InvokeChange(nameof(PhysicsName));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
}
