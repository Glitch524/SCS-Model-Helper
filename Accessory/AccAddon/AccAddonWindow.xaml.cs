using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccAddon;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class AccAddonWindow: BaseWindow {
	private readonly AccAddonBinding binding = new();
	private bool TruckExpandedETS2 {
		get => binding.TruckExpandedETS2; set => binding.TruckExpandedETS2 = value;
	}
	private bool TruckExpandedATS {
		get => binding.TruckExpandedATS; set => binding.TruckExpandedATS = value;
	}

	private ObservableCollection<OthersItem> OthersList => binding.OthersList;
	private ObservableCollection<Truck> TrucksETS2 => binding.TrucksETS2;
	private ObservableCollection<Truck> TrucksATS => binding.TrucksATS;

	private readonly ContextMenu MenuStringRes, MenuModelType, MenuLook, MenuVariant, MenuOthers;

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
		MenuOthers = (ContextMenu)Resources["MenuOthers"];

		binding.LoadLooksAndVariants();

		OthersItem.ReadLines(binding.OthersList, AccAddonHistory.Default.Others, (dataName) => {
			if (dataName.EndsWith(AccDataIO.NamePSuffix)) {
				dataName = dataName[..^AccDataIO.NamePSuffix.Length];
			}
			foreach (var physItem in AccAppIO.PhysicsItems) {
				if (physItem.PhysicsName == dataName) 
					binding.PhysicsList.Add(physItem);
			}
		});

		binding.LoadTrucks();
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => binding.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent ?? item.ContextMenu;
		if (cm == MenuStringRes) {
			var tag = (string)item.Tag;
			if (tag.Equals("openLocalization")) {
				AccessoryDataUtil.OpenLocalization(this);
			} else {
				AccessoryDataUtil.ApplyStringRes(TextDisplayName, tag);
			}
		} else if (cm == MenuOthers) {
			AccessoryDataUtil.SetOtherItem(item);
		} else {
			Truck truck = (Truck)cm.DataContext;
			if (cm == MenuModelType) {
				var type = (string)item.Tag;
				int i;
				if ((i = type.IndexOf('/')) == -1) 
					truck.ModelType = (string)item.Tag;
				else if (truck.IsETS2)
					truck.ModelType = type[..i];
				else
					truck.ModelType = type[(i + 1)..];
			} else if (cm == MenuLook) {
				truck.Look = (string)item.DataContext;
			} else if (cm == MenuVariant) {
				truck.Variant = (string)item.DataContext;
			} else
				return;
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
			else if (sender == ButtonChooseColl)
				type = AccAddonBinding.MODEL_COLL;
			else
				return;
			binding.ChooseModel(this, type);
		} catch(Exception ex) {
			MessageBox.Show(this, ex.Message);
		}
	}

	private void ButtonHideIn(object sender, RoutedEventArgs e) => PopupHideIn.IsOpen = true;

	private uint GetNumber(CheckBox checkBox) {
		if (checkBox == CheckMainView) {
			return 0x7;
		} else if (checkBox == CheckReflectionCube) {
			return 0x1F8;
		} else if (checkBox == CheckCloseMirror) {
			return 0x600;
		} else if (checkBox == CheckFarMirror) {
			return 0x1800;
		} else if (checkBox == CheckSideMirror) {
			return 0x2000;
		} else if (checkBox == CheckFrontMirror) {
			return 0x4000;
		} else if (checkBox == CheckShadows) {
			return 0x1FFE0000;
		} else if (checkBox == CheckRainReflection) {
			return 0xE0000000;
		} else
			return 0;
	}

	private void HideInChecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb) {
			binding.HideIn += GetNumber(cb);
		}
	}

	private void HideInUnchecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb) {
			binding.HideIn -= GetNumber(cb);
		}
	}

	private void ButtonOthersClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonOthersAdd) {
			OthersList.Add(new());
		} else if (sender == ButtonOthersRemove) {
			OthersItem? othersItem;
			while ((othersItem = TableOthers.SelectedItem as OthersItem)!= null) {
				OthersList.Remove(othersItem);
			}
		}
	}

	private void ButtonPhysicsClick(object sender, RoutedEventArgs e) {
		PhysicsWindow physicsWindow = new() {
			Owner = this,
			ChooseMode = true,
		};
		var result = physicsWindow.ShowDialog();
		if (result == true) {
			var selected = physicsWindow.SelectedPhysicsData;
			OthersItem others = (OthersItem)((Button)sender).DataContext;
			var physName = selected.PhysicsName;
			if (!physName.EndsWith(AccDataIO.NamePSuffix))
				physName += AccDataIO.NamePSuffix;
			others.OthersValue = physName;
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

	private void ButtonStartClick(object sender, RoutedEventArgs e) => binding.StartCreateSii();

	private void ButtonSaveClick(object sender, RoutedEventArgs e) => binding.SaveDED();

	private void ButtonLoadClick(object sender, RoutedEventArgs e) {
		try {
			binding.LoadDED();
			binding.LoadLooksAndVariants();
		} catch (Exception ex) {
			MessageBox.Show(this, GetString("MessageLoadDEDErrFail") + "\n" + ex.Message);
		}
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();

	private void ButtonTruckInitialize(object sender, RoutedEventArgs e) {
		binding.PopupAddTruckOpen = false;
		binding.ReinitTruckList();
	}

	private void ButtonAddTruckClick(object sender, RoutedEventArgs e) {
		PopupAddTruck.PlacementTarget = (Button)sender;
		binding.AddTruckID = string.Empty;
		binding.AddTruckProdYear = null;
		binding.AddTruckIngameName = string.Empty;
		binding.AddTruckDescription = string.Empty;
		PopupAddTruck.IsOpen = true;
	}

	private void ButtonAddTruckResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonAddTruckOK) {
			var isETS2 = PopupAddTruck.PlacementTarget == ButtonAddTruckETS2;
			binding.AddNewTruck(isETS2);
		}
		PopupAddTruck.IsOpen = false;
	}

	private void ContextMenu_Opened(object sender, RoutedEventArgs e) {
		return;
	}

	private void ButtonDeleteTruckClick(object sender, RoutedEventArgs e) {
		if (TableTrucksETS2.SelectedIndex == -1 && TableTrucksATS.SelectedIndex == -1)
			return;
		if (sender == ButtonDeleteTruckETS2) {
			AccAddonBinding.DeleteTruck(true, TrucksETS2, TableTrucksETS2);
		} else if (sender == ButtonDeleteTruckATS) {
			AccAddonBinding.DeleteTruck(false, TrucksATS, TableTrucksATS);
		}
	}

	private void ButtonCoverValue(object sender, RoutedEventArgs e) {
		void ForeachValue(Action<Truck> value, Action<Truck>? valueATS = null) {
			if (TruckExpandedETS2) {
				foreach (Truck t in TrucksETS2) {
					if (t.Check)
						value(t);
				}
			}
			if (TruckExpandedATS) {
				foreach (Truck t in TrucksATS) {
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

	private void TruckCheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox)sender;
		Truck selected = (Truck)box.DataContext;
		binding.OverwriteEmpty(selected, selected.Check);
	}
}