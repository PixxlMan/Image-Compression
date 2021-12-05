using SixLabors.ImageSharp;
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
	public class Fragment
	{
		public Fragment(Rgba32 aColor, Rgba32 bColor, Rgba32 cColor, Rgba32 dColor, Rgba32 centerColor, int width, int height)
		{
			this.aColor = aColor;
			this.bColor = bColor;
			this.cColor = cColor;
			this.dColor = dColor;
			this.centerColor = centerColor;
			this.width = width;
			this.height = height;
		}

		private Rgba32 aColor;
		private Rgba32 bColor;
		private Rgba32 cColor;
		private Rgba32 dColor;
		private Rgba32 centerColor;

		private int width;

		private int height;

		public Image<Rgba32> GenerateRepresentation()
		{
			Image<Rgba32> image = new(width, height, aColor);

			PathGradientBrush pathGradientBrush = new PathGradientBrush(new PointF[] { new ( 0, 0 ), new (image.Width, 0), new(image.Width, image.Height), new (0, image.Height) }, new Color[] { aColor, bColor, dColor, cColor }, centerColor);

			image.Mutate(i => i.Fill(pathGradientBrush));

			//image.SaveAsBmp(@$"R:\{Guid.NewGuid()}.bmp");

			return image;
		}

		public static Fragment GenerateFragment(Image<Rgba32> image)
		{
			Fragment fragment = new(image[0, 0], image[image.Width - 1, 0], image[0, image.Height - 1], image[image.Width - 1, image.Height - 1], image[image.Width / 2, image.Height / 2], image.Width, image.Height);

			return fragment;
		}
	}
}
