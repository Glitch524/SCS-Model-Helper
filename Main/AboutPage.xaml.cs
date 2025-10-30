using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Windows.Navigation;

namespace SCS_Mod_Helper.Main; 
/// <summary>
/// AboutWindow.xaml 的交互逻辑
/// </summary>
public partial class AboutPage: BasePage {
	private AboutBinding binding = new();

	public AboutPage() {
		InitializeComponent();
		GridMain.DataContext = binding;
	}

	private void HyperlinkClick(object sender, RequestNavigateEventArgs e) {
		System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
			FileName = e.Uri.ToString(),
			UseShellExecute = true
		});
		e.Handled = true;
	}
}

public class AboutBinding: BaseBinding {
	public bool SponserLocal => Instances.CurrentLanguage == "zh-CN";
}
