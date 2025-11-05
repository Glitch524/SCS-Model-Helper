using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.CreatedSii;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SCS_Mod_Helper.Accessory;

class AccDataIO {

	private const string FileHeader = "SiiNunit";
	private static int TabCount = 0;

	private static void WriteFileHeader(StreamWriter sw) => sw.WriteLine(FileHeader);
	private static void WriteMFHeader(StreamWriter sw) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine(NameMFHeader);
	}

	private static void WriteLine(StreamWriter sw, string name, object? valueObj, object? skipValue = null) {
		if (valueObj == skipValue || valueObj == null)
			return;
		if (valueObj is string s && s.Length == 0)
			return;
		var isArray = IsArray(name);
		var hasQuote = HasQuote(name);

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

	private static void BraceIn(StreamWriter sw) {//将括号下的内容缩进
		sw.Write(new string('\t', TabCount));
		sw.WriteLine('{');
		TabCount++;
	}
	private static void BraceOut(StreamWriter sw) {
		TabCount--;
		if (TabCount < 0)
			TabCount = 0;
		sw.Write(new string('\t', TabCount));
		sw.WriteLine('}');
	}
	private static void WriteEmptyLine(StreamWriter sw) => sw.WriteLine("");

	public static bool IsArray(string name) => name switch {//确定这个Key是否为列表，如果是，这个key要在后面添加[]
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

	public static bool HasQuote(string name) => name switch {//确定这个Key的值是否需要双引号，如果是，这个值要添加双引号
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
		NamePPMaterial or
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
	public static void SaveManifest(ManifestBinding binding) {
		if (Util.IsEmpty(binding.ProjectLocation, binding.ModDisplayName, binding.DescriptionName)) {
			MessageBox.Show(Util.GetString("MessageManifestNotFilled"));
			return;
		}


		var saveLocation = binding.ProjectLocation;
		if (!binding.IconName.EndsWith(".jpg"))
			binding.IconName += ".jpg";

		if (binding.OldIconName != null) {
			var oldIconFile = Path.Combine(binding.ProjectLocation, binding.OldIconName);
			if (File.Exists(oldIconFile))
				File.Delete(oldIconFile);
		}
		if (binding.ModIcon == null) {
			binding.ModIcon = new BitmapImage(new("pack://application:,,,/Images/IconPlaceholder.png"));
			binding.NewIcon = true;
		}
		var iconFile = Path.Combine(saveLocation, binding.IconName);
		if (binding.NewIcon || !File.Exists(iconFile)) {
			JpegBitmapEncoder encoder = new();
			encoder.Frames.Add(BitmapFrame.Create(binding.ModIcon));
			if (File.Exists(iconFile))
				File.Delete(iconFile);
			using FileStream fs = new(iconFile, FileMode.CreateNew, FileAccess.ReadWrite);
			encoder.Save(fs);
		}

		if (binding.OldDescriptionName != null && binding.DescriptionName != binding.OldDescriptionName) {
			var descDeExt = binding.OldDescriptionName[..^4];
			foreach (var file in new DirectoryInfo(binding.ProjectLocation).GetFiles()) {
				var name = file.Name;
				if (name.StartsWith(descDeExt) && name.EndsWith(".txt")) {
					file.Delete();
				}
			}
		}
		var deExt = binding.DescriptionName[..^4];
		foreach (var locale in binding.Locales) {
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
		WriteLine(sw, NameMFPackageVersion, binding.Version);
		WriteLine(sw, NameMFDisplayName, binding.ModDisplayName);
		WriteLine(sw, NameMFAuthor, binding.Author);

		foreach (var cat in binding.SelectedCategories) {
			WriteLine(sw, NameMFCategory, cat);
		}
		WriteLine(sw, NameMFIcon, binding.IconName);
		WriteLine(sw, NameMFDescriptionFile, binding.DescriptionName);
		WriteLine(sw, NameMFMPOptional, binding.MPOptional.ToString().ToLower());
		BraceOut(sw);
		BraceOut(sw);
	}

	public static void LoadManifest(ManifestBinding binding) {
		var manifest = Paths.ManifestFile(binding.ProjectLocation);
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
						binding.Version = value;
						break;
					case NameMFDisplayName:
						binding.ModDisplayName = value;
						break;
					case NameMFAuthor:
						binding.Author = value;
						break;
					case NameMFCategory:
						binding.SelectedCategories.Add(value);
						break;
					case NameMFIcon:
						binding.IconName = value;
						binding.OldIconName = value;
						var iconFile = Path.Combine(binding.ProjectLocation, binding.IconName);
						if (File.Exists(iconFile)) 
							binding.ModIcon = Util.LoadIcon(iconFile);
						break;
					case NameMFDescriptionFile:
						binding.DescriptionName = value;
						binding.OldDescriptionName = value;
						LoadDescription(binding);
						break;
					case NameMFMPOptional:
						binding.MPOptional = bool.Parse(value);
						break;
				}
			}
		} catch (Exception ex) {
			MessageBox.Show(Util.GetString("MessageLoadManifestErrFail") + "\n" + ex.Message);
		}
	}

	private static void LoadDescription(ManifestBinding binding) {
		var descriptionFile = new FileInfo(Path.Combine(binding.ProjectLocation, binding.DescriptionName));
		var ext = descriptionFile.Extension;
		var deExt = binding.DescriptionName[..^(ext.Length - 1)];
		DirectoryInfo project = new(binding.ProjectLocation);
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
				var descLocale = binding.LocaleDict[locale];
				descLocale.DescContent = sb.ToString();
			}
		}
		binding.CurrentLocale = binding.LocaleDict[Locale.LocaleValueUni];
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
	private const string NameAAIHeader = "accessory_addon_int_data";
	private const string NameAAPHeader = "accessory_addon_patch_data";
	private const string NameAAExtModel = "exterior_model";
	private const string NameAAIntModel = "interior_model";
	private const string NameAAExtModelUK = "exterior_model_uk";
	private const string NameAAIntModelUK = "interior_model_uk";
	private const string NameAAHideIn = "hide_in";
	private const string NameElectricType = "electric_type";
	private static void WriteAAIHeader(StreamWriter sw, string modelName, string truckID, string modelType) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NameAAIHeader} : {modelName}.{truckID}.{modelType}");
	}
	private static void WriteAAPHeader(StreamWriter sw, string modelName, string truckID, string modelType) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NameAAPHeader} : {modelName}.{truckID}.{modelType}");
	}
	//accessory hookup
	private const string NameAHHeader = "accessory_hookup_int_data";
	private const string NameAHSuffix = ".addon_hookup";
	private const string NameAHModel = "model";
	private static void WriteAHHeader(StreamWriter sw, string hookupName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NameAHHeader} : {hookupName + NameAHSuffix}");
	}

	//others
	public const string NameData = "data";
	public const string NameSuitableFor = "suitable_for";
	public const string NameConflictWith = "conflict_with";
	public const string NameDefaults = "defaults";
	public const string NameOverrides = "overrides";
	public const string NameRequire = "require";


	public static int CreateAccAddonSii(AccAddonBinding binding) {
		var created = 0;
		if (binding.TruckExpandedETS2)
			created += CreateSii(binding, true);
		if (binding.TruckExpandedATS)
			created += CreateSii(binding, false);
		return created;
	}
	private static int CreateSii(AccAddonBinding binding, bool isETS2) {
		int numberCreated = 0;
		foreach (var truck in isETS2 ? binding.TrucksETS2 : binding.TrucksATS) {
			var siiFile = Paths.SiiFile(Instances.ProjectLocation, truck.TruckID, truck.ModelType, binding.ModelName);
			if (truck.Check) {
				if (truck.ModelType.Length == 0 || truck.TruckID.Length == 0 || truck.Look.Length == 0 || truck.Variant.Length == 0)
					continue;
			} else {
				if (binding.DeleteUnchecked && File.Exists(siiFile))
					File.Delete(siiFile);
				continue;
			}
			DirectoryInfo sii = new(siiFile);
			if (!sii.Parent!.Exists)
				sii.Parent!.Create();
			TabCount = 0;
			bool isPatch = truck.ModelType switch {
				"flag_l" or
				"flag_r" or
				"flag_f_l" or
				"flag_f_r" => true,
				_ => false,
			};
			using StreamWriter sw = new(siiFile);
			WriteFileHeader(sw);
			BraceIn(sw);
			if (isPatch) {
				WriteAAPHeader(sw, binding.ModelName, truck.TruckID, truck.ModelType);
			} else {
				WriteAAIHeader(sw, binding.ModelName, truck.TruckID, truck.ModelType);
			}
			BraceIn(sw);
			WriteLine(sw, NameDisplayName, binding.DisplayName);
			WriteLine(sw, NamePrice, binding.Price);
			WriteLine(sw, NameUnlock, binding.UnlockLevel);
			WriteLine(sw, NamePartType, binding.PartType, "unknown");
			WriteLine(sw, NameIconName, binding.IconName);
			if (binding.ExtModelPath != null && binding.ExtModelPath.Length > 0)
				WriteLine(sw, NameAAExtModel, binding.ExtModelPath);
			else
				WriteLine(sw, NameAAExtModel, binding.ModelPath);
			WriteLine(sw, NameAAIntModel, binding.ModelPath);
			if (isETS2 && !string.IsNullOrEmpty(binding.ModelPathUK)) {
				if (binding.ExtModelPathUK != null && binding.ExtModelPathUK.Length > 0)
					WriteLine(sw, NameAAExtModelUK, binding.ExtModelPathUK);
				else
					WriteLine(sw, NameAAExtModelUK, binding.ModelPath);
				WriteLine(sw, NameAAIntModelUK, binding.ModelPathUK);
			}
			WriteLine(sw, NameCollPath, binding.CollPath);
			WriteLine(sw, NameLook, truck.Look, "default");
			WriteLine(sw, NameVariant, truck.Variant, "default");
			WriteLine(sw, NameAAHideIn, binding.HideIn, 0);
			WriteLine(sw, NameElectricType, binding.ElectricType, "vehicle");

			WriteList(sw, NameData, binding.Data);
			WriteList(sw, NameSuitableFor, binding.SuitableFor);
			WriteList(sw, NameConflictWith, binding.ConflictWith);
			WriteList(sw, NameDefaults, binding.Defaults);
			WriteList(sw, NameOverrides, binding.Overrides);
			WriteList(sw, NameRequire, binding.Require);

			BraceOut(sw);

			for (int i = 0; i < binding.Data.Count; i++) {
				string? pn = binding.Data[i];
				pn = pn.EndsWith(NamePSuffix) ? pn[..^NamePSuffix.Length] : pn;

				var physicsList = new List<PhysicsData>();
				physicsList.AddRange(binding.PhysicsList);
				for (int j = 0; j < physicsList.Count; j++) {
					var phys = physicsList[j];
					if (pn == phys.PhysicsName) {
						WritePhysicsData(sw, phys);
						physicsList.RemoveAt(j);
						break;
					}
				}
				foreach (var physItem in AccAppIO.PhysicsItems) {
					if (pn == physItem.PhysicsName) {
						WritePhysicsData(sw, physItem);
						break;
					}
				}
			}

			BraceOut(sw);
			numberCreated++;
		}
		return numberCreated;
	}

	private static void WriteList(StreamWriter sw, string listName, ObservableCollection<string> list) {
		for (int i = 0; i < list.Count; i++) {
			string? item = list[i];
			if (item.Length == 0)
				continue;
			if (listName == NameData && !item.EndsWith(NamePSuffix))
				item += NamePSuffix;
			WriteLine(sw, listName, item);
		}
	}

	public static void ReadAccAddon(CreatedModelItem item) {
		if (File.Exists(item.Path)) {
			using StreamReader sr = new(item.Path);
			string? line;
			while ((line = sr.ReadLine()?.Trim()) != null) {
				if (line == "}")
					break;
				if (line == "{" || line == "SiiNunit" || line.Length == 0 || line.StartsWith('#'))
					continue;
				int colonIndex = line.IndexOf(':');
				var name = line[..colonIndex].Trim();
				var value = ClipValue(line[(colonIndex + 1)..]);
				switch(name) {
					case NameDisplayName:
						item.IngameName = value;
						break;
					case NameLook:
						item.Look = value;
						break;
					case NameValue:
						item.Variant = value;
						break;
				}
			}
		}
	}

	private readonly static string exportPath = Instances.ProjectLocation;//"D:\\test";
	public static void SaveAddonHookup(AccHookupBinding viewModel) {
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
			TabCount = 0;
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
		sw.WriteLine("# For modders: Please do not modify this file if you want to add a new entry. Create in");
		sw.WriteLine("# this directory a new file \"<base_name>.<idofyourmod>.sii\" where <base_name> is name of");
		sw.WriteLine("# base file without the extension (e.g. \"city\" for \"/def/city.sii\") and <idofyourmod> is");
		sw.WriteLine("# some string which is unlikely to conflict with other mod.");
		sw.WriteLine("#");
		sw.WriteLine("# Warning: Even if the units are specified in more than one source file, they share the");
		sw.WriteLine("# same namespace so suffixes or prefixes should be used to avoid conflicts.");
		sw.WriteLine("");
	}
	private static void WriteInclude(StreamWriter sw, AccHookupBinding viewModel) {
		string includeFormat = "@include \"addon_hookups/{0}.sui\"";
		foreach (var sui in viewModel.SuiItems) {
			if (sui.SuiFilename.Length == 0 || (sui.HookupItems.Count == 0 && sui.PhysicsItems.Count == 0))
				continue;
			sw.WriteLine(string.Format(includeFormat, sui.SuiFilename));

			var cTab = TabCount;//@include的前面不能有空格或制表符，否则会无法读取
			TabCount = 0;
			CreateAddonHookupSui(sui);
			TabCount = cTab;
		}
	}


	//addon hookup storage
	private const string NameAHSPreffix = "addon_hookup_storage";

	public static void CreateAddonHookupSui(SuiItem sui) {
		var suiPath = Paths.AddonHookupsDir(exportPath, sui.SuiFilename);
		var physicsDatas = new List<PhysicsData>();
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
			WriteLine(sw, NameElectricType, hookup.ElectricType);

			WriteList(sw, NameData, hookup.Data);
			WriteList(sw, NameSuitableFor, hookup.SuitableFor);
			WriteList(sw, NameConflictWith, hookup.ConflictWith);
			WriteList(sw, NameDefaults, hookup.Defaults);
			WriteList(sw, NameOverrides, hookup.Overrides);
			WriteList(sw, NameRequire, hookup.Require);

			BraceOut(sw);

			for (int i = 0; i < hookup.Data.Count; i++) {
				string? pn = hookup.Data[i];
				pn = pn.EndsWith(NamePSuffix) ? pn[..^NamePSuffix.Length] : pn;

				for (int j = 0; j < physicsDatas.Count; j++) {
					var phys = physicsDatas[j];
					if (pn == phys.PhysicsName) {

						WritePhysicsData(sw, phys);
						physicsDatas.RemoveAt(j);
						break;
					}
				}
			}
		}
	}

	//physics data
	public const string NamePSuffix = ".phys_data";
	//physics toy data
	private const string NamePTHeader = "physics_toy_data";
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
	private static void WritePTHeader(StreamWriter sw, string physicsName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePTHeader} : {physicsName + NamePSuffix}");
	}

	//physics patch data
	private const string NamePPHeader = "physics_patch_data";
	public const string NamePPMaterial = "material";
	public const string NamePPAreaDensity = "area_density";
	public const string NamePPAeroModelType = "aero_model_type";
	public const string NamePPTCMinFirst = "tc_min_first";
	public const string NamePPTCMaxFirst = "tc_max_first";
	public const string NamePPTCMinSecond = "tc_min_second";
	public const string NamePPTCMaxSecond = "tc_max_second";
	public const string NamePPXRes = "x_res";
	public const string NamePPYRes = "y_res";
	public const string NamePPXSize = "x_size";
	public const string NamePPYSize = "y_size";
	public const string NamePPLinearStiffness = "linear_stiffness";
	public const string NamePPDragCoefficient = "drag_coefficient";
	public const string NamePPLiftCoefficient = "lift_coefficient";
	private static void WritePPHeader(StreamWriter sw, string physicsName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePPHeader} : {physicsName + NamePSuffix}");
	}

	private static void WritePhysicsData(StreamWriter sw, PhysicsData phys) {
		if (phys is PhysicsToyData toyData) {
			if (toyData.ModelPath == null)
				return;
			WritePTHeader(sw, toyData.PhysicsName);
			BraceIn(sw);
			WriteLine(sw, NamePTModel, toyData.ModelPath);
			WriteLine(sw, NamePTColl, toyData.CollPath);
			WriteLine(sw, NamePTLook, toyData.Look);
			WriteLine(sw, NamePTVariant, toyData.Variant);
			WriteEmptyLine(sw);
			WriteLine(sw, NamePTToyType, toyData.ToyType);
			WriteLine(sw, NamePTMass, toyData.Mass);
			WriteLine(sw, NamePTCogOffset, toyData.CogOffset);
			WriteLine(sw, NamePTLinearStiffness, toyData.LinearStiffness);
			WriteLine(sw, NamePTLinearDamping, toyData.LinearDamping);
			WriteLine(sw, NamePTLocatorHookOffset, toyData.LocatorHookOffset);
			WriteLine(sw, NamePTRestPositionOffset, toyData.RestPositionOffset);
			WriteLine(sw, NamePTRestRotationOffset, toyData.RestRotationOffset);
			foreach (var offset in toyData.InstanceOffsetList) {
				WriteLine(sw, NamePTInstanceOffset, offset);
			}
			WriteEmptyLine(sw);
			WriteLine(sw, NamePTAngularStiffness, toyData.AngularStiffness);
			WriteLine(sw, NamePTAngularDamping, toyData.AngularDamping);
			WriteLine(sw, NamePTAngularAmplitude, toyData.AngularAmplitude);
			WriteEmptyLine(sw);
			WriteLine(sw, NamePTRopeMaterial, toyData.RopeMaterial);
			WriteLine(sw, NamePTRopeWidth, toyData.RopeWidth);
			WriteLine(sw, NamePTRopeLength, toyData.RopeLength);
			WriteLine(sw, NamePTRopeHookOffset, toyData.RopeHookOffset);
			WriteLine(sw, NamePTRopeToyOffset, toyData.RopeToyOffset);
			WriteLine(sw, NamePTRopeResolution, toyData.RopeResolution);
			WriteLine(sw, NamePTRopeLinearDensity, toyData.RopeLinearDensity);
			WriteLine(sw, NamePTPositionIterations, toyData.PositionIterations);
			WriteLine(sw, NamePTNodeDamping, toyData.NodeDamping);
			BraceOut(sw);
		} else if (phys is PhysicsPatchData patchData) {
			if (patchData.Material.Length == 0 || patchData.PhysicsName.Length == 0)
				return;
			WritePPHeader(sw, patchData.PhysicsName);
			BraceIn(sw);
			WriteLine(sw, NamePPMaterial, patchData.Material);

			WriteLine(sw, NamePPAreaDensity, patchData.AreaDensity);
			if (patchData.AeroModelType != PhysicsPatchData.ATTwoSideLiftDrag)
				WriteLine(sw, NamePPAeroModelType, patchData.AeroModelType);

			WriteLine(sw, NamePPLinearStiffness, patchData.LinearStiffness);
			WriteLine(sw, NamePPDragCoefficient, patchData.DragCoefficient);
			WriteLine(sw, NamePPLiftCoefficient, patchData.LiftCoefficient);

			WriteLine(sw, NamePPTCMinFirst, patchData.TCMinFirst);
			WriteLine(sw, NamePPTCMaxFirst, patchData.TCMaxFirst);
			WriteLine(sw, NamePPTCMinSecond, patchData.TCMinSecond);
			WriteLine(sw, NamePPTCMaxSecond, patchData.TCMaxSecond);

			WriteLine(sw, NamePPXRes, patchData.XRes);
			WriteLine(sw, NamePPYRes, patchData.YRes);
			WriteLine(sw, NamePPXSize, patchData.XSize);
			WriteLine(sw, NamePPYSize, patchData.YSize);
			BraceOut(sw);
		}
	}

	public static void LoadAddonHookup(AccHookupBinding viewModel) {
		DirectoryInfo accHookupDir = new(Paths.HookupStorageDir(Instances.ProjectLocation));
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

					Application.Current.Dispatcher.Invoke(new(() => {
						viewModel.SuiItems.Add(suiItem); 
					}), DispatcherPriority.DataBind);
				}
			}
			Application.Current.Dispatcher.Invoke(new(() => {
				viewModel.CurrentSuiItem = viewModel.SuiItems.FirstOrDefault();
			}), DispatcherPriority.DataBind);
		}
	}

	private static void LoadAddonHookupSui(SuiItem sui, FileInfo hookupFile) {
		if (!hookupFile.Exists) return;
		using StreamReader sr = new(hookupFile.FullName);
		string? line = null;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line.Length == 0) continue;
			var name = line.Split(":")[1].Trim();
			name = name[..name.LastIndexOf('.')];
			if (line.StartsWith("accessory_hookup")) {
				AccessoryHookupData item = new(name);
				ReadAccHookup(sr, item);
				sui.HookupItems.Add(item);
			} else if (line.StartsWith(NamePTHeader)) {
				PhysicsToyData toyData = new(name);
				ReadPhysToyData(sr, toyData);
				sui.PhysicsItems.Add(toyData);
			} else if (line.StartsWith(NamePPHeader)) {
				PhysicsPatchData patchData = new(name);
				ReadPhysPatchData(sr, patchData);
				sui.PhysicsItems.Add(patchData);

			}
		}
	}

	private static void ReadAccHookup(StreamReader sr, AccessoryHookupData item) {
		string? line;
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line == "}")
				break;
			if (line == "{" || line.Length == 0 || line.StartsWith('#'))
				continue;
			int colonIndex = line.IndexOf(':');
			var name = line[..colonIndex].Trim();
			if (name.EndsWith("[]"))
				name = name[..^2];
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
				case NameData:
					item.Data.Add(value);
					break;
				case NameSuitableFor:
					item.SuitableFor.Add(value);
					break;
				case NameConflictWith:
					item.ConflictWith.Add(value);
					break;
				case NameDefaults:
					item.Defaults.Add(value);
					break;
				case NameOverrides:
					item.Overrides.Add(value);
					break;
				case NameRequire:
					item.Require.Add(value);
					break;
			}
		}
	}

	private static void ReadPhysToyData(StreamReader sr, PhysicsToyData data) {
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

	private static void ReadPhysPatchData(StreamReader sr, PhysicsPatchData data) {
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
				case NamePPMaterial:
					data.Material = value;
					break;
				case NamePPAreaDensity:
					data.AreaDensity = float.Parse(value);
					break;
				case NamePPAeroModelType:
					data.AeroModelType = value;
					break;
				case NamePPLinearStiffness:
					data.LinearStiffness = float.Parse(value);
					break;
				case NamePPDragCoefficient:
					data.DragCoefficient = float.Parse(value);
					break;
				case NamePPLiftCoefficient:
					data.LiftCoefficient = float.Parse(value);
					break;
				case NamePPTCMinFirst:
					FloatParse(value, (i, v) => data.TCMinFirst[i] = v);
					break;
				case NamePPTCMaxFirst:
					FloatParse(value, (i, v) => data.TCMaxFirst[i] = v);
					break;
				case NamePPTCMinSecond:
					FloatParse(value, (i, v) => data.TCMinSecond[i] = v);
					break;
				case NamePPTCMaxSecond:
					FloatParse(value, (i, v) => data.TCMaxSecond[i] = v);
					break;
				case NamePPXRes:
					data.XRes = uint.Parse(value);
					break;
				case NamePPYRes:
					data.YRes = uint.Parse(value);
					break;
				case NamePPXSize:
					data.XSize = float.Parse(value);
					break;
				case NamePPYSize:
					data.YSize = float.Parse(value);
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
		if (value.Contains('"')) {//string 类数值会有双引号
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
	private const string KeyGenerated = "Generated";//自动生成的键值对 用来判断locale文件是否为自动创建的文件 用户编写的文件gen为false

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
						if (module == null)
							return;
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
		if (locale.Dictionary.Count > 0)//如果当前字典内有值，就输出字典的值
			dict = locale.Dictionary;
		else if (universal.Count > 0) {//如果有通用字典，则输出通用字典内容，并将代表通用字典内容的generated设置为true
			dict = universal;
			Genearated = true;
		} else {//如果通用字典为空，就删除已有locale文件
			File.Delete(localeFile);
			var parent = Directory.GetParent(localeFile);
			if (parent != null && parent.GetFiles().Length == 0)
				parent.Delete();
			return;
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
			var parent = Directory.GetParent(localeFile);
			if (parent != null && parent.GetFiles().Length == 0)
				parent.Delete();
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

	/// <summary>
	/// <para>读取pit文件内的look和variant，pit内有两种方法获取</para>
	/// <para>从blender生成的pit文件前几行会有注释写明包含的所有look和variant</para>
	/// <para>使用转换软件生成的pit则没有，只能文件的具体数据读取</para>
	/// </summary>
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
		Application.Current.Dispatcher.Invoke(() => {
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
		}, DispatcherPriority.Render);
	}

	private static bool ReadNamesAlt(StreamReader sr, ObservableCollection<string> list) {
		string? line;
		if ((line = sr.ReadLine()?.Trim()) != null && line.Contains("Name:", StringComparison.OrdinalIgnoreCase)) {
			int start = line.IndexOf('\"');
			int end = line.LastIndexOf('\"');
			var look = line.Substring(start + 1, end - start);
			Application.Current.Dispatcher.Invoke(() => {
				list.Add(look);
			}, DispatcherPriority.Render);
			return true;
		}
		return false;
	}
}