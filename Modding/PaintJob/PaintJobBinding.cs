using Microsoft.Win32;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Modding.Accessories;
using SCS_Mod_Helper.Modding.Accessories.AccAddon.Items;
using SCS_Mod_Helper.Trucks;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Media.AppRecording;
using Wpf.Ui.Extensions;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public partial class PaintJobBinding: BaseBinding {
		public Window? PreviewWindow;
		public bool IsPreviewOpen => PreviewWindow != null;

		public void OpenPaintJobPreview(PaintJobWindow window) {
			if (PreviewWindow == null) {
				PreviewWindow = new PaintJobTexPreviewWindow(this) {
					Owner = window
				};
				PreviewWindow.Show();
			} else
				PreviewWindow.Focus();
		}


		private readonly AccPaintJobData mPaintJobData = new();
		public AccPaintJobData PaintJobData => mPaintJobData;
		public PaintJobBinding() {
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

		public static List<PartTypeItem> PartTypes => AccessoryData.PartTypes;

		public string PartType {
			get => PaintJobData.PartType;
			set {
				PaintJobData.PartType = value;
				InvokeChange();
			}
		}

		public ObservableCollection<Truck> Trucks => PaintJobData.Trucks;

		public Truck? CurrentTruck {
			get => PaintJobData.CurrentTruck;
			set {
				CleanAOvrAccessories();
				PaintJobData.CurrentTruck = value;
				InvokeChange();
				InvokeChange(nameof(CabinList));
				InvokeChange(nameof(AccessoryList));
			}
		}
		public List<Cabin>? CabinList => PaintJobData.CabinList;

		public Cabin? CurrentCabin {
			get => PaintJobData.CurrentCabin;
			set {
				PaintJobData.CurrentCabin = value;
				InvokeChange();
				GetCabinUV();
			}
		}

		public List<Accessory>? AccessoryList => PaintJobData.AccessoryList;


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

		public long? Price {
			get => PaintJobData.Price;
			set {
				PaintJobData.Price = value;
				InvokeChange();
				InvokeChange(nameof(PriceString));
			}
		}
		public string PriceString {
			get => Price.ToString() ?? "";
			set {
				Price = value.Length == 0 ? null : long.Parse(value);
				InvokeChange();
				InvokeChange(nameof(Price));
			}
		}

		public uint? UnlockLevel {
			get => PaintJobData.UnlockLevel;
			set {
				PaintJobData.UnlockLevel = value;
				InvokeChange();
				InvokeChange(nameof(UnlockLevelString));
			}
		}
		public string UnlockLevelString {
			get => UnlockLevel.ToString() ?? "";
			set {
				UnlockLevel = value.Length == 0 ? null : uint.Parse(value);
				InvokeChange();
				InvokeChange(nameof(UnlockLevel));
			}
		}

		//todo suitablefor conflictwith


		//以下为Accessory Paint Job Data内容

		public Color BaseColor {
			get => PaintJobData.BaseColor;
			set {
				PaintJobData.BaseColor = value;
				InvokeChange();
				InvokeChange(nameof(BaseColorVisual));
			}
		}

		public Brush BaseColorVisual => BaseColor.ToBrush();

		public bool BaseColorCustomizable {
			get => PaintJobData.BaseColorCustomizable;
			set {
				PaintJobData.BaseColorCustomizable = value;
				InvokeChange();
			}
		}
		public bool BaseColorLocked => !BaseColorCustomizable;


		public bool AlternateUVSet {
			get => PaintJobData.AlternateUVSet;
			set {
				PaintJobData.AlternateUVSet = value;
				InvokeChange();
			}
		}

		public string PaintJobTexRealPath = "";
		public string PaintJobTex {
			get => PaintJobData.PaintJobTex;
			set {
				PaintJobData.PaintJobTex = value;
				InvokeChange();
				InvokeChange(nameof(PaintJobTexVisibility));
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

		public ObservableCollection<ColorVariant> ColorVariantList => PaintJobData.ColorVariantList;

		public void AddColorVariant() {
			ColorVariantList.Add(new());
			InvokeChange(nameof(CurrentVariant));
		}

		public void RemoveColorVariant(ColorVariant variant) {
			ColorVariantList.Remove(variant);
			if (CurrentVariant == variant) {
				if (ColorVariantList.Count > 0)
					CurrentVariant = ColorVariantList[0];
				else
					CurrentVariant = null;
			}
		}

		private ColorVariant? mCurrentVariant = null;
		public ColorVariant? CurrentVariant {
			get {
				if (ColorVariantList.Count == 0)
					return null;
				mCurrentVariant ??= ColorVariantList[0];
				return mCurrentVariant;
			}
			set {
				mCurrentVariant = value;
				InvokeChange();
				InvokeChange(nameof(VariantColorBase));
				InvokeChange(nameof(VariantColorBaseBrush));
				InvokeChange(nameof(VariantColor1));
				InvokeChange(nameof(VariantColor1Brush));
				InvokeChange(nameof(VariantColor2));
				InvokeChange(nameof(VariantColor2Brush));
				InvokeChange(nameof(VariantColor3));
				InvokeChange(nameof(VariantColor3Brush));
			}
		}

		public Color VariantColorBase {
			get => CurrentVariant?.ColorBase ?? Colors.White;
			set {
				if (CurrentVariant == null)
					return;
				CurrentVariant.ColorBase = value;
				InvokeChange();
				InvokeChange(nameof(VariantColorBaseBrush));
			}
		}
		public Brush VariantColorBaseBrush => VariantColorBase.ToBrush();

		public Color VariantColor1 {
			get => CurrentVariant?.Color1 ?? (Airbrush ? Colors.Red : Colors.Blue);
			set {
				if (CurrentVariant == null)
					return;
				CurrentVariant.Color1 = value;
				InvokeChange();
				InvokeChange(nameof(VariantColor1Brush));
			}
		}
		public Brush VariantColor1Brush => VariantColor1.ToBrush();

		public Color VariantColor2 {
			get => CurrentVariant?.Color2 ?? Color.FromRgb(0, 255, 0);
			set {
				if (CurrentVariant == null)
					return;
				CurrentVariant.Color2 = value;
				InvokeChange();
				InvokeChange(nameof(VariantColor2Brush));
			}
		}
		public Brush VariantColor2Brush => VariantColor2.ToBrush();

		public Color VariantColor3 {
			get => CurrentVariant?.Color3 ?? Colors.Red;
			set {
				if (CurrentVariant == null)
					return;
				CurrentVariant.Color3 = value;
				InvokeChange();
				InvokeChange(nameof(VariantColor3Brush));
			}
		}
		public Brush VariantColor3Brush => VariantColor3.ToBrush();

		private int mTabIndex = 0;
		public int TabIndex {
			get => mTabIndex;
			set {
				mTabIndex = value;
				InvokeChange();
				InvokeChange(nameof(VariantColor1Brush));
				InvokeChange(nameof(Airbrush));
				LoadColorChannel();
			}
		}

		public bool Airbrush {
			get => TabIndex == 0;
			set {
				TabIndex = value ? 0 : 1;
				InvokeChange();
				InvokeChange(nameof(TableIndex));
			}
		}

		//以下为ColorMask内容

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

		public Color MaskRColor {
			get => PaintJobData.MaskRColor;
			set {
				PaintJobData.MaskRColor = value;
				InvokeMaskR();
			}
		}

		public Brush MaskRColorVisual => MaskRColor.ToBrush();

		public string MaskRColorHex {
			get => ColorToHex(MaskRColor);
			set {
				MaskRColor = HexToColor(value);
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
		public bool MaskRLocked => !MaskRCustomizable;


		public Color MaskGColor {
			get => PaintJobData.MaskGColor;
			set {
				PaintJobData.MaskGColor = value;
				InvokeMaskG();
			}
		}

		public Brush MaskGColorVisual => MaskGColor.ToBrush();

		public string MaskGColorHex {
			get => ColorToHex(MaskGColor);
			set {
				MaskGColor = HexToColor(value);
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
		public bool MaskGLocked => !MaskGCustomizable;

		public Color MaskBColor {
			get => PaintJobData.MaskBColor;
			set {
				PaintJobData.MaskBColor = value;
				InvokeMaskB();
			}
		}

		public Brush MaskBColorVisual => MaskBColor.ToBrush();
		public string MaskBColorHex {
			get => ColorToHex(MaskBColor);
			set {
				MaskBColor = HexToColor(value);
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
		public bool MaskBLocked => !MaskBCustomizable;

		public bool FlipFlake {
			get => PaintJobData.Flipflake;
			set {
				PaintJobData.Flipflake = value;
				InvokeChange();
			}
		}

		public bool AlternateFlipFlakeUVSet {
			get => PaintJobData.AlternateFlipFlakeUVSet;
			set {
				PaintJobData.AlternateFlipFlakeUVSet = value;
				InvokeChange();
			}
		}

		public Color FlipColor {
			get => PaintJobData.FlipColor;
			set {
				PaintJobData.FlipColor = value;
				InvokeChange();
				InvokeChange(nameof(FlipColorVisual));
			}
		}

		public Brush FlipColorVisual => FlipColor.ToBrush();

		public bool FlipColorCustomizable {
			get => PaintJobData.FlipColorCustomizable;
			set {
				PaintJobData.FlipColorCustomizable = value;
				InvokeChange();
			}
		}
		public bool FlipColorLocked => !FlipColorCustomizable;

		public float FlipStrength {
			get => PaintJobData.FlipStrength;
			set {
				PaintJobData.FlipStrength = value;
				InvokeChange();
				InvokeChange(nameof(IsFlipStrengthDefault));
			}
		}
		public bool IsFlipStrengthDefault => PaintJobData.IsFlipStrengthDefault;

		public Color FlakeColor {
			get => PaintJobData.FlakeColor;
			set {
				PaintJobData.FlakeColor = value;
				InvokeChange();
				InvokeChange(nameof(FlakeColorVisual));
			}
		}

		public Brush FlakeColorVisual => FlakeColor.ToBrush();

		public bool FlakeColorCustomizable {//固定鳞片漆颜色
			get => PaintJobData.FlakeColorCustomizable;
			set {
				PaintJobData.FlakeColorCustomizable = value;
				InvokeChange();
			}
		}
		public bool FlakeColorLocked => !FlakeColorCustomizable;

		public float FlakeShininess {
			get => PaintJobData.FlakeShininess;
			set {
				PaintJobData.FlakeShininess = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeShininessDefault));
			}
		}
		public bool IsFlakeShininessDefault => PaintJobData.IsFlakeShininessDefault;

		public float FlakeDensity {
			get => PaintJobData.FlakeDensity;
			set {
				PaintJobData.FlakeDensity = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeDensityDefault));
			}
		}
		public bool IsFlakeDensityDefault => PaintJobData.IsFlakeDensityDefault;

		public float FlakeClearcoatRolloff {
			get => PaintJobData.FlakeClearcoatRolloff;
			set {
				PaintJobData.FlakeClearcoatRolloff = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeClearcoatRolloffDefault));
			}
		}
		public bool IsFlakeClearcoatRolloffDefault => PaintJobData.IsFlakeClearcoatRolloffDefault;

		public float FlakeUVScale {
			get => PaintJobData.FlakeUVScale;
			set {
				PaintJobData.FlakeUVScale = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeUVScaleDefault));
			}
		}
		public bool IsFlakeUVScaleDefault => PaintJobData.IsFlakeUVScaleDefault;

		public float FlakeVRatio {
			get => PaintJobData.FlakeVRatio;
			set {
				PaintJobData.FlakeVRatio = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeVRatioDefault));
			}
		}
		public bool IsFlakeVRatioDefault => PaintJobData.IsFlakeVRatioDefault;

		public string FlakeNoise {
			get => PaintJobData.FlakeNoise;
			set {
				PaintJobData.FlakeNoise = value;
				InvokeChange();
				InvokeChange(nameof(IsFlakeNoiseDefault));
			}
		}
		public bool IsFlakeNoiseDefault => PaintJobData.IsFlakeNoiseDefault;

		public Visibility FlakeNoiseVisibility => PaintJobData.FlakeNoiseVisibility;



		public ObservableCollection<PaintJobOverrideData> OverrideList => PaintJobData.OverrideList;

		public PaintJobOverrideData? SelectedOverride {
			get => PaintJobData.SelectedOverride;
			set {
				ChangeCheckState = true;
				ObservableCollection<Accessory> accList;
				if (PaintJobData.SelectedOverride != null) {
					accList = PaintJobData.SelectedOverride.AccList;
					foreach(var acc in accList) {
						acc.Check = false;
					}
				}
				PaintJobData.SelectedOverride = value;
				if (value != null) {
					accList = value.AccList;
					foreach (var acc in accList) {
						acc.Check = true;
					}
				}
				ChangeCheckState = false;
				InvokeChange();
				InvokeChange(nameof(AccTex));
				InvokeChange(nameof(AccTexImage));
				InvokeChange(nameof(AccFlakeUVScale));
				InvokeChange(nameof(IsAccFlakeUVScaleDefault));
				InvokeChange(nameof(AccFlakeVRatio));
				InvokeChange(nameof(IsAccFlakeVRatioDefault));
			}
		}
		bool ChangeCheckState = false;
		public void AccessoryChecked(Accessory accessory) {
			if (ChangeCheckState)
				return;
			if (SelectedOverride == null)
				return;
			accessory.BelongingOverride?.AccList.Remove(accessory);
			SelectedOverride.AccList.Add(accessory);
			accessory.BelongingOverride = SelectedOverride;
		}

		public void AccessoryUnchecked(Accessory accessory) {
			if (ChangeCheckState)
				return;
			if (SelectedOverride == null)
				return;
			SelectedOverride.AccList.Remove(accessory);
			accessory.BelongingOverride = null;
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
				InvokeChange(nameof(IsAccFlakeUVScaleDefault));
			}
		}
		public bool IsAccFlakeUVScaleDefault => AccFlakeUVScale == 32f;
		public float? AccFlakeVRatio {
			get => PaintJobData.AccFlakeVRatio;
			set {
				PaintJobData.AccFlakeVRatio = value;
				InvokeChange();
				InvokeChange(nameof(IsAccFlakeVRatioDefault));
			}
		}
		public bool IsAccFlakeVRatioDefault => AccFlakeVRatio == 1f;

		private bool mSyncOverNetwork = false;
		public bool SyncOverNetwork {
			get => mSyncOverNetwork;
			set {
				mSyncOverNetwork = value;
				InvokeChange();
			}
		}



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
						PaintJobTexRealPath = path;
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
							if (type == TEX_PAINT_JOB) {
								LoadTexImage(imagePath);
							} else
								AccTexImage = Util.LoadPfimImage(imagePath);
						} else {
							if (type == TEX_PAINT_JOB) {
								LoadTexImage(imagePath);
							} else
								AccTexImage = Util.LoadImage(imagePath);
						}
					}
				}
			} else {
				MessageBox.Show(window, Util.GetString("DialogResultFileOutsideProject"));
			}
		}

		public void LoadTexImage(string imagePath) {
			PaintJobTexImage = null;
			PaintJobChannelR = null;
			PaintJobChannelG = null;
			PaintJobChannelB = null;
			if (IsPreviewOpen) {
				PaintJobTexImage = Util.LoadPfimImage(imagePath);
				LoadColorChannel();
			}
		}

		public void LoadColorChannel() {
			if (!Airbrush && PaintJobTexImage != null && PaintJobChannelR == null) {
				ChannelExtraction.ExtractRGBChannels(PaintJobTexImage, out WriteableBitmap rBmp, out WriteableBitmap gBmp, out WriteableBitmap bBmp);
				PaintJobChannelR = rBmp;
				PaintJobChannelG = gBmp;
				PaintJobChannelB = bBmp;
			}
		}



		private static void SaveBmp(WriteableBitmap bitmap, string filePath) {
			filePath += ".png";
			PngBitmapEncoder encode = new();
			encode.Frames.Add(BitmapFrame.Create(bitmap));
			using FileStream fs = new(filePath, FileMode.Create);
			encode.Save(fs);
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


		public BitmapSource? PaintJobTexImage {
			get => PaintJobData.PaintJobTexImage;
			set {
				PaintJobData.PaintJobTexImage = value;
				InvokeChange();
			}
		}
		public BitmapSource? PaintJobChannelR {
			get => PaintJobData.PaintJobChannelR;
			set {
				PaintJobData.PaintJobChannelR = value;
				InvokeChange();
			}
		}
		public BitmapSource? PaintJobChannelG {
			get => PaintJobData.PaintJobChannelG;
			set {
				PaintJobData.PaintJobChannelG = value;
				InvokeChange();
			}
		}
		public BitmapSource? PaintJobChannelB {
			get => PaintJobData.PaintJobChannelB;
			set {
				PaintJobData.PaintJobChannelB = value;
				InvokeChange();
			}
		}

		private BitmapImage? mCabinUV = null;
		public BitmapImage? CabinUV {
			get => mCabinUV;
			set {
				mCabinUV = value;
				InvokeChange();
			}
		}

		private void GetCabinUV() {
			if (CurrentCabin == null)
				return;
			var uri = new Uri($"/Modding/PaintJob/TruckUV/{CurrentCabin!.CabinID}.png", UriKind.Relative);
			CabinUV = new BitmapImage(uri);
		}




		public void ControlOvrTab(PaintJobOverrideData ovrData) {
			int index = OverrideList.IndexOf(ovrData);
			OverrideList.RemoveAt(index);
			for (int i = index; i < OverrideList.Count; i++) {
				OverrideList[i].Index = i;
			}
		}

		public void AddOvr() {
			OverrideList.Add(new(OverrideList.Count));
			SelectedOverride = OverrideList.Last();
		}

		public void CleanAOvrAccessories() {
			foreach(var ovr in OverrideList) {
				foreach(var acc in ovr.AccList) {
					acc.BelongingOverride = null;
				}
				ovr.AccList.Clear();
			}
		}

		public void CreatePaintJobSii(Window window) {
			if (ModelName.Length == 0
				|| CurrentTruck == null
				|| DisplayName.Length == 0
				|| Price == null
				|| UnlockLevel == null
				|| IconName.Length == 0)
				return;
			new PaintJobSCSIO().WritePaintJobData(this);
			MessageBox.Show(window, GetString("MessagePaintJobCreateSuccess"));
		}
	}
}
