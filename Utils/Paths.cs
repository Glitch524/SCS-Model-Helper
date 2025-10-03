using System.IO;

namespace SCS_Mod_Helper.Utils;

class Paths {
	public static string ManifestFile(string projectLocation) =>
		$@"{projectLocation}\manifest.sii";
	public static string DefTruckDir(string projectLocation) =>
		$@"{projectLocation}\def\vehicle\truck";
	public static string SiiFile(string projectLocation, string truckId, string modelType, string modelName) {
		string dir = @$"{projectLocation}\def\vehicle\truck\{truckId}\accessory\{modelType}";
		Directory.CreateDirectory(dir);
		return @$"{dir}\{modelName}.sii";
	}
	public static string LocaleDir(string projectLocation) => Path.Combine(projectLocation, "locale");
	public static string LocaleFile(string projectLocation, string locale, string moduleName, bool createDir = true) {
		string dir = Path.Combine(projectLocation, "locale", locale);
		if (createDir)
			Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"local_module.{moduleName}.sii");
	}
	public static string AccessoryDir(string projectLocation) =>
		$@"{projectLocation}\material\ui\accessory";
	public static string IntDecorsDir(string projectLocation) =>
		$@"{projectLocation}\vehicle\truck\upgrade\interior_decors";
	public static string HookupFile(string projectLocation, string hookupName) {
		string dir = $@"{projectLocation}\unit\hookup\vehicle";
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{hookupName}.sii");
	}
	public static string HookupStorageDir(string projectLocation) => Path.Combine(projectLocation, @"def\vehicle");
	public static string AddonHookupsDir(string projectLocation, string suiFilename) {
		var dir = $@"{projectLocation}\def\vehicle\addon_hookups";
		Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"{suiFilename}.sui");
	}

	public static string LanguageDir() => Path.Combine(Environment.CurrentDirectory, "Language");
	public static string SavedPhysicsFile() => Path.Combine(Environment.CurrentDirectory, "Physics.DEP");
	public static string DefaultDEDDir() => Path.Combine(Environment.CurrentDirectory, "Def Files");
}
