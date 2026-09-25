using SCS_Mod_Helper.Modding.Accessories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Extensions;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public class AccessoryPaintJobData(): AccessoryData("", "", null, null, "", "unknown") {
		//name 从accessoryData继承的变量
		//icon
		//price
		//unlock
		//part_type

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
		protected BitmapSource? mPaintJobTexImage = null;
		public BitmapSource? PaintJobTexImage {
			get => mPaintJobTexImage;
			set {
				mPaintJobTexImage?.Freeze();
				mPaintJobTexImage = value;
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
		public Brush BaseColorVisual => BaseColor.ToBrush();

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

		private Color mMaskRColor = Colors.Red;
		public Color MaskRColor {//6位hex 遮罩颜色（红）
			get => mMaskRColor;
			set {
				mMaskRColor = value;
				InvokeMaskR();
			}
		}
		public Brush MaskRColorVisual => MaskRColor.ToBrush();

		public string MaskRColorHex {
			get => ColorToHex(mMaskRColor);
			set {
				mMaskRColor = HexToColor(value);
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
			InvokeChange(nameof(MaskRColorVisual));
			InvokeChange(nameof(MaskRColorHex));
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
		public Brush MaskGColorVisual => MaskGColor.ToBrush();

		public string MaskGColorHex {
			get => ColorToHex(MaskGColor);
			set {
				mMaskGColor = HexToColor(value);
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
			InvokeChange(nameof(MaskGColorVisual));
			InvokeChange(nameof(MaskGColorHex));
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
		public Brush MaskBColorVisual => MaskBColor.ToBrush();

		public string MaskBColorHex {
			get => ColorToHex(mMaskBColor);
			set {
				mMaskBColor = HexToColor(value);
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
			InvokeChange(nameof(MaskBColorVisual));
			InvokeChange(nameof(MaskBColorHex));
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

		public static string ColorToHex(Color color) {
			string r = string.Format("{0:x2}", color.R).ToUpper();
			string g = string.Format("{0:x2}", color.G).ToUpper();
			string b = string.Format("{0:x2}", color.B).ToUpper();
			return r + g + b;
		}

		public static Color HexToColor(string hex) {
			byte r = (byte)Convert.ToInt32($"{hex[0]}{hex[1]}");
			byte g = (byte)Convert.ToInt32($"{hex[2]}{hex[3]}");
			byte b = (byte)Convert.ToInt32($"{hex[4]}{hex[5]}");
			return Color.FromRgb(r, g, b);
		}



		private bool mFlipflake = false;
		public bool Flipflake {
			get => mFlipflake;
			set {
				mFlipflake = value;
				InvokeChange();
			}
		}

		private Color mFlipColor = Colors.Red;
		public Color FlipColor {//变色龙漆颜色
			get => mFlipColor;
			set {
				mFlipColor = value;
				InvokeChange();
				InvokeChange(nameof(FlipColorVisual));
			}
		}

		public Brush FlipColorVisual => mFlipColor.ToBrush();

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


		private Color mFlakeColor = Color.FromRgb(0, 255, 0);
		public Color FlakeColor {//鳞片漆颜色
			get => mFlakeColor;
			set {
				mFlakeColor = value;
				InvokeChange();
				InvokeChange(nameof(FlakeColorVisual));
			}
		}
		public Brush FlakeColorVisual => FlakeColor.ToBrush();


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

		private float mFlakeDensity = 1f;
		public float FlakeDensity {//鳞片密度
			get => mFlakeDensity;
			set {
				mFlakeDensity = value;
				InvokeChange();
			}
		}

		private float mFlakeClearcoatRolloff = 2.2f;
		public float FlakeClearcoatRolloff {//鳞片漆清漆层滚落效应		调整透明涂层镜面反射高光的锐度。值越高，边缘越清晰。
			get => mFlakeClearcoatRolloff;
			set {
				mFlakeClearcoatRolloff = value;
				InvokeChange();
			}
		}

		private float mFlakeUVScale = 32f;
		public float FlakeUVScale {//UV比率
			get => mFlakeUVScale;
			set {
				mFlakeUVScale = value;
				InvokeChange();
			}
		}

		//private readonly string FlakeNoiseDefault = "/material/custom/flake_noise.tobj";
		public static string DefaultFlakeNoise = "/material/custom/flake_noise.tobj";
		private string mFlakeNoise = "/material/custom/flake_noise.tobj";
		public string FlakeNoise {//珠光噪声图
			get => mFlakeNoise;
			set {
				mFlakeNoise = value;
				InvokeChange();
			}
		}

		public Visibility FlakeNoiseVisibility => FlakeNoise.Length > 0 ? Visibility.Visible : Visibility.Collapsed;


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

		public ObservableCollection<string> AccList => SelectedOverride?.AccList ?? [];

		//private List<string> mSuitableFor = [];//不同车架


		//simple_paint_job_data : .ovr0+

		//paint_job_mask:""
		//flake_uvscale
		//flake_vratio
		//acc_list[]: ""
	}

	//all
	//base_color float3(1, 1, 1)   |定义涂装作业的默认颜色。
	//base_color_locked bool	true	锁定基础颜色，设置为false时，玩家可以使用选色器修改基础颜色 all When false, the player may change the base color via the color picker.
	//alternate_uvset bool    false	|When true, the resulting material will have the alt uv flavor. This triggers usage of the alternate UV layout, if configured.
	//stock   bool    false	|Defaults to false. This was previously used to mark paintjobs available when purchasing a truck when true, and only available from the upgrade shop when false. Currently must be set to false to avoid undesirable behavior.
	//paint_job_mask  string |定义纹理资源 （.tobj） 的路径，用于颜色遮罩 （colormask） 或混合 （喷枪）。如果为空，则忽略蒙版，最终结果纯粹由颜色属性组成。Defines the path to the texture resource (.tobj) to be used for color masking(colormask) or for blending(airbrush). If empty, mask is ignored and final result is composed purely from color attributes.
	//base_texture_override string |定义纹理资源 （.tobj） 的路径，以覆盖 truckpaint 材质的基础纹理。如果为空，则将按照 truckpaint 材质中的定义使用基础纹理。如果需要不同的镜面反射，这对于覆盖基础纹理的 alpha 通道非常有用。

	//airbrush bool	false		When true, airbrush behavior and attributes are enabled.Cannot be used with colormask.

	//colormask 
	//mask_r_color float3(1, 0, 0)   |定义应用于颜色蒙版的每个通道的默认颜色。
	//mask_g_color(0, 1, 0)
	//mask_b_color(0, 0, 1)
	//mask_r_locked bool	true	|如果为 false，则播放器可以通过颜色选择器更改每个通道的颜色。
	//mask_g_locked	true
	//mask_b_locked	true

	//flipflake   bool    false		When true, flipflake (metallic/pearlescent) behavior and attributes are enabled.Cannot be used with colormask.

	//flipflake
	//flip_color float3(1, 0, 0)   |定义翻转效果的颜色。
	//flip_color_locked bool	true	 |When false, the player may change the flip color via the color picker.
	//flip_strength float	0.27	|Defines the relative strength of the flip effect.

	//flake_color float3(0, 1, 0)   |Defines the color of the flake effect.
	//flake_color_locked bool	true	|When false, the player may change the flake color via the color picker.
	//flake_uvscale   float   32.0	|Defines how many times the flake_noise texture repeats within one UV tile. (Another way to think of this is that the UV coordinates are divided by this factor when addressing flake_noise.)
	//flake_density float	1.0	|Defines how 'tight' the flake effect is to the specular highlight.Higher values result in a smaller area of the flake effect, while smaller values result in a broad area having it.
	//flake_shininess float   50.0	|
	//flake_clearcoat_rolloff float   2.2	|Adjusts the sharpness of the clearcoat specular highlight.Higher values yield sharper edges.
	//flake_noise string  "/material/custom/flake_noise.tobj"	|翻片纹理的路径。RGB 组件将flake_color相乘，A 组件遮罩薄片效果。







	//sui
	//	name
	//	price
	//	airbrush
	//	base color
	//	icon
	//	part type

	//sii
	//	name
	//	include
	//	paintjobmask
	//	suitablefor


	//accessory

}

//accessory_paint_job_data : 喷漆名.车型.paint_job
//纯色通用贴图 4x4

//daf.2021 1:1	2048x2048
//	xg.daf.2021.cabin			驾驶室-XG
//	xg_plus.daf.2021.cabin		驾驶室-XG+
//	xf.daf.2021.cabin			驾驶室-XF

//	主后视镜 ？
//	mirror.cam_semi				主后视镜-电子镜-喷漆
//	mirror.plast				主后视镜-塑料
//	mirror.semi					主后视镜-喷漆和塑料？

//	遮阳板  8:1	1024x128
//	sunshield.paint_xg			遮阳板-喷漆 XG
//	sunshield.paint_xgp			遮阳板-喷漆 XG+
//	sunshield.paint_xf			遮阳板-喷漆 XF

//	侧裙 2:1 1024x512
//	sideskirt.pnt_4x2			侧裙-喷漆 XG/XG+
//	sideskirt.pnt_4x2_xf		侧裙-喷漆 XF
//	sideskirt.slv_4x2			侧裙-豪华喷漆 XG/XG+
//	sideskirt.slv2_4x2			侧裙-豪华喷漆II XG/XG+
//	sideskirt.slv_4x2_xf		侧裙-豪华喷漆 XG/XG+
//	sideskirt.slv2_4x2_xf		侧裙-豪华喷漆II XG/XG+

//daf.xd
//	day.daf.xd.cabin
//	sl.daf.xd.cabin
//	slh.daf.xf.cabin

//	mirror.cam_semi
//	mirror.cam_semi_h
//	mirror.plast
//	mirror.semi
//	sunshield.paint_slh
//	sideskirt.pnt_4x2s

//daf.xf
//	space_cab.daf.xf.cabin
//	s_cab_plus.daf.xf.cabin
//	super_s_cab.daf.xf.cabin

//	r_bumper.chrome
//	r_bumper.paint
//	r_bumper.steel

//	r_chs_cover.ch_4x2_et_p
//	r_chs_cover.ch_6x2_p
//	r_chs_cover.ch_6x4_p

//daf.xf_euro6
//	space.daf.xf_euro6.cabin
//	superspace.daf.xf_euro6.cabin
//	spacespoiler.daf.xf_euro6.cabin

//	r_bumper.chrome
//	r_bumper.chrome_6
//	r_bumper.paint
//	r_bumper.paint_6
//	r_bumper.steel
//	r_bumper.steel_6
//	r_chs_cover.ch_4x2_p
//	r_chs_cover.ch_6x4_p