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
using static Image_Compressor.ColorUtils;

namespace Image_Compressor
{
	public struct LinearGradientFragment : Fragment
	{
		public LinearGradientFragment(Rgb24 aColor, Rgb24 bColor, SampleLine sampleLine)
		{
			this.aColor = aColor;
			this.bColor = bColor;
			this.sampleLine = sampleLine;
		}

		private Rgb24 aColor;
		private Rgb24 bColor;
		private SampleLine sampleLine;

		public FragmentId Id => ConstId;

		public const FragmentId ConstId = FragmentId.LinearGradientFragment;

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			PointF firstPoint;
			PointF secondPoint;

			switch (sampleLine)
			{
				case SampleLine.TopLeftToBottomRight:
					firstPoint = new(rectangle.Left, rectangle.Top);
					secondPoint = new(rectangle.Right, rectangle.Bottom);
					break;
				case SampleLine.TopRightToBottomLeft:
					firstPoint = new(rectangle.Right, rectangle.Top);
					secondPoint = new(rectangle.Left, rectangle.Bottom);
					break;
				case SampleLine.TopCenterToBottomCenter:
					firstPoint = new(rectangle.Left + (rectangle.Width / 2), rectangle.Top);
					secondPoint = new(rectangle.Left + (rectangle.Width / 2), rectangle.Bottom);
					break;
				case SampleLine.RightCenterToLeftCenter:
					firstPoint = new(rectangle.Right, rectangle.Top + (rectangle.Height / 2));
					secondPoint = new(rectangle.Left, rectangle.Top + (rectangle.Height / 2));
					break;

				default:
					goto case SampleLine.TopLeftToBottomRight;
			}

			LinearGradientBrush gradientBrush = new(firstPoint, secondPoint, GradientRepetitionMode.DontFill, new ColorStop(0f, new Color(aColor)), new ColorStop(1f, new Color(bColor)));

			image.Mutate(i => i.Fill(gradientBrush, new RectangularPolygon(rectangle)));

			if (rectangle.Width > 4)
				Fragment.BlurTopAndLeftEdge(image, rectangle);
		}

		public static Fragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			var line = image.GetOptimalLinearGradientLine(rectangle);

			LinearGradientFragment fragment;

			switch (line)
			{
				case SampleLine.TopLeftToBottomRight:
					fragment = new(image[rectangle.Left, rectangle.Top], image[rectangle.Right - 1, rectangle.Bottom - 1], line);
					break;
				case SampleLine.TopRightToBottomLeft:
					fragment = new(image[rectangle.Right - 1, rectangle.Top], image[rectangle.Left, rectangle.Bottom - 1], line);
					break;
				case SampleLine.TopCenterToBottomCenter:
					fragment = new(image[rectangle.Left + (rectangle.Width / 2), rectangle.Top], image[rectangle.Left + (rectangle.Width / 2), rectangle.Bottom - 1], line);
					break;
				case SampleLine.RightCenterToLeftCenter:
					fragment = new(image[rectangle.Right, rectangle.Top + (rectangle.Height / 2)], image[rectangle.Left, rectangle.Top + (rectangle.Height / 2)], line);
					break;

				default:
					goto case SampleLine.TopLeftToBottomRight;
			}

			return fragment;
		}

		public void WriteSpecificFragmentData(BitBinaryWriter binaryWriter)
		{
			binaryWriter.WriteColor(aColor);
			binaryWriter.WriteColor(bColor);
			binaryWriter.WriteByte((byte)sampleLine, (byte)SampleLine._BitSize);
		}

		public Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader, QuadrantData quadrantData)
		{
			aColor = binaryReader.ReadColor();
			bColor = binaryReader.ReadColor();
			sampleLine = (SampleLine)binaryReader.ReadByte((byte)SampleLine._BitSize);

			return this;
		}
	}
}
