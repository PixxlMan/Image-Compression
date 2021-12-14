using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Compressor
{
	public static class ColorUtils
	{
		public static int ColorDistance(this Rgb24 a, Rgb24 b)
		{
			int rD = Math.Abs(a.R - b.R);
			int gD = Math.Abs(a.G - b.G);
			int bD = Math.Abs(a.B - b.B);

			return rD + gD + bD;
		}

		public static Rgb24 ReadColor(this BitBinaryReader binaryReader)
		{
			return new Rgb24(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
		}

		public static Rgb24 AverageColor(params Rgb24[] colors)
		{
			int totalR = 0;
			int totalG = 0;
			int totalB = 0;

			for (int i = 0; i < colors.Length; i++)
			{
				totalR += colors[i].R;
				totalG += colors[i].G;
				totalB += colors[i].B;
			}

			byte avgR = (byte)(totalR / colors.Length);
			byte avgG = (byte)(totalG / colors.Length);
			byte avgB = (byte)(totalB / colors.Length);

			return new Rgb24(avgR, avgG, avgB);
		}

		public static Rgb24[] GetFivePointSamples(this Image<Rgb24> image, Rectangle rectangle)
		{
			Rgb24[] samples = new Rgb24[5];

			GetFivePointSamples(image, rectangle, out Rgb24 topLeftSample, out Rgb24 topRightSample, out Rgb24 bottomLeftSample, out Rgb24 bottomRightSample, out Rgb24 centerSample);

			samples[0] = topLeftSample;
			samples[1] = topRightSample;
			samples[2] = bottomLeftSample;
			samples[3] = bottomRightSample;
			samples[4] = centerSample;

			return samples;
		}
		
		public static void GetFivePointSamples(this Image<Rgb24> image, Rectangle rectangle, out Rgb24 topLeftSample, out Rgb24 topRightSample, out Rgb24 bottomLeftSample, out Rgb24 bottomRightSample, out Rgb24 centerSample)
		{
			topLeftSample = image[rectangle.Left, rectangle.Top];
			topRightSample = image[rectangle.Right - 1, rectangle.Top];
			bottomLeftSample = image[rectangle.Left, rectangle.Bottom - 1];
			bottomRightSample = image[rectangle.Right - 1, rectangle.Bottom - 1];
			centerSample = image[rectangle.Left + (rectangle.Width / 2) - 1, rectangle.Top + (rectangle.Height / 2) - 1];
		}
	}
}
