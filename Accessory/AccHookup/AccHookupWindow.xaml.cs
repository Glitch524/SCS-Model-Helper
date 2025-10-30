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

	public ObservableCollection<OthersItem>? OthersList => Binding.OthersList;

	private readonly ContextMenu MenuStringRes ,MenuOthers, MenuSuitableFor;

	public AccHookupWindow() {
		InitializeComponent();

		GridMain.DataContext = Binding;

		MenuStringRes = (ContextMenu)Resources["MenuStringRes"];
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		MenuStringRes.DataContext = Binding;
		MenuOthers = (ContextMenu)Resources["MenuOthers"];
		MenuSuitableFor = (ContextMenu)Resources["MenuSuitableFor"];

		Binding.SuiChanged += OnSuiChanged;

		Binding.LoadFiles();
	}

	void OnSuiChanged(SuiItem? suiItem) => UCPhysics.CurrentSuiItem = suiItem;

	private void ButtonAddRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiAdd) {
			SuiItem newItem = new("new_sui");
			Binding.SuiItems.Add(newItem);
			Binding.CurrentSuiItem = newItem;
			TableSui.Focus();
		} else if (sender == ButtonToyAdd) {
			AccessoryHookupData newItem = new();
			newItem.OthersList.Add(new(AccDataIO.NameSuitableFor, ""));
			Binding.Hookups?.Add(newItem);
			Binding.CurrentHookupItem = newItem;
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
		} else if (cm == MenuOthers) {
			AccessoryDataUtil.SetOtherItem(item);
		} else if (cm == MenuSuitableFor) {
			OthersItem others = (OthersItem)item.DataContext;
			others.OthersValue = (string)item.Tag;
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

	private void ButtonOthersClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonOthersAdd) {
			OthersList?.Add(new());
		} else if (sender == ButtonOthersRemove) {
			OthersItem? item;
			while ((item = TableOthers.SelectedItem as OthersItem) != null) {
				OthersList?.Remove(item);
			}
		}
	}

	private void ButtonPhysDataClick(object sender, RoutedEventArgs e) {
		OthersItem others = (OthersItem)((Button)sender).DataContext;
		if (others.OthersName == AccDataIO.NameData) {
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
				others.OthersValue = physName;
				UCPhysics.PhysicsListAdd(selected);
			}
		} else if (others.OthersName == AccDataIO.NameSuitableFor) {
			MenuSuitableFor.PlacementTarget = (Button)sender;
			MenuSuitableFor.IsOpen = true;
		}
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSave) {
			Binding.SaveFiles();
		}
	}

	private void AddToOthersClick(object sender, RoutedEventArgs e) => Binding.AddPhysToOthersList(UCPhysics.CurrentPhysicsItem);
}