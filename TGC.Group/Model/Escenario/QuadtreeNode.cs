using TGC.Core.SceneLoader;

namespace TGC.Examples.Optimization.Quadtree
{
    /// <summary>
    ///     Nodo del �rbol Quadtree
    /// </summary>
    internal class QuadtreeNode
    {
        public QuadtreeNode[] children;
        public TgcMesh[] models;

        public bool isLeaf()
        {
            return children == null;
        }
    }
}