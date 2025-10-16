using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

	private ObservableCollection<ModelTypeInfo> AccessoryType => binding.ModelTypes;
	private ObservableCollection<OthersItem> OthersList => binding.OthersList;
	private ObservableCollection<Truck> TrucksETS2 => binding.TrucksETS2;
	private ObservableCollection<Truck> TrucksATS => binding.TrucksATS;

	private readonly ContextMenu MenuModelType, MenuLook, MenuVariant, MenuOthers;

	public AccAddonWindow() {
		InitializeComponent();

		GridMain.DataContext = binding;

		MenuModelType = (ContextMenu)Resources["MenuModelType"];
		MenuLook = (ContextMenu)Resources["MenuLook"];
		MenuVariant = (ContextMenu)Resources["MenuVariant"];
		MenuOthers = (ContextMenu)Resources["MenuOthers"];

		SetupModelTypeMenu();

		binding.LookList.CollectionChanged += (sender, e) => OnLVListChanged(MenuLook, e);
		binding.VariantList.CollectionChanged += (sender, e) => OnLVListChanged(MenuVariant, e);
		binding.LoadLooksAndVariants();

		OthersItem.ReadLines(binding.OthersList, AccAddonHistory.Default.Others, (dataName) => {
			if (dataName.EndsWith(AccDataIO.NamePSuffix)) {
				dataName = dataName[..^AccDataIO.NamePSuffix.Length];
			}
			foreach (var physItem in AccAppIO.PhysicsItems) {
				if (physItem.PhysicsName == dataName) {
					binding.PhysicsList.Add(physItem);
				}
			}
		});

		binding.LoadTrucks();

		SetupStringResMenu();
	}

	private void SetupModelTypeMenu() {
		MenuModelType.Items.Clear();
		foreach (var acc in AccessoryType) {
			MenuItem item = new() {
				Name = acc.Accessory,
				Header = $"{acc.Accessory.Replace("_", "__")}→{acc.Name}"
			};
			item.Click += OnMenuClicked;
			MenuModelType.Items.Add(item);
		}
	}

	private void OnLVListChanged(ContextMenu contextMenu, NotifyCollectionChangedEventArgs e) {
		switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				var nis = (string)e.NewItems![0]!;
				MenuItem item = new() {
					Name = nis,
					Header = nis.Replace("_", "__"),
				};
				item.Click += OnMenuClicked;
				contextMenu.Items.Insert(e.NewStartingIndex, item);
				break;
			case NotifyCollectionChangedAction.Reset:
				for (int i = contextMenu.Items.Count - 1; i >= 0; i--) {
					object? iItem = contextMenu.Items[i];
					((MenuItem)iItem).Click -= OnMenuClicked;
					contextMenu.Items.RemoveAt(i);
				}
				break;
		}
	}

	private readonly ContextMenu MenuStringRes = new();
	private void SetupStringResMenu() {
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		AccessoryDataUtil.SetupStringResMenu(MenuStringRes, (item) => item.Click += OnMenuClicked);
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => binding.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent;
		if (cm == MenuStringRes) {
			if (item.Name.Equals("openLocalization")) {
				AccessoryDataUtil.OpenLocalization(this, SetupStringResMenu);
			} else {
				AccessoryDataUtil.ApplyStringRes(TextDisplayName, item.Name);
			}
		} else if (cm == MenuOthers) {
			AccessoryDataUtil.SetOtherItem(item);
		} else {
			Truck truck = (Truck)item.DataContext;
			if (cm == MenuModelType) {
				truck.ModelType = item.Name;
			} else if (cm == MenuLook) {
				truck.Look = item.Name;
			} else if (cm == MenuVariant) {
				truck.Variant = item.Name;
			} else
				return;
		}
		//TextBox caller = cm.PlacementTarget;
	}

	private void SaveOnClosing(object? sender, CancelEventArgs e) => binding.SaveHistory();

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonIconNameClear)
			binding.IconName = "";
		else if (sender == ButtonChooseModelClear)
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
		void ForeachValue(Action<Truck> value, bool? extraETS2 = true, bool? extraATS = true) {
			if (TruckExpandedETS2 && (extraETS2 ?? true)) {
				foreach (Truck t in TrucksETS2) {
					if (t.Check)
						value(t);
				}
			}
			if (TruckExpandedATS && (extraATS ?? true)) {
				foreach (Truck t in TrucksATS) {
					if (t.Check)
						value(t);
				}
			}
		}
		if (sender == ButtonCoverModelType) {
			var acc = binding.CurrentModelType;
			ForeachValue((t) => t.ModelType = binding.ModelType, acc?.ForETS2, acc?.ForATS);
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