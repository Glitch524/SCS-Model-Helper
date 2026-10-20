using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using SCS_Mod_Helper.Vehicles;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Modding.Accessories.AccAddon.Popup;

/// <summary>
/// AddVehicleUC.xaml 的交互逻辑
/// </summary>
public partial class AddVehicleUC: UserControl {
	private readonly AddVehicleBinding binding = new();

	public bool IsETS2;

	public delegate void OnButtonClicked(bool ok);
	private readonly OnButtonClicked onButtonClicked;

	public AccessoryVehicle? NewVehicle;

	public AddVehicleUC(bool isETS2, OnButtonClicked onButtonClicked) {
		InitializeComponent();
		GridMain.DataContext = binding;
		IsETS2 = isETS2;
		this.onButtonClicked = onButtonClicked;
	}

	public void Clean() {
		binding.VehicleID = string.Empty;
		binding.ProdYear = null;
		binding.VehicleName = string.Empty;
		binding.Manifaturer = string.Empty;
		binding.Description = string.Empty;
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (binding.VehicleID.Length == 0 || binding.VehicleName.Length == 0) {
			MessageBox.Show(Window.GetWindow(this), Util.GetString("MessageAddErrNotFilled"));
			return;
		}
		if (sender == ButtonOK) {
			NewVehicle = new(binding.VehicleID,
				  binding.ProdYear ?? DateTime.Now.Year,
				  binding.VehicleName,
				  binding.Description,
				  binding.VehicleType,
				  binding.Manifaturer,
				  IsETS2,
				  false);
			VehiclesIO.AddVehicle(NewVehicle);
		}

		onButtonClicked(sender == ButtonOK);
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();
}

public class AddVehicleBinding: BaseBinding {
	public bool OKEnabled => VehicleID != null && ProdYear != null && VehicleName != null;

	private string mVehicleID = string.Empty;
	public string VehicleID {
		get => mVehicleID;
		set {
			mVehicleID = value;
			InvokeChange();

			InvokeChange(nameof(OKEnabled));
		}
	}
	private string mVehicleType = "Truck";
	public string VehicleType {
		get => mVehicleType;
		set {
			mVehicleType = value;
			InvokeChange();
		}
	}

	private int? mProdYear = null;
	public int? ProdYear {
		get => mProdYear;
		set {
			mProdYear = value;
			InvokeChange();

			InvokeChange(nameof(OKEnabled));
		}
	}
	private string mVehicleName = string.Empty;
	public string VehicleName {
		get => mVehicleName;
		set {
			mVehicleName = value;
			InvokeChange();

			InvokeChange(nameof(OKEnabled));
		}
	}

	private string mManifaturer = string.Empty;
	public string Manifaturer {
		get => mManifaturer;
		set {
			mManifaturer = value;
			InvokeChange();
		}
	}
	private string mDescription = string.Empty;
	public string Description {
		get => mDescription;
		set {
			mDescription = value;
			InvokeChange();
		}
	}
}