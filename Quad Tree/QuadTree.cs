namespace Quad_Tree
{
	public class QuadTree<TLeafData, TData>
	{
		public QuadTree(TLeafData initialData)
		{
			BaseNode = new QuadTreeCell<TLeafData, TData>(initialData);
		}

		public QuadTreeCell<TLeafData, TData> BaseNode;
	}
}