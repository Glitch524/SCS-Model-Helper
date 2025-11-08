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

	private ObservableCollection<Truck> TrucksETS2 => binding.TrucksETS2;
	private ObservableCollection<Truck> TrucksATS => binding.TrucksATS;

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

	private void OnLoaded(object sender, RoutedEventArgs e) {
		Task.Run(() => {
			binding.LoadTrucks();
			binding.LoadLooksAndVariants();
		});
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => binding.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent;
		if (cm == MenuStringRes) {
			var tag = (string)item.Tag;
			if (tag.Equals("openLocalization")) {
				AccessoryDataUtil.OpenLocalization(this);
			} else {
				AccessoryDataUtil.ApplyStringRes(TextDisplayName, tag);
			}
		} else if (cm == MenuModelType) {
			Truck truck = (Truck)cm.DataContext;
			var type = (string)item.Tag;
			int i;
			if ((i = type.IndexOf('/')) == -1)
				truck.ModelType = (string)item.Tag;
			else if (truck.IsETS2)
				truck.ModelType = type[..i];
			else
				truck.ModelType = type[(i + 1)..];
		} else {
			var menuName = (string)item.CommandParameter;
			if (menuName == "MenuLook")
				cm = MenuLook;
			else
				cm = MenuVariant;
			Truck truck = (Truck)cm.DataContext;
			if (cm == MenuModelType) {
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
		} catch(Exception ex) {
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
			if (!physName.EndsWith(AccDataIO.NamePSuffix))
				physName += AccDataIO.NamePSuffix;
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

	private void ButtonStartClick(object sender, RoutedEventArgs e) => binding.StartCreateSii();

	private void ButtonSaveClick(object sender, RoutedEventArgs e) => binding.SaveDED();

	private void ButtonLoadClick(object sender, RoutedEventArgs e) {
		binding.LoadDED();
		binding.LoadLooksAndVariants();
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
		if (binding.AddTruckUC == null) {
			binding.AddTruckUC = new(sender == ButtonAddTruckETS2, AddTruckResult);
			PopupAddTruck.Child = binding.AddTruckUC;
		}
		binding.AddTruckUC.Clean();
		PopupAddTruck.IsOpen = true;
	}

	private void AddTruckResult(bool OK) {
		if (OK) {
			Truck newTruck = binding.AddTruckUC?.NewTruck!;
			binding.AddNewTruck(newTruck);
		}
		PopupAddTruck.IsOpen = false;
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
		binding.SetSelected(selected);
		binding.OverwriteEmpty(selected, selected.Check);
	}
}