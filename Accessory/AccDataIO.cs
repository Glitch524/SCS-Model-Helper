using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory;

class AccDataIO {

	private const string FileHeader = "SiiNunit";
	private static int TabCount = 0;

	private static void WriteFileHeader(StreamWriter sw) => sw.WriteLine(FileHeader);
	private static void WriteMFHeader(StreamWriter sw) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine(NameMFHeader);
	}
	private static void WriteAAHeader(StreamWriter sw, string modelName, string truckID, string modelType) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NameAAHeader} : {modelName}.{truckID}.{modelType}");
	}
	private static void WriteAHHeader(StreamWriter sw, string hookupName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NameAHHeader} : {hookupName + NameAHSuffix}");
	}
	private static void WritePTHeader(StreamWriter sw, string physicsName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePTHeader} : {physicsName + NamePTSuffix}");
	}

	private static void WriteLine(StreamWriter sw, string name, object? valueObj) => WriteLine(sw, name, valueObj, IsArray(name), HasQuote(name));

	private static void WriteLine(StreamWriter sw, string name, object? valueObj, bool isArray = false, bool hasQuote = false) {
		if (valueObj == null)
			return;
		if (valueObj is string s && s.Length == 0)
			return;
		string value;
		if (valueObj is float?[] floats) {
			if (floats.Length == 2) {
				if (floats[0] == null && floats[1] == null)
					return;
				value = $"({FloatToString(floats[0])},{FloatToString(floats[1])})";
			} else {
				if (floats[0] == null && floats[1] == null && floats[2] == null)
					return;
				value = $"({FloatToString(floats[0])},{FloatToString(floats[1])},{FloatToString(floats[2])})";
			}
		} else if (valueObj is float[] floatsN) {
			if (floatsN.Length == 2) {
				value = $"({FloatToString(floatsN[0])},{FloatToString(floatsN[1])})";
			} else
				value = $"({FloatToString(floatsN[0])},{FloatToString(floatsN[1])},{FloatToString(floatsN[2])})";
		} else if (valueObj is float f) {
			value = FloatToString(f);
		} else
			value = valueObj.ToString() ?? "";
		sw.Write(new string('\t', TabCount));
		sw.Write(name);
		if (isArray)
			sw.Write("[]");
		sw.Write(": ");
		if (hasQuote) {
			sw.WriteLine($"\"{value}\"");
		} else
			sw.WriteLine(value);
	}
	private static string FloatToString(float? f) => f?.ToString("0.0#####") ?? "0.0";

	private static void BraceIn(StreamWriter sw) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine('{');
		TabCount++;
	}
	private static void BraceOut(StreamWriter sw) {
		TabCount--;
		sw.Write(new string('\t', TabCount));
		sw.WriteLine('}');
	}
	private static void WriteEmptyLine(StreamWriter sw) => sw.WriteLine("");

	public static bool IsArray(string name) => name switch {
		NameMFCategory or
		NameData or
		NameSuitableFor or
		NameConflictWith or
		NameDefaults or
		NameOverrides or
		NameRequire or
		NamePTInstanceOffset or
		NameKey or
		NameValue => true,
		_ => false,
	};

	public static bool HasQuote(string name) => name switch {
		NameMFPackageVersion or
		NameMFDisplayName or
		NameMFAuthor or
		NameMFCategory or
		NameMFIcon or
		NameMFDescriptionFile or
		NameDisplayName or
		NameIconName or
		NameAAExtModel or
		NameAAIntModel or
		NameAAExtModelUK or
		NameAAIntModelUK or
		NameCollPath or
		NameAHModel or
		NamePTModel or
		NamePTColl or
		NamePTToyType or
		NamePTRopeMaterial or
		NameKey or
		NameValue => true,
		_ => false,
	};

	private const string NameMFHeader = "mod_package : .package_name";
	private const string NameMFPackageVersion = "package_version";
	private const string NameMFDisplayName = "display_name";
	private const string NameMFAuthor = "author";
	private const string NameMFCategory = "category";
	private const string NameMFIcon = "icon";
	private const string NameMFDescriptionFile = "description_file";
	private const string NameMFMPOptional = "mp_mod_optional";
	public static void SaveManifest(ModProject modProject) {
		if (Util.IsEmpty(modProject.ProjectLocation, modProject.ModDisplayName, modProject.DescriptionName)) {
			MessageBox.Show(Util.GetString("MessageManifestNotFilled"));
			return;
		}


		var saveLocation = modProject.ProjectLocation;
		if (!modProject.IconName.EndsWith(".jpg"))
			modProject.IconName += ".jpg";

		if (modProject.OldIconName != null) {
			var oldIconFile = Path.Combine(modProject.ProjectLocation, modProject.OldIconName);
			if (File.Exists(oldIconFile))
				File.Delete(oldIconFile);
		}
		if (modProject.ModIcon == null) {
			modProject.ModIcon = new BitmapImage(new("pack://application:,,,/Images/IconPlaceholder.png"));
			modProject.NewIcon = true;
		}
		var iconFile = Path.Combine(saveLocation, modProject.IconName);
		if (modProject.NewIcon || !File.Exists(iconFile)) {
			JpegBitmapEncoder encoder = new();
			encoder.Frames.Add(BitmapFrame.Create(modProject.ModIcon));
			if (File.Exists(iconFile))
				File.Delete(iconFile);
			using FileStream fs = new(iconFile, FileMode.CreateNew, FileAccess.ReadWrite);
			encoder.Save(fs);
		}

		if (modProject.OldDescriptionName != null && modProject.DescriptionName != modProject.OldDescriptionName) {
			var descDeExt = modProject.OldDescriptionName[..^4];
			foreach (var file in new DirectoryInfo(modProject.ProjectLocation).GetFiles()) {
				var name = file.Name;
				if (name.StartsWith(descDeExt) && name.EndsWith(".txt")) {
					file.Delete();
				}
			}
		}
		var deExt = modProject.DescriptionName[..^4];
		foreach (var locale in modProject.Locales) {
			if (locale.HasDesc) {
				var isUniversal = locale.LocaleValue.Equals(Locale.LocaleValueUni);
				var dFile = Path.Combine(saveLocation, deExt);
				if (!isUniversal)
					dFile += $".{locale.LocaleValue}";
				dFile += ".txt";
				using StreamWriter dWriter = new(dFile);
				dWriter.Write(locale.DescContent);
			}
		}

		TabCount = 0;
		var manifestFile = Paths.ManifestFile(saveLocation);
		using StreamWriter sw = new(manifestFile);
		WriteFileHeader(sw);
		BraceIn(sw);
		WriteMFHeader(sw);
		BraceIn(sw);
		WriteLine(sw, NameMFPackageVersion, modProject.Version);
		WriteLine(sw, NameMFDisplayName, modProject.ModDisplayName);
		WriteLine(sw, NameMFAuthor, modProject.Author);

		foreach (var cat in modProject.CategoryList) {
			WriteLine(sw, NameMFCategory, cat);
		}
		WriteLine(sw, NameMFIcon, modProject.IconName);
		WriteLine(sw, NameMFDescriptionFile, modProject.DescriptionName);
		WriteLine(sw, NameMFMPOptional, modProject.MPOptional.ToString().ToLower());
		BraceOut(sw);
		BraceOut(sw);
	}

	public static void LoadManifest(ModProject modProject) {
		var manifest = Paths.ManifestFile(modProject.ProjectLocation);
		if (!File.Exists(manifest)) 
			return;
		try {
			using StreamReader sr = new(manifest);
			string? line = sr.ReadLine()?.Trim();
			if (line == null || !line.Equals(FileHeader))
				throw new(Util.GetString("MessageLoadManifestErrNotManifest"));
			while ((line = sr.ReadLine()?.Trim()) != null) {
				if (line.Length == 0 || line == "{" || line == "}" || line.StartsWith('#') || line == NameMFHeader)
					continue;
				int colonIndex = line.IndexOf(':');
				var name = line[..colonIndex].Trim();
				if (name.EndsWith("[]"))
					name = name[..^2];
				var value = ClipValue(line[(colonIndex + 1)..]);
				switch (name) {
					case NameMFPackageVersion:
						modProject.Version = value;
						break;
					case NameMFDisplayName:
						modProject.ModDisplayName = value;
						break;
					case NameMFAuthor:
						modProject.Author = value;
						break;
					case NameMFCategory:
						modProject.CategoryList.Add(value);
						break;
					case NameMFIcon:
						modProject.IconName = value;
						modProject.OldIconName = value;
						var iconFile = Path.Combine(modProject.ProjectLocation, modProject.IconName);
						if (File.Exists(iconFile)) {
							modProject.ModIcon = Util.LoadIcon(iconFile);
						}

						break;
					case NameMFDescriptionFile:
						modProject.DescriptionName = value;
						modProject.OldDescriptionName = value;
						LoadDescription(modProject);
						break;
					case NameMFMPOptional:
						modProject.MPOptional = bool.Parse(value);
						break;
				}
			}
		} catch (Exception ex) {
			MessageBox.Show(Util.GetString("MessageLoadManifestErrFail") + "\n" + ex.Message);
		}
	}

	private static void LoadDescription(ModProject modProject) {
		var descriptionFile = new FileInfo(Path.Combine(modProject.ProjectLocation, modProject.DescriptionName));
		var ext = descriptionFile.Extension;
		var deExt = modProject.DescriptionName[..^(ext.Length - 1)];
		DirectoryInfo project = new(modProject.ProjectLocation);
		foreach (var file in project.GetFiles()) {
			var filename = file.Name;
			var fileExt = file.Extension;
			var fileDeExt = filename[..^(fileExt.Length - 1)];
			if (fileDeExt.Length >= deExt.Length && fileDeExt.StartsWith(deExt)) {
				using StreamReader sr = new(file.FullName);
				StringBuilder sb = new();
				string? line = null;
				while ((line = sr.ReadLine()) != null) {
					sb.AppendLine(line);
				}
				string locale;
				if (fileDeExt.Length == deExt.Length)
					locale = Locale.LocaleValueUni;
				else
					locale = filename[deExt.Length..^fileExt.Length];
				var descLocale = modProject.LocaleDict[locale];
				descLocale.DescContent = sb.ToString();
			}
		}
		modProject.CurrentLocale = modProject.LocaleDict[Locale.LocaleValueUni];
	}


	//accessory data
	private const string NameDisplayName = "name";
	private const string NamePrice = "price";
	private const string NameUnlock = "unlock";
	private const string NamePartType = "part_type";
	private const string NameIconName = "icon";
	private const string NameCollPath = "coll";
	private const string NameLook = "look";
	private const string NameVariant = "variant";
	//accessory addon
	private const string NameAAHeader = "accessory_addon_int_data";
	private const string NameAAExtModel = "exterior_model";
	private const string NameAAIntModel = "interior_model";
	private const string NameAAExtModelUK = "exterior_model_uk";
	private const string NameAAIntModelUK = "interior_model_uk";
	private const string NameAAHideIn = "hide_in";
	//accessory hookup
	private const string NameAHHeader = "accessory_hookup_int_data";
	private const string NameAHSuffix = ".addon_hookup";
	private const string NameAHModel = "model";

	//others
	public const string NameData = "data";
	public const string NameSuitableFor = "suitable_for";
	public const string NameConflictWith = "conflict_with";
	public const string NameDefaults = "defaults";
	public const string NameOverrides = "overrides";
	public const string NameRequire = "require";
	//physics toy data
	private const string NamePTHeader = "physics_toy_data";
	public const string NamePTSuffix = ".phys_data";
	public const string NamePTModel = "phys_model";
	public const string NamePTColl = "phys_model_coll";
	public const string NamePTLook = "phys_model_look";
	public const string NamePTVariant = "phys_model_variant";
	public const string NamePTToyType = "toy_type";
	public const string NamePTMass = "toy_mass";
	public const string NamePTCogOffset = "toy_cog_offset";
	public const string NamePTLinearStiffness = "linear_stiffness";
	public const string NamePTLinearDamping = "linear_damping";
	public const string NamePTAngularStiffness = "angular_stiffness";
	public const string NamePTAngularDamping = "angular_damping";
	public const string NamePTAngularAmplitude = "angular_amplitude";
	public const string NamePTNodeDamping = "node_damping";
	public const string NamePTLocatorHookOffset = "locator_hook_offset";
	public const string NamePTRestPositionOffset = "rest_position_offset";
	public const string NamePTRestRotationOffset = "rest_rotation_offset";
	public const string NamePTInstanceOffset = "instance_offset";
	public const string NamePTRopeWidth = "rope_width";
	public const string NamePTRopeLength = "rope_length";
	public const string NamePTRopeHookOffset = "rope_hook_offset";
	public const string NamePTRopeToyOffset = "rope_toy_offset";
	public const string NamePTRopeResolution = "rope_resolution";
	public const string NamePTRopeLinearDensity = "rope_linear_density";
	public const string NamePTPositionIterations = "position_iterations";
	public const string NamePTRopeMaterial = "rope_material";

	//addon hookup storage
	private const string NameAHSPreffix = "addon_hookup_storage";


	public static int CreateAccAddonSii(AccessoryAddonItem accAddonItem) {
		var created = 0;
		if (accAddonItem.TruckExpandedETS2)
			created += CreateSii(accAddonItem, true);
		if (accAddonItem.TruckExpandedATS)
			created += CreateSii(accAddonItem, false);
		return created;
	}
	private static int CreateSii(AccessoryAddonItem accAddonItem, bool isETS2) {
		int numberCreated = 0;
		foreach (var truck in isETS2 ? accAddonItem.TrucksETS2 : accAddonItem.TrucksATS) {
			var siiFile = Paths.SiiFile(Instances.ProjectLocation, truck.TruckID, truck.ModelType, accAddonItem.ModelName);
			if (truck.Check) {
				if (truck.ModelType.Length == 0 || truck.TruckID.Length == 0 || truck.Look.Length == 0 || truck.Variant.Length == 0)
					continue;
			} else {
				if (accAddonItem.DeleteUnchecked && File.Exists(siiFile))
					File.Delete(siiFile);
				continue;
			}
			DirectoryInfo sii = new(siiFile);
			if (!sii.Parent!.Exists)
				sii.Parent!.Create();
			var othersList = new List<OthersItem>();
			othersList.AddRange(accAddonItem.OthersList);
			string? exterior = null, exteriorUK = null;
			for (int i = 0; i < othersList.Count; i++) {
				OthersItem? other = othersList[i];
				if (other.OthersName == NameAAExtModel) {
					exterior = other.OthersValue;
					othersList.RemoveAt(i);
					if (exteriorUK != null)
						break;
				} else if (other.OthersName == NameAAExtModelUK) {
					exteriorUK = other.OthersValue;
					othersList.RemoveAt(i);
					if (exterior != null)
						break;
				}
			}
			TabCount = 0;
			using StreamWriter sw = new(siiFile);
			WriteFileHeader(sw);
			BraceIn(sw);
			WriteAAHeader(sw, accAddonItem.ModelName, truck.TruckID, truck.ModelType);
			BraceIn(sw);
			WriteLine(sw, NameDisplayName, accAddonItem.DisplayName);
			WriteLine(sw, NamePrice, accAddonItem.Price);
			WriteLine(sw, NameUnlock, accAddonItem.UnlockLevel);
			if (accAddonItem.PartType != "unknown")
				WriteLine(sw, NamePartType, accAddonItem.PartType);
			WriteLine(sw, NameIconName, accAddonItem.IconName);
			WriteLine(sw, NameAAExtModel, exterior ?? accAddonItem.ModelPath);
			WriteLine(sw, NameAAIntModel, accAddonItem.ModelPath);
			if (isETS2 && accAddonItem.ModelPathUK.Length > 0) {
				WriteLine(sw, NameAAExtModelUK, exteriorUK ?? accAddonItem.ModelPathUK);
				WriteLine(sw, NameAAIntModelUK, accAddonItem.ModelPathUK);
			}
			WriteLine(sw, NameCollPath, accAddonItem.CollPath);
			if (truck.Look != "default")
				WriteLine(sw, NameLook, truck.Look);
			if (truck.Variant != "default")
				WriteLine(sw, NameVariant, truck.Variant);
			if (accAddonItem.HideIn != "" && accAddonItem.HideIn != "0x0")
				WriteLine(sw, NameAAHideIn, accAddonItem.HideIn);

			List<string> physName = [];
			WriteOthers(sw, othersList, physName);
			BraceOut(sw);

			foreach (var pn in physName) {
				var physicsList = accAddonItem.PhysicsList;
				for (int i = 0; i < physicsList.Count; i++) {
					var phys = physicsList[i];
					if (pn == phys.PhysicsName) {
						WritePhysicsToyData(sw, phys);
						physicsList.RemoveAt(i);
						break;
					}
				}
			}

			BraceOut(sw);
			numberCreated++;
		}
		return numberCreated;
	}

	private static void WriteOthers(StreamWriter sw, List<OthersItem> othersList, List<string> physName) {
		if (othersList.Count > 0) {
			int i = 0;
			while (i < othersList.Count) {
				var other = othersList[i];
				var name = other.OthersName;
				var value = other.OthersValue;
				if (IsUnseless(name, value)) {
					othersList.RemoveAt(i);
					continue;
				}
				int j = i + 1;
				while (j < othersList.Count) {
					var another = othersList[j];
					var nextName = another.OthersName;
					var nextValue = another.OthersValue;
					if (IsUnseless(nextName, nextValue) || (name == nextName && value == nextValue)) {
						othersList.RemoveAt(j);
						continue;
					}
					j++;
				}
				if (name == NameData) {
					if (value.EndsWith(NamePTSuffix)) {
						physName.Add(value[..^NamePTSuffix.Length]);
					} else {
						physName.Add(value);
						value += NamePTSuffix;
					}
				}
				WriteLine(sw, name, value, other.IsArray, other.UseQuoteMark);
				i++;
			}
		}
	}

	private static bool IsUnseless(string name, string value) {
		if (name.Length == 0 || value.Length == 0)
			return true;
		return name switch {
			NameDisplayName or 
			NamePrice or 
			NameUnlock or 
			NameIconName or 
			NamePartType or 
			NameAAExtModel or 
			NameAAIntModel or 
			NameAAExtModelUK or 
			NameAAIntModelUK or 
			NameCollPath or 
			NameLook or 
			NameVariant or 
			NameAAHideIn => true,
			_ => false,
		};
	}

	private readonly static string exportPath = Instances.ProjectLocation;//"D:\\test";
	public static void SaveAddonHookup(AccHookupViewModel viewModel) {
		MessageBox.Show(Util.GetString("MessageSaveBeforeStart"));
		if (viewModel.StorageName.Length == 0) {
			MessageBox.Show(Util.GetString("MessageSaveNoName"));
			return;
		}
		if (viewModel.SuiItems.Count == 0) {
			MessageBox.Show(Util.GetString("MessageSaveSui0"));
			return;
		}
		var StorageDir = Paths.HookupStorageDir(exportPath);
		Directory.CreateDirectory(StorageDir);
		var storageFilename = $"{NameAHSPreffix}.{viewModel.StorageName}.sii";
		var storageFile = Path.Combine(StorageDir, storageFilename);
		{
			using StreamWriter sw = new(storageFile);
			WriteFileHeader(sw);
			BraceIn(sw);
			WriteStorageTips(sw);
			WriteInclude(sw, viewModel);
			BraceOut(sw);
		}
		MessageBox.Show(Util.GetString("MessageSaved"));
	}

	private static void WriteStorageTips(StreamWriter sw) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# For modders: Please do not modify this file if you want to add a new entry. Create in");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# this directory a new file \"<base_name>.<idofyourmod>.sii\" where <base_name> is name of");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# base file without the extension (e.g. \"city\" for \"/def/city.sii\") and <idofyourmod> is");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# some string which is unlikely to conflict with other mod.");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("#");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# Warning: Even if the units are specified in more than one source file, they share the");
		sw.Write(new string('\t', TabCount));
		sw.WriteLine("# same namespace so suffixes or prefixes should be used to avoid conflicts.");
		sw.WriteLine("");
	}
	private static void WriteInclude(StreamWriter sw, AccHookupViewModel viewModel) {
		string includeFormat = "@include \"addon_hookups/{0}.sui\"";
		foreach (var sui in viewModel.SuiItems) {
			if (sui.SuiFilename.Length == 0 || (sui.HookupItems.Count == 0 && sui.PhysicsItems.Count == 0))
				continue;
			sw.Write(new string('\t', TabCount));
			sw.WriteLine(string.Format(includeFormat, sui.SuiFilename));

			var cTab = TabCount;
			CreateAddonHookupSui(sui);
			TabCount = cTab;
		}
	}

	public static void CreateAddonHookupSui(SuiItem sui) {
		var suiPath = Paths.AddonHookupsDir(exportPath, sui.SuiFilename);
		var physicsDatas = new List<PhysicsToyData>();
		physicsDatas.AddRange(sui.PhysicsItems);
		using StreamWriter sw = new(suiPath);
		foreach (var hookup in sui.HookupItems) {
			WriteAHHeader(sw, hookup.ModelName);
			BraceIn(sw);
			WriteLine(sw, NameDisplayName, hookup.DisplayName);
			WriteLine(sw, NamePrice, hookup.Price);
			WriteLine(sw, NameUnlock, hookup.UnlockLevel);
			WriteLine(sw, NameIconName, hookup.IconName);
			if (hookup.PartType != "unknown")
				WriteLine(sw, NamePartType, hookup.PartType);
			WriteLine(sw, NameAHModel, hookup.ModelPath);
			WriteLine(sw, NameCollPath, hookup.CollPath);
			if (hookup.Look != "default")
				WriteLine(sw, NameLook, hookup.Look);
			if (hookup.Variant != "default")
				WriteLine(sw, NameVariant, hookup.Variant);

			List<string> physName = [];
			var othersList = new List<OthersItem>();
			othersList.AddRange(hookup.OthersList);
			WriteOthers(sw, othersList, physName);
			BraceOut(sw);

			foreach (var pn in physName) {
				for (int i = 0; i < physicsDatas.Count; i++) {
					var phys = physicsDatas[i];
					if (pn == phys.PhysicsName) {
						WritePhysicsToyData(sw, phys);
						physicsDatas.RemoveAt(i);
						break;
					}
				}
			}
		}
	}

	private static void WritePhysicsToyData(StreamWriter sw, PhysicsToyData phys) {
		if (phys.ModelPath == null)
			return;
		WritePTHeader(sw, phys.PhysicsName);
		BraceIn(sw);
		WriteLine(sw, NamePTModel, phys.ModelPath);
		WriteLine(sw, NamePTColl, phys.CollPath);
		WriteLine(sw, NamePTLook, phys.Look);
		WriteLine(sw, NamePTVariant, phys.Variant);
		WriteEmptyLine(sw);
		WriteLine(sw, NamePTToyType, phys.ToyType);
		WriteLine(sw, NamePTMass, phys.Mass);
		WriteLine(sw, NamePTCogOffset, phys.CogOffset);
		WriteLine(sw, NamePTLinearStiffness, phys.LinearStiffness);
		WriteLine(sw, NamePTLinearDamping, phys.LinearDamping);
		WriteLine(sw, NamePTLocatorHookOffset, phys.LocatorHookOffset);
		WriteLine(sw, NamePTRestPositionOffset, phys.RestPositionOffset);
		WriteLine(sw, NamePTRestRotationOffset, phys.RestRotationOffset);
		foreach (var offset in phys.InstanceOffsetList) {
			WriteLine(sw, NamePTInstanceOffset, offset);
		}
		WriteEmptyLine(sw);
		WriteLine(sw, NamePTAngularStiffness, phys.AngularStiffness);
		WriteLine(sw, NamePTAngularDamping, phys.AngularDamping);
		WriteLine(sw, NamePTAngularAmplitude, phys.AngularAmplitude);
		WriteEmptyLine(sw);
		WriteLine(sw, NamePTRopeMaterial, phys.RopeMaterial);
		WriteLine(sw, NamePTRopeWidth, phys.RopeWidth);
		WriteLine(sw, NamePTRopeLength, phys.RopeLength);
		WriteLine(sw, NamePTRopeHookOffset, phys.RopeHookOffset);
		WriteLine(sw, NamePTRopeToyOffset, phys.RopeToyOffset);
		WriteLine(sw, NamePTRopeResolution, phys.RopeResolution);
		WriteLine(sw, NamePTRopeLinearDensity, phys.RopeLinearDensity);
		WriteLine(sw, NamePTPositionIterations, phys.PositionIterations);
		WriteLine(sw, NamePTNodeDamping, phys.NodeDamping);
		BraceOut(sw);
	}

	public static void LoadAddonHookup(AccHookupViewModel viewModel) {
		DirectoryInfo accHookupDir = new(Paths.HookupStorageDir(Instances.ModelProject.ProjectLocation));
		FileInfo? storageFile = null;
		bool skip = true;
		foreach (var file in accHookupDir.GetFiles()) {
			var filename = file.Name;
			if (filename.StartsWith(NameAHSPreffix) && filename.EndsWith(".sii")) {
				storageFile = file;
				if (skip) {
					skip = false;
					continue;
				}
				break;
			}
		}
		if (storageFile == null)
			return;
		viewModel.StorageName = storageFile.Name[21..^4];
		{
			using StreamReader sReader = new(storageFile.FullName);
			var line = sReader.ReadLine()?.Trim();
			if (line == null || line != FileHeader)
				return;
			while ((line = sReader.ReadLine()?.Trim()) != null) {
				if (line.StartsWith("@include")) {
					var start = line.IndexOf('"');
					var end = line.LastIndexOf('"');
					var suiPath = line[(start + 1)..end];
					suiPath = suiPath.Replace('/', '\\');
					var hookupFile = new FileInfo(Path.Combine(accHookupDir.FullName, suiPath));
					var suiFilename = hookupFile.Name;
					SuiItem suiItem = new(suiFilename[..^4]);
					LoadAddonHookupSui(suiItem, hookupFile);
					viewModel.SuiItems.Add(suiItem);
				}
			}
			viewModel.CurrentSuiItem = viewModel.SuiItems.FirstOrDefault();
		}
	}

	private static void LoadAddonHookupSui(SuiItem sui, FileInfo hookupFile) {
		if (!hookupFile.Exists) return;
		using StreamReader sr = new(hookupFile.FullName);
		string? line = null;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line.Length == 0) continue;
			if (line.StartsWith("accessory_hookup")) {
				var name = line.Split(":")[1].Trim();
				name = name[..name.LastIndexOf('.')];
				AccessoryHookupItem item = new(name);
				ReadAccHookup(sr, item);
				sui.HookupItems.Add(item);
			} else if (line.StartsWith(NamePTHeader)) {
				var name = line.Split(":")[1].Trim();
				name = name[..name.LastIndexOf('.')];
				PhysicsToyData data = new(name);
				ReadPhysData(sr, data);
				sui.PhysicsItems.Add(data);
			}
		}
	}

	private static void ReadAccHookup(StreamReader sr, AccessoryHookupItem item) {
		string? line;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line == "}")
				break;
			if (line == "{" || line.Length == 0 || line.StartsWith('#'))
				continue;
			int colonIndex = line.IndexOf(':');
			var name = line[..colonIndex].Trim();
			var value = ClipValue(line[(colonIndex + 1)..]);
			switch (name) {
				case NameDisplayName:
					item.DisplayName = value;
					break;
				case NameIconName:
					item.IconName = value;
					break;
				case NamePrice:
					item.Price = long.Parse(value);
					break;
				case NameUnlock:
					item.UnlockLevel = uint.Parse(value);
					break;
				case NamePartType:
					item.PartType = value;
					break;
				case NameAHModel:
					item.ModelPath = value;
					break;
				case NameCollPath:
					item.CollPath = value;
					break;
				case NameLook:
					item.Look = value;
					break;
				case NameVariant:
					item.Variant = value;
					break;
				default:
					if (name.Contains("[]")) 
						name = name.Replace("[]", "");
					item.OthersList.Add(new(name, value));
					break;
			}
		}
	}

	private static void ReadPhysData(StreamReader sr, PhysicsToyData data) {
		string? line;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line == "}")
				break;
			if (line == "{" || line.Length == 0 || line.StartsWith('#'))
				continue;
			int colonIndex = line.IndexOf(':');
			var name = line[..colonIndex].Trim();
			var value = ClipValue(line[(colonIndex + 1)..]);
			switch(name) {
				case NamePTModel:
					data.ModelPath = value;
					break;
				case NamePTColl:
					data.CollPath = value; 
					break;
				case NamePTLook:
					data.Look = value;
					break;
				case NamePTVariant:
					data.Variant = value;
					break;
				case NamePTToyType:
					data.ToyType = value;
					break;
				case NamePTMass:
					data.Mass = float.Parse(value);
					break;
				case NamePTCogOffset:
					FloatParse(value, (i, v) => data.CogOffset[i] = v);
					break;
				case NamePTLinearStiffness:
					data.LinearStiffness = float.Parse(value);
					break;
				case NamePTLinearDamping:
					data.LinearDamping = float.Parse(value);
					break;
				case NamePTAngularStiffness:
					FloatParse(value, (i, v) => data.AngularStiffness[i] = v);
					break;
				case NamePTAngularDamping:
					FloatParse(value, (i, v) => data.AngularDamping[i] = v);
					break;
				case NamePTAngularAmplitude:
					FloatParse(value, (i, v) => data.AngularAmplitude[i] = v);
					break;
				case NamePTNodeDamping:
					data.NodeDamping = float.Parse(value);
					break;
				case NamePTLocatorHookOffset:
					FloatParse(value, (i, v) => data.LocatorHookOffset[i] = v);
					break;
				case NamePTRestPositionOffset:
					FloatParse(value, (i, v) => data.RestPositionOffset[i] = v);
					break;
				case NamePTRestRotationOffset:
					FloatParse(value, (i, v) => data.RestRotationOffset[i] = v);
					break;
				case NamePTInstanceOffset:
					float[] nf = new float[3];
					FloatParse(value, (i, v) => nf[i] = v);
					data.InstanceOffsetList.Add(nf);
					break;
				case NamePTRopeWidth:
					data.RopeWidth = float.Parse(value);
					break;
				case NamePTRopeLength:
					data.RopeLength = float.Parse(value);
					break;
				case NamePTRopeHookOffset:
					data.RopeHookOffset = float.Parse(value);
					break;
				case NamePTRopeToyOffset:
					data.RopeToyOffset = float.Parse(value);
					break;
				case NamePTRopeResolution:
					data.RopeResolution = uint.Parse(value);
					break;
				case NamePTRopeLinearDensity:
					data.RopeLinearDensity = float.Parse(value);
					break;
				case NamePTPositionIterations:
					data.PositionIterations = uint.Parse(value);
					break;
				case NamePTRopeMaterial:
					data.RopeMaterial = value;
					break;
			}
		}
	}

	private static void FloatParse(string value, Action<int, float> setter) {
		value = value[1..^1];
		var split = value.Split(',');
		for (int i = 0; i < split.Length; i++) {
			if (split[i].EndsWith('f'))
				split[i] = split[i][..^1];
			float f;
			try { f = float.Parse(split[i]); } catch {f = 0.0f;}
			setter(i, f);
		}
	}

	private static string ClipValue(string value) {
		if (value.Contains('#'))//部分行会有注释
			value = value[..value.IndexOf('#')];
		if (value.Contains('"')) {
			value = value[(value.IndexOf('"') + 1)..value.LastIndexOf('"')];
		} else
			value = value.Trim();
		if (value.EndsWith('{'))
			value = value[..^1].Trim();
		return value;
	}

	private const string NameLocHeader = "localization_db : .localization";
	private const string DictPreffix = "local_module";
	private const string NameKey = "key";
	private const string NameValue = "val";
	private const string KeyGenerated = "Generated";


	public static void ReadLocaleDict(Window window, ObservableCollection<LocaleModule> moduleList) {
		var localeDir = new DirectoryInfo(Paths.LocaleDir(Instances.ProjectLocation));
		if (!localeDir.Exists)
			return;
		foreach (var dir in localeDir.GetDirectories()) {//地区文件夹
			foreach (var file in dir.GetFiles()) {
				if (file.Name.StartsWith(DictPreffix)) {
					try {
						string moduleName = GetContent(file.Name, '.');
						LocaleModule? module = null;
						foreach (var item in moduleList) {
							if (item.ModuleName == moduleName)
								module = item;
						}
						if (module == null) {
							module = new(moduleName);
							moduleList.Add(module);
						}
						var lang = dir.Name;
						var langDict = module.GetLocale(lang)!;
						using StreamReader sr = new(file.FullName);
						string? line = sr.ReadLine()?.Trim();
						if (line == null || line != FileHeader)
							throw new(Util.GetString("MessageLoadErrNotLocale"));
						string? stringKey = null;
						bool copyToUni = false;
						while ((line = sr.ReadLine()?.Trim()) != null) {
							if (line.Length == 0 || line == "{" || line.StartsWith("localization"))
								continue;
							if (line == "}")
								break;
							if (line.StartsWith(NameKey)) {
								stringKey = GetContent(line);
							} else if (line.StartsWith(NameValue)) {
								var stringValue = GetContent(line);
								if (stringKey == KeyGenerated) {
									if (stringValue == bool.TrueString) {
										if (module.UniversalDict.Count == 0)
											copyToUni = true;
										else {
											langDict.ClearDict();
											break;
										}
									}
								} else if (stringKey != null) 
									langDict.AddPair(stringKey, stringValue);
								stringKey = null;


							}
						}
						if (copyToUni) {
							foreach (var p2 in langDict.Dictionary) {
								module.UniversalDict.Add(new(p2.Key, p2.Value));
							}
							langDict.ClearDict();
						}
					} catch (Exception ex) {
						MessageBox.Show(window, Util.GetString("MessageLoadErr") + "\n" + ex.Message);
					}
				}
			}
		}
	}

	public static void CheckLocaleDict(Dictionary<string, List<string>> localeDict) {
		var localeDir = new DirectoryInfo(Paths.LocaleDir(Instances.ProjectLocation));
		if (!localeDir.Exists)
			return;
		List<string> generatedRead = [];
		foreach (var dir in localeDir.GetDirectories()) {
			foreach (var file in dir.GetFiles()) {
				if (file.Name.StartsWith(DictPreffix)) {
					try {
						using StreamReader sr = new(file.FullName);
						string? line = sr.ReadLine()?.Trim();
						if (line == null || line != FileHeader)
							continue;
						string? stringKey = null;
						while ((line = sr.ReadLine()?.Trim()) != null) {
							if (line.Length == 0 || line == "{" || line.StartsWith("localization"))
								continue;
							if (line == "}")
								break;
							if (line.StartsWith(NameKey)) {
								stringKey = GetContent(line);
							} else if (line.StartsWith(NameValue)) {
								var stringValue = GetContent(line);
								if (stringKey == KeyGenerated) {
									if (stringValue == bool.TrueString) {
										if (generatedRead.Contains(file.Name)) {
											break;
										} else
											generatedRead.Add(file.Name);
									}
								} else if (stringKey != null) {
									List<string> valueDict;
									if (localeDict.TryGetValue(stringKey, out List<string>? dict)) {
										valueDict = dict;
									} else {
										valueDict = [];
										localeDict.Add(stringKey, valueDict);
									}
									if (!valueDict.Contains(stringValue))
										valueDict.Add(stringValue);
									stringKey = null;
								}
							}
						}

					} catch (Exception) {
					}
				}
			}
		}
	}

	private static string GetContent(string line, char indexValue = '"') {
		var start = line.IndexOf(indexValue);
		var end = line.LastIndexOf(indexValue);
		return line[(start + 1)..end];
	}

	public static void SaveLocaleDict(string projectLocation, ObservableCollection<LocaleModule> moduleList, ObservableCollection<LocaleModule> deletedModuleList) {
		foreach (var module in moduleList) {
			if (module.ModuleName.Length == 0) {
				MessageBox.Show(Util.GetString("MessageSaveErrNoName"));
				return;
			}
		}
		Instances.LocaleDictReset(projectLocation);
		foreach (var module in moduleList) {
			var moduleName = module.ModuleName;
			var universal = module.UniversalDict;
			foreach (var locale in module.LocaleList) {
				if (locale.LocaleValue == Locale.LocaleValueUni) {
					foreach (var pair in locale.Dictionary) {
						Instances.LocaleDictAdd(pair.Key, pair.Value);
					}
					continue;
				}
				CreateLocaleSii(moduleName, locale, universal);
			}
		}
		foreach (var module in deletedModuleList) {
			DeleteLocaleSii(module);
		}
	}

	private static void CreateLocaleSii(string moduleName, ModLocale locale, ObservableCollection<LocalePair> universal) {
		TabCount = 0;
		ObservableCollection<LocalePair> dict;
		bool Genearated = false;
		var localeFile = Paths.LocaleFile(Instances.ProjectLocation, locale.LocaleValue, moduleName);
		if (locale.Dictionary.Count != 0)
			dict = locale.Dictionary;
		else if (universal.Count == 0) {
			File.Delete(localeFile);
			var parent = Directory.GetParent(localeFile);
			if (parent != null && parent.GetFiles().Length == 0)
				parent.Delete();
			return;
		} else {
			dict = universal;
			Genearated = true;
		}
		var hasValue = false;
		{
			using StreamWriter sw = new(localeFile);
			WriteFileHeader(sw);
			BraceIn(sw);
			WriteLoHeader(sw);
			BraceIn(sw);
			WriteLine(sw, NameKey, KeyGenerated);
			WriteLine(sw, NameValue, Genearated);
			foreach (var pair in dict) {
				if (pair.Key.Length == 0)
					continue;
				WriteEmptyLine(sw);
				if (!hasValue){
					hasValue = true;
				}
				WriteLine(sw, NameKey, pair.Key);
				WriteLine(sw, NameValue, pair.Value);
				if (!Genearated)
					Instances.LocaleDictAdd(pair.Key, pair.Value);
			}
			BraceOut(sw);
			BraceOut(sw);
		}
		if (!hasValue) {
			File.Delete(localeFile);
		}
	}

	private static void DeleteLocaleSii(LocaleModule module) {
		var moduleName = module.ModuleName;
		foreach (var locale in module.LocaleList) {
			var localeFile = Paths.LocaleFile(Instances.ProjectLocation, locale.LocaleValue, moduleName, false);
			File.Delete(localeFile);
			var parent = Directory.GetParent(localeFile)!;
			if (parent.GetFiles().Length == 0)
				parent.Delete();
		}
		var localeDir = new DirectoryInfo(Paths.LocaleDir(Instances.ProjectLocation));
		if (localeDir.GetDirectories().Length == 0 && localeDir.GetFiles().Length == 0)
			localeDir.Delete();
	}

	private static void WriteLoHeader(StreamWriter sw) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine(NameLocHeader);
	}




	public static void ReadLookAndVariant(string pitFile, ObservableCollection<string> LookList, ObservableCollection<string> VariantList) {
		if (File.Exists(pitFile)) {
			using StreamReader sr = new(pitFile);
			string? line;
			do {
				line = sr.ReadLine()?.Trim();
			} while (string.IsNullOrEmpty(line));
			line = line.Trim();
			if (line.StartsWith('#')) {
				do {
					line = line[1..].Trim();
					if (line == "Look Names:")
						ReadNames(sr, LookList);
					else if (line == "Variant Names:")
						ReadNames(sr, VariantList);
				} while ((line = sr.ReadLine()?.Trim()) != null && line.Contains('#'));
			} else {
				int variantCount = -1;
				int variantFound = 0;
				while ((line = sr.ReadLine()?.Trim()) != null) {
					if (line.StartsWith("VariantCount:", StringComparison.OrdinalIgnoreCase)) {
						variantCount = int.Parse(line["VariantCount:".Length..].Trim());
					} else if (line.StartsWith("Look {", StringComparison.OrdinalIgnoreCase)) {
						ReadNamesAlt(sr, LookList);
					} else if (line.StartsWith("Variant {", StringComparison.OrdinalIgnoreCase)) {
						if (ReadNamesAlt(sr, VariantList)) {
							variantFound++;
							if (variantCount > 0 && variantFound >= variantCount)
								break;
						}
					}
				}
			}
		}
	}

	private static void ReadNames(StreamReader sr, ObservableCollection<string> list) {
		string? line;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line.StartsWith('#')) {
				line = line[1..].Trim();
				if (line.Length > 0)
					list.Add(line);
				else
					break;
			} else {
				break;
			}
		}
	}

	private static bool ReadNamesAlt(StreamReader sr, ObservableCollection<string> list) {
		string? line;
		if ((line = sr.ReadLine()?.Trim()) != null && line.Contains("Name:", StringComparison.OrdinalIgnoreCase)) {
			int start = line.IndexOf('\"');
			int end = line.LastIndexOf('\"');
			var look = line.Substring(start + 1, end - start);
			list.Add(look);
			return true;
		}
		return false;
	}
}