using SCS_Mod_Helper.Base;

namespace SCS_Mod_Helper.Accessory.Physics; 
public class PhysicsData: BaseBinding {

	protected PhysicsData(string physicsName) {
		mPhysicsName = physicsName;
	}

	private string mPhysicsName;

	public string PhysicsName {
		get => mPhysicsName;
		set {
			mPhysicsName = value;
			InvokeChange();
		}
	}
}
