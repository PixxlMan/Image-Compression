namespace Quad_Tree
{
	public class QuadTreeCell<TLeafData>
	{
		public QuadTreeCell(TLeafData leafData)
		{
			LeafData = leafData;
		}

		public QuadTreeCell<TLeafData>? A, B, C, D;

		public TLeafData? LeafData;

		public bool IsLeaf = true;

		public void Split(TLeafData a, TLeafData b, TLeafData c, TLeafData d)
		{
			IsLeaf = false;

			LeafData = default;

			A = new(a);
			B = new(b);
			C = new(c);
			D = new(d);
		}

		public void Unify(TLeafData leafData)
		{
			IsLeaf = true;

			LeafData = leafData;

			A = null;
			B = null;
			C = null;
			D = null;
		}
	}
}