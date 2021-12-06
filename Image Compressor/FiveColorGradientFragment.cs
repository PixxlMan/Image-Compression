using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Compressor
{
	public sealed class FiveColorGradientFragment : Fragment
	{
		public FiveColorGradientFragment(Rgba32 aColor, Rgba32 bColor, Rgba32 cColor, Rgba32 dColor, Rgba32 centerColor, int width, int height) : base(width, height)
		{
			this.aColor = aColor;
			this.bColor = bColor;
			this.cColor = cColor;
			this.dColor = dColor;
			this.centerColor = centerColor;
		}

		private Rgba32 aColor;
		private Rgba32 bColor;
		private Rgba32 cColor;
		private Rgba32 dColor;
		private Rgba32 centerColor;

		public override Image<Rgba32> GenerateRepresentation()
		{
			Image<Rgba32> image = new(Width, Height);

			PathGradientBrush pathGradientBrush = new PathGradientBrush(new PointF[] { new(0, 0), new(image.Width, 0), new(image.Width, image.Height), new(0, image.Height) }, new Color[] { aColor, bColor, dColor, cColor }, centerColor);

			image.Mutate(i => i.Fill(pathGradientBrush));

			//image.SaveAsBmp(@$"R:\{Guid.NewGuid()}.bmp");

			return image;
		}

		public static FiveColorGradientFragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			FiveColorGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Top], image[rectangle.Left, rectangle.Bottom - 1], image[rectangle.Right - 1, rectangle.Bottom - 1], image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)], rectangle.Width, rectangle.Height);

			return fragment;
		}
	}
}
