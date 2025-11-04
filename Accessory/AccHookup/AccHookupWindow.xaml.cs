using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccHookup;

/// <summary>
/// AddonHookup.xaml 的交互逻辑
/// </summary>
public partial class AccHookupWindow: BaseWindow {
	private readonly AccHookupBinding Binding = new();

	private readonly ContextMenu MenuStringRes, MenuSuitableFor;

	public AccHookupWindow() {
		InitializeComponent();

		GridMain.DataContext = Binding;

		MenuStringRes = (ContextMenu)Resources["MenuStringRes"];
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		MenuStringRes.DataContext = Binding;
		MenuSuitableFor = (ContextMenu)Resources["MenuSuitableFor"];

		Binding.SuiChanged += OnSuiChanged;

		Loaded += OnLoaded;

	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
		Task.Run(Binding.LoadFiles);
	}

	void OnSuiChanged(SuiItem? suiItem) => UCPhysics.CurrentSuiItem = suiItem;

	private void ButtonAddRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiAdd) {
			SuiItem newItem = new("new_sui");
			Binding.SuiItems.Add(newItem);
			Binding.CurrentSuiItem = newItem;
			TableSui.Focus();
		}
	}

	private void ButtonDeleteRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiRemove) {
			if (Binding.CurrentSuiItem != null)
				Binding.SuiItems.Remove(Binding.CurrentSuiItem);
		} else if (sender == ButtonToyRemove) {
			if (Binding.CurrentHookupItem != null)
				Binding.Hookups?.Remove(Binding.CurrentHookupItem);
		}
	}

	private void SortButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiUp || sender == ButtonSuiDown) {
			CollectionUtil.MoveDataGridItems(sender == ButtonSuiUp, TableSui, Binding.SuiItems);
		} else if (sender == ButtonToyUp || sender == ButtonToyDown) {
			CollectionUtil.MoveListBoxItems(sender == ButtonToyUp, ListBoxHookup, Binding.Hookups!);
		}
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => Binding.CheckNameStringRes();

	private void OnMenuClicked(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		ContextMenu cm = (ContextMenu)item.Parent ?? item.ContextMenu;
		if (cm == MenuStringRes) {
			if (item.Name.Equals("openLocalization")) {
				AccessoryDataUtil.OpenLocalization(this);
			} else
				AccessoryDataUtil.ApplyStringRes(TextDisplayName, (string)item.Tag);
		} else if (cm == MenuSuitableFor) {
			var newSF = (string)item.Tag;
			Binding.NewSuitableFor = newSF;
			Binding.AddNewSuitableFor();
			TextSuitableFor.Focus();
		}
	}

	private void ButtonChooseIcon(object sender, RoutedEventArgs e) => Binding.ChooseIcon(this);

	private void ButtonChooseModelClick(object sender, RoutedEventArgs e) => Binding.ChooseModel(sender == ButtonChooseColl);

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonIconNameClear) {
			Binding.IconName = "";
		} else if (sender == ButtonModelPathClear) {
			Binding.ModelPath = "";
		} else if (sender == ButtonCollPathClear) {
			Binding.CollPath = "";
		} else
			return;
	}

	private void NumberOnly(object sender, TextCompositionEventArgs e) => TextControl.NumberOnly(sender, e);
	private void ButtonListClick(object sender, RoutedEventArgs e) {
		TextBox target;
		if (sender == ButtonAddData) {
			target = TextData;
			Binding.AddNewData();
		} else if (sender == ButtonAddSuitableFor) {
			target = TextSuitableFor;
			Binding.AddNewSuitableFor();
		} else if (sender == ButtonAddConflictWith) {
			target = TextConflictWith;
			Binding.AddNewConflictWith();
		} else if (sender == ButtonAddDefaults) {
			target = TextDefaults;
			Binding.AddNewDefaults();
		} else if (sender == ButtonAddOverrides) {
			target = TextOverrides;
			Binding.AddNewOverrides();
		} else if (sender == ButtonAddRequire) {
			target = TextRequire;
			Binding.AddNewRequire();
		} else
			return;
		target.Focus();
	}

	private void ListDataGotFocus(object sender, RoutedEventArgs e) => ShowPopupListData((TextBox)sender);

	private void ListDataLostFocus(object sender, RoutedEventArgs e) => PopupListData.IsOpen = false;

	private void ButtonPhysicsClick(object sender, RoutedEventArgs e) {
		PhysicsWindow physicsWindow = new() {
			Owner = this,
			ChooseMode = true,
		};
		if (physicsWindow.ShowDialog() == true) {
			var selected = physicsWindow.SelectedPhysicsData;
			Binding.AddNewData(selected);
			TextData.Focus();

			UCPhysics.PhysicsListAdd(selected);
		}
	}

	private void AddToDataClick(object sender, RoutedEventArgs e) {
		var newPhys = UCPhysics.CurrentPhysicsItem;
		Binding.AddNewData(newPhys);
		TextData.Focus();
	}

	private void ButtonSuitableForClick(object sender, RoutedEventArgs e) {
		MenuSuitableFor.PlacementTarget = (Button)sender;
		MenuSuitableFor.IsOpen = true;
	}

	private void ShowPopupListData(TextBox target) {
		if (Binding.ListDataUC == null) {
			Binding.ListDataUC = new();
			PopupListData.Child = Binding.ListDataUC;
		}
		Binding.OpeningList = target.Name;
		PopupListData.PlacementTarget = target;
		PopupListData.IsOpen = true;
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSave) {
			Binding.SaveFiles();
		}
	}
}