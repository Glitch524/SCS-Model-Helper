using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccAddon.Popup;

/// <summary>
/// AddTruckUC.xaml 的交互逻辑
/// </summary>
public partial class AddTruckUC: UserControl {
	private readonly AddTruckBinding binding = new();

	public bool IsETS2;

	public delegate void OnButtonClicked(bool ok);
	private readonly OnButtonClicked onButtonClicked;
	
	public Truck? NewTruck;

    public AddTruckUC(bool isETS2, OnButtonClicked onButtonClicked) {
		InitializeComponent();
		GridMain.DataContext = binding;
		IsETS2 = isETS2;
		this.onButtonClicked = onButtonClicked;
	}

	public void Clean() {
		binding.TruckID = string.Empty;
		binding.ProdYear = null;
		binding.IngameName = string.Empty;
		binding.Description = string.Empty;
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (binding.TruckID.Length == 0 || binding.IngameName.Length == 0) {
			MessageBox.Show(Util.GetString("MessageAddErrNotFilled"));
			return;
		}
		if (sender == ButtonOK)
			NewTruck = new(binding.TruckID, binding.ProdYear ?? DateTime.Now.Year, binding.IngameName, binding.Description, false, "", "" , "" , IsETS2);
		onButtonClicked(sender == ButtonOK);
	}
	private void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();
}

public class AddTruckBinding : BaseBinding {
	public bool OKEnabled => TruckID != null && ProdYear != null && IngameName != null;

	private string mTruckID = string.Empty;
    public string TruckID {
		get => mTruckID;
        set {
			mTruckID = value;
			InvokeChange();

			InvokeChange(nameof(OKEnabled));
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
	private string mIngameName = string.Empty;
	public string IngameName {
		get => mIngameName;
		set {
			mIngameName = value;
			InvokeChange();

			InvokeChange(nameof(OKEnabled));
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