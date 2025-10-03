using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.AccAddon {
	/// <summary>
	/// Window1.xaml 的交互逻辑
	/// </summary>
	public partial class NewTruckWindow: BaseWindow {
		private readonly bool IsETS2;
		private readonly ObservableCollection<Truck> Trucks;

		public NewTruckWindow(bool IsETS2, ObservableCollection<Truck> trucks) {
			InitializeComponent();
			this.IsETS2 = IsETS2;
			Trucks = trucks;
			Title = GetString("TitleAddTruck") + " " + GetString(IsETS2 ? "ETS2Short" : "ATSShort");
		}

		private void ButtonClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonOK)
				AddNewTruck();
			Close();
		}

		private void AddNewTruck() {
			var truckID = TextTruckID.Text;
			var ingameName = TextIngameName.Text;
			var description = TextDescription.Text;
			try {
				if (truckID.Length == 0 || ingameName.Length == 0) 
					throw new(GetString("MessageAddErrNotFilled"));
				if (truckID[0] <= 'm') {
					for (int i = 0; i < Trucks.Count; i++) {
						var cResult = string.Compare(truckID, Trucks[i].TruckID);
						if (cResult == 0) {
							throw new(GetString("MessageAddErrSameID"));
						} else if (cResult < 0) {
							Trucks.Insert(i, new(truckID, ingameName, description, false));
							break;
						}
					}
				} else {
					for (int i = Trucks.Count - 1; i >= 0; i--) {
						var cResult = string.Compare(truckID, Trucks[i].TruckID);
						if (cResult == 0) {
							throw new(GetString("MessageAddErrSameID"));
						} else if (cResult > 0) {
							Trucks.Insert(i + 1, new(truckID, ingameName, description, false));
							break;
						}
					}
				}
				if (IsETS2)
					AccAddonHistory.Default.TruckHistoryETS2 = Truck.JoinTruck(Trucks);
				else
					AccAddonHistory.Default.TruckHistoryATS = Truck.JoinTruck(Trucks);
				AccAddonHistory.Default.Save();
				DialogResult = true;
			} catch (Exception ex) {
				MessageBox.Show(this, ex.Message, GetString("MessageTitleErr"));
			}
		}
	}
}
