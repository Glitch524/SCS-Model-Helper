using SCS_Mod_Helper.Modding.Accessories.AccAddon.Items;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Trucks; 
class AccessoryType {
	private static ObservableCollection<ModelTypeInfo>? mModelTypes = null;
	public static ObservableCollection<ModelTypeInfo> ModelTypes {
		get {
			mModelTypes ??= GetModelTypes();
			DictionaryUtil.ChangeLanguage += AccessoryChangeLanguage;
			return mModelTypes;
		}
	}

	public const string TypeCupHolder = "cup_holder";
	public const string TypeCurtainF = "curtain_f";
	public const string TypeDrvPlate = "drv_plate";
	public const string TypeCoDrvPlate = "codrv_plate";
	public const string TypeFlagL = "flag_l";
	public const string TypeFlagR = "flag_r";
	public const string TypeFlagFL = "flag_f_l";
	public const string TypeFlagFR = "flag_f_r";
	public const string TypeInlightBck = "inlight_bck";
	public const string TypeBckPanel = "bckpanel";
	public const string TypeLPillow = "l_pillow";
	public const string TypeSetCabinet = "set_cabinet";
	public const string TypeSetGlass = "set_glass";
	public const string TypeSetLGlass = "set_lglass";
	public const string TypeSterringW = "sterring_w";
	public const string TypeToyHang = "toyhang";
	public const string TypeToyAc = "toyac";
	public const string TypeToyStand = "toystand";
	public const string TypeToyDash = "toydash";
	public const string TypeToySeat = "toyseat";
	public const string TypeToyBed = "toybed";
	public const string TypeToyBig = "toybig";
	public const string TypeToyBack = "toyback";
	public const string TypeToySofa = "toysofa";
	public const string TypeToyTable = "toytable";
	public static ObservableCollection<ModelTypeInfo> GetModelTypes() {
		return[
			new(TypeCupHolder),
			new(TypeCurtainF, true),
			new(TypeDrvPlate),
			new(TypeCoDrvPlate),
			new(TypeFlagL, TypeFlagFL, "Acc.flag_l"),
			new(TypeFlagR, TypeFlagFR, "Acc.flag_r"),
			new(TypeInlightBck, TypeBckPanel, "Acc.back_wall"),
			new(TypeLPillow),
			new(TypeSetCabinet, false),
			new(TypeSetGlass),
			new(TypeSetLGlass),
			new(TypeSterringW),
			new(TypeToyHang),
			new(TypeToyAc),
			new(TypeToyStand),
			new(TypeToyDash, false),
			new(TypeToySeat),
			new(TypeToyBed),
			new(TypeToyBig),
			new(TypeToyBack, false),
			new(TypeToySofa, false),
			new(TypeToyTable, false),
		];
	}

	public static void AccessoryChangeLanguage() {
		foreach(var acc in ModelTypes) {
			acc.RefreshName();
		}
	}
}