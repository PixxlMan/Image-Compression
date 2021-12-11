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
	public abstract class Fragment
	{
		public Fragment()
		{
		}
		
		/*public Fragment(BinaryReader binaryReader)
		{
		}*/

		public abstract byte Id { get; }


		public abstract void DrawRepresentation(Image<Rgba32> image, Rectangle rectangle);

		protected abstract void WriteSpecificFragmentData(BinaryWriter binaryWriter);

		protected abstract Fragment ReadSpecificFragmentData(BinaryReader binaryReader);

		public static void WriteFragmentData(Fragment fragment, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(fragment.Id);
			fragment.WriteSpecificFragmentData(binaryWriter);
		}

		public static Fragment ReadFragmentData(BinaryReader binaryReader)
		{
			byte id = binaryReader.ReadByte();

			return id switch
			{
				0 => new SingleColorFragment().ReadSpecificFragmentData(binaryReader),
				1 => new FiveColorGradientFragment().ReadSpecificFragmentData(binaryReader),
				2 => new LinearGradientFragment().ReadSpecificFragmentData(binaryReader),
			};
		}

		public static Fragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			Rgba32 topLeftSample = image[rectangle.Left, rectangle.Top];
			Rgba32 topRightSample = image[rectangle.Right - 1, rectangle.Top];
			Rgba32 bottomLeftSample = image[rectangle.Left, rectangle.Bottom - 1];
			Rgba32 bottomRightSample = image[rectangle.Right - 1, rectangle.Bottom - 1];
			Rgba32 centerSample = image[rectangle.Left + (rectangle.Width / 2) - 1, rectangle.Top + (rectangle.Height / 2) - 1];

			if (rectangle.Width < 16 && ColorUtils.ColorDistance(centerSample, topLeftSample) < 16)
			{
				return SingleColorFragment.GenerateFragment(image, rectangle);
			}
			else if (rectangle.Width < 128)
			{
				if (topLeftSample.ColorDistance(bottomRightSample) > 16 || topRightSample.ColorDistance(bottomLeftSample) > 16)
					return LinearGradientFragment.GenerateFragment(image, rectangle);
			}

			if (topLeftSample.ColorDistance(bottomRightSample) > 32 && topRightSample.ColorDistance(bottomLeftSample) > 32)
				return FiveColorGradientFragment.GenerateFragment(image, rectangle);

			return SingleColorFragment.GenerateFragment(image, rectangle);
		}
	}
}
