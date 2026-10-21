using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Vehicles {
	/// <summary>
	/// DBMigrationWindow.xaml 的交互逻辑
	/// </summary>
	public partial class DBMigrationWindow: BaseWindow {
		private readonly DBMigrationBinding Binding;
		private readonly bool VeEdited, VaEdited;

		public Dictionary<string, Vehicle> KeepVehiclesETS2 => Binding.KeepETS2;
		public Dictionary<string, Vehicle> KeepVehiclesATS => Binding.KeepATS;

		public DBMigrationWindow(int oldVersion, int newVersion, bool veEdited, bool vaEdited) {
			InitializeComponent();
			Binding = new(oldVersion, newVersion, veEdited, vaEdited);
			GridMain.DataContext = Binding;
			VeEdited = veEdited;
			VaEdited = vaEdited;

			Closing += OnWindowClosing;
		}

		private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e) {
			var result = MessageBox.Show(this, GetString("MessageDBClose"), GetString("MessageTitleWarning"), MessageBoxButton.YesNo);
			if (result == MessageBoxResult.No) {
				e.Cancel = true;
			}
		}

		protected override void OnContentRendered(EventArgs e) {
			base.OnContentRendered(e);
			if (VeEdited) {
				VehiclesIO.LoadVehiclesUpdate(true, Binding.OldVehiclesETS2, false, true);
				VehiclesIO.LoadVehiclesUpdate(true, Binding.NewVehiclesETS2, true, false);
			}
			if (VaEdited) {
				VehiclesIO.LoadVehiclesUpdate(false, Binding.OldVehiclesATS, false, true);
				VehiclesIO.LoadVehiclesUpdate(false, Binding.NewVehiclesATS, true, false);
			}
		}

		private void ButtonAddRemoveClick(object sender, RoutedEventArgs e) {
			if (Binding.ETS2Selected) {
				AddRemoveVehicle((Button)sender, Binding.OldVehiclesETS2, Binding.NewVehiclesETS2, Binding.KeepETS2);
			} else {
				AddRemoveVehicle((Button)sender, Binding.OldVehiclesATS, Binding.NewVehiclesATS, Binding.KeepATS);
			}
		}

		

		private void AddRemoveVehicle(
			Button sender,
			ObservableCollection<Vehicle> oldVehicles,
			ObservableCollection<Vehicle> newVehicles,
			Dictionary<string, Vehicle> keepVehicles) {
			if (sender == ButtonAdd) {
				if (oldVehicles.Count == 0)
					return;
				TableNew.SelectedItems.Clear();
				var items = TableOld.SelectedItems;
				Vehicle? lastV = null;
				while (items.Count > 0) {
					Vehicle vehicle = (Vehicle)items[0]!;
					oldVehicles.Remove(vehicle);

					keepVehicles[vehicle.VehicleID] = vehicle;
					newVehicles.Add(vehicle);
					TableNew.SelectedItems.Add(vehicle);
					lastV = vehicle;
				}
				if (lastV != null)
					TableNew.ScrollIntoView(lastV);
			} else if (sender == ButtonRemove) {
				if (keepVehicles.Count == 0)
					return;
				var items = TableNew.SelectedItems;
				Vehicle? lastV = null;
				while (items.Count > 0) {
					Vehicle vehicle = (Vehicle)items[0]!;
					if (keepVehicles.Remove(vehicle.VehicleID)) {

						newVehicles.Remove(vehicle);

						oldVehicles.Add(vehicle);
						lastV = vehicle;

						TableOld.SelectedItems.Add(vehicle);
					} else
						items.RemoveAt(0);
				}
				if (lastV != null)
					TableOld.ScrollIntoView(lastV);
			} else if (sender == ButtonAddAll) {
				if (oldVehicles.Count == 0)
					return;
				foreach (var vehicle in oldVehicles) {
					newVehicles.Add(vehicle);
					keepVehicles.Add(vehicle.VehicleID, vehicle);
					TableNew.SelectedItems.Add(vehicle);
				}
				oldVehicles.Clear();
				TableNew.ScrollIntoView(keepVehicles.Last().Value);
			} else if (sender == ButtonRemoveAll) {
				if (keepVehicles.Count == 0)
					return;
				foreach (var vehicle in keepVehicles.Values) {
					newVehicles.Remove(vehicle);
					oldVehicles.Add(vehicle);
					TableOld.SelectedItems.Add(vehicle);
				}
				keepVehicles.Clear();
				TableOld.ScrollIntoView(oldVehicles.Last());
			}
			if (Binding.ETS2Selected)
				Binding.InvokeChange(nameof(Binding.KeepETS2Count));
			else
				Binding.InvokeChange(nameof(Binding.KeepATSCount));
		}

		private void ButtonFinishClick(object sender, RoutedEventArgs e) {
			StringBuilder message = new(GetString("MessageDBFinish1"));
			if (Binding.OldVehiclesETS2.Count + Binding.OldVehiclesATS.Count > 0)
				message.Append('\n').Append(GetString("MessageDBFinishWithUnselected"));

			var result = MessageBox.Show(this, message.ToString(), GetString("TitleVehicleMigration"), MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				DialogResult = true;
				Close();
			}
		}
	}
}
