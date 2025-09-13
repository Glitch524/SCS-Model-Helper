using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Def_Writer;

/// <summary>
/// AddonHookup.xaml 的交互逻辑
/// </summary>
public partial class AccessoryHookupWindow: BaseWindow {
	readonly string ProjectLocation;
	public AccessoryHookupWindow(string ProjectLocation) {
		InitializeComponent();
		this.ProjectLocation = ProjectLocation;
	}
}//读取文件时 记录physic的值，但不显示，输出的时候也按照获得的输出
 //读取到physic model直接整段读完，输出时直接输出
 //

public class AddonHookupItem {
	public string ModelName { get; set; }
	public string Price { get; set; }
	public string UnlockLevel { get; set; }
	public string ModelIcon { get; set; }





	public string PartType { get; set; }
}


//accessory_hookup_int_data


//info array<string>   default: []

//options: aftermarket, factory, licensed, unknown
//suitable_for    array<string>	default: []
//conflict_with array<string>   default: []
//defaults    array<string>	default: []
//overrides   array<string>	default: []
//require array<token>	default: []

//model   string  default: ""
//coll    string  default: ""
//look token	default: default
//variant token	default: default
//electric_type   enum    default: aux_light
//options: aux_light, vehicle
//Name Type    Data
//data    array<owner_ptr<physics_toy_data>>	default: []