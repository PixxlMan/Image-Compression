﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Image_Compressor
{
	public abstract class Fragment
	{
		public Fragment()
		{
		}

		public abstract void DrawRepresentation(Image<Rgba32> image, Rectangle rectangle);

		public static Fragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			return FiveColorGradientFragment.GenerateFragment(image, rectangle);

			//return SingleColorFragment.GenerateFragment(image, rectangle);
		}
	}
}
