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
	public struct SingleColorFragment : Fragment
	{
		public SingleColorFragment(Rgb24 color)
		{
			this.color = color;
		}

		private Rgb24 color;

		public FragmentId Id => ConstId;

		public const FragmentId ConstId = FragmentId.SingleColorFragment;

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			Rgb24 avgColor = ColorUtils.AverageColor(image.GetFivePointSamples(rectangle));

			SingleColorFragment fragment = new(avgColor);
			
			//SingleColorFragment fragment = new(image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

			return fragment;
		}

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			var @this = this;

			image.Mutate(i => i.Fill(new SolidBrush(@this.color), new RectangularPolygon(rectangle)));

			if (rectangle.Width > 4)
				Fragment.BlurTopAndLeftEdge(image, rectangle);
		}

		public void WriteSpecificFragmentData(BitBinaryWriter binaryWriter)
		{
			binaryWriter.WriteColor(color);
		}

		public Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader, QuadrantData quadrantData)
		{
			color = binaryReader.ReadColor(quadrantData);

			return this;
		}
	}
}
