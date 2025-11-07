using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SCS_Mod_Helper.Accessory; 
    class AccAppIO {

	private const string FileHeader = "SCS Model Helper";
	private const string EqualMark = "|=|";

	private static int TabCount = 0;
	private static void WriteFileHeader(StreamWriter sw, string extra = "") {
		sw.Write(FileHeader);
		sw.WriteLine(extra);
	}

	private const string NamePhysToyName = "phys_toy_name";
	private static void WritePhysToyHeader(StreamWriter sw, string physName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePhysToyName}:{physName}");
	}
	private const string NamePhysPatchName = "phys_patch_name";
	private static void WritePhysPatchHeader(StreamWriter sw, string physName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePhysPatchName}:{physName}");
	}

	private static void WriteLine(StreamWriter sw, string name, object? valueObj) {
		if (valueObj == null)
			return;
		if (valueObj is string s && s.Length == 0)
			return;
		string value;
		if (valueObj is float?[] floats) {
			value = string.Join(',', floats);
			if (value == ",," || value == ",")
				return;
		} else
			value = valueObj.ToString() ?? "";
		sw.Write(new string('\t', TabCount));
		sw.Write(name);
		sw.Write(EqualMark);
		sw.WriteLine(value);
	}

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
	private static uint? UIntParse(string value) {
		if (value == "null" || value.Length == 0)
			return null;
		return uint.Parse(value);
	}
	private static long? LongParse(string value) {
		if (value == "null" || value.Length == 0)
			return null;
		return long.Parse(value);
	}
	private static float? FloatParse(string value) {
		if (value == "null" || value.Length == 0)
			return null;
		return float.Parse(value);
	}
	private static float?[] FloatsParse(string value) {
		var s = value.Split(',');
		float?[] f = new float?[s.Length];
		for (int i = 0; i < s.Length; i++) {
			if (s[i].Length == 0)
				f[i] = null;
			else
				f[i] = float.Parse(s[i]);
		}
		return f;
	}
	private static float[] FloatsParseNonNull(string value) {
		var s = value.Split(',');
		float[] f = new float[s.Length];
		for (int i = 0; i < s.Length; i++) {
			f[i] = float.Parse(s[i]);
		}
		return f;
	}

	public static void SavePhysicsList() {
		if (PhysicsItems == null)
			return;
		var physFile = Paths.SavedPhysicsFile();
		if (PhysicsItems.Count == 0) {
			File.Delete(physFile);
			return;
		}
		TabCount = 0;
		using StreamWriter sw = new(physFile);
		WriteFileHeader(sw, ":Physics");
		foreach (var phys in PhysicsItems) {
			if (phys is PhysicsToyData toyData) {
				WritePhysToyHeader(sw, toyData.PhysicsName);
				BraceIn(sw);
				WriteLine(sw, AccDataIO.NamePTModel, toyData.ModelPath);
				WriteLine(sw, AccDataIO.NamePTColl, toyData.CollPath);
				WriteLine(sw, AccDataIO.NamePTLook, toyData.Look);
				WriteLine(sw, AccDataIO.NamePTVariant, toyData.Variant);

				WriteLine(sw, AccDataIO.NamePTToyType, toyData.ToyType);
				WriteLine(sw, AccDataIO.NamePTMass, toyData.Mass);
				WriteLine(sw, AccDataIO.NamePTCogOffset, toyData.CogOffset);
				WriteLine(sw, AccDataIO.NamePTLinearStiffness, toyData.LinearStiffness);
				WriteLine(sw, AccDataIO.NamePTLinearDamping, toyData.LinearDamping);
				WriteLine(sw, AccDataIO.NamePTLocatorHookOffset, toyData.LocatorHookOffset);
				WriteLine(sw, AccDataIO.NamePTRestPositionOffset, toyData.RestPositionOffset);
				WriteLine(sw, AccDataIO.NamePTRestRotationOffset, toyData.RestRotationOffset);
				foreach (var offset in toyData.InstanceOffsetList) {
					WriteLine(sw, AccDataIO.NamePTInstanceOffset, offset);
				}
				WriteLine(sw, AccDataIO.NamePTRopeMaterial, toyData.RopeMaterial);

				WriteLine(sw, AccDataIO.NamePTAngularStiffness, toyData.AngularStiffness);
				WriteLine(sw, AccDataIO.NamePTAngularDamping, toyData.AngularDamping);
				WriteLine(sw, AccDataIO.NamePTAngularAmplitude, toyData.AngularAmplitude);

				WriteLine(sw, AccDataIO.NamePTRopeWidth, toyData.RopeWidth);
				WriteLine(sw, AccDataIO.NamePTRopeLength, toyData.RopeLength);
				WriteLine(sw, AccDataIO.NamePTRopeHookOffset, toyData.RopeHookOffset);
				WriteLine(sw, AccDataIO.NamePTRopeToyOffset, toyData.RopeToyOffset);
				WriteLine(sw, AccDataIO.NamePTRopeResolution, toyData.RopeResolution);
				WriteLine(sw, AccDataIO.NamePTPositionIterations, toyData.PositionIterations);
				WriteLine(sw, AccDataIO.NamePTRopeLinearDensity, toyData.RopeLinearDensity);
				WriteLine(sw, AccDataIO.NamePTNodeDamping, toyData.NodeDamping);
				BraceOut(sw);
			} else if (phys is PhysicsPatchData patchData) {
				WritePhysPatchHeader(sw, patchData.PhysicsName);
				BraceIn(sw);
				WriteLine(sw, AccDataIO.NamePPMaterial, patchData.Material);
				WriteLine(sw, AccDataIO.NamePPAreaDensity, patchData.AreaDensity);
				WriteLine(sw, AccDataIO.NamePPAeroModelType, patchData.AeroModelType);
				WriteLine(sw, AccDataIO.NamePPTCMinFirst, patchData.TCMinFirst);
				WriteLine(sw, AccDataIO.NamePPTCMaxFirst, patchData.TCMaxFirst);
				WriteLine(sw, AccDataIO.NamePPTCMinSecond, patchData.TCMinSecond);
				WriteLine(sw, AccDataIO.NamePPTCMaxSecond, patchData.TCMaxSecond);
				WriteLine(sw, AccDataIO.NamePPXRes, patchData.XRes);
				WriteLine(sw, AccDataIO.NamePPYRes, patchData.YRes);
				WriteLine(sw, AccDataIO.NamePPXSize, patchData.XSize);
				WriteLine(sw, AccDataIO.NamePPYSize, patchData.YSize);
				WriteLine(sw, AccDataIO.NamePPLinearStiffness, patchData.LinearStiffness);
				WriteLine(sw, AccDataIO.NamePPDragCoefficient, patchData.DragCoefficient);
				WriteLine(sw, AccDataIO.NamePPLiftCoefficient, patchData.LiftCoefficient);
				BraceOut(sw);
			}
		}
	}

	public static void AddPhysicsToList(PhysicsData? physics) {
		if (physics == null) 
			return;
		PhysicsItems.Add(physics);
	}

	private static ObservableCollection<PhysicsData>? mPhysicsItems;
	public static ObservableCollection<PhysicsData> PhysicsItems {
		get {
			if (mPhysicsItems == null)
				LoadPhysicsList();
			return mPhysicsItems!;
		}
	}

	private static void LoadPhysicsList() {
		mPhysicsItems = [];
		var physFile = Paths.SavedPhysicsFile();
		if (File.Exists(physFile)) {
			using StreamReader sr = new(physFile);
			string? line = sr.ReadLine();
			if (line == null || !line.StartsWith($"{FileHeader}")) {
				File.Delete(physFile);
				return;
			}
			while ((line = sr.ReadLine()?.Trim()) != null) {
				if (line.Length == 0 || line == "{")
					continue;
				if (line == "}")
					break;
				var name = line.Split(":");
				if (name[0] == NamePhysToyName) {
					PhysicsToyData toyData = new(name[1].Trim());
					while ((line = sr.ReadLine()?.Trim()) != null) {
						if (line.Length == 0 || line == "{")
							continue;
						if (line == "}")
							break;
						var read = line.Split(EqualMark);
						if (read.Length == 1)
							continue;
						if (read.Length > 2)
							read[1] = string.Join(EqualMark, read, 1, read.Length - 1);
						read[1] = read[1].Trim();
						switch (read[0].Trim()) {
							case AccDataIO.NamePTModel:
								toyData.ModelPath = read[1];
								break;
							case AccDataIO.NamePTColl:
								toyData.CollPath = read[1];
								break;
							case AccDataIO.NamePTLook:
								toyData.Look = read[1];
								break;
							case AccDataIO.NamePTVariant:
								toyData.Variant = read[1];
								break;
							case AccDataIO.NamePTToyType:
								toyData.ToyType = read[1];
								break;
							case AccDataIO.NamePTMass:
								toyData.Mass = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTCogOffset:
								toyData.CogOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTLinearStiffness:
								toyData.LinearStiffness = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTLinearDamping:
								toyData.LinearDamping = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTLocatorHookOffset:
								toyData.LocatorHookOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRestPositionOffset:
								toyData.RestPositionOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRestRotationOffset:
								toyData.RestRotationOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTInstanceOffset:
								var offset = FloatsParseNonNull(read[1]);
								toyData.InstanceOffsetList.Add(offset);
								break;
							case AccDataIO.NamePTRopeMaterial:
								toyData.RopeMaterial = read[1];
								break;
							case AccDataIO.NamePTAngularStiffness:
								toyData.AngularStiffness = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTAngularDamping:
								toyData.AngularDamping = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTAngularAmplitude:
								toyData.AngularAmplitude = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRopeWidth:
								toyData.RopeWidth = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeLength:
								toyData.RopeLength = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeHookOffset:
								toyData.RopeHookOffset = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeToyOffset:
								toyData.RopeToyOffset = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeResolution:
								toyData.RopeResolution = UIntParse(read[1]);
								break;
							case AccDataIO.NamePTPositionIterations:
								toyData.PositionIterations = UIntParse(read[1]);
								break;
							case AccDataIO.NamePTRopeLinearDensity:
								toyData.RopeLinearDensity = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTNodeDamping:
								toyData.NodeDamping = FloatParse(read[1]);
								break;
						}
					}
					mPhysicsItems.Add(toyData);
				} else if (name[0] == NamePhysPatchName) {
					PhysicsPatchData patchData = new(name[1].Trim());
					while ((line = sr.ReadLine()?.Trim()) != null) {
						if (line.Length == 0 || line == "{")
							continue;
						if (line == "}")
							break;
						var read = line.Split(EqualMark);
						if (read.Length == 1)
							continue;
						if (read.Length > 2)
							read[1] = string.Join(EqualMark, read, 1, read.Length - 1);
						read[1] = read[1].Trim();
						switch (read[0].Trim()) {
							case AccDataIO.NamePPMaterial:
								patchData.Material = read[1];
								break;
							case AccDataIO.NamePPAreaDensity:
								patchData.AreaDensity = FloatParse(read[1]);
								break;
							case AccDataIO.NamePPAeroModelType:
								patchData.AeroModelType = read[1];
								break;
							case AccDataIO.NamePPTCMinFirst:
								patchData.TCMinFirst = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePPTCMaxFirst:
								patchData.TCMaxFirst = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePPTCMinSecond:
								patchData.TCMinSecond = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePPTCMaxSecond:
								patchData.TCMaxSecond = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePPXRes:
								patchData.XRes = UIntParse(read[1]);
								break;
							case AccDataIO.NamePPYRes:
								patchData.YRes = UIntParse(read[1]);
								break;
							case AccDataIO.NamePPXSize:
								patchData.XSize = FloatParse(read[1]);
								break;
							case AccDataIO.NamePPYSize:
								patchData.YSize = FloatParse(read[1]);
								break;
							case AccDataIO.NamePPLinearStiffness:
								patchData.LinearStiffness = FloatParse(read[1]);
								break;
							case AccDataIO.NamePPDragCoefficient:
								patchData.DragCoefficient = FloatParse(read[1]);
								break;
							case AccDataIO.NamePPLiftCoefficient:
								patchData.LiftCoefficient = FloatParse(read[1]);
								break;
						}
					}
					mPhysicsItems.Add(patchData);
				}
			}
		}
	}


	private const string NameProjectLocation = "ProjectLocation";
	private const string NameModelName = "ModelName";
	private const string NameDisplayName = "DisplayName";
	private const string NamePrice = "Price";
	private const string NameUnlockLevel = "UnlockLevel";
	private const string NameIconName = "IconName";
	private const string NamePartType = "PartType";
	private const string NameModelPath = "ModelPath";
	private const string NameModelPathUK = "ModelPathUK";
	private const string NameExtModelPath = "ExtModelPath";
	private const string NameExtModelPathUK = "ExtModelPathUK";
	private const string NameModelColl = "ModelColl";
	private const string NameModelType = "ModelType";
	private const string NameLook = "Look";
	private const string NameVariant = "Variant";
	private const string NameHideIn = "HideIn";
	private const string NameElectricType = "ElectricType";
	private const string NameData = "Data";
	private const string NameSuitableFor = "SuitableFor";
	private const string NameConflictWith = "ConflictWith";
	private const string NameDefaults = "Defaults";
	private const string NameOverrides = "Overrides";
	private const string NameRequire = "Require";
	private const string NameTrucksHeader = "TrucksHeader";
	private const string NameTrucksETS2 = "TrucksETS2";
	private const string NameTrucksATS = "TrucksATS";

	public static void SaveAccAddon(AccAddonBinding binding, string dedFile) {
		TabCount = 0;
		using StreamWriter sw = new(dedFile);
		WriteFileHeader(sw);
		BraceIn(sw);
		WriteLine(sw, NameProjectLocation, AccAddonBinding.ProjectLocation);
		WriteLine(sw, NameModelName, binding.ModelName);

		WriteLine(sw, NameDisplayName, binding.DisplayName);
		WriteLine(sw, NamePrice, binding.Price);
		WriteLine(sw, NameUnlockLevel, binding.UnlockLevel);
		WriteLine(sw, NameIconName, binding.IconName);
		WriteLine(sw, NamePartType, binding.PartType);
		WriteLine(sw, NameModelPath, binding.ModelPath);
		WriteLine(sw, NameModelPathUK, binding.ModelPathUK);
		WriteLine(sw, NameExtModelPath, binding.ExtModelPath);
		WriteLine(sw, NameExtModelPathUK, binding.ExtModelPathUK);
		WriteLine(sw, NameModelColl, binding.CollPath);
		WriteLine(sw, NameModelType, binding.ModelType);
		WriteLine(sw, NameLook, binding.Look);
		WriteLine(sw, NameVariant, binding.Variant);
		if (binding.HideIn != 0)
			WriteLine(sw, NameHideIn, binding.HideIn);
		if (binding.ElectricType != "vehicle")
			WriteLine(sw, NameElectricType, binding.ElectricType);

		void WriteLists(string name, ObservableCollection<string> list) {
			if (list.Count > 0)
				WriteLine(sw, name, string.Join(DefaultData.ItemSplit, list));
		}
		WriteLists(NameData, binding.Data);
		WriteLists(NameSuitableFor, binding.SuitableFor);
		WriteLists(NameConflictWith, binding.ConflictWith);
		WriteLists(NameDefaults, binding.Defaults);
		WriteLists(NameOverrides, binding.Overrides);
		WriteLists(NameRequire, binding.Require);

		var setTruckHeader = true;
		if (binding.TrucksETS2.Count > 0 || binding.TrucksATS.Count > 0) {
			foreach (var truck in binding.TrucksETS2) {
				if (truck.Check && (truck.ModelType.Length != 0 || truck.Look.Length != 0 || truck.Variant.Length != 0)) {
					if (setTruckHeader) {
						WriteLine(sw, NameTrucksHeader, Truck.DEDHeader());
						setTruckHeader = false;
					}
					WriteLine(sw, NameTrucksETS2, truck.ToDEDLine());
				}
			}
			foreach (var truck in binding.TrucksATS) {
				if (truck.Check && (truck.ModelType.Length != 0 || truck.Look.Length != 0 || truck.Variant.Length != 0)) {
					if (setTruckHeader) {
						WriteLine(sw, NameTrucksHeader, Truck.DEDHeader());
						setTruckHeader = false;
					}
					WriteLine(sw, NameTrucksATS, truck.ToDEDLine());
				}
			}
		}
		BraceOut(sw);
	}

	public static void LoadAccAddon(AccAddonBinding binding, string dedFile) {
		using StreamReader sr = new(dedFile);
		string? line = sr.ReadLine();
		if (line == null || !line.StartsWith($"{FileHeader}"))
			throw new(Util.GetString("MessageLoadDEDErrNotDED"));
		string modelName = "";
		string displayName = "";
		long? price = null;
		uint? unlockLevel = null;
		string iconName = "";
		string partType = "unknown";
		string modelPath = "";
		string modelPathUK = "";
		string extModelPath = "";
		string extModelPathUK = "";
		string collPath = "";
		string modelType = "";
		string look = "";
		string variant = "";
		uint hideIn = 0;
		string electricType = "vehicle";
		string dataList = "";
		string suitableForList = "";
		string conflictWithList = "";
		string defaultsList = "";
		string overridesList = "";
		string requireList = "";
		string[]? trucksHeader = null;
		List<string> trucksETS2Strings = [];
		List<string> trucksATSStrings = [];
		while ((line = sr.ReadLine()?.Trim()) != null) {
			if (line.Length == 0 || line == "{")
				continue;
			if (line == "}")
				break;
			var read = line.Split(EqualMark);
			if (read.Length == 1)
				continue;
			if (read.Length > 2)
				read[1] = string.Join(EqualMark, read, 1, read.Length - 1);
			read[1] = read[1].Trim();
			switch (read[0].Trim()) {
				case NameProjectLocation:
					var currentProjectLocation = Instances.ProjectLocation;
					if (currentProjectLocation != read[1]) {
						var message = Util.GetString("MessageLoadDEDDifferentProject", read[1], currentProjectLocation);
						var result = MessageBox.Show(message, Util.GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
						if (result != MessageBoxResult.Yes)
							return;
					}
					break;
				case NameModelName:
					modelName = read[1];
					break;
				case NameDisplayName:
					displayName = read[1];
					break;
				case NamePrice:
					price = LongParse(read[1]);
					break;
				case NameUnlockLevel:
					unlockLevel = UIntParse(read[1]);
					break;
				case NameIconName:
					iconName = read[1];
					break;
				case NamePartType:
					partType = read[1];
					break;
				case NameModelPath:
					modelPath = read[1];
					break;
				case NameModelPathUK:
					modelPathUK = read[1];
					break;
				case NameExtModelPath:
					extModelPath = read[1];
					break;
				case NameExtModelPathUK:
					extModelPathUK = read[1];
					break;
				case NameModelColl:
					collPath = read[1];
					break;
				case NameModelType:
					modelType = read[1];
					break;
				case NameLook:
					look = read[1];
					break;
				case NameVariant:
					variant = read[1];
					break;
				case NameHideIn:
					hideIn = UIntParse(read[1]) ?? 0;
					break;
				case NameElectricType:
					electricType = read[1];
					break;
				case NameData:
					dataList = read[1];
					break;
				case NameSuitableFor:
					suitableForList = read[1];
					break;
				case NameConflictWith:
					conflictWithList = read[1];
					break;
				case NameDefaults:
					defaultsList = read[1];
					break;
				case NameOverrides:
					overridesList = read[1];
					break;
				case NameRequire:
					requireList = read[1];
					break;
				case NameTrucksHeader:
					trucksHeader = read[1].Split(DefaultData.ItemSplit);
					break;
				case NameTrucksETS2:
					trucksETS2Strings.Add(read[1]);
					break;
				case NameTrucksATS:
					trucksATSStrings.Add(read[1]);
					break;
			}
		}
		binding.ModelName = modelName;
		binding.DisplayName = displayName;
		binding.Price = price;
		binding.UnlockLevel = unlockLevel;
		binding.IconName = iconName;
		binding.LoadIcon();
		binding.PartType = partType;
		binding.ModelPath = modelPath;
		binding.ModelPathUK = modelPathUK;
		binding.ExtModelPath = extModelPath;
		binding.ExtModelPathUK = extModelPathUK;
		binding.CollPath = collPath;
		binding.UseCollPath = collPath.Length > 0;
		binding.mCurrentModelType = null;
		binding.ModelType = modelType;
		binding.Look = look;
		binding.Variant = variant;
		binding.HideIn = hideIn;
		binding.ElectricType = electricType;

		static void ReadLists(ObservableCollection<string> list, string reads) {
			list.Clear();
			if (reads.Length == 0)
				return;
			foreach (var i in reads.Split(DefaultData.ItemSplit)) {
				list.Add(i);
			}
		}
		ReadLists(binding.Data, dataList);
		ReadLists(binding.SuitableFor, suitableForList);
		ReadLists(binding.ConflictWith, conflictWithList);
		ReadLists(binding.Defaults, defaultsList);
		ReadLists(binding.Overrides, overridesList);
		ReadLists(binding.Require, requireList);

		binding.SelectAllETS2 = false;
		binding.SelectAllATS = false;
		if (trucksHeader != null && trucksETS2Strings.Count + trucksATSStrings.Count > 0) {
			var indexesOfTrucks = new int[trucksHeader!.Length];
			for (int i = 0; i < trucksHeader!.Length; i++) {
				string? header = trucksHeader[i];
				switch (header) {
					case nameof(Truck.TruckID):
						indexesOfTrucks[Truck.IndexDTruckID] = i;
						break;
					case nameof(Truck.ModelType):
						indexesOfTrucks[Truck.IndexDModelType] = i;
						break;
					case nameof(Truck.Look):
						indexesOfTrucks[Truck.IndexDLook] = i;
						break;
					case nameof(Truck.Variant):
						indexesOfTrucks[Truck.IndexDVariant] = i;
						break;
				}
			}
			SetSelected(indexesOfTrucks, trucksETS2Strings, true, binding);
			SetSelected(indexesOfTrucks, trucksATSStrings, false, binding);
		}
	}

	private static void SetSelected(int[] indexes, List<string> lines, bool isETS2, AccAddonBinding binding) {
		ObservableCollection<Truck>? truckList;
		if (isETS2) {
			binding.SelectedCountETS2 = 0;
			truckList = binding.TrucksETS2;
		} else {
			binding.SelectedCountATS = 0;
			truckList = binding.TrucksATS;
		}
		foreach (var truck in truckList) {
			truck.Check = false;
			truck.ModelType = "";
			truck.Look = "";
			truck.Variant = "";
			int i = 0;
			while (i < lines.Count) {
				string? line = lines[i];
				var s = line.Split(DefaultData.ItemSplit);
				if (truck.TruckID.Equals(s[indexes[Truck.IndexDTruckID]])) {
					truck.Check = true;
					if (isETS2)
						binding.SelectedCountETS2++;
					else
						binding.SelectedCountATS++;
					truck.ModelType = s[indexes[Truck.IndexDModelType]];
					truck.Look = s[indexes[Truck.IndexDLook]];
					truck.Variant = s[indexes[Truck.IndexDVariant]];
					lines.RemoveAt(i);
				} else
					i++;
			}
		}
	}


	private const string NameTruckVersion = "TruckVersion";
	public static void SaveTruckList(bool ets2, ObservableCollection<Truck> trucks) {
		var truckFile = ets2 ? Paths.TrucksETS2Path() : Paths.TrucksATSPath();
		if (trucks.Count == 0) {
			File.Delete(truckFile);
			return;
		}
		using StreamWriter sw = new(truckFile);
		WriteFileHeader(sw);
		BraceIn(sw);
		WriteLine(sw, NameTruckVersion, DefaultData.TruckVersion);
		WriteLine(sw, NameTrucksHeader, Truck.TruckHeader());
		foreach (var truck in trucks) {
			WriteLine(sw, ets2 ? NameTrucksETS2 : NameTrucksATS, truck.ToTruckLine());
		}
		BraceOut(sw);
	}

	public static void ClearTruckList() {
		var ePath = Paths.TrucksETS2Path();
		File.Delete(ePath);
		var aPath = Paths.TrucksATSPath();
		File.Delete(aPath);
	}

	public static int LoadTruckList(bool ets2, ObservableCollection<Truck> trucks) {
		var truckFile = ets2 ? Paths.TrucksETS2Path() : Paths.TrucksATSPath();
		List<Truck> loadedTrucks = [];
		bool updateTruckName = false;
		if (File.Exists(truckFile)) {
			using StreamReader sr = new(truckFile);
			string? line = sr.ReadLine();
			if (line != null && line.StartsWith($"{FileHeader}")) {
				int selectedCount = 0;
				string[]? trucksHeader = null;
				List<string> trucksStrings = [];
				while ((line = sr.ReadLine()?.Trim()) != null) {
					if (line.Length == 0 || line == "{")
						continue;
					if (line == "}")
						break;
					var read = line.Split(EqualMark);
					if (read.Length == 1)
						continue;
					if (read.Length > 2)
						read[1] = string.Join(EqualMark, read, 1, read.Length - 1);
					read[1] = read[1].Trim();
					switch (read[0].Trim()) {
						case NameTrucksHeader:
							trucksHeader = read[1].Split(DefaultData.ItemSplit);
							break;
						case NameTruckVersion:
							updateTruckName = read[1] != DefaultData.TruckVersion;
							break;
						case NameTrucksETS2:
							if (ets2)
								trucksStrings.Add(read[1]);
							break;
						case NameTrucksATS:
							if (!ets2)
								trucksStrings.Add(read[1]);
							break;
					}
				}
				if (trucksHeader != null && trucksStrings.Count > 0) {
					int[] indexes = Truck.Indexes;
					for (int i = 0; i < trucksHeader!.Length; i++) {
						string? header = trucksHeader[i];
						switch (header) {
							case nameof(Truck.TruckID):
								indexes[Truck.IndexTTruckID] = i;
								break;
							case nameof(Truck.ProductionYear):
								indexes[Truck.IndexTProductionYear] = i;
								break;
							case nameof(Truck.IngameName):
								indexes[Truck.IndexTIngameName] = i;
								break;
							case nameof(Truck.Description):
								indexes[Truck.IndexTDescription] = i;
								break;
							case nameof(Truck.Check):
								indexes[Truck.IndexTCheck] = i;
								break;
							case nameof(Truck.ModelType):
								indexes[Truck.IndexTModelType] = i;
								break;
							case nameof(Truck.Look):
								indexes[Truck.IndexTLook] = i;
								break;
							case nameof(Truck.Variant):
								indexes[Truck.IndexTVariant] = i;
								break;
						}
					}
					foreach (var tString in trucksStrings) {
						var s = tString.Split(DefaultData.ItemSplit);
						var truckID = s[indexes[Truck.IndexTTruckID]];
						int productionYear = DateTime.Now.Year;
						if (indexes[Truck.IndexTProductionYear] != -1)
							productionYear = int.Parse(s[indexes[Truck.IndexTProductionYear]]);
						string ingameName = s[indexes[Truck.IndexTIngameName]];
						string description = "";
						if (indexes[Truck.IndexTDescription] != -1)
							description = s[indexes[Truck.IndexTDescription]];
						bool check = false;
						if (indexes[Truck.IndexTCheck] != -1) {
							check = bool.Parse(s[indexes[Truck.IndexTCheck]]);
							if (check)
								selectedCount++;
						}
						string modelType = "";
						if (indexes[Truck.IndexTModelType] != -1)
							modelType = s[indexes[Truck.IndexTModelType]];
						string look = "";
						if (indexes[Truck.IndexTLook] != -1)
							look = s[indexes[Truck.IndexTLook]];
						string variant = "";
						if (indexes[Truck.IndexTVariant] != -1)
							variant = s[indexes[Truck.IndexTVariant]];
						Truck t = new(truckID, productionYear, ingameName, description, check, modelType, look, variant, ets2);
						loadedTrucks.Add(t);
					}
				}
				loadedTrucks.Sort();
				if (updateTruckName) {
					List<Truck> defaultTrucks = LoadDefaultTrucks(ets2);

					Application.Current.Dispatcher.Invoke(new(() => {
						foreach (var truck in loadedTrucks) {
							int i = 0;
							while (i < defaultTrucks.Count) {
								if (truck.TruckID == defaultTrucks[i].TruckID) {
									truck.IngameName = defaultTrucks[i].IngameName;
									defaultTrucks.RemoveAt(i);
									break;
								}
								i++;
							}
							trucks.Add(truck);
						}
					}), DispatcherPriority.DataBind);
					SaveTruckList(ets2, trucks);
				} else {
					Application.Current.Dispatcher.Invoke(new(() => {
						foreach (var truck in loadedTrucks) {
							trucks.Add(truck);
						}
					}), DispatcherPriority.DataBind);
				}
				return selectedCount;
			} else
				loadedTrucks = LoadDefaultTrucks(ets2);
		} else
			loadedTrucks = LoadDefaultTrucks(ets2);
		Application.Current.Dispatcher.Invoke(new(() => {
			foreach (var truck in loadedTrucks) {
				trucks.Add(truck);
			}
		}), DispatcherPriority.DataBind);
		SaveTruckList(ets2, trucks);
		return ets2 ? loadedTrucks.Count : 0;
	}

	public static List<Truck> LoadDefaultTrucks(bool ets2) {
		if (ets2)
			return DefaultData.DefaultTrucksETS2;
		else
			return DefaultData.DefaultTrucksATS;
	}
}
