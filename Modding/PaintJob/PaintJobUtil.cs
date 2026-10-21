using System.Windows.Media;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public static class PaintJobUtil {
		public static string IntToHex(int i) => i.ToString("X").PadLeft(2, '0');

		public static float[] ToFloat3(this Color color) => ConvertRGBToFloat3(color.R, color.G, color.B);

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
			return (float)value;
		}
		//255颜色到float的公式是((x/255+0.055)/1.055)^2.4
		//反过来可以从float获得255

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