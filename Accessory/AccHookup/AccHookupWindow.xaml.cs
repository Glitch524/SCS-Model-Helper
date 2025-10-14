using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.AccHookup;

/// <summary>
/// AddonHookup.xaml 的交互逻辑
/// </summary>
public partial class AccHookupWindow: BaseWindow {
	private readonly AccHookupBinding Binding = new();

	public ObservableCollection<OthersItem>? OthersList => Binding.OthersList;

	private readonly ContextMenu MenuOthers;

	public AccHookupWindow() {
		InitializeComponent();

		GridMain.DataContext = Binding;

		MenuOthers = (ContextMenu)Resources["MenuOthers"];

		Binding.SuiChanged += OnSuiChanged;

		Binding.LoadFiles();

		SetupStringResMenu();
	}

	void OnSuiChanged(SuiItem? suiItem) => UCPhysics.CurrentSuiItem = suiItem;

	private void ButtonAddRowClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSuiAdd) {
			SuiItem newItem = new("new_sui");
			Binding.SuiItems.Add(newItem);
			Binding.CurrentSuiItem = newItem;
		} else if (sender == ButtonToyAdd) {
			AccessoryHookupData newItem = new();
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
			DataGridUtil.MoveItems(sender == ButtonSuiUp, TableSui, Binding.SuiItems);
		} else if (sender == ButtonToyUp || sender == ButtonToyDown) {
			DataGridUtil.MoveItems(sender == ButtonToyUp, TableToyDatas, Binding.Hookups!);
		}
	}

	private readonly ContextMenu MenuStringRes = new();
	private void SetupStringResMenu() {
		MenuStringRes.PlacementTarget = ButtonChooseRes;
		AccessoryDataUtil.SetupStringResMenu(MenuStringRes, (item) => item.Click += OnMenuClicked);
	}

	private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

	private void CheckStringRes(object sender, RoutedEventArgs e) => Binding.CheckNameStringRes();

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
			UCPhysics.PhysicsListAdd(selected);
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