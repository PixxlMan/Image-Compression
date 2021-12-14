﻿using SixLabors.ImageSharp;
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
		public FiveColorGradientFragment(Rgb24 aColor, Rgb24 bColor, Rgb24 cColor, Rgb24 dColor, Rgb24 centerColor)
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

		private Rgb24 aColor;
		private Rgb24 bColor;
		private Rgb24 cColor;
		private Rgb24 dColor;
		private Rgb24 centerColor;

		public byte Id => 30;

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			PathGradientBrush pathGradientBrush = new PathGradientBrush(new RectangularPolygon(rectangle).Points.ToArray(), new Color[] { aColor, bColor, dColor, cColor }, centerColor);

			image.Mutate(i => i.Fill(pathGradientBrush));
		}

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			FiveColorGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Top], image[rectangle.Left, rectangle.Bottom - 1], image[rectangle.Right - 1, rectangle.Bottom - 1], image[rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2)]);

			return fragment;
		}

		public void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(aColor);
			binaryWriter.Write(bColor);
			binaryWriter.Write(cColor);
			binaryWriter.Write(dColor);
			binaryWriter.Write(centerColor);
		}

		public Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			aColor = binaryReader.ReadColor();
			bColor = binaryReader.ReadColor();
			cColor = binaryReader.ReadColor();
			dColor = binaryReader.ReadColor();
			centerColor = binaryReader.ReadColor();

			return this;
		}
	}
}
