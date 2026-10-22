using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace SCS_Mod_Helper.Modding.Accessories.Physics; 
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
		AccSCSIO.ReadLookAndVariant(path, mLookList, mVariantList);
		setValue(mLookList, oldLook, (set) => Look = set);
		setValue(mVariantList, oldVariant, (set) => Variant = set);
	}

	private string mToyType = "TT_rope";
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

	private float? mMass = 0.1f;
	public float? Mass {
		get => mMass;
		set {
			mMass = value;
			InvokeChange(nameof(Mass));
		}
	}

	private float?[] mCogOffset = [0f, 0f, 0f];
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

	private float? mLinearStiffness = 1f;
	public float? LinearStiffness {
		get => mLinearStiffness;
		set {
			mLinearStiffness = value;
			InvokeChange(nameof(LinearStiffness));
		}
	}

	private float? mLinearDamping = 0.5f;
	public float? LinearDamping {
		get => mLinearDamping;
		set {
			mLinearDamping = value;
			InvokeChange(nameof(LinearDamping));
		}
	}

	private float?[] mLocatorHookOffset = [0f, 0f, 0f];
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

	private float?[] mRestPositionOffset = [0f, 0f, 0f];
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

	private float?[] mRestRotationOffset = [0f, 0f, 0f];
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

	private ObservableCollection<InstanceOffset> mInstanceOffsetList = [];
	public ObservableCollection<InstanceOffset> InstanceOffsetList {
		get => mInstanceOffsetList;
		set {
			mInstanceOffsetList = value;
			InvokeChange();
		}
	}

	private InstanceOffset? mCurrentInstanceOffset = null;
	public InstanceOffset? CurrentInstanceOffset {
		get => mCurrentInstanceOffset;
		set {
			mCurrentInstanceOffset = value;
			InvokeChange();
		}
	}

	public ICommand AddInstanceOffsetCommand => new RelayCommand<object>(AddInstanceOffset);
	private void AddInstanceOffset(object? parameter) {
		InstanceOffset io = new();
		InstanceOffsetList.Add(io);
		CurrentInstanceOffset = io;
		Debug.WriteLine("add");
	}
	public ICommand RemoveInstanceOffsetCommand => new RelayCommand<InstanceOffset?>(RemoveInstanceOffset);
	private void RemoveInstanceOffset(InstanceOffset? parameter) {
		if (parameter == null)
			return;
		InstanceOffsetList.Remove(parameter);
	}


	private string mRopeMaterial = "";
	public string RopeMaterial {
		get => mRopeMaterial;
		set {
			mRopeMaterial = value;
			InvokeChange(nameof(RopeMaterial));
		}
	}

	private float?[] mAngularStiffness = [0.2f, 0.2f];
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

	private float?[] mAngularDamping = [0.2f, 0.2f];
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

	private float?[] mAngularAmplitude = [0f, 0f, 0f];
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

	private float? mRopeWidth = 0.5f;
	public float? RopeWidth {
		get => mRopeWidth;
		set {
			mRopeWidth = value;
			InvokeChange(nameof(RopeWidth));
		}
	}
	private float? mRopeLenght = 0f;
	public float? RopeLength {
		get => mRopeLenght;
		set {
			mRopeLenght = value;
			InvokeChange(nameof(RopeLength));
		}
	}
	private float? mRopeHookOffset = 0f;
	public float? RopeHookOffset {
		get => mRopeHookOffset;
		set {
			mRopeHookOffset = value;
			InvokeChange(nameof(RopeHookOffset));
		}
	}
	private float? mRopeToyOffset = 0f;
	public float? RopeToyOffset {
		get => mRopeToyOffset;
		set {
			mRopeToyOffset = value;
			InvokeChange(nameof(RopeToyOffset));
		}
	}
	private uint? mRopeResolution = 8;
	public uint? RopeResolution {
		get => mRopeResolution;
		set {
			mRopeResolution = value;
			InvokeChange(nameof(RopeResolution));
		}
	}
	private uint? mPositionIterations = 10;
	public uint? PositionIterations {
		get => mPositionIterations;
		set {
			mPositionIterations = value;
			InvokeChange(nameof(PositionIterations));
		}
	}
	private float? mRopeLinearDensity = 1f;
	public float? RopeLinearDensity {
		get => mRopeLinearDensity;
		set {
			mRopeLinearDensity = value;
			InvokeChange(nameof(RopeLinearDensity));
		}
	}

	private float? mNodeDamping = 0f;
	public float? NodeDamping {
		get => mNodeDamping;
		set {
			mNodeDamping = value;
			InvokeChange(nameof(mNodeDamping));
		}
	}
}

public class InstanceOffset : BaseBinding {

	public InstanceOffset() {
	}

	public InstanceOffset(float[] offsets) {
		Offset = offsets;
	}
	
	public float[] Offset {
		get => [offset0 ?? 0f, offset1 ?? 0f, offset2 ?? 0f];
		set {
			Offset0 = value[0];
			Offset1 = value[1];
			Offset2 = value[2];
		}
	}

	private float? offset0 = 0f;
	public float? Offset0 {
		get => offset0;
		set {
			offset0 = value;
			InvokeChange();
		}
	}
	private float? offset1 = 0f;
	public float? Offset1 {
		get => offset1;
		set {
			offset1 = value;
			InvokeChange();

		}
	}
	private float? offset2 = 0f;
	public float? Offset2 {
		get => offset2;
		set {
			offset2 = value;
			InvokeChange();
		}
	}

	public override string ToString() => $"InstanceOffset=({string.Join(',', Offset)})";
}