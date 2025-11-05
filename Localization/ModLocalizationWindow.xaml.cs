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
	private readonly LocaleBinding binding = new();

	public static string ProjectLocation => Instances.ProjectLocation;

	private ObservableCollection<LocaleModule> Modules => binding.LocaleModules;
	private ObservableCollection<LocaleModule> DeletedModules => binding.DeletedModules;

	private LocaleModule CurrentModule {
		get => binding.CurrentModule!; set => binding.CurrentModule = value;
	}

	public ObservableCollection<LocalePair> CurrentDict => binding.CurrentDict!;
	public ObservableCollection<LocalePair> UniversalDict => binding.UniversalDict!;

	public bool HasChanges = false;
	public ModLocalizationWindow()
    {
        InitializeComponent();
		GridMain.DataContext = binding;

		Loaded += OnLoaded;
    }

	private void OnLoaded(object sender, RoutedEventArgs e) {
		var task = Task.Run(() => AccDataIO.ReadLocaleDict(this, Modules));
		task.Wait();
		Modules.CollectionChanged += ModulesCollectionChanged;
		foreach (var module in Modules) {
			MenuModule.Items.Add(NewModuleItem(module));
		}
		binding.CurrentModule = null;//参考LocaleBinding内的CurrentModule 设为空时会获取LocaleModules列表的第一个module
	}


	private readonly ContextMenu MenuModule = new();

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
			Header = module.ModuleName.Replace("_", "__"),
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
				binding.SetModuleNull();
			}
		}
	}

	private void ButtonSyncingClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSyncWithUni) {
			binding.SyncWithUniversal(this);
		} else if (sender == ButtonSyncOrder) {
			binding.SyncOrder();
		}
	}

	private void DeleteLocaleClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			ModLocale locale = (ModLocale)button.DataContext;
			locale.Dictionary.Clear();
		}
	}

	private void OperateButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonAdd) {
			CollectionUtil.AddItem(TableDict, CurrentDict, new("",""));
		} else if (sender == ButtonDelete) {
			CollectionUtil.RemoveItem(TableDict, CurrentDict);
		} else if (sender == ButtonUp || sender == ButtonDown) {
			CollectionUtil.MoveDataGridItems(sender == ButtonUp, TableDict, CurrentDict);
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
		binding.CleanSameDict();
	}
}