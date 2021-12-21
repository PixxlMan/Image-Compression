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
	public struct LinearGradientFragment : Fragment
	{
		public LinearGradientFragment(Rgb24 aColor, Rgb24 bColor)
		{
			this.aColor = aColor;
			this.bColor = bColor;
		}

		private Rgb24 aColor;
		private Rgb24 bColor;

		public FragmentId Id => ConstId;

		public const FragmentId ConstId = FragmentId.LinearGradientFragment;

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			LinearGradientBrush gradientBrush = new(new PointF(rectangle.Left, rectangle.Top), new PointF(rectangle.Right, rectangle.Bottom), GradientRepetitionMode.DontFill, new ColorStop(0f, new Color(aColor)), new ColorStop(1f, new Color(bColor)));

			image.Mutate(i => i.Fill(gradientBrush, new RectangularPolygon(rectangle)));

			if (rectangle.Width > 4)
				Fragment.BlurTopAndLeftEdge(image, rectangle);
		}

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			var line = image.GetOptimalLinearGradientLine(rectangle);

			switch (line)
			{
				case ColorUtils.SampleLine.TopLeftToBottomRight:
					break;
				case ColorUtils.SampleLine.TopRightToBottomLeft:
					break;
				case ColorUtils.SampleLine.TopCenterToBottomCenter:
					break;
				case ColorUtils.SampleLine.RightCenterToLeftCenter:
					break;
			}

			LinearGradientFragment fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Bottom - 1]);

			return fragment;
		}

		public void WriteSpecificFragmentData(BitBinaryWriter binaryWriter)
		{
			binaryWriter.WriteColor(aColor);
			binaryWriter.WriteColor(bColor);
		}

		public Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader)
		{
			aColor = binaryReader.ReadColor();
			bColor = binaryReader.ReadColor();

			return this;
		}
	}
}
