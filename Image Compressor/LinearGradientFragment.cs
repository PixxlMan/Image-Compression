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
	public sealed class LinearGradientFragment : Fragment
	{
		public LinearGradientFragment(Rgb24 aColor, Rgb24 bColor)
		{
			this.aColor = aColor;
			this.bColor = bColor;
		}

		public LinearGradientFragment()
		{

		}

		private Rgb24 aColor;
		private Rgb24 bColor;

		public override byte Id => 20;

		public override void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			LinearGradientBrush gradientBrush = new(new PointF(rectangle.Left, rectangle.Top), new PointF(rectangle.Right, rectangle.Bottom), GradientRepetitionMode.DontFill, new ColorStop(0f, new Color(aColor)), new ColorStop(1f, new Color(bColor)));

			image.Mutate(i => i.Fill(gradientBrush, new RectangularPolygon(rectangle)));
		}

		public static LinearGradientFragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			LinearGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Bottom - 1]);

			return fragment;
		}

		protected override void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(aColor);
			binaryWriter.Write(bColor);
		}

		protected override Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			aColor = binaryReader.ReadColor();
			bColor = binaryReader.ReadColor();

			return this;
		}
	}
}
