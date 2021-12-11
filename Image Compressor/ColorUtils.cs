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
		public static int ColorDistance(this Rgba32 a, Rgba32 b)
		{
			int rD = Math.Abs(a.R - b.R);
			int gD = Math.Abs(a.G - b.G);
			int bD = Math.Abs(a.B - b.B);

			return rD + gD + bD;
		}
	}
}
