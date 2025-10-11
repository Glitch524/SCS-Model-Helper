using SCS_Mod_Helper.Language;
using SCS_Mod_Helper.Utils;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

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
		// 处理异常，例如显示错误消息
		// 可以在这里记录日志，或者显示一个友好的错误消息给用户
		// 注意：不要在这里直接处理异常，否则异常不会被外部的调试器捕获
		// e.Handled = true; // 如果你想要阻止异常继续传播，可以设置这个属性为true

		// 记录异常信息
		// LogException(e.Exception);

		// 显示错误消息
		// ShowErrorMessage(e.Exception.Message);

		// 可以在这里重启应用程序或者执行其他恢复操作
		// RestartApplication();

		// 如果你不希望异常被进一步传播，可以设置e.Handled为true
		// e.Handled = true;
	}
}

