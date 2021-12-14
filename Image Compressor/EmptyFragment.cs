using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_Compressor
{
	public sealed class EmptyFragment : Fragment
	{
		public EmptyFragment()
		{

		}

		public byte Id => 0;

		public static SingleColorFragment GenerateFragment(Image<Rgb24> image, Rectangle rectangle)
		{
			return new();
		}

		public void DrawRepresentation(Image<Rgb24> image, Rectangle rectangle)
		{
			
		}

		void Fragment.WriteSpecificFragmentData(BinaryWriter binaryWriter)
		{

		}

		static abstract Fragment Fragment.ReadSpecificFragmentData(BinaryReader binaryReader)
		{
			return new EmptyFragment();
		}
	}
}