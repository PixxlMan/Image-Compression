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
	public sealed class SingleColorFragment : Fragment
	{
		public SingleColorFragment(Rgba32 color)
		{
			this.color = color;
		}

		public SingleColorFragment()
		{

		}

		private Rgba32 color;

		public override byte Id => 0;

		public static SingleColorFragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			int aR = ((int)image[rectangle.Left, rectangle.Top].R + (int)image[rectangle.Right - 1, rectangle.Top].R + (int)image[rectangle.Left, rectangle.Bottom - 1].R + (int)image[rectangle.Right - 1, rectangle.Bottom - 1].R + (int)image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)].R) / 5;
			int aG = ((int)image[rectangle.Left, rectangle.Top].G + (int)image[rectangle.Right - 1, rectangle.Top].G + (int)image[rectangle.Left, rectangle.Bottom - 1].G + (int)image[rectangle.Right - 1, rectangle.Bottom - 1].G + (int)image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)].G) / 5;
			int aB = ((int)image[rectangle.Left, rectangle.Top].B + (int)image[rectangle.Right - 1, rectangle.Top].B + (int)image[rectangle.Left, rectangle.Bottom - 1].B + (int)image[rectangle.Right - 1, rectangle.Bottom - 1].B + (int)image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)].B) / 5;

			SingleColorFragment fragment = new(new Rgba32((byte)aR, (byte)aG, (byte)aB));
			
			//SingleColorFragment fragment = new(image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

			return fragment;
		}

		public override void DrawRepresentation(Image<Rgba32> image, Rectangle rectangle)
		{
			image.Mutate(i => i.Fill(new SolidBrush(color), new RectangularPolygon(rectangle)));
		}

		protected override void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(color.PackedValue);
		}

		protected override Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			color = new Rgba32(binaryReader.ReadUInt32());

			return this;
		}
	}
}
