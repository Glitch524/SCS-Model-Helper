using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Utils;
using System.IO;
using System.Windows;

namespace SCS_Mod_Helper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	App() {

	}
	private void UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
		var exceptionPath = Paths.ExcpPath();
		using StreamWriter sw = new(exceptionPath);
		sw.WriteLine(e.Exception.Message);
		sw.Write(e.Exception.StackTrace);
	}

	private void OnExit(object sender, ExitEventArgs e) {
		Instances.BasicInfo.Save();
		PhysicsUC.SavePhysicsList();
	}
}

