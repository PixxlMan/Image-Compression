namespace Quad_Tree
{
	public class QuadTreeCell<TLeafData> where TLeafData : class
	{
		public QuadTreeCell(TLeafData leafData)
		{
			LeafData = leafData;
		}

		public QuadTreeCell<TLeafData>? A, B, C, D;

		public TLeafData? LeafData;

		public void Split(TLeafData a, TLeafData b, TLeafData c, TLeafData d)
		{
			LeafData = null;

			A = new(a);
			B = new(b);
			C = new(c);
			D = new(d);
		}

		public void Unify(TLeafData leafData)
		{
			LeafData = leafData;

			A = null;
			B = null;
			C = null;
			D = null;
		}
	}
}