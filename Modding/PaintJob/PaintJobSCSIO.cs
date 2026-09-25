using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Modding.Accessories;
using SCS_Mod_Helper.Utils;
using System.IO;
using System.Windows.Media;

namespace SCS_Mod_Helper.Modding.PaintJob; 
public class PaintJobSCSIO(): SCSIO {
	protected const string HeaderAccPaintJob = "accessory_paint_job_data";
	private const string NameDisplayName = "name";
	private const string NamePrice = "price";
	private const string NameUnlock = "unlock";
	private const string NameIcon = "icon";
	private const string NamePartType = "part_type";
	private const string NameSuitableFor = "suitable_for";
	private const string NameConflictWith = "conflict_with";
	private const string NameDefaults = "defaults";
	private const string NameOverrides = "overrides";
	private const string NameRequire = "require";
	private const string NameSyncOverNetwork = "sync_over_network";

	private const string NameBaseColor = "base_color";
	private const string NameBaseColorLocked = "base_color_locked";
	private const string NamePaintJobMask = "paint_job_mask";
	private const string NameBaseTextureOverride = "base_texture_override";
	private const string NameAlternateUVSet = "alternate_uvset";

	private const string NameAirbrush = "airbrush";
	private const string NameFlipFlake = "flipflake";
	private const string NameAlternateFlipFlakeUVSet = "alternate_flipflake_uvset";
	private const string NameFlipColor = "flip_color";
	private const string NameFlipColorLocked = "flip_color_locked";
	private const string NameFlakeColor = "flake_color";
	private const string NameFlakeColorLocked = "flake_color_locked";
	private const string NameFlipStrength = "flip_strength";
	private const string NameFlakeShininess = "flake_shininess";
	private const string NameFlakeDensity = "flake_density";
	private const string NameFlakeClearcoatRolloff = "flake_clearcoat_rolloff";
	private const string NameFlakeUVScale = "flake_uvscale";
	private const string NameFlakeVRatio = "flake_vratio";
	private const string NameFlakeNoise = "flake_noise";

	private const string NameMaskRColor = "mask_r_color";
	private const string NameMaskRLocked = "mask_r_locked";
	private const string NameMaskGColor = "mask_g_color";
	private const string NameMaskGLocked = "mask_g_locked";
	private const string NameMaskBColor = "mask_b_color";
	private const string NameMaskBLocked = "mask_b_locked";
	private const string NameColorVariant = "color_variant";


	protected const string HeaderSimplePaintJob = "simple_paint_job_data";
	private const string NameAccList = "acc_list";



	protected override bool HasQuote(string name) => name switch {
		NameDisplayName or
		NameIcon or
		NamePartType or
		NameSuitableFor or
		NameConflictWith or
		NameDefaults or
		NameOverrides or
		NameRequire or
		NameBaseTextureOverride or
		NamePaintJobMask or
		NameFlakeNoise => true,
		_ => false
	};

	protected override bool IsArray(string name) => name switch {
		NameSuitableFor or
		NameConflictWith or
		NameDefaults or
		NameOverrides or
		NameRequire or
		NameColorVariant => true,
		_ => false
	};

	private void WriteAccPaintJobData(StreamWriter sw, string paintJobName, string truckID, Action data) {
		WriteDataHeader(sw, HeaderAccPaintJob, $"{paintJobName}.{truckID}.paint_job", data);
	}

