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

	public static void GetRectangleQuadrants(Rectangle rectangle, out Rectangle a, out Rectangle b, out Rectangle c, out Rectangle d)
	{
		Size size = new Size(rectangle.Width / 2, rectangle.Height / 2);

		a = new Rectangle(new Point(rectangle.Left, rectangle.Top), size);
		b = new Rectangle(new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top), size);
		c = new Rectangle(new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2), size);
		d = new Rectangle(new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2), size);
	}
}
