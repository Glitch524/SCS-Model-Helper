using SCS_Mod_Helper.Language;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Manifest {
	public class ModProject: INotifyPropertyChanged {
		public ModProject() {
			mProjectLocation = ModBasic.Default.ProjectLocation;
			mAuthor = ModBasic.Default.Author;

			mLocales = DescLocale.GetLocales(out Dictionary<string, DescLocale> localeDict);
			LocaleDict = localeDict;
			LanguageUtil.ChangeLanguage += ChangeLocales;
		}

		public void InitData() {
			Version = "";
			ModDisplayName = "";
			Author = "";
			MPOptional = false;
			IconName = "";
			ModIcon = null;
			CategoryList.Clear();
			DescriptionName = "";
			foreach (var l in Locales) {
				l.DescContent = "";
			}
		}

		private readonly ObservableCollection<LanguageItem> mLanguages = [];
		public ObservableCollection<LanguageItem> Languages => mLanguages;

		private string mCurrentLanguage = LanguageUtil.CurrentLanguage;
		public string CurrentLanguage {
			get => mCurrentLanguage;
			set {
				mCurrentLanguage = value;
				InvokeChange();
				LanguageUtil.SwitchLanguage(value);
			}
		}

		private string mProjectLocation;
		public string ProjectLocation {
			get => mProjectLocation;
			set {
				mProjectLocation = value;
				InvokeChange();
				InvokeChange(nameof(ProjectExist));
			}
		}
		public bool ProjectExist => Directory.Exists(ProjectLocation);

		private string mVersion = "";
		public string Version {
			get => mVersion;
			set {
				mVersion = value;
				InvokeChange();
			}
		}

		private string mModDisplayName = "";
		public string ModDisplayName {
			get => mModDisplayName;
			set {
				mModDisplayName = value;
				InvokeChange();
			}
		}

		private string mAuthor;

		public string Author {
			get => mAuthor;
			set {
				mAuthor = value;
				InvokeChange();
			}
		}

		private bool mMPOptional = false;

		public bool MPOptional {
			get => mMPOptional;
			set {
				mMPOptional = value;
				InvokeChange();
			}
		}

		private string mIconName = "";
		public string IconName {
			get => mIconName;
			set {
				mIconName = value;
				InvokeChange();
			}
		}

		public string? OldIconName = null;
		private readonly BitmapImage DefaultIcon = new(new("pack://application:,,,/Images/IconPlaceholder.png"));
		private BitmapImage? mModIcon = null;
		public BitmapImage? ModIcon {
			get => mModIcon ?? DefaultIcon;
			set {
				mModIcon?.Freeze();
				mModIcon = value;
				InvokeChange();
			}
		}
		public bool NewIcon = false;

		private readonly ObservableCollection<string> mCategoryList = [];
		public ObservableCollection<string> CategoryList => mCategoryList;

		public bool CategoryTruck { get => GetCategory("truck"); set => SetCategory(value, "truck"); }
		public bool CategoryTrailer { get => GetCategory("trailer"); set => SetCategory(value, "trailer"); }
		public bool CategoryInterior { get => GetCategory("interior"); set => SetCategory(value, "interior"); }
		public bool CategoryTuningParts { get => GetCategory("tuning_parts"); set => SetCategory(value, "tuning_parts"); }
		public bool CategoryAiTraffic { get => GetCategory("ai_traffic"); set => SetCategory(value, "ai_traffic"); }
		public bool CategorySound { get => GetCategory("sound"); set => SetCategory(value, "sound"); }
		public bool CategoryPaintJob { get => GetCategory("paint_job"); set => SetCategory(value, "paint_job"); }
		public bool CategoryCargoPack { get => GetCategory("cargo_pack"); set => SetCategory(value, "cargo_pack"); }
		public bool CategoryMap { get => GetCategory("map"); set => SetCategory(value, "map"); }
		public bool CategoryUI { get => GetCategory("ui"); set => SetCategory(value, "ui"); }
		public bool CategoryWeatherSetup { get => GetCategory("weather_setup"); set => SetCategory(value, "weather_setup"); }
		public bool CategoryPhysics { get => GetCategory("physics"); set => SetCategory(value, "physics"); }
		public bool CategoryGraphics { get => GetCategory("graphics"); set => SetCategory(value, "graphics"); }
		public bool CategoryModel { get => GetCategory("models"); set => SetCategory(value, "models"); }
		public bool CategoryMovers { get => GetCategory("movers"); set => SetCategory(value, "movers"); }
		public bool CategoryWalkers { get => GetCategory("walkers"); set => SetCategory(value, "walkers"); }
		public bool CategoryPrefabs { get => GetCategory("prefabs"); set => SetCategory(value, "prefabs"); }
		public bool CategoryOther { get => GetCategory("other"); set => SetCategory(value, "other"); }

		private bool GetCategory(string cate) => CategoryList.Contains(cate);
		private void SetCategory(bool value, string cate, [CallerMemberName] string name = "") {
			if (value) {
				CategoryList.Add(cate);
				if (CategoryList.Count > 2)
					CategoryList.RemoveAt(0);
			} else
				CategoryList.Remove(cate);
			InvokeChange(name);
		}

		public string? OldDescriptionName = null;
		private string mDescriptionName = "";
		public string DescriptionName {
			get => mDescriptionName;
			set {
				mDescriptionName = value;
				InvokeChange();
			}
		}

		public ObservableCollection<DescLocale> mLocales;
		public ObservableCollection<DescLocale> Locales => mLocales;
		public Dictionary<string, DescLocale> LocaleDict {
			get;
			private set;
		}

		private DescLocale? mCurrentLocale = null;
		public DescLocale CurrentLocale {
			get {
				mCurrentLocale ??= Locales.First();
				return mCurrentLocale;
			}
			set {
				mCurrentLocale = value ?? Locales.First();
				InvokeChange();

				InvokeChange(nameof(DescContent));
			}
		}

		public string DescContent {
			get => CurrentLocale.DescContent;
			set => CurrentLocale.DescContent = value;
		}


		public void ChangeLocales() {
			foreach (var locale in Locales) {
				locale.RefreshName();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void InvokeChange([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));
	}
}