	public void WritePaintJobData(PaintJobBinding binding) {
		var truckID = binding.CurrentTruck!.TruckID;
		var paintJobName = binding.ModelName;
		var siiFile = Paths.AccPaintJobFile(truckID, paintJobName);
		using StreamWriter sw = new(siiFile);
		WriteFileStructure(sw, () => {
			WriteAccPaintJobData(sw, paintJobName, truckID, () => {
				bool customBase, custom1, custom2, custom3;

				WriteLine(sw, NameDisplayName, binding.DisplayName);
				WriteLine(sw, NamePrice, binding.Price);
				WriteLine(sw, NameUnlock, binding.UnlockLevel);
				WriteLine(sw, NameIcon, binding.IconName);
				WriteLine(sw, NamePartType, binding.PartType, "unknown");
				WriteLine(sw, NameSyncOverNetwork, binding.SyncOverNetwork, false);
				WriteLine(sw, NameSuitableFor, binding.CurrentCabin!.CabinID);//当选择多个卡车时需要输出多个卡车
				//WriteLine(sw, NameConflictWith, binding.ConflictWith);

				WriteEmptyLine(sw);
				WriteLine(sw, NameBaseColor, binding.BaseColor, Colors.White);
				customBase = binding.BaseColorCustomizable;
				WriteLine(sw, NameBaseColorLocked, binding.BaseColorLocked, true);
				WriteLine(sw, NamePaintJobMask, binding.PaintJobTex);
				WriteLine(sw, NameBaseTextureOverride, binding.BaseTexOverride);
				WriteLine(sw, NameAlternateUVSet, binding.AlternateUVSet, false);

				WriteEmptyLine(sw);
				bool airbrush = binding.Airbrush;
				if (airbrush) {
					WriteLine(sw, NameAirbrush, airbrush, false);
					WriteLine(sw, NameFlipFlake, binding.FlipFlake, false);
					WriteLine(sw, NameAlternateFlipFlakeUVSet, binding.AlternateFlipFlakeUVSet, false);
					WriteLine(sw, NameFlipColor, binding.FlipColor, Colors.Red);
					custom1 = binding.FlipColorCustomizable;
					WriteLine(sw, NameFlipColorLocked, binding.FlipColorLocked, true);
					WriteLine(sw, NameFlakeColor, binding.FlakeColor, Color.FromRgb(0, 255, 0));
					custom2 = binding.FlakeColorCustomizable;
					custom3 = false;
					WriteLine(sw, NameFlakeColorLocked, binding.FlakeColorLocked, true);
					WriteLine(sw, NameFlipStrength, binding.FlipStrength, 0.27f);
					WriteLine(sw, NameFlakeShininess, binding.FlakeShininess, 50f);
					WriteLine(sw, NameFlakeDensity, binding.FlakeDensity, 1f);
					WriteLine(sw, NameFlakeClearcoatRolloff, binding.FlakeClearcoatRolloff, 2.2f);
					WriteLine(sw, NameFlakeUVScale, binding.FlakeUVScale, 32f);
					WriteLine(sw, NameFlakeVRatio, binding.FlakeVRatio, 1f);
					WriteLine(sw, NameFlakeNoise, binding.FlakeNoise, "/material/custom/flake_noise.tobj");
				} else {
					WriteLine(sw, NameMaskRColor, binding.MaskRColor, Colors.Red);
					custom3 = binding.MaskRCustomizable;
					WriteLine(sw, NameMaskRLocked, binding.MaskRLocked, true);
					WriteLine(sw, NameMaskGColor, binding.MaskGColor, Color.FromRgb(0, 255, 0));
					custom2 = binding.MaskGCustomizable;
					WriteLine(sw, NameMaskGLocked, binding.MaskGLocked, true);
					WriteLine(sw, NameMaskBColor, binding.MaskBColor, Colors.Blue);
					custom1 = binding.MaskBCustomizable;
					WriteLine(sw, NameMaskBLocked, binding.MaskBLocked, true);
				}

				if (customBase && custom1 && custom2 && custom3) {
					WriteEmptyLine(sw);
					Color blank = Color.FromRgb(255,255,255);
					foreach (var variant in binding.ColorVariantList) {
						Color color = customBase ? variant.ColorBase ?? blank : blank;
						WriteLine(sw, NameColorVariant, color);
						color = custom1 ? variant.Color1 ?? (airbrush ? Colors.Red : Colors.Blue) : blank;
						WriteLine(sw, NameColorVariant, color);
						color = custom2 ? variant.Color2 ?? Color.FromRgb(0, 255, 0) : blank;
						WriteLine(sw, NameColorVariant, color);
						color = custom3 ? variant.Color3 ?? Colors.Red : blank;
						WriteLine(sw, NameColorVariant, color);
					}
				}
			});
		});

		WriteOvrPaintJobData(binding);
	}
	private void WriteSimplePaintJobData(StreamWriter sw, string ovrName, Action data) {
		WriteDataHeader(sw, HeaderSimplePaintJob, ovrName, data);
	}

	private void WriteOvrPaintJobData(PaintJobBinding binding) {
		if (binding.OverrideList.Count == 0)
			return;
		var truckID = binding.CurrentTruck!.TruckID;
		var paintJobName = binding.ModelName;
		var siiFile = Paths.SimplePaintJobFile(truckID, paintJobName);
		using StreamWriter sw = new(siiFile);
		WriteFileStructure(sw, () => {
			for (int i = 0; i < binding.OverrideList.Count; i++) {
				if (i > 0)
					WriteEmptyLine(sw);
				PaintJobOverrideData? ovr = binding.OverrideList[i];
				WriteSimplePaintJobData(sw, ovr.OvrName, () => {
					WriteLine(sw, NamePaintJobMask, ovr.AccTex);
					WriteLine(sw, NameFlakeUVScale, ovr.AccFlakeUVScale);
					WriteLine(sw, NameFlakeVRatio, ovr.AccFlakeVRatio);
					foreach(var acc in ovr.AccList) {
						WriteLine(sw, NameAccList, acc.AccID);
					}
				});
			}
		});
	}

	//注意simple的文件名字对应paintjob的模型名（而不是文件名）
}
