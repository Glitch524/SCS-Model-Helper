using System.IO;

namespace SCS_Mod_Helper.Utils;

class Paths {
	private static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string ManifestFile(string projectLocation) => Path.Combine(projectLocation, "manifest.sii");
	public static string ManifestFile() => Path.Combine(Instances.ProjectLocation, "manifest.sii");

	public static string LocaleDir(string projectLocation) => Path.Combine(projectLocation, "locale");
	public static string LocaleDir() => Path.Combine(Instances.ProjectLocation, "locale");
	public static string LocaleFile(string projectLocation, string locale, string moduleName, bool createDir = true) {
		string dir = Path.Combine(projectLocation, "locale", locale);
		if (createDir)
			Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"local_module.{moduleName}.sii");
	}
	public static string LocaleFile(string locale, string moduleName, bool createDir = true) => LocaleFile(Instances.ProjectLocation, locale, moduleName, createDir);



	public static string DefTruckDir(string projectLocation) => Path.Combine(projectLocation, @"def\vehicle\truck");
	public static string DefTruckDir() => Path.Combine(Instances.ProjectLocation, @"def\vehicle\truck");
	private static string DefVehicleDir(string projectLocation, string vehicleType, string vehicleID) => 
		Path.Combine(projectLocation, @"def\vehicle", vehicleType, vehicleID);

	private static string AccAddonFile(string projectLocation, string vehicleType, string vehicleID, string modelType, string modelName) {
		string dir = Path.Combine(DefVehicleDir(projectLocation, vehicleType, vehicleID), "accessory", modelType);
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{modelName}.sii");
	}
	public static string AccAddonFile(string vehicleType, string vehicleID, string modelType, string modelName) => 
		AccAddonFile(Instances.ProjectLocation, vehicleType, vehicleID, modelType, modelName);
	public static string AccAddonFile(string vehicleID, string modelType, string modelName) =>
		AccAddonFile(Instances.ProjectLocation, "truck", vehicleID, modelType, modelName);

	public static string AccPaintJobFile(string projectLocation, string vehicleType, string vehicleID, string paintJobName) {
		string dir = Path.Combine(DefVehicleDir(projectLocation, vehicleType, vehicleID), "paint_job");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{paintJobName}.sii");
	}
	public static string AccPaintJobFile(string vehicleType, string vehicleID, string paintJobName) =>
		AccPaintJobFile(Instances.ProjectLocation, vehicleType, vehicleID, paintJobName);
	public static string AccPaintJobFile(string vehicleID, string paintJobName) =>
		AccPaintJobFile(Instances.ProjectLocation, "truck", vehicleID, paintJobName);

	public static string SimplePaintJobFile(string projectLocation, string vehicleType, string vehicleID, string paintJobName) {
		string dir = Path.Combine(DefVehicleDir(projectLocation, vehicleType, vehicleID), @"paint_job\accessory");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{paintJobName}.sii");
	}
	public static string SimplePaintJobFile(string vehicleType, string vehicleID, string paintJobName) =>
		SimplePaintJobFile(Instances.ProjectLocation, vehicleType, vehicleID, paintJobName);
	public static string SimplePaintJobFile(string vehicleID, string paintJobName) =>
		SimplePaintJobFile(Instances.ProjectLocation, "truck", vehicleID, paintJobName);



	public static string AccessoryIconDir(string projectLocation, string sub = "") {
		var p = Path.Combine(projectLocation, @"material\ui\accessory");
		if (sub.Length > 0)
			p = Path.Combine(p, sub);
		return p;
	}
	public static string AccessoryIconDir(string sub = "") => AccessoryIconDir(Instances.ProjectLocation, sub);

	public static string HookupFile(string projectLocation, string hookupName) {
		string dir = Path.Combine(projectLocation, @"unit\hookup\vehicle");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{hookupName}.sii");
	}
	public static string HookupFile(string hookupName) => HookupFile(Instances.ProjectLocation, hookupName);

	public static string HookupStorageDir(string projectLocation) => Path.Combine(projectLocation, @"def\vehicle");
	public static string HookupStorageDir() => HookupStorageDir(Instances.ProjectLocation);

	public static string AddonHookupsDir(string projectLocation, string suiFilename) {
		string dir = Path.Combine(projectLocation, @"def\vehicle\addon_hookups");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{suiFilename}.sui");
	}
	public static string AddonHookupsDir(string suiFilename) => AddonHookupsDir(Instances.ProjectLocation, suiFilename);

	public static string IntDecorsDir(string projectLocation) => Path.Combine(projectLocation, @"vehicle\truck\upgrade\interior_decors");

	public static string EmbDBFile() => Path.Combine(LocalAppData, "SCS Mod Helper", "TempVehicles.db");
	public static string EmbHashFile() => Path.Combine(LocalAppData, "SCS Mod Helper", "TempVehicles.hash");
	public static string CustomDBPath() => @"Vehicles\CustomVehicles.db";
	public static string VehiclesDBPath() => "Vehicles.db";
	public static string StorageDBPath() => "Storage.dll";

	public static string LanguageDir() => "Language";
	public static string VehiclesLanguageDir() => "Vehicles";
	public static string SavedPhysicsFile() => "Physics.DEP";
	public static string DefaultDEDDir() => "Def Files";

	public static string ExcpPath() => $"Excp_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
}
