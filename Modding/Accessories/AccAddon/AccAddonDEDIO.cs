using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Trucks;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Xml;

namespace SCS_Mod_Helper.Modding.Accessories.AccAddon
{
    public class AccAddonDEDIO(string savePath): AppIO
    {
        private const string TITLE_ACCESSORY_ADDON = "AccessoryAddon";
		public const string ATTR_GAME_VERSION = "GameVersion";
		private const string TITLE_MODEL_NAME = "ModelName";
        private const string TITLE_DISPLAY_NAME = "DisplayName";
        private const string TITLE_PRICE = "Price";
        private const string TITLE_UNLOCK_LEVEL = "UnlockLevel";
        private const string TITLE_ICON_NAME = "IconName";
        private const string TITLE_PART_TYPE = "PartType";
        private const string TITLE_MODEL_PATH = "ModelPath";
        private const string TITLE_MODEL_PATH_UK = "ModelPathUK";
        private const string TITLE_EXT_MODEL_PATH = "ExternalModelPath";
        private const string TITLE_EXT_MODEL_PATH_UK = "ExternalModelPathUK";
        private const string TITLE_COLLISION_PATH = "CollPath";
        private const string TITLE_MODEL_TYPE = "ModelType";
        private const string TITLE_LOOK = "Look";
        private const string TITLE_VARIANT = "variant";

        private const string TITLE_HIDE_IN = "HideIn";
        private const string TITLE_ELECTRIC_TYPE = "ElectricType";

        private const string TITLE_DATA_LIST = "DataList";
        private const string TITLE_DATA = "Data";
        private const string TITLE_SUITABLE_FOR_LIST = "SuitableForList";
        private const string TITLE_SUITABLE_FOR = "SuitableFor";
        private const string TITLE_CONFLICT_WITH_LIST = "ConflictWithList";
        private const string TITLE_CONFLICT_WITH = "ConflictWith";
        private const string TITLE_DEFAULTS_LIST = "DefaultsList";
		private const string TITLE_DEFAULTS = "Defaults";
        private const string TITLE_OVERRIDES_LIST = "OverridesList";
        private const string TITLE_OVERRIDES = "Overrides";
        private const string TITLE_REQUIRE_LIST = "RequireList";
        private const string TITLE_REQUIRE = "Require";

        private const string TITLE_TRUCKS_ETS2 = "TrucksETS2";
        private const string TITLE_TRUCKS_ATS = "TrucksATS";
        private const string TITLE_TRUCK = "Truck";
		private const string ATTR_TRUCK_CHECK = "Check";
		private const string ATTR_TRUCK_ID = "TruckID";
        private const string ATTR_MODEL_TYPE = "ModelType";
        private const string ATTR_LOOK = "Look";
        private const string ATTR_VARIANT = "Variant";
		private readonly string SavePath = savePath;

        public void SaveAddon(AccAddonBinding data) {
            CreateXmlDeclaration();
            SkipCreatingIFEmpty = true;

            WriteElement(TITLE_ACCESSORY_ADDON, () => {
				WriteAttribute(ATTR_GAME_VERSION, Instances.GameVersion);

                WriteValueElement(TITLE_MODEL_NAME, data.ModelName);
                WriteValueElement(TITLE_DISPLAY_NAME, data.DisplayName);
                WriteValueElement(TITLE_PRICE, data.Price ?? 1);
                WriteValueElement(TITLE_UNLOCK_LEVEL, data.UnlockLevel ?? 0);
                WriteValueElement(TITLE_ICON_NAME, data.IconName);
                WriteValueElement(TITLE_PART_TYPE, data.PartType);
                WriteValueElement(TITLE_MODEL_PATH, data.ModelPath);
                WriteValueElement(TITLE_MODEL_PATH_UK, data.ModelPathUK);
                WriteValueElement(TITLE_EXT_MODEL_PATH, data.ExtModelPath);
                WriteValueElement(TITLE_EXT_MODEL_PATH_UK, data.ExtModelPathUK);
                WriteValueElement(TITLE_COLLISION_PATH, data.CollPath);
                WriteValueElement(TITLE_MODEL_TYPE, data.ModelType);
                WriteValueElement(TITLE_LOOK, data.Look);
                WriteValueElement(TITLE_VARIANT, data.Variant);
                WriteValueElement(TITLE_HIDE_IN, data.HideIn);
                WriteValueElement(TITLE_ELECTRIC_TYPE, data.ElectricType);

				WriteList(TITLE_DATA_LIST, data.Data, TITLE_DATA);
				WriteList(TITLE_SUITABLE_FOR_LIST, data.SuitableFor, TITLE_SUITABLE_FOR);
				WriteList(TITLE_CONFLICT_WITH_LIST, data.ConflictWith, TITLE_CONFLICT_WITH);
				WriteList(TITLE_DEFAULTS_LIST, data.Defaults, TITLE_DEFAULTS);
				WriteList(TITLE_OVERRIDES_LIST, data.Overrides, TITLE_OVERRIDES);
				WriteList(TITLE_REQUIRE_LIST, data.Require, TITLE_REQUIRE);

				WriteTrucks(TITLE_TRUCKS_ETS2, data.TrucksETS2);
				WriteTrucks(TITLE_TRUCKS_ATS, data.TrucksATS);
			});
            SaveDocument(SavePath);
		}

		private void WriteList(string listTitle, Collection<string> list, string subTitle) {
			if (list.Count == 0)
				return;
			WriteElement(listTitle, () => {
				foreach (var item in list) {
					WriteValueElement(subTitle, item);
				}
			});
		}

