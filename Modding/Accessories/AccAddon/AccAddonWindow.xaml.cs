using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Modding.Accessories.Physics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Modding.Accessories.AccAddon;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class AccAddonWindow: BaseWindow {
	private readonly AccAddonBinding binding = new();
	private bool VehicleExpandedETS2 {
		get => binding.VehicleExpandedETS2; 
		set => binding.VehicleExpandedETS2 = value;
	}
	private bool VehicleExpandedATS {
		get => binding.VehicleExpandedATS; 
		set => binding.VehicleExpandedATS = value;
	}

	private ObservableCollection<AccessoryVehicle> VehiclesETS2 => binding.VehiclesETS2;
	private ObservableCollection<AccessoryVehicle> VehiclesATS => binding.VehiclesATS;

	private readonly ContextMenu MenuStringRes, MenuModelType, MenuLook, MenuVariant;

	public AccAddonWindow() {
		InitializeComponent();

		GridMain.DataContext = binding;

		MenuStringRes = (ContextMenu)Resources["MenuStringRes"];
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		MenuStringRes.DataContext = binding;
		MenuModelType = (ContextMenu)Resources["MenuModelType"];
		MenuLook = (ContextMenu)Resources["MenuLook"];
		MenuLook.ItemsSource = binding.LookList;
		MenuVariant = (ContextMenu)Resources["MenuVariant"];
		MenuVariant.ItemsSource = binding.VariantList;

		Loaded += OnLoaded;
	}

	private async void OnLoaded(object sender, RoutedEventArgs e) {
		var tLoad = binding.LoadVehicles();
		await tLoad;
		binding.LoadHistory();
		binding.ModelIcon = AccessoryDataUtil.LoadModelIconByIconName(binding.IconName);

		binding.LoadLooksAndVariants(this);
		binding.UpdateOthersChecked();
		binding.Loading = false;
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => binding.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent;
		if (cm == MenuModelType) {
			AccessoryVehicle vehicle = (AccessoryVehicle)cm.DataContext;
			var type = (string)item.Tag;
			int i;
			if ((i = type.IndexOf('/')) == -1)
				vehicle.ModelType = (string)item.Tag;
			else if (vehicle.IsETS2)
				vehicle.ModelType = type[..i];
			else
				vehicle.ModelType = type[(i + 1)..];
		} else {
			var menuName = (string)item.CommandParameter;
			cm = menuName switch {
				"MenuStringRes" => MenuStringRes,
				"MenuLook" => MenuLook,
				_ => MenuVariant,
			};
			if (cm == MenuStringRes) {
				var tag = (string)item.Tag;
				if (tag.Equals("openLocalization")) {
					StringResUtil.OpenLocalization(this);
				} else {
					StringResUtil.ApplyStringRes(TextDisplayName, tag);
				}
			} else {
				AccessoryVehicle vehicle = (AccessoryVehicle)cm.DataContext;
				if (cm == MenuModelType) {
				} else if (cm == MenuLook) {
					vehicle.Look = (string)item.DataContext;
				} else if (cm == MenuVariant) {
					vehicle.Variant = (string)item.DataContext;
				} else
					return;
			}
		}
	}

	private void SaveOnClosing(object? sender, CancelEventArgs e) => binding.SaveHistory();

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonIconNameClear) {
			binding.IconName = "";
			binding.ModelIcon = null;
		} else if (sender == ButtonChooseModelClear)
			binding.ModelPath = "";
		else if (sender == ButtonChooseModelUKClear)
			binding.ModelPathUK = "";
		else if (sender == ButtonChooseExtModelClear)
			binding.ExtModelPath = "";
		else if (sender == ButtonChooseExtModelUKClear)
			binding.ExtModelPathUK = "";
		else if (sender == ButtonChooseCollClear)
			binding.CollPath = "";
		else
			return;
	}

	private void ButtonChooseIcon(object sender, RoutedEventArgs e) => binding.ChooseIcon(this);

	private void ButtonChooseModelClick(object sender, RoutedEventArgs e) {
		try {
			int type;
			if (sender == ButtonChooseModel)
				type = AccAddonBinding.MODEL;
			else if (sender == ButtonChooseModelUK)
				type = AccAddonBinding.MODEL_UK;
			else if (sender == ButtonChooseExtModel)
				type = AccAddonBinding.EXT_MODEL;
			else if (sender == ButtonChooseExtModelUK)
				type = AccAddonBinding.EXT_MODEL_UK;
			else if (sender == ButtonChooseColl)
				type = AccAddonBinding.MODEL_COLL;
			else
				return;
			binding.ChooseModel(this, type);
		} catch (Exception ex) {
			MessageBox.Show(this, ex.Message);
		}
	}

	private void ButtonHideIn(object sender, RoutedEventArgs e) {
		if (binding.HideInUC == null) {
			binding.HideInUC = new(binding);
			PopupHideIn.Child = binding.HideInUC;
		}
		PopupHideIn.IsOpen = true;
	}

	private void ButtonListClick(object sender, RoutedEventArgs e) {
		TextBox target;
		if (sender == ButtonAddData) {
			target = TextData;
			binding.AddNewData();
		} else if (sender == ButtonAddSuitableFor) {
			target = TextSuitableFor;
			binding.AddNewSuitableFor();
		} else if (sender == ButtonAddConflictWith) {
			target = TextConflictWith;
			binding.AddNewConflictWith();
		} else if (sender == ButtonAddDefaults) {
			target = TextDefaults;
			binding.AddNewDefaults();
		} else if (sender == ButtonAddOverrides) {
			target = TextOverrides;
			binding.AddNewOverrides();
		} else if (sender == ButtonAddRequire) {
			target = TextRequire;
			binding.AddNewRequire();
		} else
			return;
		target.Focus();
	}

	private void ListDataGotFocus(object sender, RoutedEventArgs e) {
		ShowPopupListData((TextBox)sender);
	}

	private void ListDataLostFocus(object sender, RoutedEventArgs e) {
		PopupListData.IsOpen = false;
	}

	private void ButtonPhysicsClick(object sender, RoutedEventArgs e) {
		PhysicsWindow physicsWindow = new() {
			Owner = this,
			ChooseMode = true,
		};
		var result = physicsWindow.ShowDialog();
		if (result == true) {
			var selected = physicsWindow.SelectedPhysicsData;
			var physName = selected.PhysicsName;
			if (!physName.EndsWith(AccSCSIO.NamePSuffix))
				physName += AccSCSIO.NamePSuffix;
			binding.AddNewData(physName);
			TextData.Focus();

			int i = 0;
			while (i < binding.PhysicsList.Count) {
				var p = binding.PhysicsList[i];
				if (p.PhysicsName == selected.PhysicsName) {
					binding.PhysicsList.Remove(p);
					continue;
				}
				i++;
			}
			binding.PhysicsList.Add(selected);
		}
	}

	private void ShowPopupListData(TextBox target) {
		if (binding.ListDataUC == null) {
			binding.ListDataUC = new();
			PopupListData.Child = binding.ListDataUC;
		}
		binding.OpeningList = target.Name;
		PopupListData.PlacementTarget = target;
		PopupListData.IsOpen = true;
	}

	private void ButtonStartClick(object sender, RoutedEventArgs e) => binding.StartCreateSii(this);

	private void ButtonSaveClick(object sender, RoutedEventArgs e) {

		//AddonHistoryIO aahio = new();
		//aahio.SaveAddon(binding);

		binding.SaveDED(this);
	}

	private void ButtonLoadClick(object sender, RoutedEventArgs e) {
		//AddonHistoryIO aahio = new();
		//aahio.LoadAddon(binding);

		binding.LoadDED(this);
		binding.LoadLooksAndVariants(this);
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();

	private void ButtonVehicleReinit(object sender, RoutedEventArgs e) {
		binding.PopupAddVehicleOpen = false;
		binding.ReinitVehicleList();
	}

	private void ButtonAddVehicleClick(object sender, RoutedEventArgs e) {
		PopupAddVehicle.PlacementTarget = (Button)sender;
		if (binding.AddVehicleUC == null) {
			binding.AddVehicleUC = new(sender == ButtonAddVehicleETS2, AddVehicleResult);
			PopupAddVehicle.Child = binding.AddVehicleUC;
		}
		binding.AddVehicleUC.Clean();
		PopupAddVehicle.IsOpen = true;
	}

	private void AddVehicleResult(bool OK) {
		if (OK) {
			AccessoryVehicle newVehicle = binding.AddVehicleUC?.NewVehicle!;
			binding.AddNewVehicle(this, newVehicle);
		}
		PopupAddVehicle.IsOpen = false;
	}

	private void ButtonDeleteVehicleClick(object sender, RoutedEventArgs e) {
		if (TableVehiclesETS2.SelectedIndex == -1 && TableVehiclesATS.SelectedIndex == -1)
			return;
		if (sender == ButtonDeleteVehicleETS2) {
			AccAddonBinding.DeleteVehicle(true, VehiclesETS2, TableVehiclesETS2);
		} else if (sender == ButtonDeleteVehicleATS) {
			AccAddonBinding.DeleteVehicle(false, VehiclesATS, TableVehiclesATS);
		}
	}

	private void ButtonCoverValue(object sender, RoutedEventArgs e) {
		void ForeachValue(Action<AccessoryVehicle> value, Action<AccessoryVehicle>? valueATS = null) {
			if (VehicleExpandedETS2) {
				foreach (var t in VehiclesETS2) {
					if (t.Check)
						value(t);
				}
			}
			if (VehicleExpandedATS) {
				foreach (var t in VehiclesATS) {
					if (t.Check) {
						if (valueATS != null) {
							valueATS(t);
						} else {
							value(t);
						}
					}
				}
			}
		}
		if (sender == ButtonCoverModelType) {
			var acc = binding.CurrentModelType;
			ForeachValue((t) => {
				if (acc.ForETS2)
					t.ModelType = acc.AccessoryETS2!;
			}, (t) => {
				if (acc.ForATS)
					t.ModelType = acc.AccessoryATS!;
			});
		} else if (sender == ButtonCoverLook) {
			ForeachValue((t) => t.Look = TextLook.Text);
		} else if (sender == ButtonCoverVariant) {
			ForeachValue((t) => t.Variant = TextVariant.Text);
		}
	}

	private void VehicleCheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox)sender;
		AccessoryVehicle selected = (AccessoryVehicle)box.DataContext;
		binding.SetSelected(selected);
		binding.OverwriteEmpty(selected, selected.Check);
	}
}