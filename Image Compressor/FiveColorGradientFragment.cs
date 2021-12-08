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

		public FiveColorGradientFragment()
		{

		}

		private Rgba32 aColor;
		private Rgba32 bColor;
		private Rgba32 cColor;
		private Rgba32 dColor;
		private Rgba32 centerColor;

		public override byte Id => 1;

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

		protected override void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(aColor.PackedValue);
			binaryWriter.Write(bColor.PackedValue);
			binaryWriter.Write(cColor.PackedValue);
			binaryWriter.Write(dColor.PackedValue);
			binaryWriter.Write(centerColor.PackedValue);
		}

		protected override Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			aColor = new Rgba32(binaryReader.ReadUInt32());
			bColor = new Rgba32(binaryReader.ReadUInt32());
			cColor = new Rgba32(binaryReader.ReadUInt32());
			dColor = new Rgba32(binaryReader.ReadUInt32());
			centerColor = new Rgba32(binaryReader.ReadUInt32());

			return this;
		}
	}
}
