using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_Compressor
{
	public struct EmptyFragment : Fragment
	{
		public byte Id => 0;

		public static SingleColorFragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			return new();
		}

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			
		}

		void Fragment.WriteSpecificFragmentData(BitBinaryWriter binaryWriter)
		{

		}

		public Fragment ReadSpecificFragmentData(BitBinaryReader binaryReader)
		{
			return this;
		}
	}
}