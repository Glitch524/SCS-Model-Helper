using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.PhysicsToy {
	/// <summary>
	/// PhysicsToyUC.xaml 的交互逻辑
	/// </summary>
	public partial class PhysicsToyUC: UserControl {
		private readonly PhysicsToyViewModel ViewModel = new();

		private Window? mWindow;
		private Window Window {
			get {
				mWindow ??= Window.GetWindow(this);
				return mWindow;
			}
		}

		private static readonly DependencyProperty CurrentPhysicsItemProperty = DependencyProperty.Register(
			"CurrentPhysicsItem",
			typeof(PhysicsToyData),
			typeof(PhysicsToyUC));

		public PhysicsToyData? CurrentPhysicsItem {
			get => (PhysicsToyData)GetValue(CurrentPhysicsItemProperty); set => SetValue(CurrentPhysicsItemProperty, value);
		}

		private static readonly DependencyProperty LocalPhysicsProperty = DependencyProperty.Register(
			"LocalPhysics", 
			typeof(bool), 
			typeof(PhysicsToyUC));

		public bool LocalPhysics {
			get => (bool)GetValue(LocalPhysicsProperty); set => SetValue(LocalPhysicsProperty, value);
		}

		private static readonly RoutedEvent AddToOthersClickEvent = EventManager.RegisterRoutedEvent(
			"AddToOthersClick",
			RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(PhysicsToyUC));

		public event RoutedEventHandler? AddToOthersClick { 
			add => AddHandler(AddToOthersClickEvent, value); remove => RemoveHandler(AddToOthersClickEvent, value);
		}

		public SuiItem? CurrentSuiItem {
			get => ViewModel.CurrentSuiItem; set => ViewModel.CurrentSuiItem = value;
		}

		public string RopeMaterial {
			get => ViewModel.RopeMaterial; set => ViewModel.RopeMaterial = value;
		}

		public PhysicsToyUC() {
			InitializeComponent();
			//SetupInstanceOffsetMenu();

			GridMain.DataContext = ViewModel;

			ViewModel.physicsCallback += PhysicsCallback;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			ViewModel.AllVisible = LocalPhysics;
			if (LocalPhysics) {
				ViewModel.CurrentPhysicsItem = ViewModel.PhysicsItems.FirstOrDefault();
				PanelSaveButton.Visibility = Visibility.Collapsed;
			}
		}

		public void PhysicsCallback(PhysicsToyData? physics) => CurrentPhysicsItem = physics;

		public void PhysicsListAdd(PhysicsToyData physics) => ViewModel.PhysicsListAdd(physics);

		private void ButtonPhysicsClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonAddToOthers) {
				RoutedEventArgs args = new(AddToOthersClickEvent);
				RaiseEvent(args);
			} else if (sender == ButtonSaveToPhysList) {
				var physics = ViewModel.CurrentPhysicsItem;
				if (physics == null)
					return;
				AccAppIO.AddPhysicsToList(physics);
				MessageBox.Show(Util.GetString("MessageResultPhysSaved"));
			}
		}

		public static void SavePhysicsList() => PhysicsToyViewModel.SavePhysicsList();

		private void ButtonAddRowClick(object sender, RoutedEventArgs e) => ViewModel.PhysicsItems?.Add(new());

		private void ButtonDeleteRowClick(object sender, RoutedEventArgs e) {
			if (ViewModel.CurrentPhysicsItem != null)
				ViewModel.PhysicsItems?.Remove(ViewModel.CurrentPhysicsItem);
		}
		private void SortButtonClick(object sender, RoutedEventArgs e) {
			MoveItem(sender == ButtonPhysicUp, TablePhysicDatas, ViewModel.PhysicsItems!);
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

		private void ButtonChooseModelClick(object sender, RoutedEventArgs e) {
			try {
				ViewModel.ChooseModel(sender == ButtonChoosePhysicsColl);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonPhysicsModelPathClear) {
				ViewModel.ModelPath = "";
			} else if (sender == ButtonPhysicsCollPathClear) {
				ViewModel.CollPath = "";
			} else if (sender == ButtonRopeMaterialClear) {
				ViewModel.RopeMaterial = "";
			} else
				return;
		}

		private void NumberOnly(object sender, TextCompositionEventArgs e) => TextControl.NumberOnly(sender, e);

		private void FloatOnly(object sender, TextCompositionEventArgs e) => TextControl.FloatOnly(sender, e);

		private ContextMenu? MenuInstanceOffset => CurrentPhysicsItem?.MenuInstanceOffset;

		private void ButtonInstanceOffsetClicked(object sender, RoutedEventArgs e) {
			if (sender == ButtonInstanceOffsetAdd) {
				CurrentPhysicsItem?.AddMenuInstanceOffset();
			} else if (sender == ButtonInstanceOffsetRemove) {
				CurrentPhysicsItem?.RemoveMenuInstanceOffset();
			} else if (sender == ButtonInstanceOffsetList) {
				if (MenuInstanceOffset != null)
					MenuInstanceOffset.IsOpen = true;
			}
		}

		private void ButtonChooseRopeClick(object sender, RoutedEventArgs e) {
			var mat = AccessoryDataUtil.ChooseRope();
			if (mat != null)
				RopeMaterial = mat;
		}
	}
}
