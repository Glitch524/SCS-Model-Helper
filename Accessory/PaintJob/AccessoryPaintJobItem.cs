namespace SCS_Mod_Helper.Accessory.PaintJob
{
    public class AccessoryPaintJobItem() : AccessoryData("", "", null, null, "", "unknown", "", "", "")
    {
		private new readonly string? CollPath, Look, Variant;//屏蔽accdata原本没有的变量（主要是addon和hookup都有就放在里面了）

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
		private string? mPaintJobMask;
		public string? PaintJobMask {
			get => mPaintJobMask;
			set {
				mPaintJobMask = value; 
				InvokeChange();
			}
		}
		private string? mBaseTextureOverride;
		public string? BaseTextureOverride {
			get => mBaseTextureOverride;
			set {
				mBaseTextureOverride = value;
				InvokeChange();
			}
		}

		private int?[] mBaseColor = [null, null, null];
		public int?[] BaseColor {//6位hex 基本颜色
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
			}
		}

		private bool mAlternateUVSet = false;
		public bool AlternateUVSet {//交替UV集？
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



		private int?[] mMaskRColor = [null, null, null];
		public int?[] MaskRColor {//6位hex 遮罩颜色（红）
			get => mMaskRColor;
			set {
				mMaskRColor = value;
				InvokeChange();
			}
		}
		private bool mMaskRLocked = true;
		public bool MaskRLocked {//固定遮罩颜色（红）
			get => mMaskRLocked;
			set {
				mMaskRLocked = value;
				InvokeChange();
			}
		}
		private int?[] mMaskGColor = [null, null, null];
		public int?[] MaskGColor {//6位hex 遮罩颜色（绿）
			get => mMaskGColor;
			set {
				mMaskGColor = value;
				InvokeChange();
			}
		}
		private bool mMaskGLocked = true;
		public bool MaskGLocked {//固定遮罩颜色（绿）
			get => mMaskGLocked;
			set {
				mMaskGLocked = value;
				InvokeChange();
			}
		}
		private int?[] mMaskBColor = [null, null, null];
		public int?[] MaskBColor {//6位hex 遮罩颜色（蓝）
			get => mMaskBColor;
			set {
				mMaskBColor = value;
				InvokeChange();
			}
		}
		private bool mMaskBLocked = true;
		public bool MaskBLocked {//固定遮罩颜色（蓝）
			get => mMaskBLocked;
			set {
				mMaskBLocked = value;
				InvokeChange();
			}
		}



		private bool mFlipflake = false;
		public bool Flipflake { //金属/珠光
			get => mFlipflake;
			set {
				mFlipflake = value;
				InvokeChange();
			}
		}

		private int?[] mFlipColor = [null, null, null];
		public int?[] FlipColor {//金属漆颜色
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
				InvokeChange();
			}
		}
		private float? mFlipStrength;
		public float? FlipStrength {//金属漆效果强度
			get => mFlipStrength;
			set {
				mFlipStrength = value;
				InvokeChange();
			}
		}


		private int?[] mFlakeColor = [null, null, null];
		public int?[] FlakeColor {//珠光漆颜色
			get => mFlakeColor;
			set {
				mFlakeColor = value;
				InvokeChange();
			}
		}
		private bool mFlakeColorLocked = true;
		public bool FlakeColorLocked {//固定珠光漆颜色
			get => mFlakeColorLocked;
			set {
				mFlakeColorLocked = value;
				InvokeChange();
			}
		}
		private float? mFlakeUVScale;
		public float? FlakeUVScale {//UV缩放
			get => mFlakeUVScale;
			set {
				mFlakeUVScale = value;
				InvokeChange();
			}
		}
		private float? mFlakeDensity;
		public float? FlakeDensity {//珠光密度
			get => mFlakeDensity;
			set {
				mFlakeDensity = value;
				InvokeChange();
			}
		}
		private float? mFlakeShininess;
		public float? FlakeShininess {//光泽度
			get => mFlakeShininess;
			set {
				mFlakeShininess = value;
				InvokeChange();
			}
		}
		private float? mFlakeClearcoatRolloff = null;
		public float? FlakeClearcoatRolloff {//透明图层滚降		调整透明涂层镜面反射高光的锐度。值越高，边缘越清晰。
			get => mFlakeClearcoatRolloff;
			set {
				mFlakeClearcoatRolloff = value;
				InvokeChange();
			}
		}
		//private readonly string FlakeNoiseDefault = "/material/custom/flake_noise.tobj";
		private string mFlakeNoise = "/material/custom/flake_noise.tobj";
		public string FlakeNoise {//珠光噪声图
			get => mFlakeNoise;
			set {
				mFlakeNoise = value;
				InvokeChange();
			}
		}


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
	//alternate_uvset bool    false	|When true, the resulting material will have the altuv flavor. This triggers usage of the alternate UV layout, if configured.
	//stock   bool    false	|Defaults to false. This was previously used to mark paintjobs available when purchasing a truck when true, and only available from the upgrade shop when false. Currently must be set to false to avoid undesirable behavior.
	//paint_job_mask  string |定义纹理资源 （.tobj） 的路径，用于颜色遮罩 （colormask） 或混合 （喷枪）。如果为空，则忽略蒙版，最终结果纯粹由颜色属性组成。Defines the path to the texture resource (.tobj) to be used for color masking(colormask) or for blending(airbrush). If empty, mask is ignored and final result is composed purely from color attributes.
	//base_texture_override string |定义纹理资源 （.tobj） 的路径，以覆盖 truckpaint 材质的基础纹理。如果为空，则将按照 truckpaint 材质中的定义使用基础纹理。如果需要不同的镜面反射，这对于覆盖基础纹理的 alpha 通道非常有用。

	//airbrush bool	false		When true, airbrush behavior and attributes are enabled.Cannot be used with colormask.

	//colormask 
	//	mask_r_color float3(1, 0, 0)   |定义应用于颜色蒙版的每个通道的默认颜色。
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