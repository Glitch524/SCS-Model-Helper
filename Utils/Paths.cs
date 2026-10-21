using System.IO;

namespace SCS_Mod_Helper.Utils;

partial class Paths {
	private static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string ManifestFile(string projectLocation) => Path.Combine(projectLocation, "manifest.sii");
	public static string ManifestFile() => Path.Combine(Instances.ProjectLocation, "manifest.sii");


	public static string EmbDBFile() => Path.Combine(LocalAppData, "SCS Mod Helper", "TempVehicles.db");
	public static string EmbHashFile() => Path.Combine(LocalAppData, "SCS Mod Helper", "TempVehicles.hash");
	public static string CustomDBPath() => @"Vehicles\CustomVehicles.db";
	public static string VehiclesDBPath() => "Vehicles.db";


	public static string StorageDBPath() => "Storage.dll";

	public static string LanguageDir() => "Language";
	public static string VehiclesLanguageDir() => "Vehicles";


	public static string ExcpPath() => $"Excp_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
}
