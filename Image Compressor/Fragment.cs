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
		public Fragment(Rgba32 aColor, Rgba32 bColor, Rgba32 cColor, Rgba32 dColor, int width, int height)
		{
			this.aColor = aColor;
			this.bColor = bColor;
			this.cColor = cColor;
			this.dColor = dColor;
			this.width = width;
			this.height = height;
		}

		private Rgba32 aColor;
		private Rgba32 bColor;
		private Rgba32 cColor;
		private Rgba32 dColor;

		private int width;

		private int height;

		public Image<Rgba32> GenerateRepresentation()
		{
			Image<Rgba32> image = new(width, height, aColor);

			LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0f, 0f), new PointF(image.Width, image.Height), GradientRepetitionMode.DontFill, new ColorStop(0f, new Color(aColor)), new ColorStop(1f, new Color(dColor)));

			image.Mutate(x => x.Fill(new GraphicsOptions(), gradientBrush));

			//image.SaveAsBmp(@$"R:\{Guid.NewGuid()}.bmp");

			return image;
		}

		public static Fragment GenerateFragment(Image<Rgba32> image)
		{
			Fragment fragment = new(image[0, 0], image[image.Width - 1, 0], image[0, image.Height - 1], image[image.Width - 1, image.Height - 1], image.Width, image.Height);

			return fragment;
		}
	}
}
