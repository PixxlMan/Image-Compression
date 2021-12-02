using Quad_Tree;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

int count = 0;

Image image = Image.Load(@$"R:\Image.jpeg");

image.Mutate(i => i.Pad(Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height)));

image.SaveAsBmp(@$"R:\\padded.bmp");

QuadTree<Image> quadTree = new QuadTree<Image>(image);

RecursivelyQuadrantise(quadTree.BaseNode, 4);

void RecursivelyQuadrantise(QuadTreeCell<Image> cell, int limit, int parentSegment = -1, int level = 0)
{
	if (level >= limit)
		return;

	count++;

	GetImageQuadrants(cell.LeafData, out var a1, out var b1, out var c1, out var d1);

	cell.Split(a1, b1, c1, d1);

	cell.A.LeafData.SaveAsBmp(@$"R:\\{count}a.bmp");
	cell.B.LeafData.SaveAsBmp(@$"R:\\{count}b.bmp");
	cell.C.LeafData.SaveAsBmp(@$"R:\\{count}c.bmp");
	cell.D.LeafData.SaveAsBmp(@$"R:\\{count}d.bmp");

	for (int i = 0; i < 4; i++)
	{
		RecursivelyQuadrantise(i switch
		{
			0 => cell.A,
			1 => cell.B,
			2 => cell.C,
			3 => cell.D,
		}, limit, i + 1, level + 1);
	}
}

static void GetImageQuadrants(Image image, out Image a, out Image b, out Image c, out Image d)
{
	a = image.Clone(i => i.Crop(new Rectangle(0, 0, image.Width / 2, image.Height / 2)));

	b = image.Clone(i => i.Crop(new Rectangle(image.Width / 2, 0, image.Width / 2, image.Height / 2)));

	c = image.Clone(i => i.Crop(new Rectangle(0, image.Height / 2, image.Width / 2, image.Height / 2)));

	d = image.Clone(i => i.Crop(new Rectangle(image.Width / 2, image.Height / 2, image.Width / 2, image.Height / 2)));
}
