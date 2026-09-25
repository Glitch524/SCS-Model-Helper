using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Modding.PaintJob; 
public class ChannelExtraction {
	public static unsafe void ExtractRGBChannels(BitmapSource source, out WriteableBitmap rBmp, out WriteableBitmap gBmp, out WriteableBitmap bBmp) {
		int width = source.PixelWidth;
		int height = source.PixelHeight;

		var converted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
		byte[] srcPixels = new byte[width * height * 4];
		converted.CopyPixels(srcPixels, width * 4, 0);

		rBmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
		gBmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
		bBmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);

		rBmp.Lock();
		gBmp.Lock();
		bBmp.Lock();

		fixed(byte* pSrc = srcPixels) {
			byte* pRed = (byte*)rBmp.BackBuffer;
			byte* pGreen = (byte*)gBmp.BackBuffer;
			byte* pBlue = (byte*)bBmp.BackBuffer;
			byte* pCurrentSrc = pSrc;
			
			for(int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					byte b = pCurrentSrc[0];
					byte g = pCurrentSrc[1];
					byte r = pCurrentSrc[2];

					byte preR = (byte)((r * r) / 255);
					byte preG = (byte)((g * g) / 255);
					byte preB = (byte)((b * b) / 255);

					pRed[0] = 0;
					pRed[1] = 0;
					pRed[2] = r;
					pRed[3] = r;

					pGreen[0] = 0;
					pGreen[1] = g;
					pGreen[2] = 0;
					pGreen[3] = g;

					pBlue[0] = b;
					pBlue[1] = 0;
					pBlue[2] = 0;
					pBlue[3] = b;

					pCurrentSrc += 4;
					pRed += 4;
					pGreen += 4;
					pBlue += 4;
				}
			}
		}

		Int32Rect rect = new(0, 0, width, height);
		rBmp.AddDirtyRect(rect);
		rBmp.Unlock();
		rBmp.Freeze();
		gBmp.AddDirtyRect(rect);
		gBmp.Unlock();
		gBmp.Freeze();
		bBmp.AddDirtyRect(rect);
		bBmp.Unlock();
		bBmp.Freeze();
	}
}
