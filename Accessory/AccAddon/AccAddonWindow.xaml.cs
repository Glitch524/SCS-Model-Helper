using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Manifest;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccAddon;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class AccAddonWindow: BaseWindow {
	private readonly AccessoryAddonItem AddonItem = new();
	private readonly ModProject ModelProject = Instances.ModelProject;
	private bool TruckExpandedETS2 {
		get => AddonItem.TruckExpandedETS2; set => AddonItem.TruckExpandedETS2 = value;
	}
	private bool TruckExpandedATS {
		get => AddonItem.TruckExpandedATS; set => AddonItem.TruckExpandedATS = value;
	}

	private ObservableCollection<ModelTypeInfo> AccessoryType => AddonItem.ModelTypes;
	private ObservableCollection<OthersItem> OthersList => AddonItem.OthersList;
	private ObservableCollection<Truck> TrucksETS2 => AddonItem.TrucksETS2;
	private ObservableCollection<Truck> TrucksATS => AddonItem.TrucksATS;

	private readonly ContextMenu MenuModelType, MenuLook, MenuVariant, MenuOthers;

	public AccAddonWindow() {
		InitializeComponent();

		GridMain.DataContext = AddonItem;
		TextProjectLocation.DataContext = ModelProject;

		MenuModelType = (ContextMenu)Resources["MenuModelType"];
		MenuLook = (ContextMenu)Resources["MenuLook"];
		MenuVariant = (ContextMenu)Resources["MenuVariant"];
		MenuOthers = (ContextMenu)Resources["MenuOthers"];

		SetupModelTypeMenu();

		AddonItem.LookList.CollectionChanged += (sender, e) => OnLVListChanged(MenuLook, e);
		AddonItem.VariantList.CollectionChanged += (sender, e) => OnLVListChanged(MenuVariant, e);
		AddonItem.LoadLooksAndVariants();

		OthersItem.ReadLines(AddonItem.OthersList, AccAddonHistory.Default.Others);

		AddonItem.LoadTrucks();

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

	private void CheckStringRes(object sender, RoutedEventArgs e) => AddonItem.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent;
		if (cm == MenuStringRes) {
			if (item.Name.Equals("openLocalization")) {
				var modLocalization = new ModLocalizationWindow() {
					Owner = this
				};
				modLocalization.ShowDialog();
				if (modLocalization.HasChanges)
					SetupStringResMenu();
			} else {
				var start = TextDisplayName.SelectionStart;
				TextDisplayName.SelectedText = "";
				var insert = $"@@{item.Name}@@";
				AddonItem.DisplayName = AddonItem.DisplayName.Insert(start, insert);
				start += insert.Length;
				TextDisplayName.SelectionStart = start;
				TextDisplayName.Focus();
			}
		} else if (cm == MenuOthers) {
			OthersItem o = (OthersItem)item.DataContext;
			var name = (string)item.Tag;
			o.OthersName = name;
			o.OthersNameTip = (string)item.Header;
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

	private void SaveOnClosing(object? sender, CancelEventArgs e) => AddonItem.SaveHistory();

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonIconNameClear)
			AddonItem.IconName = "";
		else if (sender == ButtonChooseModelClear)
			AddonItem.ModelPath = "";
		else if (sender == ButtonChooseModelUKClear)
			AddonItem.ModelPathUK = "";
		else if (sender == ButtonChooseCollClear)
			AddonItem.CollPath = "";
		else
			return;
	}

	private void ButtonChooseIcon(object sender, RoutedEventArgs e) => AddonItem.ChooseIcon(this);

	private void ButtonChooseModelClick(object sender, RoutedEventArgs e) {
		try {
			int type;
			if (sender == ButtonChooseModel)
				type = AccessoryAddonItem.MODEL;
			else if (sender == ButtonChooseModelUK)
				type = AccessoryAddonItem.MODEL_UK;
			else if (sender == ButtonChooseColl)
				type = AccessoryAddonItem.MODEL_COLL;
			else
				return;
			AddonItem.ChooseModel(type);
		} catch(Exception ex) {
			MessageBox.Show(this, ex.Message);
		}
	}

	private void ButtonHideIn(object sender, RoutedEventArgs e) {
		PopupHideIn.IsOpen = true;
	}

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
			AddonItem.HideIn += GetNumber(cb);
		}
	}

	private void HideInUnchecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb) {
			AddonItem.HideIn -= GetNumber(cb);
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
		PhysicsToyWindow physicsWindow = new() {
			Owner = this,
			ChooseMode = true,
		};
		var result = physicsWindow.ShowDialog();
		if (result == true) {
			var selected = physicsWindow.SelectedPhysicsData;
			OthersItem others = (OthersItem)((Button)sender).DataContext;
			var physName = selected.PhysicsName;
			if (!physName.EndsWith(AccDataIO.NamePTSuffix))
				physName += AccDataIO.NamePTSuffix;
			others.OthersValue = physName;
			int i = 0;
			while (i < AddonItem.PhysicsList.Count) {
				var p = AddonItem.PhysicsList[i];
				if (p.PhysicsName == selected.PhysicsName) {
					AddonItem.PhysicsList.Remove(p);
					continue;
				}
				i++;
			}
			AddonItem.PhysicsList.Add(selected);
		}
	}

	private void ButtonStartClick(object sender, RoutedEventArgs e) => AddonItem.StartCreateSii();

	private void ButtonSaveClick(object sender, RoutedEventArgs e) => AddonItem.SaveDED();

	private void ButtonLoadClick(object sender, RoutedEventArgs e) {
		try {
			AddonItem.LoadDED();
			AddonItem.LoadLooksAndVariants();
		} catch (Exception ex) {
			MessageBox.Show(this, GetString("MessageLoadDEDErrFail") + "\n" + ex.Message);
		}
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) => e.Handled = RegexNumber().IsMatch(e.Text);

	[GeneratedRegex("[^0-9]+")]
	private partial Regex RegexNumber();

	private void ButtonTruckInitialize(object sender, RoutedEventArgs e) {
		AddonItem.PopupAddTruckOpen = false;
		AccAddonHistory.Default.TruckHistoryETS2 = "init";
		AccAddonHistory.Default.TruckHistoryATS = "init";
		AccAddonHistory.Default.Save();
		AddonItem.LoadTrucks();
	}

	private void ButtonAddTruckClick(object sender, RoutedEventArgs e) {
		PopupAddTruck.PlacementTarget = (Button)sender;
		AddonItem.AddTruckID = string.Empty;
		AddonItem.AddTruckIngameName = string.Empty;
		AddonItem.AddTruckDescription = string.Empty;
		PopupAddTruck.IsOpen = true;
	}

	private void ButtonAddTruckResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonAddTruckOK) {
			var isETS2 = PopupAddTruck.PlacementTarget == ButtonAddTruckETS2;
			AddonItem.AddNewTruck(isETS2);
		}
		PopupAddTruck.IsOpen = false;
	}

	private void ButtonDeleteTruckClick(object sender, RoutedEventArgs e) {
		if (TableTrucksETS2.SelectedIndex == -1 && TableTrucksATS.SelectedIndex == -1)
			return;
		var changed = false;
		if (sender == ButtonDeleteTruckETS2) {
			while (TableTrucksETS2.SelectedIndex != -1) {
				changed = true;
				var selected = TrucksETS2[TableTrucksETS2.SelectedIndex];
				TrucksETS2.Remove(selected);
			}
			TableTrucksETS2.UnselectAll();
			if (changed)
				AccAddonHistory.Default.TruckHistoryETS2 = Truck.JoinTruck(TrucksETS2);
		} else if (sender == ButtonDeleteTruckATS) {
			while (TableTrucksATS.SelectedIndex != -1) {
				changed = true;
				var selected = TrucksATS[TableTrucksATS.SelectedIndex];
				TrucksATS.Remove(selected);
			}
			TableTrucksATS.UnselectAll();
			if (changed)
				AccAddonHistory.Default.TruckHistoryETS2 = Truck.JoinTruck(TrucksATS);
		}
		AccAddonHistory.Default.Save();
	}

	private void ButtonCoverValue(object sender, RoutedEventArgs e) {
		void ForeachValue(Action<Truck> value, bool extraETS2 = true, bool extraATS = true) {
			if (TruckExpandedETS2 && extraETS2) {
				foreach (Truck t in TrucksETS2) {
					if (t.Check)
						value(t);
				}
			}
			if (TruckExpandedATS && extraATS) {
				foreach (Truck t in TrucksATS) {
					if (t.Check)
						value(t);
				}
			}
		}
		if (sender == ButtonCoverModelType) {
			var acc = (ModelTypeInfo)TextModelType.SelectedItem!;
			ForeachValue((t) => t.ModelType = TextModelType.Text, acc.ForETS2, acc.ForATS);
		} else if (sender == ButtonCoverLook) {
			ForeachValue((t) => t.Look = TextLook.Text);
		} else if (sender == ButtonCoverVariant) {
			ForeachValue((t) => t.Variant = TextVariant.Text);
		}
	}

	private void TruckCheckBoxClick(object sender, RoutedEventArgs e) {
		CheckBox box = (CheckBox)sender;
		Truck selected = (Truck)box.DataContext;
		AddonItem.OverwriteEmpty(selected, selected.Check);
	}
}