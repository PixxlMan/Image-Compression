﻿using Quad_Tree;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Image_Compressor;

public static class ImageCompressor
{
	public static QuadTree<Fragment> Compress(Image<Rgb24> image, float complexityThreshold, int maxLimit, int minLimit)
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
				limit = i;

				Console.WriteLine($"actual limit used: {limit}");

				break;
			}
		}

		Rectangle imageBounds = image.Bounds();

		image.Mutate(i => i.Pad(edge, edge));

		QuadTree<Rectangle> quadTree = new(image.Bounds());

		imageBounds.Offset((edge - imageBounds.Width) / 2, (edge - imageBounds.Height) / 2);

		//image.Mutate(i => i.DrawPolygon(new Pen(Color.Blue, 4), new RectangularPolygon(imageBounds).Points.ToArray()));

		RecursivelyQuadrantise(image, quadTree.BaseNode, ((i, r) => CalculateImageComplexity(i, r) < complexityThreshold), imageBounds, limit, minLimit);

		Console.WriteLine("quadrantized");

		QuadTree<Fragment> fragmentTree = new(null);

		RecursivelyPopulateFragmentTree(fragmentTree.BaseNode, quadTree.BaseNode, image);

		Console.WriteLine("populated frag tree");

		return fragmentTree;
	}

	public static Image<Rgb24> Decompress(QuadTree<Fragment> fragmentTree, int width, int height)
	{
		Image<Rgb24> outputImage = new Image<Rgb24>(width, height);
		RecursivelyAssembleOutputImage(fragmentTree.BaseNode, outputImage, new Rectangle(0, 0, width, height));
		outputImage.SaveAsBmp(@$"R:\output.bmp");

		Console.WriteLine("assembled");

		return outputImage;
	}

	public static void SaveToFile(QuadTree<Fragment> fragmentTree, string path)
	{
		File.Delete(path);
		using FileStream fileStream = File.OpenWrite(path);
		using BinaryWriter binaryWriter = new BinaryWriter(fileStream);

		RecursivelyWriteQuadTreeData(fragmentTree.BaseNode, binaryWriter);

		Console.WriteLine("saved");
	}

	public static void RecursivelyWriteQuadTreeData(QuadTreeCell<Fragment> quadTreeCell, BinaryWriter binaryWriter)
	{
		binaryWriter.Write(quadTreeCell.IsLeaf);

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

	public static QuadTree<Fragment> LoadFromFile(string path)
	{
		using FileStream fileStream = File.OpenRead(path);
		using BinaryReader binaryReader = new BinaryReader(fileStream);

		QuadTree<Fragment> fragmentTree = new QuadTree<Fragment>(null);

		RecursivelyReadQuadTreeData(fragmentTree.BaseNode, binaryReader);

		Console.WriteLine("loaded");

		return fragmentTree;
	}

	public static void RecursivelyReadQuadTreeData(QuadTreeCell<Fragment> quadTreeCell, BinaryReader binaryReader)
	{
		if (binaryReader.ReadBoolean() == true)
		{// The current cell is a leaf!
			quadTreeCell.LeafData = Fragment.ReadFragmentData(binaryReader);

			return;
		}

		quadTreeCell.Split(null, null, null, null);

		RecursivelyReadQuadTreeData(quadTreeCell.A, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.B, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.C, binaryReader);
		RecursivelyReadQuadTreeData(quadTreeCell.D, binaryReader);
	}

	private static void RecursivelyAssembleOutputImage(QuadTreeCell<Fragment> fragmentCell, Image<Rgb24> outputImage, Rectangle rectangle)
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
	
	public static void GetRectangleQuadrantsForWholeImage(Rectangle rectangle, out Rectangle a, out Rectangle b, out Rectangle c, out Rectangle d)
	{
		a = new Rectangle(0, 0, rectangle.Width / 2, rectangle.Height / 2);

		b = new Rectangle(rectangle.Width / 2, 0, rectangle.Width / 2, rectangle.Height / 2);

		c = new Rectangle(0, rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);

		d = new Rectangle(rectangle.Width / 2, rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);
	}

	private static void RecursivelyPopulateFragmentTree(QuadTreeCell<Fragment> fragmentCell, QuadTreeCell<Rectangle> imageRectangleCell, Image<Rgb24> image)
	{
		if (imageRectangleCell.IsLeaf)
		{
			fragmentCell.LeafData = Fragment.GenerateFragment(image, imageRectangleCell.LeafData);

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
				0 => imageRectangleCell.A,
				1 => imageRectangleCell.B,
				2 => imageRectangleCell.C,
				3 => imageRectangleCell.D,
			}, image);
		}
	}

	static void RecursivelyQuadrantise(Image<Rgb24> image, QuadTreeCell<Rectangle> cell, Func<Image<Rgb24>, Rectangle, bool> quadrantizePredicate, Rectangle imageBounds, int limit, int minLimit, int parentSegment = -1, int level = 0)
	{
		if (level > limit)
			return;

		if (!imageBounds.IntersectsWith(cell.LeafData))
			return;

		if (level > 5)
			if (quadrantizePredicate(image, cell.LeafData))
				return;

		GetRectangleQuadrants(cell.LeafData, out var a1, out var b1, out var c1, out var d1);

		cell.Split(a1, b1, c1, d1);

		for (int i = 0; i < 4; i++)
		{
			RecursivelyQuadrantise(image,
			i switch
			{
				0 => cell.A,
				1 => cell.B,
				2 => cell.C,
				3 => cell.D,
			}, quadrantizePredicate, imageBounds, limit, minLimit, i + 1, level + 1);
		}
	}

	public static double CalculateImageComplexity(Image<Rgb24> image, Rectangle rectangle)
	{
		return GetEntropy(image, rectangle, 256);
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static int[] GenerateHistogram(Image<Rgb24> image, Rectangle rectangle, int numberOfBins)
	{
		int[] bins = new int[numberOfBins];
		int intensity;
		for (int i = rectangle.Left; i <= rectangle.Right - 1; i++)
		{
			for (int j = rectangle.Top; j <= rectangle.Bottom - 1; j++)
			{
				intensity = (image[i, j].R / 3) + (image[i, j].G / 3) + (image[i, j].B / 3);
				bins[intensity]++;
			}
		}
		return bins;
	}

	// (Modified and translated to C#) From: https://github.com/Jeanvit/CakeImageAnalyzer/blob/master/src/cake/classes/ImageUtils.java
	public static double GetEntropy(Image<Rgb24> image, Rectangle rectangle, int maxValue)
	{
		int[] bins = GenerateHistogram(image, rectangle, maxValue);
		double entropyValue = 0, temp;
		double totalSize = rectangle.Height * rectangle.Width; //total size of all symbols in an image

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
