using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SCS_Mod_Helper.Accessory.PaintJob
{
    class PaintJobUtil {
		public static string IntToHex(int i) => i.ToString("X").PadLeft(2, '0');

		public static float[] ConvertRGBToFloat3(Color color) {
			return ConvertRGBToFloat3(color.R, color.G, color.B);
		}

		public static float[] ConvertRGBToFloat3(int R, int G, int B) {
			var f3 = new float[3];
			f3[0] = ColorHexToFloat(R);
			f3[1] = ColorHexToFloat(G);
			f3[2] = ColorHexToFloat(B);
			return f3;
		}

		private static float ColorHexToFloat(int color) {
			if (color == 0) return 0;
			if (color == 255) return 1;
			double value = color;
			value /= 255;
			value += 0.055;
			value /= 1.055;
			value = Math.Pow(value, 2.4);
			return (float) value;
		}
		//255颜色到float的公式是((x/255+0.055)/1.055)^2.4
		//反过来就是反过来

		public static Color ConvertFloat3ToRGB(float[] f3) {
			return Color.FromRgb(ColorFloatToHex(f3[0]), ColorFloatToHex(f3[1]), ColorFloatToHex(f3[2]));
		}

		private static byte ColorFloatToHex(float color) {
			if (color == 0) return 0;
			if (color == 1) return 255;
			double value = color;
			value = Math.Pow(value, 1 / 2.4);
			value *= 1.055;
			value -= 0.055;
			value *= 255;
			return (byte)Math.Round(value);
		}
	}
}

//Let's try to encode via sRGB Transformations manually...

//We have color 235.
//Let f denote a fractional (from the word "float") number.

//We encode "directly" (according to the upper formula).
//f = 235 / 255 = 0.92156862745098039215686274509804.
//The number f turned out to be greater than 0.04045, so we consider this: ((f + 0.055) / 1.055) and then raise it to the power of 2.4.
//We get: 0.83076987677465456326680486629645.That is as in def.

//We encode "inversely" (according to the lower formula).
//Number f = 0.8307 (taken from def).
//We have a number f greater than 0.0031308, so we consider this: we raise f to the power (1/2.4), then multiply by 1.055, then subtract 0.055.
//We get: 0.92153440159716157976920448100598.Multiplying it by 255, we get 234.99127240727620284114714265653, i.e., rounded up, 235.