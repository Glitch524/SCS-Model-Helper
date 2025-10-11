using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace SCS_Mod_Helper.Accessory; 
    class AccAppIO {

	private const string FileHeader = "SCS Model Helper";
	private const string NamePhysName = "phys_name";
	private const string EqualMark = "|=|";

	private static int TabCount = 0;
	private static void WriteFileHeader(StreamWriter sw, string extra = "") {
		sw.Write(FileHeader);
		sw.WriteLine(extra);
	}

	private static void WritePhysHeader(StreamWriter sw, string physName) {
		sw.Write(new string('\t', TabCount));
		sw.WriteLine($"{NamePhysName}:{physName}");
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
			WritePhysHeader(sw, phys.PhysicsName);
			BraceIn(sw);
			WriteLine(sw, AccDataIO.NamePTModel, phys.ModelPath);
			WriteLine(sw, AccDataIO.NamePTColl, phys.CollPath);
			WriteLine(sw, AccDataIO.NamePTLook, phys.Look);
			WriteLine(sw, AccDataIO.NamePTVariant, phys.Variant);

			WriteLine(sw, AccDataIO.NamePTToyType, phys.ToyType);
			WriteLine(sw, AccDataIO.NamePTMass, phys.Mass);
			WriteLine(sw, AccDataIO.NamePTCogOffset, phys.CogOffset);
			WriteLine(sw, AccDataIO.NamePTLinearStiffness, phys.LinearStiffness);
			WriteLine(sw, AccDataIO.NamePTLinearDamping, phys.LinearDamping);
			WriteLine(sw, AccDataIO.NamePTLocatorHookOffset, phys.LocatorHookOffset);
			WriteLine(sw, AccDataIO.NamePTRestPositionOffset, phys.RestPositionOffset);
			WriteLine(sw, AccDataIO.NamePTRestRotationOffset, phys.RestRotationOffset);
			foreach (var offset in phys.InstanceOffsetList) {
				WriteLine(sw, AccDataIO.NamePTInstanceOffset, offset);
			}
			WriteLine(sw, AccDataIO.NamePTRopeMaterial, phys.RopeMaterial);

			WriteLine(sw, AccDataIO.NamePTAngularStiffness, phys.AngularStiffness);
			WriteLine(sw, AccDataIO.NamePTAngularDamping, phys.AngularDamping);
			WriteLine(sw, AccDataIO.NamePTAngularAmplitude, phys.AngularAmplitude);

			WriteLine(sw, AccDataIO.NamePTRopeWidth, phys.RopeWidth);
			WriteLine(sw, AccDataIO.NamePTRopeLength, phys.RopeLength);
			WriteLine(sw, AccDataIO.NamePTRopeHookOffset, phys.RopeHookOffset);
			WriteLine(sw, AccDataIO.NamePTRopeToyOffset, phys.RopeToyOffset);
			WriteLine(sw, AccDataIO.NamePTRopeResolution, phys.RopeResolution);
			WriteLine(sw, AccDataIO.NamePTPositionIterations, phys.PositionIterations);
			WriteLine(sw, AccDataIO.NamePTRopeLinearDensity, phys.RopeLinearDensity);
			WriteLine(sw, AccDataIO.NamePTNodeDamping, phys.NodeDamping);
			BraceOut(sw);
		}
	}

	public static void AddPhysicsToList(PhysicsToyData? physics) {
		if (physics == null) return;
		PhysicsItems.Add(physics);
	}

	private static ObservableCollection<PhysicsToyData>? mPhysicsItems;
	public static ObservableCollection<PhysicsToyData> PhysicsItems {
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
				if (name[0] == NamePhysName) {
					PhysicsToyData phys = new(name[1].Trim());
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
								phys.ModelPath = read[1];
								break;
							case AccDataIO.NamePTColl:
								phys.CollPath = read[1];
								break;
							case AccDataIO.NamePTLook:
								phys.Look = read[1];
								break;
							case AccDataIO.NamePTVariant:
								phys.Variant = read[1];
								break;
							case AccDataIO.NamePTToyType:
								phys.ToyType = read[1];
								break;
							case AccDataIO.NamePTMass:
								phys.Mass = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTCogOffset:
								phys.CogOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTLinearStiffness:
								phys.LinearStiffness = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTLinearDamping:
								phys.LinearDamping = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTLocatorHookOffset:
								phys.LocatorHookOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRestPositionOffset:
								phys.RestPositionOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRestRotationOffset:
								phys.RestRotationOffset = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTInstanceOffset:
								var offset = FloatsParseNonNull(read[1]);
								phys.InstanceOffsetList.Add(offset);
								break;
							case AccDataIO.NamePTRopeMaterial:
								phys.RopeMaterial = read[1];
								break;
							case AccDataIO.NamePTAngularStiffness:
								phys.AngularStiffness = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTAngularDamping:
								phys.AngularDamping = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTAngularAmplitude:
								phys.AngularAmplitude = FloatsParse(read[1]);
								break;
							case AccDataIO.NamePTRopeWidth:
								phys.RopeWidth = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeLength:
								phys.RopeLength = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeHookOffset:
								phys.RopeHookOffset = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeToyOffset:
								phys.RopeToyOffset = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTRopeResolution:
								phys.RopeResolution = UIntParse(read[1]);
								break;
							case AccDataIO.NamePTPositionIterations:
								phys.PositionIterations = UIntParse(read[1]);
								break;
							case AccDataIO.NamePTRopeLinearDensity:
								phys.RopeLinearDensity = FloatParse(read[1]);
								break;
							case AccDataIO.NamePTNodeDamping:
								phys.NodeDamping = FloatParse(read[1]);
								break;
						}
					}
					mPhysicsItems.Add(phys);
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
	private const string NameModelColl = "ModelColl";
	private const string NameModelType = "ModelType";
	private const string NameLook = "Look";
	private const string NameVariant = "Variant";
	private const string NameHideIn = "HideIn";
	private const string NameOthersHeader = "OthersHeader";
	private const string NameOthers = "Others";
	private const string NameTrucksHeader = "TrucksHeader";
	private const string NameTrucksETS2 = "TrucksETS2";
	private const string NameTrucksATS = "TrucksATS";

	public static void SaveAccAddon(AccAddonBinding binding, string dedFile) {
		TabCount = 0;
		using StreamWriter sw = new(dedFile);
		WriteFileHeader(sw);
		BraceIn(sw);
		WriteLine(sw, NameProjectLocation, binding.ProjectLocation);
		WriteLine(sw, NameModelName, binding.ModelName);

		WriteLine(sw, NameDisplayName, binding.DisplayName);
		WriteLine(sw, NamePrice, binding.Price);
		WriteLine(sw, NameUnlockLevel, binding.UnlockLevel);
		WriteLine(sw, NameIconName, binding.IconName);
		WriteLine(sw, NamePartType, binding.PartType);
		WriteLine(sw, NameModelPath, binding.ModelPath);
		WriteLine(sw, NameModelPathUK, binding.ModelPathUK);
		WriteLine(sw, NameModelColl, binding.CollPath);
		WriteLine(sw, NameModelType, binding.ModelType);
		WriteLine(sw, NameLook, binding.Look);
		WriteLine(sw, NameVariant, binding.Variant);
		if (binding.HideIn != 0)
			WriteLine(sw, NameHideIn, binding.HideIn);

		var setOtherHeader = true;
		if (binding.OthersList.Count > 0) {
			foreach (var other in binding.OthersList) {
				if (other.OthersName.Length > 0 || other.OthersValue.Length > 0) {
					if (setOtherHeader) {
						WriteLine(sw, NameOthersHeader, OthersItem.GetHeaderLine());
						setOtherHeader = false;
					}
					WriteLine(sw, NameOthers, other.ToLine());
				}
			}
		}
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
		string collPath = "";
		string modelType = "";
		string look = "";
		string variant = "";
		uint hideIn = 0;
		string[]? otherHeader = null;
		List<string> otherStrings = [];
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
					var currentProjectLocation = Instances.ModelProject.ProjectLocation;
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
				case NameOthersHeader:
					otherHeader = read[1].Split(DefaultData.ItemSplit);
					break;
				case NameOthers:
					otherStrings.Add(read[1]);
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
		binding.PartType = partType;
		binding.ModelPath = modelPath;
		binding.ModelPathUK = modelPathUK;
		binding.CollPath = collPath;
		binding.ModelType = modelType;
		binding.Look = look;
		binding.Variant = variant;
		binding.HideIn = hideIn;

		binding.OthersList.Clear();
		if (otherHeader != null && otherStrings.Count == 0) {
			var indexesOfOthers = new int[otherHeader.Length];
			for (int i = 0; i < otherHeader!.Length; i++) {
				string? header = otherHeader[i];
				switch (header) {
					case nameof(OthersItem.OthersName):
						indexesOfOthers[OthersItem.IndexOthersName] = i;
						break;
					case nameof(OthersItem.OthersValue):
						indexesOfOthers[OthersItem.IndexOthersValue] = i;
						break;
				}
			}
			foreach (var oString in otherStrings) {
				OthersItem others = new();
				var oSplit = oString.Split(DefaultData.ItemSplit);
				others.OthersName = oSplit[indexesOfOthers[OthersItem.IndexOthersName]];
				others.OthersValue = oSplit[indexesOfOthers[OthersItem.IndexOthersValue]];
			}
		}

		binding.SelectAllETS2 = false;
		binding.SelectAllATS = false;
		if (trucksHeader != null && trucksETS2Strings.Count + trucksATSStrings.Count > 0) {
			var indexesOfTrucks = new int[trucksHeader!.Length];
			for (int i = 0; i < trucksHeader!.Length; i++) {
				string? header = trucksHeader[i];
				switch (header) {
					case nameof(Truck.TruckID):
						indexesOfTrucks[Truck.IndexTruckID] = i;
						break;
					case nameof(Truck.ModelType):
						indexesOfTrucks[Truck.IndexModelType] = i;
						break;
					case nameof(Truck.Look):
						indexesOfTrucks[Truck.IndexLook] = i;
						break;
					case nameof(Truck.Variant):
						indexesOfTrucks[Truck.IndexVariant] = i;
						break;
				}
			}
			SetSelected(indexesOfTrucks, trucksETS2Strings, binding.TrucksETS2);
			SetSelected(indexesOfTrucks, trucksATSStrings, binding.TrucksATS);
		}
	}

	private static void SetSelected(int[] indexes, List<string> lines, ObservableCollection<Truck> truckList) {
		foreach (var truck in truckList) {
			truck.Check = false;
			truck.ModelType = "";
			truck.Look = "";
			truck.Variant = "";
			int i = 0;
			while (i < lines.Count) {
				string? line = lines[i];
				var s = line.Split(DefaultData.ItemSplit);
				if (truck.TruckID.Equals(s[Truck.IndexTruckID])) {
					truck.Check = true;
					truck.ModelType = s[indexes[Truck.IndexModelType]];
					truck.Look = s[indexes[Truck.IndexLook]];
					truck.Variant = s[indexes[Truck.IndexVariant]];
					lines.RemoveAt(i);
				} else
					i++;
			}
		}
	}
}
