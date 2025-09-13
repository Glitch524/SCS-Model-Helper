using System.IO;

namespace Def_Writer.Utils;

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
	public static string LocaleFile(string projectLocation, string locale, string localeName) {
		string dir = $@"{projectLocation}\locale\{locale}";
		Directory.CreateDirectory(dir);
		return $@"{dir}\local_module.{localeName}.sii";
	}
	public static string AccessoryDir(string projectLocation) =>
		$@"{projectLocation}\material\ui\accessory";
	public static string IntDecorsDir(string projectLocation) =>
		$@"{projectLocation}\vehicle\truck\upgrade\interior_decors";
	public static string HookupFile(string projectLocation, string hookupName) {
		string dir = $@"{projectLocation}\unit\hookup\vehicle";
		Directory.CreateDirectory(dir);
		return $@"{dir}\{hookupName}.sii";
	}
}
