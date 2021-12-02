namespace Quad_Tree
{
	public class QuadTree<TLeafData> where TLeafData : class
	{
		public QuadTree(TLeafData initialData)
		{
			BaseNode = new QuadTreeCell<TLeafData>(initialData);
		}

		public QuadTreeCell<TLeafData> BaseNode;
	}
}