		private void WriteTrucks(string listTitle, Collection<Truck> trucks) {
			if (trucks.Count == 0)
				return;
			WriteElement(listTitle, () => {
				foreach (var te in trucks) {
					WriteElement(TITLE_TRUCK, () => {
						WriteAttribute(ATTR_TRUCK_CHECK, te.Check);
						WriteAttribute(ATTR_TRUCK_ID, te.TruckID);
						WriteAttribute(ATTR_MODEL_TYPE, te.ModelType);
						WriteAttribute(ATTR_LOOK, te.Look);
						WriteAttribute(ATTR_VARIANT, te.Variant);
					});
				}
			});
		}

        public void LoadAddon(AccAddonBinding data) {
            doc.Load(SavePath);
            XmlNode? addonNode = doc.SelectSingleNode(TITLE_ACCESSORY_ADDON);
            if (addonNode == null)
                return;
            foreach (XmlNode child in addonNode.ChildNodes) {
                switch(child.Name) {
                    case TITLE_MODEL_NAME:
                        data.ModelName = child.InnerText;
                        break;
                    case TITLE_DISPLAY_NAME:
						data.DisplayName = child.InnerText;
						break;
                    case TITLE_PRICE:
						data.Price = long.Parse(child.InnerText);
						break;
                    case TITLE_UNLOCK_LEVEL:
						data.UnlockLevel = uint.Parse(child.InnerText);
						break;
                    case TITLE_ICON_NAME:
						data.IconName = child.InnerText;
						break;
                    case TITLE_PART_TYPE:
						data.PartType = child.InnerText;
						break;
                    case TITLE_MODEL_PATH:
						data.ModelPath = child.InnerText;
						break;
                    case TITLE_MODEL_PATH_UK:
						data.ModelPathUK = child.InnerText;
						break;
                    case TITLE_EXT_MODEL_PATH:
						data.ExtModelPath = child.InnerText;
						break;
                    case TITLE_EXT_MODEL_PATH_UK:
						data.ExtModelPathUK = child.InnerText;
						break;
                    case TITLE_COLLISION_PATH:
						data.CollPath = child.InnerText;
						break;
                    case TITLE_MODEL_TYPE:
						data.ModelType = child.InnerText;
						break;
					case TITLE_LOOK:
						data.Look = child.InnerText;
						break;
					case TITLE_VARIANT:
						data.Variant = child.InnerText;
						break;
					case TITLE_HIDE_IN:
						data.HideIn = uint.Parse(child.InnerText);
						break;
					case TITLE_ELECTRIC_TYPE:
						data.ElectricType = child.InnerText;
						break;
					case TITLE_DATA_LIST:
						ReadList(child, TITLE_DATA, data.Data);
						data.InvokeChange(nameof(data.DataListContent));
						break;
					case TITLE_SUITABLE_FOR_LIST:
						ReadList(child, TITLE_SUITABLE_FOR, data.SuitableFor);
						data.InvokeChange(nameof(data.SuitableForListContent));
						break;
					case TITLE_CONFLICT_WITH_LIST:
						ReadList(child, TITLE_CONFLICT_WITH, data.ConflictWith);
						data.InvokeChange(nameof(data.ConflictWithListContent));
						break;
					case TITLE_DEFAULTS_LIST:
						ReadList(child, TITLE_DEFAULTS, data.Defaults);
						data.InvokeChange(nameof(data.DefaultsListContent));
						break;
					case TITLE_OVERRIDES_LIST:
						ReadList(child, TITLE_OVERRIDES, data.Overrides);
						data.InvokeChange(nameof(data.OverridesListContent));
						break;
					case TITLE_REQUIRE_LIST:
						ReadList(child, TITLE_REQUIRE, data.Require);
						data.InvokeChange(nameof(data.RequireListContent));
						break;
					case TITLE_TRUCKS_ETS2:
						ReadTruck(child, data.TrucksETS2);
						break;
					case TITLE_TRUCKS_ATS:
						ReadTruck(child, data.TrucksATS);
						break;
				}
            }
        }

		private static void ReadList(XmlNode node, string title, Collection<string> list) {
			foreach (XmlNode c in node.ChildNodes) {
				if (c.Name == title)
					list.Add(c.InnerText);
			}
		}

		private void ReadTruck(XmlNode node, Collection<Truck> trucks) {
			List<int> indexes = Enumerable.Range(0, trucks.Count).ToList();
			int i = 0;
			foreach (XmlNode t in node.ChildNodes) {
				if (t.Name != TITLE_TRUCK) 
					continue;
				var truckID = GetAttribute(t, ATTR_TRUCK_ID);
				for (int j = 0; j < indexes.Count; j++) {
					i = (i + j) % indexes.Count;
					var truck = trucks[indexes[i]];
					if (truckID == truck.TruckID) {
						truck.Check = GetAttributeBool(t, ATTR_TRUCK_CHECK);
						truck.ModelType = GetAttribute(t, ATTR_MODEL_TYPE);
						truck.Look = GetAttribute(t, ATTR_LOOK);
						truck.Variant = GetAttribute(t, ATTR_VARIANT);
						indexes.RemoveAt(i);
						i -= 1;
						break;
					}
				}
				i += 1;
			}
		}
	}

	public class AccAddonHistoryIO(): AccAddonDEDIO(Paths.HistoryFile()) {

	}
}