﻿using SixLabors.ImageSharp;
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
	public interface Fragment
	{
		public static abstract byte StaticId { get; }
		public byte Id => StaticId;

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle);

		public void WriteSpecificFragmentData(BinaryWriter binaryWriter);

		public static abstract Fragment ReadSpecificFragmentData(BinaryReader binaryReader);

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
				10 => SingleColorFragment.ReadSpecificFragmentData(binaryReader),
				20 => LinearGradientFragment.ReadSpecificFragmentData(binaryReader),
				30 => FiveColorGradientFragment.ReadSpecificFragmentData(binaryReader),
			};
		}

		public static abstract Fragment GenerateSpecificFragment(Image<Rgb24> image, Rectangle rectangle);

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			image.GetFivePointSamples(rectangle, out Rgb24 topLeftSample, out Rgb24 topRightSample, out Rgb24 bottomLeftSample, out Rgb24 bottomRightSample, out Rgb24 centerSample);

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
