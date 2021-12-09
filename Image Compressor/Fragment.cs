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
			};
		}

		public static Fragment GenerateFragment(Image<Rgba32> image, Rectangle rectangle)
		{
			if (rectangle.Width < 16)
			{
				return SingleColorFragment.GenerateFragment(image, rectangle);
			}

			return FiveColorGradientFragment.GenerateFragment(image, rectangle);
		}
	}
}
