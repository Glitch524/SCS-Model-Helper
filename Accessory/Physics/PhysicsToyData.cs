using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.Physics
{
	public class PhysicsToyData(string physicsName = ""): PhysicsData(physicsName) {

		private string mModelPath = "";
		public string ModelPath {
			get => mModelPath;
			set {
				mModelPath = value;
				InvokeChange(nameof(ModelPath));

				LoadLooksAndVariants();
			}
		}

		private string mCollPath = "";
		public string CollPath {
			get => mCollPath;
			set {
				mCollPath = value;
				InvokeChange(nameof(CollPath));
			}
		}

		private string mLook = "default";
		public string Look {
			get => mLook;
			set {
				mLook = value;
				InvokeChange(nameof(Look));
			}
		}

		private string mVariant = "default";
		public string Variant {
			get => mVariant;
			set {
				mVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		private ObservableCollection<string>? mLookList = null;
		private ObservableCollection<string>? mVariantList = null;
		public ObservableCollection<string> LookList {
			get {
				if (mLookList == null)
					LoadLooksAndVariants();
				return mLookList!;
			}
		}
		public ObservableCollection<string> VariantList {
			get {
				if (mVariantList == null)
					LoadLooksAndVariants();
				return mVariantList!;
			}
		}

		public void LoadLooksAndVariants() {
			string oldLook = Look, oldVariant = Variant;
			if (mLookList == null)
				mLookList = [];
			else
				mLookList.Clear();
			if (mVariantList == null)
				mVariantList = [];
			else
				mVariantList.Clear();
			var path = mModelPath;
			if (path.Length < 5 || path.EndsWith('.'))
				return;

			path = path.Replace('/', '\\');
			path = path[..^4] + ".pit";
			path = Instances.ProjectLocation + path;

			static void setValue(ObservableCollection<string> list, string oldValue, Action<string> set) {
				if (list.Count > 0) {
					if (list.Contains(oldValue)) {
						set(oldValue);
					} else
						set(list[0]);
				}
			}
			AccDataIO.ReadLookAndVariant(path, mLookList, mVariantList);
			setValue(mLookList, oldLook, (set) => Look = set);
			setValue(mVariantList, oldVariant, (set) => Variant = set);
		}

		private string mToyType = "";
		public string ToyType {
			get => mToyType;
			set {
				mToyType = value;
				InvokeChange(nameof(ToyType));

				InvokeChange(nameof(AngularVisibility));
				InvokeChange(nameof(RopeDataVisibility));
			}
		}
		public Visibility AngularVisibility =>
			ToyType == "TT_joint" || ToyType == "TT_joint_free" ? Visibility.Visible : Visibility.Collapsed;
		public Visibility RopeDataVisibility =>
			ToyType == "TT_rope" || ToyType == "TT_double_rope" ? Visibility.Visible : Visibility.Hidden;

		private float? mMass = null;
		public float? Mass {
			get => mMass;
			set {
				mMass = value;
				InvokeChange(nameof(Mass));
			}
		}

		private float?[] mCogOffset = [null, null, null];
		public float?[] CogOffset {
			get => mCogOffset;
			set {
				mCogOffset = value;
				InvokeChange(nameof(CogOffset));
			}
		}
		public float? CogOffset0 {
			get => CogOffset[0];
			set {
				CogOffset[0] = value;
				InvokeChange(nameof(CogOffset0));
			}
		}
		public float? CogOffset1 {
			get => CogOffset[1];
			set {
				CogOffset[1] = value;
				InvokeChange(nameof(CogOffset1));
			}
		}
		public float? CogOffset2 {
			get => CogOffset[2];
			set {
				CogOffset[2] = value;
				InvokeChange(nameof(CogOffset2));
			}
		}

		private float? mLinearStiffness = null;
		public float? LinearStiffness {
			get => mLinearStiffness;
			set {
				mLinearStiffness = value;
				InvokeChange(nameof(LinearStiffness));
			}
		}

		private float? mLinearDamping = null;
		public float? LinearDamping {
			get => mLinearDamping;
			set {
				mLinearDamping = value;
				InvokeChange(nameof(LinearDamping));
			}
		}

		private float?[] mLocatorHookOffset = [null, null, null];
		public float?[] LocatorHookOffset {
			get => mLocatorHookOffset;
			set {
				mLocatorHookOffset = value;
				InvokeChange(nameof(LocatorHookOffset));
			}
		}
		public float? LocatorHookOffset0 {
			get => LocatorHookOffset[0];
			set {
				LocatorHookOffset[0] = value;
				InvokeChange(nameof(LocatorHookOffset0));
			}
		}
		public float? LocatorHookOffset1 {
			get => LocatorHookOffset[1];
			set {
				LocatorHookOffset[1] = value;
				InvokeChange(nameof(LocatorHookOffset1));
			}
		}
		public float? LocatorHookOffset2 {
			get => LocatorHookOffset[2];
			set {
				LocatorHookOffset[2] = value;
				InvokeChange(nameof(LocatorHookOffset2));
			}
		}

		private float?[] mRestPositionOffset = [null, null, null];
		public float?[] RestPositionOffset {
			get => mRestPositionOffset;
			set {
				mRestPositionOffset = value;
				InvokeChange(nameof(RestPositionOffset));
			}
		}
		public float? RestPositionOffset0 {
			get => RestPositionOffset[0];
			set {
				RestPositionOffset[0] = value;
				InvokeChange(nameof(RestPositionOffset0));
			}
		}
		public float? RestPositionOffset1 {
			get => RestPositionOffset[1];
			set {
				RestPositionOffset[1] = value;
				InvokeChange(nameof(RestPositionOffset1));
			}
		}
		public float? RestPositionOffset2 {
			get => RestPositionOffset[2];
			set {
				RestPositionOffset[2] = value;
				InvokeChange(nameof(RestPositionOffset2));
			}
		}

		private float?[] mRestRotationOffset = [null, null, null];
		public float?[] RestRotationOffset {
			get => mRestRotationOffset;
			set {
				mRestRotationOffset = value;
				InvokeChange(nameof(RestRotationOffset));
			}
		}
		public float? RestRotationOffset0 {
			get => RestRotationOffset[0];
			set {
				RestRotationOffset[0] = value;
				InvokeChange(nameof(RestRotationOffset0));
			}
		}
		public float? RestRotationOffset1 {
			get => RestRotationOffset[1];
			set {
				RestRotationOffset[1] = value;
				InvokeChange(nameof(RestRotationOffset1));
			}
		}
		public float? RestRotationOffset2 {
			get => RestRotationOffset[2];
			set {
				RestRotationOffset[2] = value;
				InvokeChange(nameof(RestRotationOffset2));
			}
		}

		private float?[] mInstanceOffset = [null, null, null];
		public float?[] InstanceOffset {
			get => mInstanceOffset;
			set {
				mInstanceOffset = value;
				InvokeChange(nameof(InstanceOffset));
			}
		}
		public float? InstanceOffset0 {
			get => InstanceOffset[0];
			set {
				InstanceOffset[0] = value;
				InvokeChange(nameof(InstanceOffset0));
			}
		}
		public float? InstanceOffset1 {
			get => InstanceOffset[1];
			set {
				InstanceOffset[1] = value;
				InvokeChange(nameof(InstanceOffset1));
			}
		}
		public float? InstanceOffset2 {
			get => InstanceOffset[2];
			set {
				InstanceOffset[2] = value;
				InvokeChange(nameof(InstanceOffset2));
			}
		}

		public ObservableCollection<float[]> InstanceOffsetList = [];

		private ContextMenu? mMenuInstanceOffset = null;
		public ContextMenu MenuInstanceOffset {
			get {
				if (mMenuInstanceOffset == null) {
					mMenuInstanceOffset = new();
					foreach (var offset in InstanceOffsetList) {
						MenuInstanceOffset.Items.Add(NewMenuItem(offset));
					}
				}
				return mMenuInstanceOffset;
			}
		}

		float[]? OffsetAction(Action<int> hasSame) {
			if (InstanceOffset0 == null || InstanceOffset1 == null || InstanceOffset2 == null)
				return null;
			float[] offset = [
				InstanceOffset0 ?? 0,
				InstanceOffset1 ?? 0,
				InstanceOffset2 ?? 0];
			for (int i = 0; i < InstanceOffsetList.Count; i++) {
				float[]? eOffset = InstanceOffsetList[i];
				if (offset[0] == eOffset[0] && offset[1] == eOffset[1] && offset[2] == eOffset[2]) {
					hasSame(i);
					return null;
				}
			}
			return offset;
		}

		public void AddMenuInstanceOffset() {
			var offset = OffsetAction((i) => {
				MessageBox.Show("已有相同的值，offset不会新增入数组中");
			});
			if (offset != null) {
				InstanceOffsetList?.Add(offset);
				MenuInstanceOffset.Items.Add(NewMenuItem(offset));
				MessageBox.Show("offset已新增入数组中");
			}
		}

		public void RemoveMenuInstanceOffset() {
			OffsetAction((i) => {
				InstanceOffsetList?.RemoveAt(i);
				MenuInstanceOffset.Items.RemoveAt(i);
				MessageBox.Show("已将offset值从数组中删除");
			});
		}

		private MenuItem NewMenuItem(float[] offset) {
			MenuItem item = new() {
				Header = $"({offset[0]},{offset[1]},{offset[2]})",
				Tag = offset
			};
			item.Click += OnInstanceOffsetClicked;
			return item;
		}

		private void OnInstanceOffsetClicked(object sender, RoutedEventArgs e) {
			var item = (MenuItem)sender;
			var offset = (float[])item.Tag;
			InstanceOffset0 = offset[0];
			InstanceOffset1 = offset[1];
			InstanceOffset2 = offset[2];
		}


		private string mRopeMaterial = "";
		public string RopeMaterial {
			get => mRopeMaterial;
			set {
				mRopeMaterial = value;
				InvokeChange(nameof(RopeMaterial));
			}
		}

		private float?[] mAngularStiffness = [null, null];
		public float?[] AngularStiffness {
			get => mAngularStiffness;
			set {
				mAngularStiffness = value;
				InvokeChange(nameof(AngularStiffness));
			}
		}
		public float? AngularStiffness0 {
			get => AngularStiffness[0];
			set {
				AngularStiffness[0] = value;
				InvokeChange(nameof(AngularStiffness0));
			}
		}
		public float? AngularStiffness1 {
			get => AngularStiffness[1];
			set {
				AngularStiffness[1] = value;
				InvokeChange(nameof(AngularStiffness1));
			}
		}

		private float?[] mAngularDamping = [null, null];
		public float?[] AngularDamping {
			get => mAngularDamping;
			set {
				mAngularDamping = value;
				InvokeChange(nameof(AngularDamping));
			}
		}
		public float? AngularDamping0 {
			get => AngularDamping[0];
			set {
				AngularDamping[0] = value;
				InvokeChange(nameof(AngularDamping0));
			}
		}
		public float? AngularDamping1 {
			get => AngularDamping[1];
			set {
				AngularDamping[1] = value;
				InvokeChange(nameof(AngularDamping1));
			}
		}

		private float?[] mAngularAmplitude = [null, null, null];
		public float?[] AngularAmplitude {
			get => mAngularAmplitude;
			set {
				mAngularAmplitude = value;
				InvokeChange(nameof(AngularAmplitude));
			}
		}
		public float? AngularAmplitude0 {
			get => AngularAmplitude[0];
			set {
				AngularAmplitude[0] = value;
				InvokeChange(nameof(AngularAmplitude0));
			}
		}
		public float? AngularAmplitude1 {
			get => AngularAmplitude[1];
			set {
				AngularAmplitude[1] = value;
				InvokeChange(nameof(AngularAmplitude1));
			}
		}
		public float? AngularAmplitude2 {
			get => AngularAmplitude[2];
			set {
				AngularAmplitude[2] = value;
				InvokeChange(nameof(AngularAmplitude2));
			}
		}

		private float? mRopeWidth = null;
		public float? RopeWidth {
			get => mRopeWidth;
			set {
				mRopeWidth = value;
				InvokeChange(nameof(RopeWidth));
			}
		}
		private float? mRopeLenght = null;
		public float? RopeLength {
			get => mRopeLenght;
			set {
				mRopeLenght = value;
				InvokeChange(nameof(RopeLength));
			}
		}
		private float? mRopeHookOffset = null;
		public float? RopeHookOffset {
			get => mRopeHookOffset;
			set {
				mRopeHookOffset = value;
				InvokeChange(nameof(RopeHookOffset));
			}
		}
		private float? mRopeToyOffset = null;
		public float? RopeToyOffset {
			get => mRopeToyOffset;
			set {
				mRopeToyOffset = value;
				InvokeChange(nameof(RopeToyOffset));
			}
		}
		private uint? mRopeResolution = null;
		public uint? RopeResolution {
			get => mRopeResolution;
			set {
				mRopeResolution = value;
				InvokeChange(nameof(RopeResolution));
			}
		}
		private uint? mPositionIterations = null;
		public uint? PositionIterations {
			get => mPositionIterations;
			set {
				mPositionIterations = value;
				InvokeChange(nameof(PositionIterations));
			}
		}
		private float? mRopeLinearDensity = null;
		public float? RopeLinearDensity {
			get => mRopeLinearDensity;
			set {
				mRopeLinearDensity = value;
				InvokeChange(nameof(RopeLinearDensity));
			}
		}

		private float? mNodeDamping = null;
		public float? NodeDamping {
			get => mNodeDamping;
			set {
				mNodeDamping = value;
				InvokeChange(nameof(mNodeDamping));
			}
		}
	}
}
