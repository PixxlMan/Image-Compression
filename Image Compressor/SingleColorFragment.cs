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
		public SingleColorFragment(Rgb24 color)
		{
			this.color = color;
		}

		public SingleColorFragment()
		{

		}

		private Rgb24 color;

		public byte Id => 10;

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			Rgb24 avgColor = ColorUtils.AverageColor(image.GetFivePointSamples(rectangle));

			SingleColorFragment fragment = new(avgColor);
			
			//SingleColorFragment fragment = new(image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

			return fragment;
		}

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			image.Mutate(i => i.Fill(new SolidBrush(color), new RectangularPolygon(rectangle)));
		}

		public void WriteSpecificFragmentData(BitBinaryWriter binaryWriter)
		{
			binaryWriter.WriteColor(color);
		}

		public Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader)
		{
			color = binaryReader.ReadColor();

			return this;
		}
	}
}
