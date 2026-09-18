using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Xml;

namespace SCS_Mod_Helper.Trucks;

public class CustomTruckIO: AppIO {
	public const string TITLE_VEHICLE = "Vehicle";
	public const string ATTR_IS_ETS2 = "isETS2";
	public const string ATTR_GAME_VERSION = "GameVersion";
	public const string TITLE_TRUCKS = "Trucks";
	public const string TITLE_TRUCK = "Truck";
	public const string ATTR_TRUCK_ID = "truckID";
	public const string ATTR_TRUCK_PROD_YEAR = "prodYear";
	public const string ATTR_TRUCK_NAME = "truckName";
	public const string ATTR_TRUCK_DESC = "truckDesc";

	public const string TITLE_CABINS = "Cabins";
	public const string TITLE_CABIN = "Cabin";
	public const string ATTR_CABIN_ID = "cabinID";
	public const string ATTR_CABIN_NAME = "cabinName";

	public const string TITLE_ACCESSORIES = "Accessories";
	public const string TITLE_ACCESSORY = "Accessory";
	public const string ATTR_ACC_ID = "accID";
	public const string ATTR_ACC_NAME = "accName";

	public const string PATH_ETS2_CUSTOM_TRUCK = "TrucksETS2T.DET";
	public const string PATH_ATS_CUSTOM_TRUCK = "TrucksATST.DET";



	public void SaveCustomTruck(bool isETS2, List<Truck> trucks) {
		WriteXmlDeclaration();

		WriteElement(TITLE_VEHICLE, () => {
			WriteAttribute(ATTR_IS_ETS2, isETS2.ToString());
			WriteAttribute(ATTR_GAME_VERSION, Instances.GameVersion);
			WriteElement(TITLE_TRUCKS, () => {
				foreach(Truck truck in trucks) {
					WriteElement(TITLE_TRUCK, () => {
						WriteAttribute(ATTR_TRUCK_ID, truck.TruckID);
						WriteAttribute(ATTR_TRUCK_PROD_YEAR, truck.ProductionYear);
						WriteAttribute(ATTR_TRUCK_NAME, truck.IngameName);
						WriteAttribute(ATTR_TRUCK_DESC, truck.Description);
						if(truck.Cabins.Count > 0) {
							WriteElement(TITLE_CABINS, () => {
								foreach (Cabin cabin in truck.Cabins) {
									WriteElement(TITLE_CABIN, () => {
										WriteAttribute(ATTR_CABIN_ID, cabin.CabinID);
										WriteAttribute(ATTR_CABIN_NAME, cabin.CabinName);
									});
								}
							});
						}
						if (truck.Accessories.Count > 0) {
							WriteElement(TITLE_ACCESSORIES, () => {
								foreach (Accessory accessory in truck.Accessories) {
									WriteElement(TITLE_ACCESSORIES, () => {
										WriteAttribute(ATTR_ACC_ID, accessory.AccID);
										WriteAttribute(ATTR_ACC_NAME, accessory.AccName);
									});
								}
							});
						}
					});
				}
			});
		});
		SaveDocument(isETS2 ? PATH_ETS2_CUSTOM_TRUCK : PATH_ATS_CUSTOM_TRUCK);
	}

	public List<Truck> LoadCustomTruck(bool isETS2) {
		doc.Load(isETS2 ? PATH_ETS2_CUSTOM_TRUCK : PATH_ATS_CUSTOM_TRUCK);
		List<Truck> trucks = [];
		XmlNode? node = doc.SelectSingleNode($"{TITLE_VEHICLE}/{TITLE_TRUCKS}");
		if (node != null) {
			foreach (XmlNode truckNode in node.ChildNodes) {
				if (truckNode.Name == TITLE_TRUCK) {
					var truck = LoadTruckNode(truckNode, isETS2);
					trucks.Add(truck);
				}
			}
		}
		return trucks;
	}

	private Truck LoadTruckNode(XmlNode truckNode, bool isETS2) {
		var id = GetAttribute(truckNode, ATTR_TRUCK_ID);
		int prodYear = int.Parse(GetAttribute(truckNode, ATTR_TRUCK_PROD_YEAR));
		var name = GetAttribute(truckNode, ATTR_TRUCK_NAME);
		var desc = GetAttribute(truckNode, ATTR_TRUCK_DESC);

		var cabinsNode = truckNode.SelectSingleNode(TITLE_CABINS);
		if (cabinsNode != null) {
			List<Cabin> cabins = [];
			foreach (XmlNode cabinNode in cabinsNode.ChildNodes) {
				if (cabinNode.Name == TITLE_CABIN) {
					var cabinID = GetAttribute(cabinNode, ATTR_CABIN_ID);
					var cabinName = GetAttribute(cabinNode, ATTR_CABIN_NAME);
					Cabin cabin = new(cabinID, cabinName);
					cabins.Add(cabin);
				}
			}
		}

		var accsNode = truckNode.SelectSingleNode(TITLE_ACCESSORIES);
		if (accsNode != null) {
			List<Accessory> accessories = [];
			foreach (XmlNode accNode in accsNode.ChildNodes) {
				if (accNode.Name == TITLE_ACCESSORY) {
					var accID = GetAttribute(accNode, ATTR_ACC_ID);
					var accName = GetAttribute(accNode, ATTR_ACC_NAME);
					Accessory accessory = new(accID, accName);
					accessories.Add(accessory);
				}
			}
		}
		return new(isETS2, id, prodYear, name, desc);
	}
}
