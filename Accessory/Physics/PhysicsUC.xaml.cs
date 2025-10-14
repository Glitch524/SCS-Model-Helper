using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCS_Mod_Helper.Accessory.Physics {
	/// <summary>
	/// PhysicsToyUC.xaml 的交互逻辑
	/// </summary>
	public partial class PhysicsUC: UserControl {
		private readonly PhysicsBinding Binding = new();

		private static readonly DependencyProperty CurrentPhysicsItemProperty = DependencyProperty.Register(
			"CurrentPhysicsItem",
			typeof(PhysicsData),
			typeof(PhysicsUC));

		public PhysicsData? CurrentPhysicsItem {
			get => (PhysicsData)GetValue(CurrentPhysicsItemProperty); set => SetValue(CurrentPhysicsItemProperty, value);
		}

		private static readonly DependencyProperty LocalPhysicsProperty = DependencyProperty.Register(
			"LocalPhysics", 
			typeof(bool), 
			typeof(PhysicsUC));

		private PhysicsToyData? CurrentToyData {
			get => Binding.CurrentToyData; set => Binding.CurrentToyData = value;
		}

		private PhysicsPatchData? CurrentPatchData {
			get => Binding.CurrentPatchData; set => Binding.CurrentPatchData = value;
		}

		public bool LocalPhysics {
			get => (bool)GetValue(LocalPhysicsProperty); set => SetValue(LocalPhysicsProperty, value);
		}

		private static readonly RoutedEvent AddToOthersClickEvent = EventManager.RegisterRoutedEvent(
			"AddToOthersClick",
			RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(PhysicsUC));

		public event RoutedEventHandler? AddToOthersClick { 
			add => AddHandler(AddToOthersClickEvent, value); remove => RemoveHandler(AddToOthersClickEvent, value);
		}

		public SuiItem? CurrentSuiItem {
			get => Binding.CurrentSuiItem; set => Binding.CurrentSuiItem = value;
		}

		public string RopeMaterial {
			get => Binding.RopeMaterial; set => Binding.RopeMaterial = value;
		}

		public string PatchMaterial {
			get => Binding.PatchMaterial; set => Binding.PatchMaterial = value;
		}

		private readonly ContextMenu MenuPhysicsType;

		public PhysicsUC() {
			InitializeComponent();

			GridMain.DataContext = Binding;

			MenuPhysicsType = (ContextMenu)Resources["MenuPhysicsType"];

			Binding.physicsCallback += PhysicsCallback;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (LocalPhysics) {
				GridAngular.Visibility = Visibility.Visible;
				GridRope.Visibility = Visibility.Visible;
				Grid.SetRow(GridRope, 2);

				Binding.CurrentPhysicsItem = Binding.PhysicsItems.FirstOrDefault();
				PanelSaveButton.Visibility = Visibility.Collapsed;
			}
		}

		public void PhysicsCallback(PhysicsData? physics) => CurrentPhysicsItem = physics;

		public void PhysicsListAdd(PhysicsData physics) => Binding.PhysicsListAdd(physics);

		private void ButtonPhysicsClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonAddToOthers) {
				RoutedEventArgs args = new(AddToOthersClickEvent);
				RaiseEvent(args);
			} else if (sender == ButtonSaveToPhysList) {
				var physics = Binding.CurrentPhysicsItem;
				if (physics == null)
					return;
				AccAppIO.AddPhysicsToList(physics);
				MessageBox.Show(Util.GetString("MessageResultPhysSaved"));
			}
		}

		public static void SavePhysicsList() => AccAppIO.SavePhysicsList();

		private void ButtonAddRowClick(object sender, RoutedEventArgs e) => MenuPhysicsType.IsOpen = true;
		private void AddPhysicDataClick(object sender, RoutedEventArgs e) {
			MenuItem item = (MenuItem)sender;
			ContextMenu cm = (ContextMenu)item.Parent;
			if (cm == MenuPhysicsType) {
				if (item.Name == "PhysicsToyData") {
					Binding.PhysicsItems?.Add(new PhysicsToyData());
				} else if (item.Name == "PhysicsPatchData") {
					Binding.PhysicsItems?.Add(new PhysicsPatchData());
				}
			}
		}

		private void ButtonDeleteRowClick(object sender, RoutedEventArgs e) {
			if (Binding.CurrentPhysicsItem != null)
				Binding.PhysicsItems?.Remove(Binding.CurrentPhysicsItem);
		}
		private void SortButtonClick(object sender, RoutedEventArgs e) {
			DataGridUtil.MoveItems(sender == ButtonPhysicUp, TablePhysicDatas, Binding.PhysicsItems!);
		}

		private void ButtonChooseModelClick(object sender, RoutedEventArgs e) {
			try {
				Binding.ChooseModel(sender == ButtonChoosePhysicsColl);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonPhysicsModelPathClear) {
				Binding.ModelPath = "";
			} else if (sender == ButtonPhysicsCollPathClear) {
				Binding.CollPath = "";
			} else if (sender == ButtonRopeMaterialClear) {
				Binding.RopeMaterial = "";
			} else if (sender == ButtonPatchMaterialClear) {
				Binding.PatchMaterial = "";
			} else 
				return;
		}

		private void NumberOnly(object sender, TextCompositionEventArgs e) => TextControl.NumberOnly(sender, e);

		private void FloatOnly(object sender, TextCompositionEventArgs e) => TextControl.FloatOnly(sender, e);

		private ContextMenu? MenuInstanceOffset => CurrentToyData?.MenuInstanceOffset;

		private void ButtonInstanceOffsetClicked(object sender, RoutedEventArgs e) {
			if (sender == ButtonInstanceOffsetAdd) {
				CurrentToyData?.AddMenuInstanceOffset();
			} else if (sender == ButtonInstanceOffsetRemove) {
				CurrentToyData?.RemoveMenuInstanceOffset();
			} else if (sender == ButtonInstanceOffsetList) {
				if (MenuInstanceOffset != null)
					MenuInstanceOffset.IsOpen = true;
			}
		}

		private void ButtonChooseMaterialClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonChooseRopeMaterial) {
				var mat = AccessoryDataUtil.ChooseRope();
				if (mat != null)
					RopeMaterial = mat;
			} else if (sender == ButtonChoosePatchMaterial) {
				var mat = AccessoryDataUtil.ChoosePatch();
				if (mat != null)
					PatchMaterial = mat;
			}
		}
	}
}
