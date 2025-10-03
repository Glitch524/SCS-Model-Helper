using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccHookup;

/// <summary>
/// AddonHookup.xaml 的交互逻辑
/// </summary>
public partial class AccHookupWindow: BaseWindow {
	private readonly AccHookupViewModel ViewModel = new();

	public ObservableCollection<OthersItem>? OthersList => ViewModel.OthersList;

	private readonly ContextMenu MenuOthers;

	public AccHookupWindow() {
		InitializeComponent();

		GridMain.DataContext = ViewModel;

		MenuOthers = (ContextMenu)Resources["MenuOthers"];

		ViewModel.SuiChanged += OnSuiChanged;

		ViewModel.LoadFiles();

		SetupStringResMenu();
	}

	void OnSuiChanged(SuiItem? suiItem) => UCPhysics.CurrentSuiItem = suiItem;

	private void ButtonAddRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiAdd) {
			SuiItem newItem = new("new_sui");
			ViewModel.SuiItems.Add(newItem);
			ViewModel.CurrentSuiItem = newItem;
		} else if (sender == ButtonToyAdd) {
			AccessoryHookupItem newItem = new();
			ViewModel.Hookups?.Add(newItem);
			ViewModel.CurrentHookupItem = newItem;
		}
	}

	private void ButtonDeleteRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiRemove) {
			if (ViewModel.CurrentSuiItem != null)
				ViewModel.SuiItems.Remove(ViewModel.CurrentSuiItem);
		} else if (sender == ButtonToyRemove) {
			if (ViewModel.CurrentHookupItem != null)
				ViewModel.Hookups?.Remove(ViewModel.CurrentHookupItem);
		}
	}

	private void SortButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiUp || sender == ButtonSuiDown) {
			MoveItem(sender == ButtonSuiUp, TableSui, ViewModel.SuiItems);
		} else if (sender == ButtonToyUp || sender == ButtonToyDown) {
			MoveItem(sender == ButtonToyUp, TableToyDatas, ViewModel.Hookups!);
		}
	}

	private static void MoveItem<T>(bool up, DataGrid table, ObservableCollection<T> list) {
		if (table.SelectedIndex == -1)
			return;
		var index = table.SelectedIndex;
		var selected = (T)table.SelectedItem;
		list.RemoveAt(index);
		int target = index;
		if (up) {
			if (target > 0) 
				target--;
		} else {
			if (target < list.Count)
				target++;
		}
		list.Insert(target, selected);
		table.SelectedIndex = target;
	}

	private readonly ContextMenu MenuStringRes = new();
	private void SetupStringResMenu() {
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		AccessoryDataUtil.SetupStringResMenu(MenuStringRes, (item) => item.Click += OnMenuClicked);
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

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
				ViewModel.DisplayName = ViewModel.DisplayName.Insert(start, $"@@{item.Name}@@");
			}
		} else if (cm == MenuOthers) {
			OthersItem o = (OthersItem)item.DataContext;
			var name = (string)item.Tag;
			o.OthersName = name;
			o.OthersNameTip = (string)item.Header;
		}
	}

	private void ButtonChooseIcon(object sender, RoutedEventArgs e) => ViewModel.ChooseIcon(this);

	private void ButtonChooseModelClick(object sender, RoutedEventArgs e) => ViewModel.ChooseModel(sender == ButtonChooseColl);

	private void ButtonClearClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonIconNameClear) {
			ViewModel.IconName = "";
		} else if (sender == ButtonModelPathClear) {
			ViewModel.ModelPath = "";
		} else if (sender == ButtonCollPathClear) {
			ViewModel.CollPath = "";
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
			UCPhysics.PhysicsListAdd(selected);
		}
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSave) {
			ViewModel.SaveFiles();
		}
	}

	private void AddToOthersClick(object sender, RoutedEventArgs e) => ViewModel.AddPhysToOthersList(UCPhysics.CurrentPhysicsItem);
}