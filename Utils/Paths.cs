using System.IO;
using Windows.Data.Xml.Dom;

namespace SCS_Mod_Helper.Utils;

class Paths {
	public static string DefTruckDir(string projectLocation) => Path.Combine(projectLocation, @"def\vehicle\truck");
	public static string DefTruckDir() => Path.Combine(Instances.ProjectLocation, @"def\vehicle\truck");

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

	public static string AccAddonFile(string projectLocation, string truckID, string modelType, string modelName) {
		string dir = Path.Combine(projectLocation, @"def\vehicle\truck", truckID, "accessory", modelType);
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{modelName}.sii");
	}
	public static string AccAddonFile(string truckID, string modelType, string modelName) => AccAddonFile(Instances.ProjectLocation, truckID, modelType, modelName);
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

	public static string AccPaintJobFile(string projectLocation, string truckID, string paintJobName) {
		string dir = Path.Combine(projectLocation, @"def\vehicle\truck", truckID, "paint_job");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{paintJobName}.sii");
	}
	public static string AccPaintJobFile(string truckID, string paintJobName) => AccPaintJobFile(Instances.ProjectLocation, truckID, paintJobName);

	public static string SimplePaintJobFile(string projectLocation, string truckID, string paintJobName) {
		string dir = Path.Combine(projectLocation, @"def\vehicle\truck", truckID, @"paint_job\accessory");
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{paintJobName}.sii");
	}
	public static string SimplePaintJobFile(string truckID, string paintJobName) => SimplePaintJobFile(Instances.ProjectLocation, truckID, paintJobName);

	public static string IntDecorsDir(string projectLocation) => Path.Combine(projectLocation, @"vehicle\truck\upgrade\interior_decors");

	public static string TrucksDBPath() => "Trucks.db";

	public static string LanguageDir() => "Language";
	public static string TrucksLanguageDir() => "Language\\Trucks";
	public static string SavedPhysicsFile() => "Physics.DEP";
	public static string DefaultDEDDir() => "Def Files";

	public static string ExcpPath() => $"Excp_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
}
