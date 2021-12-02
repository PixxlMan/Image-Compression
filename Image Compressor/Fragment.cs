using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Compressor
{
	public class Fragment
	{
		public Fragment(Rgba32 color, int width, int height)
		{
			this.color = color;
			this.width = width;
			this.height = height;
		}

		private Rgba32 color;

		private int width;

		private int height;

		public Image<Rgba32> GenerateRepresentation()
		{
			Image<Rgba32> image = new(width, height, color);

			//image.SaveAsBmp(@$"R:\{Guid.NewGuid()}.bmp");

			return image;
		}

		public static Fragment GenerateFragment(Image<Rgba32> image)
		{
			Fragment fragment = new(image[0, 0], image.Width, image.Height);

			return fragment;
		}
	}
}
