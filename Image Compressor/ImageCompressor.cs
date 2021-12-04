using Quad_Tree;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Image_Compressor;

public static class ImageCompressor
{
	public static void Compress(Image<Rgba32> image, float complexityThreshold, int maxLimit)
	{
		int limit = maxLimit;

		int quadEdge = Math.Max(image.Width, image.Height);
		for (int i = 0; i < maxLimit; i++)
		{
			if (quadEdge % 2 == 0)
			{
				quadEdge /= 2;
			}
			else
			{
				limit = i;

				//Console.WriteLine($"actual limit used: {limit}");

				break;
			}
		}

		image.Mutate(i => i.Pad(Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height)));

		QuadTree<Image<Rgba32>> quadTree = new(image);

		int count = 0;
		RecursivelyQuadrantise(quadTree.BaseNode, (i => CalculateImageComplexity(i) < complexityThreshold), limit, ref count);

		QuadTree<Fragment> fragmentTree = new(null);

		RecursivelyPopulateFragmentTree(fragmentTree.BaseNode, quadTree.BaseNode);

		Image<Rgba32> outputImage = new Image<Rgba32>(image.Width, image.Height);
		RecursivelyAssembleOutputImage(fragmentTree.BaseNode, ref outputImage, image.Bounds());
		outputImage.SaveAsBmp(@$"R:\output.bmp");
	}

	private static void RecursivelyAssembleOutputImage(QuadTreeCell<Fragment> fragmentCell, ref Image<Rgba32> outputImage, Rectangle rectangle)
	{
		Point point = new Point(rectangle.X, rectangle.Y);
		Size size = new Size(rectangle.Width, rectangle.Height);

		if (fragmentCell.LeafData is not null)
		{
			Image<Rgba32> image = fragmentCell.LeafData.GenerateRepresentation();

			outputImage.Mutate(i => i.DrawImage(image, point, 1f));

			//outputImage.SaveAsBmp(@$"R:\{Guid.NewGuid()}.bmp");

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
			}, ref outputImage, i switch
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
	
	public static void GetRectangleQuadrantsForWholeImage(Rectangle rectangle, out Rectangle a, out Rectangle b, out Rectangle c, out Rectangle d)
	{
		a = new Rectangle(0, 0, rectangle.Width / 2, rectangle.Height / 2);

		b = new Rectangle(rectangle.Width / 2, 0, rectangle.Width / 2, rectangle.Height / 2);

		c = new Rectangle(0, rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);

		d = new Rectangle(rectangle.Width / 2, rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);
	}

	private static void RecursivelyPopulateFragmentTree(QuadTreeCell<Fragment> fragmentCell, QuadTreeCell<Image<Rgba32>> imageCell)
	{
		if (imageCell.LeafData is not null)
		{
			fragmentCell.LeafData = Fragment.GenerateFragment(imageCell.LeafData);

			imageCell.LeafData.Dispose();

			return;
		}

		fragmentCell.Split(null, null, null, null);

		for (int i = 0; i < 4; i++)
		{
			RecursivelyPopulateFragmentTree(i switch
			{
				0 => fragmentCell.A,
				1 => fragmentCell.B,
				2 => fragmentCell.C,
				3 => fragmentCell.D,
			}, i switch
			{
				0 => imageCell.A,
				1 => imageCell.B,
				2 => imageCell.C,
				3 => imageCell.D,
			});
		}
	}

	static void RecursivelyQuadrantise(QuadTreeCell<Image<Rgba32>> cell, Func<Image, bool> quadrantizePredicate, int limit, ref int count, int parentSegment = -1, int level = 0)
	{
		if (level > limit)
			return;

		if (quadrantizePredicate(cell.LeafData))
			return;

		GetImageQuadrants(cell.LeafData, out var a1, out var b1, out var c1, out var d1);

		cell.LeafData.Dispose();

		cell.Split(a1, b1, c1, d1);

		count++;

#if file_debugging
		File.WriteAllBytes(@$"R:\{count}------{level}", new byte[] { 69 });

		cell.A.LeafData.SaveAsBmp(@$"R:\{count}a{level}.bmp");
		cell.B.LeafData.SaveAsBmp(@$"R:\{count}b{level}.bmp");
		cell.C.LeafData.SaveAsBmp(@$"R:\{count}c{level}.bmp");
		cell.D.LeafData.SaveAsBmp(@$"R:\{count}d{level}.bmp");
#endif

		for (int i = 0; i < 4; i++)
		{
			RecursivelyQuadrantise(i switch
			{
				0 => cell.A,
				1 => cell.B,
				2 => cell.C,
				3 => cell.D,
			}, quadrantizePredicate, limit, ref count, i + 1, level + 1);
		}
	}

	public static void GetImageQuadrants(Image<Rgba32> image, out Image<Rgba32> a, out Image<Rgba32> b, out Image<Rgba32> c, out Image<Rgba32> d)
	{
		GetRectangleQuadrantsForWholeImage(new Rectangle(Point.Empty, new Size(image.Width, image.Height)), out var aQuadr, out var bQuadr, out var cQuadr, out var dQuadr);

		a = image.Clone(i => i.Crop(aQuadr));

		b = image.Clone(i => i.Crop(bQuadr));

		c = image.Clone(i => i.Crop(cQuadr));

		d = image.Clone(i => i.Crop(dQuadr));
	}

	public static double CalculateImageComplexity(Image image)
	{
		Image<Rgba32> greyScale = (Image<Rgba32>)image.Clone(i => i.Grayscale());
		return GetEntropy(greyScale, 256);
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static uint[] GenerateHistogram(Image<Rgba32> image, uint numberOfBins)
	{
		uint[] bins = new uint[numberOfBins];
		uint intensity;
		for (int i = 0; i <= image.Width - 1; i++)
		{
			for (int j = 0; j <= image.Height - 1; j++)
			{
				intensity = image[i, j].R;
				bins[intensity]++;
			}
		}
		return bins;
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static double GetEntropy(Image<Rgba32> image, uint maxValue)
	{
		uint[] bins = GenerateHistogram(image, maxValue);
		double entropyValue = 0, temp = 0;
		double totalSize = image.Height * image.Width; //total size of all symbols in an image

		for (int i = 0; i < maxValue; i++)
		{ //the number of times a sybmol has occured
			if (bins[i] > 0)
			{ //log of zero goes to infinity
				temp = (bins[i] / totalSize) * (Math.Log(totalSize / bins[i]));
				entropyValue += temp;
			}
		}
		return entropyValue;
	}
}
