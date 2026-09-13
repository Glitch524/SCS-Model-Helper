using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory.PaintJob {
	internal partial class PaintJobBinding: BaseBinding {

		private readonly AccessoryPaintJobData mPaintJobData = new();
		public AccessoryPaintJobData PaintJobData => mPaintJobData;
		public PaintJobBinding() {
		}


		//Display Name
		public string DisplayName {
			get => PaintJobData.DisplayName;
			set {
				PaintJobData.DisplayName = value;
				InvokeChange();
				InvokeChange(nameof(CheckResVisibility));
			}
		}

		private string? mCheckResResult = null;
		public string? CheckResResult {
			get => mCheckResResult;
			set {
				mCheckResResult = value;
				InvokeChange();
			}
		}

		private bool mPopupCheckOpen = false;
		public bool PopupCheckOpen {
			get => mPopupCheckOpen;
			set {
				mPopupCheckOpen = value;
				InvokeChange();
			}
		}

		public static ObservableCollection<StringResItem> StringResList => StringResUtil.StringResList;
		public Visibility CheckResVisibility => PaintJobData.CheckResVisibility;

		public void CheckNameStringRes() {
			CheckResResult ??= StringResUtil.GetStringResResults(DisplayName, out _);
			PopupCheckOpen = true;
		}

		//Icon
		public string IconName {
			get => PaintJobData.IconName;
			set {
				PaintJobData.IconName = value;
				InvokeChange();

				InvokeChange(nameof(IconNameClearVisibility));
			}
		}
		public Visibility IconNameClearVisibility => IconName.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

		public BitmapSource? ModelIcon {
			get => PaintJobData.ModelIcon;
			set {
				PaintJobData.ModelIcon = value;
				InvokeChange();
			}
		}

		public string ModelName {
			get => PaintJobData.ModelName;
			set {
				PaintJobData.ModelName = value;
				InvokeChange();

				InvokeChange(nameof(NameOver12));
			}
		}

		public bool NameOver12 => ModelName.Length > 12;

		public string PartType {
			get => PaintJobData.PartType;
			set {
				PaintJobData.PartType = value;
				InvokeChange();
			}
		}

		public static List<PartTypeItem> PartTypes => AccessoryData.PartTypes;

		public long? Price {
			get => PaintJobData.Price;
			set {
				PaintJobData.Price = value;
				InvokeChange();
			}
		}

		public uint? UnlockLevel {
			get => PaintJobData.UnlockLevel;
			set {
				PaintJobData.UnlockLevel = value;
				InvokeChange();
			}
		}

		public string SuitableForListContent => PaintJobData.SuitableForListContent;


		//以下为Accessory Paint Job Data内容

		public Color BaseColor {
			get => PaintJobData.BaseColor;
			set {
				PaintJobData.BaseColor = value;
				InvokeChange();
				InvokeChange(nameof(BaseColorVisual));
			}
		}
		public Brush BaseColorVisual => PaintJobData.BaseColorVisual;

		public bool BaseColorCustomizable {
			get => PaintJobData.BaseColorCustomizable;
			set {
				PaintJobData.BaseColorCustomizable = value;
				InvokeChange();
			}
		}

		public bool AlternateUVSet {
			get => PaintJobData.AlternateUVSet;
			set {
				PaintJobData.AlternateUVSet = value;
				InvokeChange();
			}
		}


		public string PaintJobTex {
			get => PaintJobData.PaintJobTex;
			set {
				PaintJobData.PaintJobTex = value;
				InvokeChange();
				InvokeChange(nameof(PaintJobTexVisibility));
			}
		}
		public BitmapSource? PaintJobTexImage {
			get => PaintJobData.PaintJobTexImage;
			set {
				PaintJobData.PaintJobTexImage = value;
				InvokeChange();
			}
		}
		public Visibility PaintJobTexVisibility => PaintJobData.PaintJobTexVisibility;

		public string BaseTexOverride {
			get => PaintJobData.BaseTexOverride;
			set {
				PaintJobData.BaseTexOverride = value;
				InvokeChange();
				InvokeChange(nameof(BaseTexOverrideVisibility));
			}
		}

		public Visibility BaseTexOverrideVisibility => PaintJobData.BaseTexOverrideVisibility;


		//以下为ColorMask内容

		public Color MaskRColor {
			get => PaintJobData.MaskRColor;
			set {
				PaintJobData.MaskRColor = value;
				InvokeMaskR();
			}
		}
		public Brush MaskRColorVisual => PaintJobData.MaskRColorVisual;
		public string MaskRColorHex {
			get => PaintJobData.MaskRColorHex;
			set {
				PaintJobData.MaskRColorHex = value;
				InvokeMaskR();
			}
		}
		public byte MaskRColorR {
			get => PaintJobData.MaskRColorR;
			set {
				PaintJobData.MaskRColorR = value;
				InvokeMaskR();
			}
		}
		public byte MaskRColorG {
			get => PaintJobData.MaskRColorG;
			set {
				PaintJobData.MaskRColorG = value;
				InvokeMaskR();
			}
		}
		public byte MaskRColorB {
			get => PaintJobData.MaskRColorB;
			set {
				PaintJobData.MaskRColorB = value;
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

		public bool MaskRCustomizable {
			get => PaintJobData.MaskRCustomizable;
			set {
				PaintJobData.MaskRCustomizable = value;
				InvokeChange();
			}
		}


		public Color MaskGColor {
			get => PaintJobData.MaskGColor;
			set {
				PaintJobData.MaskGColor = value;
				InvokeMaskG();
			}
		}
		public Brush MaskGColorVisual => PaintJobData.MaskGColorVisual;
		public string MaskGColorHex {
			get => PaintJobData.MaskGColorHex;
			set {
				PaintJobData.MaskGColorHex = value;
				InvokeMaskG();
			}
		}
		public byte MaskGColorR {
			get => PaintJobData.MaskGColorR;
			set {
				PaintJobData.MaskGColorR = value;
				InvokeMaskG();
			}
		}
		public byte MaskGColorG {
			get => PaintJobData.MaskGColorG;
			set {
				PaintJobData.MaskGColorG = value;
				InvokeMaskG();
			}
		}
		public byte MaskGColorB {
			get => PaintJobData.MaskGColorB;
			set {
				PaintJobData.MaskGColorB = value;
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

		public bool MaskGCustomizable {
			get => PaintJobData.MaskGCustomizable;
			set {
				PaintJobData.MaskGCustomizable = value;
				InvokeChange();
			}
		}

		public Color MaskBColor {
			get => PaintJobData.MaskBColor;
			set {
				PaintJobData.MaskBColor = value;
				InvokeMaskB();
			}
		}
		public Brush MaskBColorVisual => PaintJobData.MaskBColorVisual;
		public string MaskBColorHex {
			get => PaintJobData.MaskBColorHex;
			set {
				PaintJobData.MaskBColorHex = value;
				InvokeMaskB();
			}
		}
		public byte MaskBColorR {
			get => PaintJobData.MaskBColorR;
			set {
				PaintJobData.MaskBColorR = value;
				InvokeMaskB();
			}
		}
		public byte MaskBColorG {
			get => PaintJobData.MaskBColorG;
			set {
				PaintJobData.MaskBColorG = value;
				InvokeMaskB();
			}
		}
		public byte MaskBColorB {
			get => PaintJobData.MaskBColorB;
			set {
				PaintJobData.MaskBColorB = value;
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

		public bool MaskBCustomizable {
			get => PaintJobData.MaskBCustomizable;
			set {
				PaintJobData.MaskBCustomizable = value;
				InvokeChange();
			}
		}

		public bool FlipFlake {
			get => PaintJobData.Flipflake;
			set {
				PaintJobData.Flipflake = value;
				InvokeChange();
			}
		}

		public Brush FlipColorVisual => PaintJobData.FlipColorVisual;

		public bool FlipColorCustomizable {
			get => PaintJobData.FlipColorCustomizable;
			set {
				PaintJobData.FlipColorCustomizable = value;
				InvokeChange();
			}
		}

		public float FlipStrength {
			get => PaintJobData.FlipStrength;
			set {
				PaintJobData.FlipStrength = value;
				InvokeChange();
			}
		}

		public Brush FlakeColorVisual => PaintJobData.FlakeColorVisual;

		public bool FlakeColorCustomizable {//固定鳞片漆颜色
			get => PaintJobData.FlakeColorCustomizable;
			set {
				PaintJobData.FlakeColorCustomizable = value;
				InvokeChange();
			}
		}

		public float FlakeShininess {
			get => PaintJobData.FlakeShininess;
			set {
				PaintJobData.FlakeShininess = value;
				InvokeChange();
			}
		}

		public float FlakeDensity {
			get => PaintJobData.FlakeDensity;
			set {
				PaintJobData.FlakeDensity = value;
				InvokeChange();
			}
		}

		public float FlakeClearcoatRolloff {
			get => PaintJobData.FlakeClearcoatRolloff;
			set {
				PaintJobData.FlakeClearcoatRolloff = value;
				InvokeChange();
			}
		}

		public float FlakeUVScale {
			get => PaintJobData.FlakeUVScale;
			set {
				PaintJobData.FlakeUVScale = value;
				InvokeChange();
			}
		}

		public string FlakeNoise {
			get => PaintJobData.FlakeNoise;
			set {
				PaintJobData.FlakeNoise = value;
				InvokeChange();
			}
		}

		public Visibility FlakeNoiseVisibility => PaintJobData.FlakeNoiseVisibility;

		public ObservableCollection<PaintJobOverrideData> OverrideList => PaintJobData.OverrideList;

		public PaintJobOverrideData? SelectedOverride {
			get => PaintJobData.SelectedOverride;
			set {
				PaintJobData.SelectedOverride = value;
				InvokeChange();
			}
		}
		public string AccTex {
			get => PaintJobData.AccTex;
			set {
				PaintJobData.AccTex = value;
				InvokeChange();
				InvokeChange(nameof(AccTexVisibility));
			}
		}
		public Visibility AccTexVisibility => PaintJobData.AccTexVisibility;
		public BitmapSource? AccTexImage {
			get => PaintJobData.AccTexImage;
			set {
				PaintJobData.AccTexImage = value;
				InvokeChange();
			}
		}
		public float? AccFlakeUVScale {
			get => PaintJobData.AccFlakeUVScale;
			set {
				PaintJobData.AccFlakeUVScale = value;
				InvokeChange();
			}
		}
		public float? AccFlakeVRatio {
			get => PaintJobData.AccFlakeVRatio;
			set {
				PaintJobData.AccFlakeVRatio = value;
				InvokeChange();
			}
		}
		public ObservableCollection<string> AccList => PaintJobData.AccList;




		public void ChooseIcon(Window window) {
			var icon = AccessoryDataUtil.ChooseIcon(window, out string? iconPath);
			if (icon != null) {
				ModelIcon = AccessoryDataUtil.LoadModelIcon(iconPath);
				IconName = icon;
			}
		}

		public const int TEX_PAINT_JOB = 0;
		public const int TEX_BASE_TEX_OVR = 1;
		public const int TEX_FLAKE_NOISE = 2;
		public const int TEX_ACC_TEX = 3;
		public void ChooseTex(Window window, int type) {
			string title;
			string filter = Util.GetFilter("DialogFilterTextureFile");
			switch (type) {
				case TEX_PAINT_JOB:
				case TEX_ACC_TEX:
					title = Util.GetString("TitlePaintJobTexture");
					break;
				case TEX_BASE_TEX_OVR:
					title = Util.GetString("TitleBaseTexOverride");
					break;
				case TEX_FLAKE_NOISE:
					title = Util.GetString("TitleFlakeNoise");
					break;
				default:
					return;
			}

			var textureDialog = new OpenFileDialog {
				Title = title,
				Multiselect = false,
				DefaultDirectory = Instances.ProjectLocation,
				DefaultExt = "tobj",
				Filter = filter,
			};
			if (textureDialog.ShowDialog() != true)
				return;
			string path = textureDialog.FileName;
			CheckTexPath(window, type, path);
		}

		public void CheckTexPath(Window window, int type, string path) {
			string projectLocation = Instances.ProjectLocation;
			if (path.StartsWith(projectLocation)) {
				string? imagePath;
				string tobjPath;
				if (path.EndsWith(".tobj")) {
					imagePath = GetPathFromTobj(path);
					tobjPath = path;
				} else if (path.EndsWith(".tga") || path.EndsWith(".dds")) {
					imagePath = path;
					tobjPath = path.Substring(0, path.Length - 4) + ".tobj";
					if (!File.Exists(tobjPath)) {
						throw new NotImplementedException();
						//create tobj
					}
				} else {
					imagePath = path;
					throw new NotImplementedException();
					//转换为tga或dds 然后问是否创建tobj？
				}
				string inProjectpath = tobjPath[projectLocation.Length..];
				inProjectpath = inProjectpath.Replace("\\", "/");
				if (!inProjectpath.StartsWith('/'))
					inProjectpath = "/" + inProjectpath;
				switch (type) {
					case TEX_PAINT_JOB:
						PaintJobTex = inProjectpath;
						break;
					case TEX_BASE_TEX_OVR:
						BaseTexOverride = inProjectpath;
						break;
					case TEX_FLAKE_NOISE:
						FlakeNoise = inProjectpath;
						break;
					case TEX_ACC_TEX:
						AccTex = inProjectpath;
						break;
					default:
						return;
				}
				if (imagePath != null) {
					if (type == TEX_PAINT_JOB || type == TEX_ACC_TEX) {
						imagePath = imagePath.ToLower();
						if (imagePath.EndsWith(".dds") || imagePath.EndsWith(".tga")) {
							if (type == TEX_PAINT_JOB)
								PaintJobTexImage = Util.LoadPfimIcon(imagePath);
							else
								AccTexImage = Util.LoadPfimIcon(imagePath);
						} else {
							if (type == TEX_PAINT_JOB)
								PaintJobTexImage = Util.LoadIcon(imagePath);
							else
								AccTexImage = Util.LoadIcon(imagePath);
						}
					}
				}
			} else {
				MessageBox.Show(window, Util.GetString("DialogResultFileOutsideProject"));
			}
		}

		private static string? GetPathFromTobj(string tobjPath) {
			using StreamReader reader = new(tobjPath, Encoding.UTF8);
			string? line = reader.ReadLine();
			if (line == null) 
				return null;
			string result;
			if (line.StartsWith('\u0001')) {
				line = reader.ReadLine();
				if (line == null)
					return null;
				result = '/' + line[(line.IndexOf('/') + 1)..];
			} else {
				result = deMap2D().Replace(line, "").Trim();
				if (result.Length == 0) {//路径有可能在下一行
					line = reader.ReadLine()?.Trim();
					if (line == null || line.StartsWith("addr"))
						return null;
					result = line;
				}
			}
			result = result.Replace('/', '\\');
			if (result.StartsWith('\\')) {
				result = Instances.ProjectLocation + result;
			} else {
				result = new DirectoryInfo(tobjPath).Parent + "\\" + result;
			}
			return result;
		}

		[GeneratedRegex(@"map[\s\t]2d")]
		private static partial Regex deMap2D();
	}
}
