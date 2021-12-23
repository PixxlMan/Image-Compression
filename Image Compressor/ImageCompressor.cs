using Quad_Tree;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Image_Compressor;

public static class ImageCompressor
{
	public static QuadTree<Fragment, QuadrantData> Compress(Image<Rgb24> image, float complexityThreshold, int maxLimit, int minLimit)
	{
		int limit = maxLimit;

		int edge = (int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(image.Width, image.Height));

		int quadEdge = edge;
		for (int i = 0; i < maxLimit; i++)
		{
			if ((quadEdge / 2) % 2 == 0)
			{
				quadEdge /= 2;
			}
			else
			{
				limit = i + 1;

				Console.WriteLine($"actual limit used: {limit}");

				break;
			}
		}

		Rectangle imageBounds = image.Bounds();

		image.Mutate(i => i.Pad(edge, edge));

		imageBounds.Offset((edge - imageBounds.Width) / 2, (edge - imageBounds.Height) / 2);

		//image.Mutate(i => i.DrawPolygon(new Pen(Color.Blue, 4), new RectangularPolygon(imageBounds).Points.ToArray()));

		QuadTree<Fragment, QuadrantData> fragmentTree = new(null);

		RecursivelyGenerateFragmentTree(fragmentTree.BaseNode, image, ((i, r) => CalculateImageComplexity(i, r) < complexityThreshold), imageBounds, image.Bounds(), limit, minLimit);

		Console.WriteLine("generated fragment tree");

		return fragmentTree;
	}

	public static Image<Rgb24> Decompress(QuadTree<Fragment, QuadrantData> fragmentTree, int width, int height)
	{
		Image<Rgb24> outputImage = new Image<Rgb24>(width, height);
		RecursivelyAssembleOutputImage(fragmentTree.BaseNode, outputImage, new Rectangle(0, 0, width, height));
		outputImage.SaveAsBmp(@$"R:\output.bmp");

		Console.WriteLine("assembled");

		return outputImage;
	}

	public static void SaveToFile(QuadTree<Fragment, QuadrantData> fragmentTree, string path)
	{
		File.Delete(path);
		using FileStream fileStream = File.OpenWrite(path);
		using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		using BitBinaryWriter bitBinaryWriter = new BitBinaryWriter(binaryWriter);

		RecursivelyWriteQuadTreeData(fragmentTree.BaseNode, bitBinaryWriter);

		Console.WriteLine("saved");
	}

	public static void RecursivelyWriteQuadTreeData(QuadTreeCell<Fragment, QuadrantData> quadTreeCell, BitBinaryWriter binaryWriter)
	{
		binaryWriter.WriteBit(quadTreeCell.IsLeaf);

		if (quadTreeCell.IsLeaf)
		{
			Fragment.WriteFragmentData(quadTreeCell.LeafData, binaryWriter);

			return;
		}

		RecursivelyWriteQuadTreeData(quadTreeCell.A, binaryWriter);
		RecursivelyWriteQuadTreeData(quadTreeCell.B, binaryWriter);
		RecursivelyWriteQuadTreeData(quadTreeCell.C, binaryWriter);
		RecursivelyWriteQuadTreeData(quadTreeCell.D, binaryWriter);
	}

	public static QuadTree<Fragment, QuadrantData> LoadFromFile(string path)
	{
		using FileStream fileStream = File.OpenRead(path);
		using BinaryReader binaryReader = new BinaryReader(fileStream);
		BitBinaryReader bitBinaryReader = new BitBinaryReader(binaryReader);

		QuadTree<Fragment, QuadrantData> fragmentTree = new QuadTree<Fragment, QuadrantData>(null);

		RecursivelyReadQuadTreeData(fragmentTree.BaseNode, bitBinaryReader);

		Console.WriteLine("loaded");

		return fragmentTree;
	}

