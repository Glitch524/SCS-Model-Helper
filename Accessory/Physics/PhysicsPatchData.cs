namespace SCS_Mod_Helper.Accessory.Physics {
	public class PhysicsPatchData(string physicsName = ""): PhysicsData(physicsName) {

		private string mMaterial = "";
		public string Material {
			get => mMaterial;
			set {
				mMaterial = value;
				InvokeChange();
			}
		}

		private float? mAreaDensity = null;
		public float? AreaDensity {
			get => mAreaDensity;
			set {
				mAreaDensity = value;
				InvokeChange();
			}
		}

		public const string ATFaceOneSide = "AT_face_one_side";
		public const string ATFaceTwoSide = "AT_face_two_side";
		public const string ATFaceTwoSideLiftDrag = "AT_face_two_side_lift_drag";
		public const string ATOneSide = "AT_one_side";
		public const string ATPoint = "AT_point";
		public const string ATTwoSide = "AT_two_side";
		public const string ATTwoSideLiftDrag = "AT_two_side_lift_drag";

		private string mAeroModelType = ATTwoSideLiftDrag;

		public string AeroModelType {
			get => mAeroModelType;
			set {
				mAeroModelType = value;
				InvokeChange();
			}
		}

		private float? mLinearStiffness = null;
		public float? LinearStiffness {
			get => mLinearStiffness;
			set {
				mLinearStiffness = value;
				InvokeChange();
			}
		}

		private float? mDragCoefficient = null;
		public float? DragCoefficient {
			get => mDragCoefficient;
			set {
				mDragCoefficient = value;
				InvokeChange();
			}
		}

		private float? mLiftCoefficient = null;
		public float? LiftCoefficient {
			get => mLiftCoefficient;
			set {
				mLiftCoefficient = value;
				InvokeChange();
			}
		}

		private float?[] mTCMinFirst = [null, null];
		public float?[] TCMinFirst {
			get => mTCMinFirst;
			set {
				mTCMinFirst = value;
				InvokeChange();

				InvokeChange(nameof(TCMinFirst0));
				InvokeChange(nameof(TCMinFirst1));
			}
		}

		public float? TCMinFirst0 {
			get => TCMinFirst[0];
			set {
				TCMinFirst[0] = value;
				InvokeChange();
			}
		}

		public float? TCMinFirst1 {
			get => TCMinFirst[1];
			set {
				TCMinFirst[1] = value;
				InvokeChange();
			}
		}

		private float?[] mTCMaxFirst = [null, null];
		public float?[] TCMaxFirst {
			get => mTCMaxFirst;
			set {
				mTCMaxFirst = value;
				InvokeChange();

				InvokeChange(nameof(TCMaxFirst0));
				InvokeChange(nameof(TCMaxFirst1));
			}
		}

		public float? TCMaxFirst0 {
			get => TCMaxFirst[0];
			set {
				TCMaxFirst[0] = value;
				InvokeChange();
			}
		}

		public float? TCMaxFirst1 {
			get => TCMaxFirst[1];
			set {
				TCMaxFirst[1] = value;
				InvokeChange();
			}
		}

		private float?[] mTCMinSecond = [null, null];
		public float?[] TCMinSecond {
			get => mTCMinSecond;
			set {
				mTCMinSecond = value;
				InvokeChange();

				InvokeChange(nameof(TCMinSecond0));
				InvokeChange(nameof(TCMinSecond1));
			}
		}

		public float? TCMinSecond0 {
			get => TCMinSecond[0];
			set {
				TCMinSecond[0] = value;
				InvokeChange();
			}
		}

		public float? TCMinSecond1 {
			get => TCMinSecond[1];
			set {
				TCMinSecond[1] = value;
				InvokeChange();
			}
		}

		private float?[] mTCMaxSecond = [null, null];
		public float?[] TCMaxSecond {
			get => mTCMaxSecond;
			set {
				mTCMaxSecond = value;
				InvokeChange();

				InvokeChange(nameof(TCMaxSecond0));
				InvokeChange(nameof(TCMaxSecond1));
			}
		}

		public float? TCMaxSecond0 {
			get => TCMaxSecond[0];
			set {
				TCMaxSecond[0] = value;
				InvokeChange();
			}
		}

		public float? TCMaxSecond1 {
			get => TCMaxSecond[1];
			set {
				TCMaxSecond[1] = value;
				InvokeChange();
			}
		}

		private uint? mXRes = null;
		public uint? XRes {
			get => mXRes;
			set {
				mXRes = value;
				InvokeChange();
			}
		}

		private uint? mYRes = null;
		public uint? YRes {
			get => mYRes;
			set {
				mYRes = value;
				InvokeChange();
			}
		}

		private float? mXSize = null;
		public float? XSize {
			get => mXSize;
			set {
				mXSize = value;
				InvokeChange();
			}
		}

		private float? mYSize = null;
		public float? YSize {
			get => mYSize;
			set {
				mYSize = value;
				InvokeChange();
			}
		}
	}
}
