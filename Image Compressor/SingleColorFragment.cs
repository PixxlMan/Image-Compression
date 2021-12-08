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
			SingleColorFragment fragment = new(image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

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