	public static void RecursivelyReadQuadTreeData(QuadTreeCell<Fragment, QuadrantData> quadTreeCell, BitBinaryReader binaryReader)
	{
		if (binaryReader.ReadBit() == true)
		{// The current cell is a leaf!
#if Debug_Reading_Deep
			Console.Write("-Leaf");
#endif

			quadTreeCell.Unify(Fragment.ReadFragmentData(binaryReader));

			return;
		}
#if Debug_Reading_Deep
		else
				Console.Write("-Branch");
#endif

		quadTreeCell.Split(null, null, null, null);

		RecursivelyReadQuadTreeData(quadTreeCell.A, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.B, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.C, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.D, binaryReader);
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

	public static void GetRectangleQuadrants(Rectangle rectangle, out Rectangle a, out Rectangle b, out Rectangle c, out Rectangle d)
	{
		Size size = new Size(rectangle.Width / 2, rectangle.Height / 2);

		a = new Rectangle(new Point(rectangle.Left, rectangle.Top), size);
		b = new Rectangle(new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top), size);
		c = new Rectangle(new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2), size);
		d = new Rectangle(new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2), size);
	}

	private static void RecursivelyGenerateFragmentTree(QuadTreeCell<Fragment, QuadrantData> fragmentCell, Image<Rgb24> image, Func<Image<Rgb24>, Rectangle, bool> quadrantizePredicate, Rectangle imageBounds, Rectangle rectangle, int limit, int minLimit, int level = 0)
	{

		if (!imageBounds.IntersectsWith(rectangle))
		{
			fragmentCell.LeafData = new EmptyFragment();
			return;
		}

		if (level > minLimit)
			if (quadrantizePredicate(image, rectangle))
			{
				fragmentCell.LeafData = Fragment.GenerateFragment(image, rectangle);
				return;
			}

		if (level + 1 > limit)
		{
			fragmentCell.LeafData = Fragment.GenerateFragment(image, rectangle);
			return;
		}

		GetRectangleQuadrants(rectangle, out var a1, out var b1, out var c1, out var d1);

		fragmentCell.Split(null, null, null, null);

		RecursivelyGenerateFragmentTree(fragmentCell.A, image, quadrantizePredicate, imageBounds, a1, limit, minLimit, level + 1);
		RecursivelyGenerateFragmentTree(fragmentCell.B, image, quadrantizePredicate, imageBounds, b1, limit, minLimit, level + 1);
		RecursivelyGenerateFragmentTree(fragmentCell.C, image, quadrantizePredicate, imageBounds, c1, limit, minLimit, level + 1);
		RecursivelyGenerateFragmentTree(fragmentCell.D, image, quadrantizePredicate, imageBounds, d1, limit, minLimit, level + 1);
	}

	public static double CalculateImageComplexity(Image<Rgb24> image, Rectangle rectangle)
	{
		return GetEntropy(image, rectangle, 256);
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static byte[] GenerateHistogram(Image<Rgb24> image, Rectangle rectangle, int numberOfBins)
	{
		byte[] bins = new byte[numberOfBins];
		int intensity;
		for (int i = rectangle.Left; i <= rectangle.Right - 1; i++)
		{
			for (int j = rectangle.Top; j <= rectangle.Bottom - 1; j++)
			{
				intensity = (image[i, j].R / 3) + (image[i, j].G / 3) + (image[i, j].B / 3);

				// Crush away color data, to attempt to allow the alghoritm to only focus on significant color differences.
				intensity /= 4;
				intensity *= 4;

				bins[intensity]++;
			}
		}
		return bins;
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static float GetEntropy(Image<Rgb24> image, Rectangle rectangle, int maxValue)
	{
		byte[] bins = GenerateHistogram(image, rectangle, maxValue);
		float entropyValue = 0, temp;
		float totalSize = rectangle.Height * rectangle.Width; //total size of all symbols in an image

		for (int i = 0; i < maxValue; i++)
		{ //the number of times a sybmol has occured
			if (bins[i] > 0)
			{ //log of zero goes to infinity
				temp = (float)((bins[i] / totalSize) * (Math.Log(totalSize / bins[i])));
				entropyValue += temp;
			}
		}
		return entropyValue;
	}
}
