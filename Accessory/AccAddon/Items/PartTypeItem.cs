namespace SCS_Mod_Helper.Accessory.AccAddon.Items; 

public class PartTypeItem(string partTypeValue, string partTypeDisplay) {
	private string mPartTypeValue = partTypeValue;
	public string PartTypeValue {
		get => mPartTypeValue; set => mPartTypeValue = value;
	}
	private string mPartTypeDisplay = partTypeDisplay;
	public string PartTypeDisplay {
		get => mPartTypeDisplay; set => mPartTypeDisplay = value;
	}
}

