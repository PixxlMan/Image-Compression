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
		public LinearGradientFragment(Rgba32 aColor, Rgba32 bColor)
		{
			this.aColor = aColor;
			this.bColor = bColor;
		}

		public LinearGradientFragment()
		{

		}

		private Rgba32 aColor;
		private Rgba32 bColor;

		public override byte Id => 2;

		public override void DrawRepresentation(Image<Rgba32> image, Rectangle rectangle)
		{
			LinearGradientBrush linearGradientBrush = new(new PointF(rectangle.Left, rectangle.Top), new PointF(rectangle.Right - 1, rectangle.Bottom - 1), GradientRepetitionMode.DontFill, new ColorStop(0, aColor), new ColorStop(1, bColor));

			image.Mutate(i => i.Fill(linearGradientBrush));
		}

		public static LinearGradientFragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			LinearGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Bottom - 1]);

			return fragment;
		}

		protected override void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(aColor.PackedValue);
			binaryWriter.Write(bColor.PackedValue);
		}

		protected override Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			aColor = new Rgba32(binaryReader.ReadUInt32());
			bColor = new Rgba32(binaryReader.ReadUInt32());

			return this;
		}
	}
}
