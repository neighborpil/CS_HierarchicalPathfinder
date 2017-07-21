using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;
using HPAStarTest01.Search;
using System;
using System.Collections.Generic;

namespace HPAStarTest01
{
    /// <summary>
    /// 방향
    /// </summary>
    public enum Orientation
    {
        Horizontal, Vertical, Hdiag1, Hdiag2, Vdiag1, Vdiag2
    }

    /// <summary>
	/// An Entrance Point represents a point inside a cluster   
	/// that belongs to an entrance. It holds a reference to the
	/// abstract node it belongs to
    /// Entrance Point는 enterance에 속해 있는 클러스트의 내부를 나타낸다
    /// 이것은 이것이 속해 있는 추상노드에 대한 참고를 가지고 있다
	/// </summary>
    public class EntrancePoint
    {
        public Id<AbstractNode> AbstractNodeId { get; set; }
        public Position RelativePosition { get; set; }

        public EntrancePoint(Id<AbstractNode> abstractNodeId, Position relativePosition)
        {
            AbstractNodeId = abstractNodeId;
            RelativePosition = relativePosition;
        }
    }

    public class Cluster
    {
        public Id<Cluster> Id { get; set; }
        public int ClusterX { get; set; }
        public int ClusterY { get; set; }

        /// <summary>
	    /// A 2D array which represents a distance between 2 entrances.
	    /// This array could be represented as a Dictionary, but it's faster
	    /// to use an array.
        /// 2 enterance간의 거리를 나타낸다
	    /// </summary>
        private readonly Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, int> _distances;

        private readonly Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, List<Id<ConcreteNode>>> _cachedPaths;

        // Tells whether a path has already been calculated for 2 node ids
        // 2노드 아이디간의 path가 이미 결정되어 있는지 판단
        private readonly Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, bool> _distanceCalculated;

        public List<EntrancePoint> EntrancePoints { get; set; }

        // This concreteMap object contains the subregion of the main grid that this cluster contains.
        // Necessary to do local search to find paths and distances between local entrances
        // 이 콘크리트 오브젝트는 이 클러스터를 포함하는 메인그리드의 subregion을 가지고 있다
        // 지역의 출입구 간의 거리 및 경로를 찾기 위하여 필요하다
        public ConcreteMap SubConcreteMap { get; set; }
        public Size Size { get; set; }
        public Position Origin { get; set; }

        public Cluster(ConcreteMap concreteMap, Id<Cluster> id, int clusterX, int clusterY, Position origin, Size size)
        {
            SubConcreteMap = concreteMap.Slice(origin.X, origin.Y, size.Width, size.Height, concreteMap.Passability);
            Id = id;
            ClusterX = clusterX;
            ClusterY = clusterY;
            Origin = origin;
            Size = size;
            _distances = new Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, int>();
            _cachedPaths = new Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, List<Id<ConcreteNode>>>();
            _distanceCalculated = new Dictionary<Tuple<Id<AbstractNode>, Id<AbstractNode>>, bool>();
            EntrancePoints = new List<EntrancePoint>();
        }

        public void CreateIntraClusterEdges()
        {
            foreach (EntrancePoint point1 in EntrancePoints)
                foreach (EntrancePoint point2 in EntrancePoints)
                    ComputePathBetweenEntrances(point1, point2);
        }

        /// <summary>
        /// Gets the index of the entrance point inside this cluster
        /// 이 클러스터 내의 entrance point의 인덱스를 가져온다
        /// </summary>
        /// <param name="entrancePoint"></param>
        /// <returns></returns>
        private int GetEntrancePositionIndex(EntrancePoint entrancePoint)
        {
            return entrancePoint.RelativePosition.Y * Size.Width + entrancePoint.RelativePosition.X;
        }

        private void ComputePathBetweenEntrances(EntrancePoint e1, EntrancePoint e2)
        {
            if (e1.AbstractNodeId == e2.AbstractNodeId)
                return;

            Tuple<Id<AbstractNode>, Id<AbstractNode>> tuple = Tuple.Create(e1.AbstractNodeId, e2.AbstractNodeId);
            Tuple<Id<AbstractNode>, Id<AbstractNode>> invtuple = Tuple.Create(e2.AbstractNodeId, e1.AbstractNodeId);

            if (_distanceCalculated.ContainsKey(tuple))
                return;

            Id<ConcreteNode> startNodeId = Id<ConcreteNode>.From(GetEntrancePositionIndex(e1));
            Id<ConcreteNode> targetNodeId = Id<ConcreteNode>.From(GetEntrancePositionIndex(e2));
            AStar<ConcreteNode> search = new AStar<ConcreteNode>(SubConcreteMap, startNodeId, targetNodeId);
        }









    }
}
