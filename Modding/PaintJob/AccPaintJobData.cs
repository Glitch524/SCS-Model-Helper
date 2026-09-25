using SCS_Mod_Helper.Modding.Accessories;
using SCS_Mod_Helper.Trucks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Extensions;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public class AccPaintJobData(): AccessoryData("", "", null, null, "", "unknown") {
		//name 从accessoryData继承的变量
		//icon
		//price
		//unlock
		//part_type
		
		private ObservableCollection<Truck>? mTrucks = null;
		public ObservableCollection<Truck> Trucks {
			get {
				if (mTrucks == null) {
					mTrucks = [];
					TrucksIO.LoadPaintJobTrucks(mTrucks);
				}
				return mTrucks;
			}
		}

		private Truck? mCurrentTruck = null;
		public Truck? CurrentTruck {
			get => mCurrentTruck;
			set {
				mCurrentTruck = value;
				InvokeChange();
			}
		}

		public List<Cabin>? CabinList => CurrentTruck?.Cabins ?? null;

		private Cabin? mCurrentCabin = null;
		public Cabin? CurrentCabin {
			get => mCurrentCabin;
			set {
				mCurrentCabin = value;
				InvokeChange();
			}
		}
		public List<Accessory>? AccessoryList => CurrentTruck?.Accessories ?? null;

		private bool mAirbrush = false;
		public bool Airbrush {
			get => mAirbrush;
			set {
				mAirbrush = value;
				InvokeChange();
			}
		}
		private string mPaintJobTex = "";
		public string PaintJobTex {
			get => mPaintJobTex;
			set {
				mPaintJobTex = value;
				InvokeChange();
			}
		}
		public Visibility PaintJobTexVisibility => PaintJobTex.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

		private string mBaseTexOverride = "";
		public string BaseTexOverride {
			get => mBaseTexOverride;
			set {
				mBaseTexOverride = value;
				InvokeChange();
			}
		}

		public Visibility BaseTexOverrideVisibility {
			get => BaseTexOverride.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private Color mBaseColor = Colors.White;
		public Color BaseColor {//6位hex 基本颜色
			get => mBaseColor;
			set {
				mBaseColor = value;
				InvokeChange();
			}
		}

		private bool mBaseColorLocked = true;
		public bool BaseColorLocked {//固定基本颜色
			get => mBaseColorLocked;
			set {
				mBaseColorLocked = value;
				InvokeChange();
				InvokeChange(nameof(BaseColorCustomizable));
			}
		}

		public bool BaseColorCustomizable {
			get => !mBaseColorLocked;
			set {
				mBaseColorLocked = !value;
				InvokeChange();
				InvokeChange(nameof(BaseColorLocked));
			}
		}

		private bool mAlternateUVSet = false;
		public bool AlternateUVSet {//额外UV集？
			get => mAlternateUVSet;
			set {
				mAlternateUVSet = value;
				InvokeChange();
			}
		}

		private bool mStock = false;
		public bool Stock { //应该不需要显示因为默认必须false
			get => mStock;
			set {
				mStock = value;
				InvokeChange();
			}
		}

		public ObservableCollection<ColorVariant> ColorVariantList = [];

		protected BitmapSource? mPaintJobTexImage = null;
		public BitmapSource? PaintJobTexImage {
			get => mPaintJobTexImage;
			set {
				mPaintJobTexImage?.Freeze();
				mPaintJobTexImage = value;
				InvokeChange();
			}
		}
		protected BitmapSource? mPaintJobMaskR = null;
		public BitmapSource? PaintJobChannelR {
			get => mPaintJobMaskR;
			set {
				mPaintJobMaskR?.Freeze();
				mPaintJobMaskR = value;
				InvokeChange();
			}
		}
		protected BitmapSource? mPaintJobMaskG = null;
		public BitmapSource? PaintJobChannelG {
			get => mPaintJobMaskG;
			set {
				mPaintJobMaskG?.Freeze();
				mPaintJobMaskG = value;
				InvokeChange();
			}
		}
		protected BitmapSource? mPaintJobMaskB = null;
		public BitmapSource? PaintJobChannelB {
			get => mPaintJobMaskB;
			set {
				mPaintJobMaskB?.Freeze();
				mPaintJobMaskB = value;
				InvokeChange();
			}
		}





		private Color mMaskRColor = Colors.Red;
		public Color MaskRColor {//6位hex 遮罩颜色（红）
			get => mMaskRColor;
			set {
				mMaskRColor = value;
				InvokeMaskR();
			}
		}

		public byte MaskRColorR {
			get => mMaskRColor.R;
			set {
				mMaskRColor.R = value;
				InvokeMaskR();
			}
		}
		public byte MaskRColorG {
			get => mMaskRColor.G;
			set {
				mMaskRColor.G = value;
				InvokeMaskR();
			}
		}
		public byte MaskRColorB {
			get => mMaskRColor.B;
			set {
				mMaskRColor.B = value;
				InvokeMaskR();
			}
		}

		private void InvokeMaskR() {
			InvokeChange(nameof(MaskRColor));
			InvokeChange(nameof(MaskRColorR));
			InvokeChange(nameof(MaskRColorG));
			InvokeChange(nameof(MaskRColorB));
		}

		private bool mMaskRLocked = true;
		public bool MaskRLocked {//固定遮罩颜色（红）
			get => mMaskRLocked;
			set {
				mMaskRLocked = value;
				InvokeChange();
				InvokeChange(nameof(MaskRCustomizable));
			}
		}

		public bool MaskRCustomizable {
			get => !mMaskRLocked;
			set {
				mMaskRLocked = !value;
				InvokeChange();
				InvokeChange(nameof(MaskRLocked));
			}
		}



		private Color mMaskGColor = Color.FromRgb(0, 255, 0);
		public Color MaskGColor {//6位hex 遮罩颜色（绿）
			get => mMaskGColor;
			set {
				mMaskGColor = value;
				InvokeMaskG();
			}
		}

		public byte MaskGColorR {
			get => mMaskGColor.R;
			set {
				mMaskGColor.R = value;
				InvokeMaskG();
			}
		}
		public byte MaskGColorG {
			get => mMaskGColor.G;
			set {
				mMaskGColor.G = value;
				InvokeMaskG();
			}
		}
		public byte MaskGColorB {
			get => mMaskGColor.B;
			set {
				mMaskGColor.B = value;
				InvokeMaskG();
			}
		}

		private void InvokeMaskG() {
			InvokeChange(nameof(MaskGColor));
			InvokeChange(nameof(MaskGColorR));
			InvokeChange(nameof(MaskGColorG));
			InvokeChange(nameof(MaskGColorB));
		}

		private bool mMaskGLocked = true;
		public bool MaskGLocked {//固定遮罩颜色（绿）
			get => mMaskGLocked;
			set {
				mMaskGLocked = value;
				InvokeChange();
				InvokeChange(nameof(MaskGCustomizable));
			}
		}

		public bool MaskGCustomizable {
			get => !mMaskGLocked;
			set {
				mMaskGLocked = !value;
				InvokeChange();
				InvokeChange(nameof(MaskGLocked));
			}
		}



		private Color mMaskBColor = Colors.Blue;
		public Color MaskBColor {//6位hex 遮罩颜色（蓝）
			get => mMaskBColor;
			set {
				mMaskBColor = value;
				InvokeMaskB();
			}
		}

		public byte MaskBColorR {
			get => mMaskBColor.R;
			set {
				mMaskBColor.R = value;
				InvokeMaskB();
			}
		}
		public byte MaskBColorG {
			get => mMaskBColor.G;
			set {
				mMaskBColor.G = value;
				InvokeMaskB();
			}
		}
		public byte MaskBColorB {
			get => mMaskBColor.B;
			set {
				mMaskBColor.B = value;
				InvokeMaskB();
			}
		}

		private void InvokeMaskB() {
			InvokeChange(nameof(MaskBColor));
			InvokeChange(nameof(MaskBColorR));
			InvokeChange(nameof(MaskBColorG));
			InvokeChange(nameof(MaskBColorB));
		}


		private bool mMaskBLocked = true;
		public bool MaskBLocked {//固定遮罩颜色（蓝）
			get => mMaskBLocked;
			set {
				mMaskBLocked = value;
				InvokeChange();
				InvokeChange(nameof(MaskBCustomizable));
			}
		}

		public bool MaskBCustomizable {
			get => !mMaskBLocked;
			set {
				mMaskBLocked = !value;
				InvokeChange();
				InvokeChange(nameof(MaskBLocked));
			}
		}


		private bool mFlipflake = false;
		public bool Flipflake {
			get => mFlipflake;
			set {
				mFlipflake = value;
				InvokeChange();
			}
		}

		private bool mAlternateFlipFlakeUVSet = false;
		public bool AlternateFlipFlakeUVSet {
			get => mAlternateFlipFlakeUVSet;
			set {
				mAlternateFlipFlakeUVSet = value;
				InvokeChange();
			}
		}

		private Color mFlipColor = Colors.Red;
		public Color FlipColor {//变色龙漆颜色
			get => mFlipColor;
			set {
				mFlipColor = value;
				InvokeChange();
			}
		}

		private bool mFlipColorLocked = true;
		public bool FlipColorLocked {//固定金属漆颜色
			get => mFlipColorLocked;
			set {
				mFlipColorLocked = value;
				InvokeChange(nameof(FlipColorLocked));
				InvokeChange(nameof(FlipColorCustomizable));
			}
		}

		public bool FlipColorCustomizable {
			get => !mFlipColorLocked;
			set {
				mFlipColorLocked = !value;
				InvokeChange(nameof(FlipColorLocked));
				InvokeChange(nameof(FlipColorCustomizable));
			}
		}

		private float mFlipStrength = 0.27f;
		public float FlipStrength {//金属漆效果强度
			get => mFlipStrength;
			set {
				mFlipStrength = value;
				InvokeChange();
			}
		}
		public bool IsFlipStrengthDefault => FlipStrength == 0.27f;


		private Color mFlakeColor = Color.FromRgb(0, 255, 0);
		public Color FlakeColor {//鳞片漆颜色
			get => mFlakeColor;
			set {
				mFlakeColor = value;
				InvokeChange();
			}
		}

		private bool mFlakeColorLocked = true;
		public bool FlakeColorLocked {//固定鳞片漆颜色
			get => mFlakeColorLocked;
			set {
				mFlakeColorLocked = value;
				InvokeChange();
			}
		}

		public bool FlakeColorCustomizable {
			get => !mFlakeColorLocked;
			set {
				mFlakeColorLocked = !value;
				InvokeChange(nameof(FlakeColorLocked));
				InvokeChange(nameof(FlakeColorCustomizable));
			}
		}

		private float mFlakeShininess = 50f;
		public float FlakeShininess {//鳞片漆光泽度
			get => mFlakeShininess;
			set {
				mFlakeShininess = value;
				InvokeChange();
			}
		}
		public bool IsFlakeShininessDefault => FlakeShininess == 50f;

		private float mFlakeDensity = 1f;
		public float FlakeDensity {//鳞片密度
			get => mFlakeDensity;
			set {
				mFlakeDensity = value;
				InvokeChange();
			}
		}
		public bool IsFlakeDensityDefault => FlakeDensity == 1f;

		private float mFlakeClearcoatRolloff = 2.2f;
		public float FlakeClearcoatRolloff {//鳞片漆清漆层滚落效应		调整透明涂层镜面反射高光的锐度。值越高，边缘越清晰。
			get => mFlakeClearcoatRolloff;
			set {
				mFlakeClearcoatRolloff = value;
				InvokeChange();
			}
		}
		public bool IsFlakeClearcoatRolloffDefault => FlakeClearcoatRolloff == 2.2f;

		private float mFlakeUVScale = 32f;
		public float FlakeUVScale {//UV比率
			get => mFlakeUVScale;
			set {
				mFlakeUVScale = value;
				InvokeChange();
			}
		}
		public bool IsFlakeUVScaleDefault => FlakeUVScale == 32f;

		private float mFlakeVRatio = 1f;
		public float FlakeVRatio {//UV比率
			get => mFlakeVRatio;
			set {
				mFlakeVRatio = value;
				InvokeChange();
			}
		}
		public bool IsFlakeVRatioDefault => FlakeVRatio == 1f;

		public const string DefaultFlakeNoise = "/material/custom/flake_noise.tobj";
		private string mFlakeNoise = "/material/custom/flake_noise.tobj";
		public string FlakeNoise {//珠光噪声图
			get => mFlakeNoise;
			set {
				mFlakeNoise = value;
				InvokeChange();
			}
		}
		public bool IsFlakeNoiseDefault => FlakeNoise == "/material/custom/flake_noise.tobj";

		public Visibility FlakeNoiseVisibility => FlakeNoise == DefaultFlakeNoise ? Visibility.Visible : Visibility.Collapsed;


		//Override 部分


		private readonly ObservableCollection<PaintJobOverrideData> mOverrideList = [];

		public ObservableCollection<PaintJobOverrideData> OverrideList => mOverrideList;

		private PaintJobOverrideData? mSelectedOverride = null;
		public PaintJobOverrideData? SelectedOverride {
			get => mSelectedOverride;
			set {
				mSelectedOverride = value;
				InvokeChange();
			}
		}
		private string mOvrName = "";
		public string OvrName {
			get => mOvrName;
			set {
				mOvrName = value;
				InvokeChange();
			}
		}

		public string AccTex {
			get => SelectedOverride?.AccTex ?? "";
			set {
				if (SelectedOverride != null) {
					SelectedOverride.AccTex = value!;
					InvokeChange();
					InvokeChange(nameof(AccTexVisibility));
				}
			}
		}
		public Visibility AccTexVisibility => AccTex.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

		protected BitmapSource? mAccTexImage = null;
		public BitmapSource? AccTexImage {
			get => SelectedOverride?.AccTexImage;
			set {
				if (SelectedOverride != null) {
					SelectedOverride.AccTexImage = value;
				}
				InvokeChange();
			}
		}
		public float? AccFlakeUVScale {
			get => SelectedOverride?.AccFlakeUVScale;
			set {
				if (SelectedOverride != null && value != null) {
					SelectedOverride.AccFlakeUVScale = (float)value;
					InvokeChange();
				}
			}
		}

		public float? AccFlakeVRatio {
			get => SelectedOverride?.AccFlakeVRatio;
			set {
				if (SelectedOverride != null && value != null) {
					SelectedOverride.AccFlakeVRatio = (float)value;
					InvokeChange();
				}
			}
		}
	}
}