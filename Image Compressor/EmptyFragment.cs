using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_Compressor
{
	public class EmptyFragment : Fragment
	{
		public EmptyFragment()
		{

		}

		public override byte Id => 0;

		public static SingleColorFragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			return new();
		}

		public override void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			
		}

		protected override void WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{

		}

		protected override Fragment ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			return this;
		}
	}
}