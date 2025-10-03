using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Localization;

/// <summary>
/// ModLocalization.xaml 的交互逻辑
/// </summary>
public partial class ModLocalizationWindow : BaseWindow
{

	private readonly LocaleInfo LocaleInfo = new();

	public static string ProjectLocation => Instances.ProjectLocation;

	private ObservableCollection<LocaleModule> Modules => LocaleInfo.LocaleModules;
	private ObservableCollection<LocaleModule> DeletedModules => LocaleInfo.DeletedModules;

	private LocaleModule CurrentModule {
		get => LocaleInfo.CurrentModule!; set => LocaleInfo.CurrentModule = value;
	}

	public ObservableCollection<LocalePair> CurrentDict => LocaleInfo.CurrentDict!;
	public ObservableCollection<LocalePair> UniversalDict => LocaleInfo.UniversalDict!;

	public bool HasChanges = false;
	public ModLocalizationWindow()
    {
        InitializeComponent();
		LoadLocale();

		GridMain.DataContext = LocaleInfo;
		SetupMenuModule();
    }

	private void LoadLocale() => AccDataIO.ReadLocaleDict(this, Modules);

	private readonly ContextMenu MenuModule = new();
	private void SetupMenuModule() {
		Modules.CollectionChanged += ModulesCollectionChanged;
		foreach (var module in Modules) {
			MenuModule.Items.Add(NewModuleItem(module));
		}
	}

	private void OnModuleChanged(object sender, RoutedEventArgs e) {
		MenuItem item = (MenuItem)sender;
		//ContextMenu cm = (ContextMenu)item.Parent;
		CurrentModule = (LocaleModule)item.DataContext;
	}

	private void ModulesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
		switch (e.Action) {
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				var index = e.NewStartingIndex;
				var module = Modules[index];
				MenuModule.Items.Insert(index, NewModuleItem(module));
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				index = e.OldStartingIndex;
				MenuItem item = (MenuItem)MenuModule.Items[index];
				item.Click -= OnModuleChanged;
				MenuModule.Items.RemoveAt(index);
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				for (int i = MenuModule.Items.Count - 1; i >= 0; i--) {
					MenuItem mItem = (MenuItem)MenuModule.Items[i];
					mItem.Click -= OnModuleChanged;
					MenuModule.Items.RemoveAt(i);
				}
				break;
		}
	}

	private MenuItem NewModuleItem(LocaleModule module) {
		MenuItem item = new() {
			Header = module.ModuleName,
			DataContext = module,
		};
		item.Click += OnModuleChanged;
		return item;
	}

	private void ButtonModuleClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonChooseModule) {
			MenuModule.IsOpen = true;
		} else if (sender == ButtonAddModule) {
			var newModule = new LocaleModule("new_translation");
			Modules.Add(newModule);
			CurrentModule = newModule;
		} else if (sender == ButtonDeleteModule) {
			var result = MessageBox.Show(GetString("MessageDeleteLocale"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				DeletedModules.Add(CurrentModule);
				Modules.Remove(CurrentModule);
				LocaleInfo.SetModuleNull();
			}
		}
	}

	private void ButtonSyncingClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSyncWithUni) {
			LocaleInfo.SyncWithUniversal(this);
		} else if (sender == ButtonSyncOrder) {
			LocaleInfo.SyncOrder();
		}
	}

	private void DeleteLocalClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			ModLocale locale = (ModLocale)button.DataContext;
			locale.Dictionary.Clear();
		}
	}

	private void OperateButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonAdd) {
			if (TableDict.SelectedIndex == -1) {
				CurrentDict.Add(new("", ""));
			} else
				CurrentDict.Insert(TableDict.SelectedIndex, new("",""));
		} else if (sender == ButtonDelete) {
			if (TableDict.SelectedIndex == -1) 
				return;
			while (TableDict.SelectedItem != null) {
				CurrentDict.Remove((LocalePair)TableDict.SelectedItem);
			}
		} else if (sender == ButtonUp || sender == ButtonDown) {
			MoveItem(sender == ButtonUp);
		}
	}

	private void MoveItem(bool up) {
		if (TableDict.SelectedIndex == -1)
			return;
		int target = TableDict.SelectedIndex;
		List<LocalePair> l = [];
		while (TableDict.SelectedIndex != -1) {
			if (TableDict.SelectedIndex < target) {
				target--;
			}
			l.Add(CurrentDict[TableDict.SelectedIndex]);
			CurrentDict.RemoveAt(TableDict.SelectedIndex);
		}
		if (up) {
			if (target > 0)
				target--;
		} else if (target < CurrentDict.Count)
			target++;
		var IList = TableDict.SelectedItems;
		foreach (LocalePair pair in l) {
			CurrentDict.Insert(target, pair);
			IList.Add(pair);
			target++;
		}
	}

	private void ButtonCloseClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSave) {
			if (UniversalDict.Count > 0) 
				RunSave();
			else {
				var result = MessageBox.Show(GetString("MessageUniEmpty"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
					RunSave();
			}
		}
	}
	private void RunSave() {
		AccDataIO.SaveLocaleDict(ProjectLocation, Modules, DeletedModules);
		HasChanges = true;
		MessageBox.Show(this, GetString("MessageSaveSuccess"));
	}


	private void ButtonCleanClick(object sender, RoutedEventArgs e) {
		LocaleInfo.CleanSameDict();
	}
}