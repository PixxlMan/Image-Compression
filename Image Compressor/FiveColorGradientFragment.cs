using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
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
		public FiveColorGradientFragment(Rgba32 aColor, Rgba32 bColor, Rgba32 cColor, Rgba32 dColor, Rgba32 centerColor)
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

		public override void DrawRepresentation(Image<Rgba32> image, Rectangle rectangle)
		{
			PathGradientBrush pathGradientBrush = new PathGradientBrush(new RectangularPolygon(rectangle).Points.ToArray(), new Color[] { aColor, bColor, dColor, cColor }, centerColor);

			image.Mutate(i => i.Fill(pathGradientBrush));
		}

		public static FiveColorGradientFragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			FiveColorGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Top], image[rectangle.Left, rectangle.Bottom - 1], image[rectangle.Right - 1, rectangle.Bottom - 1], image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

			return fragment;
		}
	}
}
