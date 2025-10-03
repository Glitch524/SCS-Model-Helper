namespace SCS_Mod_Helper.Accessory.PaintJob
{
	//255颜色到float的公式是((x/255+0.055)/1.055)^2.4
	//反过来就是反过来
    public class AccessoryPaintJobItem() : AccessoryItem("", "", null, null, "", "unknown", "", "", "")
    {
  //      private new readonly string? CollPath;
		//private new readonly string? Look;
		//private new readonly string? Variant;

		//private string mPaintJobMask = "";
		//private string mBaseTextureOverride = "";

		//private bool mMaskRLocked = true;
		//private bool mMaskGLocked = true;
		//private bool mMaskBLocked = true;
		//private bool mBaseColorLocked = true;
		//private bool mFlipColorLocked = true;
		//private bool mFlakeColorLocked = true;
		//private bool mAlternateUVset = false;
		//private bool mAlternateFlipflakeUVset = false;
		//private bool mStock = false;

		//private float?[] mMaskRColor = [null, null, null];
		//private float?[] mMaskGColor = [null, null, null];
		//private float?[] mMaskBColor = [null, null, null];
		//private float?[] mBaseColor = [null, null, null];

		//private float?[] mFlipColor = [null, null, null];
		//private float? mFlipStrength = null;

		//private float?[] mFlakeColor = [null, null, null];
		//private float? mFlakeUVScale = null;
		//private float? mFlakeVRatio = null;
		//private float? mFlakeDensity = null;
		//private float? mFlakeShininess = null;
		//private float? mFlakeClearcoatRolloff = null;
		//private string mFlakeNoise = "";

		//private bool mFlipflake = false;

		//private bool mAirbrush = false;

		//private List<float[]> mColorVariant = [];

		//private List<string> mSuitableFor = [];//不同车架

	}

//	mask_r_color float3(1, 0, 0)   colormask 定义应用于颜色蒙版的每个通道的默认颜色。
//mask_g_color(0, 1, 0)
//mask_b_color(0, 0, 1)
//base_color float3(1, 1, 1)   all 定义涂装作业的默认颜色。
//mask_r_locked bool	true	colormask 如果为 false，则播放器可以通过颜色选择器更改每个通道的颜色。
//mask_g_locked	true
//mask_b_locked	true
//base_color_locked bool	true	all When false, the player may change the base color via the color picker.
//flip_color_locked bool	true	flipflake When false, the player may change the flip color via the color picker.
//flake_color_locked bool	true	flipflake When false, the player may change the flake color via the color picker.
//flip_color float3(1, 0, 0)   flipflake 定义翻转效果的颜色。
//flip_strength float	0.27	flipflake Defines the relative strength of the flip effect.
//flake_color float3(0, 1, 0)   flipflake Defines the color of the flake effect.
//flake_uvscale   float   32.0	flipflake Defines how many times the flake_noise texture repeats within one UV tile. (Another way to think of this is that the UV coordinates are divided by this factor when addressing flake_noise.)
//flake_density float	1.0	flipflake Defines how 'tight' the flake effect is to the specular highlight.Higher values result in a smaller area of the flake effect, while smaller values result in a broad area having it.
//flake_shininess float   50.0	flipflake
//flake_clearcoat_rolloff float   2.2	flipflake Adjusts the sharpness of the clearcoat specular highlight.Higher values yield sharper edges.
//flake_noise string  "/material/custom/flake_noise.tobj"	flipflake 翻片纹理的路径。RGB 组件将flake_color相乘，A 组件遮罩薄片效果。
//flipflake   bool    false		When true, flipflake (metallic/pearlescent) behavior and attributes are enabled.Cannot be used with colormask.
//airbrush bool	false		When true, airbrush behavior and attributes are enabled.Cannot be used with colormask.
//alternate_uvset bool    false	all When true, the resulting material will have the altuv flavor. This triggers usage of the alternate UV layout, if configured.
//stock   bool    false	all Defaults to false. This was previously used to mark paintjobs available when purchasing a truck when true, and only available from the upgrade shop when false. Currently must be set to false to avoid undesirable behavior.
//paint_job_mask  string all Defines the path to the texture resource (.tobj) to be used for color masking(colormask) or for blending(airbrush). If empty, mask is ignored and final result is composed purely from color attributes.
//base_texture_override string all 定义纹理资源 （.tobj） 的路径，以覆盖 truckpaint 材质的基础纹理。如果为空，则将按照 truckpaint 材质中的定义使用基础纹理。如果需要不同的镜面反射，这对于覆盖基础纹理的 alpha 通道非常有用。






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