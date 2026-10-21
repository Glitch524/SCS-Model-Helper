using SCS_Mod_Helper.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCS_Mod_Helper.Utils;

partial class Paths {
	public static string LocaleDir(string projectLocation) => Path.Combine(projectLocation, "locale");
	public static string LocaleDir() => Path.Combine(Instances.ProjectLocation, "locale");
	public static string LocaleFile(string projectLocation, string locale, string moduleName, bool createDir = true) {
		string dir = Path.Combine(projectLocation, "locale", locale);
		if (createDir)
			Directory.CreateDirectory(dir);
		return Path.Combine(dir, $"local_module.{moduleName}.sii");
	}
	public static string LocaleFile(string locale, string moduleName, bool createDir = true) => LocaleFile(Instances.ProjectLocation, locale, moduleName, createDir);
}
