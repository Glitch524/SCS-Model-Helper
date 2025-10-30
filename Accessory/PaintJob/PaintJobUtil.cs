using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.Accessory.PaintJob
{
    class PaintJobUtil {
		public static string IntToHex(int i) => i.ToString("X").PadLeft(2, '0');
		public static float[] ConvertRGBToFloat3(int R, int G, int B) {
			var f3 = new float[3];
			f3[0] = ColorHexToFloat(R);
			f3[1] = ColorHexToFloat(G);
			f3[2] = ColorHexToFloat(B);
			return f3;
		}

		private static float ColorHexToFloat(int color) {
			if (color == 0) return 0;
			if (color == 1) return 1;
			double value = color;
			value /= 255;
			value += 0.055;
			value /= 1.055;
			value = Math.Pow(value, 2.4);
			return (float) value;
		}
		//255颜色到float的公式是((x/255+0.055)/1.055)^2.4
		//反过来就是反过来
	}
}
