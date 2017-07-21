using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01
{
    #region Abstract ConcreteMap support classes

    public struct Connection<TNode>
    {
        public Id<TNode> Target;
        public int Cost;

        public Connection(Id<TNode> target, int cost)
        {
            Target = target;
            Cost = cost;
        }
    }

    public enum AbsType
    {
        ABSTRACT_TILE,
        ABSTRACT_OCTILE,
        ABSTRACT_OCTILE_UNICOST
    }

    #endregion

    /// <summary>
    /// Abstract maps represent, as the name implies, an abstraction
    /// built over the concrete map.
    /// 
    /// </summary>
    public class HierarchicalMap : IMap<AbstractNode>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public AbstractGraph AbstractGraph { get; set; }
        public int ClusterSize { get; set; }
        public int MaxLevel { get; set; }
        public List<Cluster> Clusters { get; set; }
        public int NrNodes { get { return AbstractGraph.Nodes.Count; } }

        // This list, indexed by a node id from the low level, 
        // indicates to which abstract node id it maps. It is a sparse
        // array for quick access. For saving memory space, this could be implemented as a dictionary
        // NOTE: It is currently just used for insert and remove STAL
        // 로우레벨에서 노드 아이디로 인덱스된 이 리스트는 어떤 추상 노드가 맵을 형성하고 있는지 나타낸다
        // 이는 빠른 액세스를 위한 드문 배열이다. 메모리를 줄이기 위해 이것은 딕셔너리를 사용한다
        // 이것은 현재 단지 STAL의 입력과 삭제를 위해 사용된다
        public Dictionary<Id<ConcreteNode>, Id<AbstractNode>> ConcreteNodeIdToAbstractNodeIdMap { get; set; }
        public AbsType Type { get; set; }

        private int currentLevel;
        private int currentClusterY0;
        private int currentClusterY1;
        private int currentClusterX0;
        private int currentClusterX1;

        public void SetType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Tile:
                    Type = AbsType.ABSTRACT_TILE;
                    break;
                case TileType.Octile:
                    Type = AbsType.ABSTRACT_OCTILE;
                    break;
                case TileType.OctileUnicost:
                    Type = AbsType.ABSTRACT_OCTILE_UNICOST;
                    break;
            }
        }

        public HierarchicalMap(ConcreteMap concreteMap, int clusterSize, int maxLevel)
        {
            ClusterSize = clusterSize;
            MaxLevel = maxLevel;

            SetType(concreteMap.TileType);
            this.Width = concreteMap.Width;
            this.Height = concreteMap.Height;
            ConcreteNodeIdToAbstractNodeIdMap = new Dictionary<Id<ConcreteNode>, Id<AbstractNode>>();

            Clusters = new List<Cluster>();
            AbstractGraph = new AbstractGraph();
        }

        public int GetHeuristic(Id<AbstractNode> startNodeId, Id<AbstractNode> targetNodeId)
        {
            Position startPos = AbstractGraph.GetNodeInfo(startNodeId).Position;
            Position targetPos = AbstractGraph.GetNodeInfo(targetNodeId).Position;
            int diffY = Math.Abs(startPos.Y - targetPos.Y);
            int diffX = Math.Abs(startPos.X - targetPos.X);
            // Manhattan distance, after testing a bit for hierarchical searches we do not need
            // the level of precision of Diagonal distance or euclidean distance
            // 몇몇 계층적 탐색을 테스트 해본 이후 우리는 대각선 또는 기하학적인 거리의 레벨이 필요없으므로
            // 맨하튼 디스턴스 휴리스틱을 사용한다.
            return (diffX + diffX) * Constants.COST_ONE;
        }

        public Cluster FindClusterForPosition(Position pos)
        {
            Cluster foundCluster = null;
            foreach(Cluster cluster in Clusters)
            {
                if (cluster.Origin.Y <= pos.Y &&
                    pos.Y < cluster.Origin.Y + cluster.Size.Height &&
                    cluster.Origin.X <= pos.X &&
                    pos.X < cluster.Origin.X + cluster.Size.Width)
                {
                    foundCluster = cluster;
                    break;
                }
            }
            return foundCluster;
        }
    }
}
