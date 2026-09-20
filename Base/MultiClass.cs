using CommunityToolkit.Diagnostics;
using HelixToolkit.Maths;
using Microsoft.Extensions.Logging;
using SCS_Mod_Helper.Trucks;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SCS_Mod_Helper.Base;
public class Double<T1, T2>(T1 first, T2 second) {
	public T1 First = first;
	public T2 Second = second;
}

public class Triple<T1, T2, T3>(T1 first, T2 second, T3 third) {
	public T1 First = first;
	public T2 Second = second;
	public T3 Third = third;
}