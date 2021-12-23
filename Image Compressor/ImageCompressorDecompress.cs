using Quad_Tree;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Image_Compressor;

public static partial class ImageCompressor
{
	public static Image<Rgb24> Decompress(QuadTree<Fragment, QuadrantData> fragmentTree, int width, int height)
	{
		Image<Rgb24> outputImage = new Image<Rgb24>(width, height);
		RecursivelyAssembleOutputImage(fragmentTree.BaseNode, outputImage, new Rectangle(0, 0, width, height));
		outputImage.SaveAsBmp(@$"R:\output.bmp");

		Console.WriteLine("assembled");

		return outputImage;
	}

	private static void RecursivelyAssembleOutputImage(QuadTreeCell<Fragment, QuadrantData> fragmentCell, Image<Rgb24> outputImage, Rectangle rectangle)
	{
		if (fragmentCell.LeafData is not null)
		{
			fragmentCell.LeafData.DrawRepresentation(outputImage, rectangle);

			return;
		}

		GetRectangleQuadrants(rectangle, out var a, out var b, out var c, out var d);
		for (int i = 0; i < 4; i++)
		{
			RecursivelyAssembleOutputImage(i switch
			{
				0 => fragmentCell.A,
				1 => fragmentCell.B,
				2 => fragmentCell.C,
				3 => fragmentCell.D,
			}, outputImage, i switch
			{
				0 => a,
				1 => b,
				2 => c,
				3 => d,
			});
		}
	}
}
