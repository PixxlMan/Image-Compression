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
	public enum FragmentId : byte
	{
		EmptyFragment,
		SingleColorFragment,
		LinearGradientFragment,
		FiveColorGradientFragment,

		_BitSize = 2,
	}

	public interface Fragment
	{
		public FragmentId Id { get; }

		public static void BlurTopAndLeftEdge(Image<Rgb24> image, Rectangle rectangle)
		{
			image.Mutate(i => i.BoxBlur(3, new Rectangle(rectangle.X, Math.Max(rectangle.Y - ((rectangle.Height / 6) / 2), 0), rectangle.Width, Math.Max(rectangle.Height / 6, 1))));

			image.Mutate(i => i.BoxBlur(3, new Rectangle(Math.Max(rectangle.X - ((rectangle.Width / 6) / 2), 0), rectangle.Y - (rectangle.Height / 6), Math.Max(rectangle.Width / 6, 1), rectangle.Height + (rectangle.Height / 6))));
		}

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle);

		protected void WriteSpecificFragmentData(BitBinaryWriter binaryWriter);

		protected Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader, QuadrantData quadrantData);

		public static void WriteFragmentData(Fragment fragment, BitBinaryWriter binaryWriter)
		{
			binaryWriter.WriteByte((byte)fragment.Id, 8, (byte)(8 - FragmentId._BitSize));
			fragment.WriteSpecificFragmentData(binaryWriter);
		}

		public static Fragment ReadFragmentData(BitBinaryReader binaryReader, QuadrantData quadrantData)
		{
			FragmentId id = (FragmentId)binaryReader.ReadByte(8, 6);

#if Debug_Reading_Deep
			Console.Write($"-ReadFragmentData{{{id}}}");
#endif

			return id switch
			{
				EmptyFragment.ConstId => new EmptyFragment(),
				SingleColorFragment.ConstId => new SingleColorFragment().ReadSpecificFragmentData(binaryReader, quadrantData),
				LinearGradientFragment.ConstId => new LinearGradientFragment().ReadSpecificFragmentData(binaryReader, quadrantData),
				FiveColorGradientFragment.ConstId => new FiveColorGradientFragment().ReadSpecificFragmentData(binaryReader, quadrantData),
			};
		}

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
