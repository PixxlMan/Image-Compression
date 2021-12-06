using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Compressor
{
	public sealed class SingleColorFragment : Fragment
	{
		public SingleColorFragment(Rgba32 color, int width, int height) : base(width, height)
		{
			this.color = color;
		}

		private Rgba32 color;

		public static SingleColorFragment GenerateFragment(Image<Rgba32> image)
		{
			SingleColorFragment fragment = new(image[image.Width / 2, image.Height / 2], image.Width, image.Height);

			return fragment;
		}

		public override Image<Rgba32> GenerateRepresentation()
		{
			Image<Rgba32> image = new(Width, Height, color);

			return image;
		}
	}
}
