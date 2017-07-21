using HPAStarTest01.Infrastructure;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01.Search
{
    /// <summary>
	/// An A* node embeds the status of a processed node, containing information like
	/// the cost it's taken to reach it (Cost So far, G), the expected cost to reach the goal
	/// (The heuristic, H), the parent where this node came from (which will serve later to reconstruct best paths)
	/// the current Status of the node (Open, Closed, Unexplored, see CellStatus documentation for more information) and the F-score
	/// that serves to compare which nodes are the best
	/// </summary>
    public struct AStarNode<TNode>
    {
        public Id<TNode> Parent;
        public CellStatus Status;
        public int H;
        public int G;
        public int F;

        public AStarNode(Id<TNode> parent, int g, int h, CellStatus status)
        {
            Parent = parent;
            G = g;
            H = h;
            F = g + h;
            Status = status;
        }
    }

    /// <summary>
    /// The cell status indicates whether a node has not yet been processed 
    /// but it lies in the open queue (Open) or the node has been processed (Closed)
    /// 이 CellStatus는 노드가 처리되었는지 안되었는지를 나타낸다
    /// 하지만 이 노드들은 Open queue에 있거나 닫힌 노드들을 나타낸다
    /// </summary>
    public enum CellStatus
    {
        Open,
        Close
    }

    /// <summary>
    /// 경로 클래스
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class Path<TNode>
    {
        public int PathCost { get; private set; }
        public List<Id<TNode>> PathNodes { get; private set; }

        public Path(List<Id<TNode>> pathNodes, int pathCost)
        {
            PathCost = pathCost;
            PathNodes = pathNodes;
        }
    }

    /// <summary>
    /// 노드 검색 클래스
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class NodeLookup<TNode>
    {
        private AStarNode<TNode>?[] _astarNodes;

        public NodeLookup(int numberOfNodes)
        {
            _astarNodes = new AStarNode<TNode>?[numberOfNodes];
        }

        /// <summary>
        /// 노드에 Id부여
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="value"></param>
        public void SetNodeValue(Id<TNode> nodeId, AStarNode<TNode> value)
        {
            _astarNodes[nodeId.IdValue] = value;
        }

        /// <summary>
        /// 노드가 Open노드 또는 Closed노드에 있는지(노드를 검색한 적 있는지) 반환
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public bool NodeIsVisited(Id<TNode> nodeId)
        {
            return _astarNodes[nodeId.IdValue].HasValue;
        }

        /// <summary>
        /// 노드 Id반환
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public AStarNode<TNode> GetNodeValue(Id<TNode> nodeId)
        {
            return _astarNodes[nodeId.IdValue].Value;
        }
    }

    public class AStar<TNode>
    {
        private readonly Func<Id<TNode>, bool> _isGoal;
        private readonly Func<Id<TNode>, int> _calculateHeuristic;
        private readonly IMap<TNode> _map;
        private readonly SimplePriorityQueue<Id<TNode>> _openQueue;
        private readonly NodeLookup<TNode> _nodeLookup;

        public AStar(IMap<TNode> map, Id<TNode> startNodeId, Id<TNode> targetNodeId)
        {
            _isGoal = nodeId => nodeId == targetNodeId; //delegate(nodeId){ if(nodeId == targetNodId) return nodeId }와 같다
            _calculateHeuristic = nodeId => map.GetHeuristic(nodeId, targetNodeId);
            _map = map;

            int estimatedCost = _calculateHeuristic(startNodeId);

            AStarNode<TNode> startNode = new AStarNode<TNode>(startNodeId, 0, estimatedCost, CellStatus.Open);
            _openQueue = new SimplePriorityQueue<Id<TNode>>(); //외부참조 
            _openQueue.Enqueue(startNodeId, startNode.F);

            _nodeLookup = new NodeLookup<TNode>(map.NrNodes);
            _nodeLookup.SetNodeValue(startNodeId, startNode);
        }

        /// <summary>
        /// 닫힌 노드인지 판단
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public bool NodeIsClosed(Id<TNode> nodeId)
        {
            return _nodeLookup.NodeIsVisited(nodeId) && _nodeLookup.GetNodeValue(nodeId).Status == CellStatus.Close;
        }

        public bool CanExpand
        {
            get { return _openQueue != null && _openQueue.Count != 0; }
        }

        public static Path<TNode> FindBidPath(IMap<TNode> map, Id<TNode> startNodeId, Id<TNode> targetNodeId)
        {
            AStar<TNode> search1 = new AStar<TNode>(map, startNodeId, targetNodeId);
            AStar<TNode> search2 = new AStar<TNode>(map, targetNodeId, startNodeId);
            int expand = 0;

            while(search1.CanExpand && search2.CanExpand)
            {
                Id<TNode> frontier = search1.Expand();
                expand++;
                if (search2.NodeIsClosed(frontier)) // TODO: Need to add a condition to tell that the node was reachable
                {
                    return ReconstructPath(search1, search2, frontier);
                }
                frontier = search2.Expand();
                expand++;
                if(search1.NodeIsClosed(frontier)) // TODO: Need to add a condition to tell that the node was reachable
                {
                    return ReconstructPath(search1, search2, frontier);
                }
            }

            return new Path<TNode>(new List<Id<TNode>>(), -1);
        }

        /// <summary>
        /// 양방향에서 구해진 A* Path 2개를 하나를 reverse하여 합친다
        /// </summary>
        /// <param name="search1"></param>
        /// <param name="search2"></param>
        /// <param name="frontier"></param>
        /// <returns></returns>
        private static Path<TNode> ReconstructPath(AStar<TNode> search1, AStar<TNode> search2, Id<TNode> frontier)
        {
            Path<TNode> halfPath1 = search1.ReconstructPathFrom(frontier);
            Path<TNode> halfPath2 = search2.ReconstructPathFrom(frontier);

            halfPath2.PathNodes.Reverse();
            List<Id<TNode>> p = halfPath2.PathNodes;
            if(p.Count > 0)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    halfPath1.PathNodes.Add(p[i]);
                }
            }
            return halfPath1;
        }

        public Path<TNode> FindPath()
        {
            while (CanExpand)
            {
                Id<TNode> nodeId = Expand();
                if (_isGoal(nodeId))
                {
                    return ReconstructPathFrom(nodeId);
                }
            }

            return new Path<TNode>(new List<Id<TNode>>(), -1);
        }

        private Id<TNode> Expand()
        {
            Id<TNode> nodeId = _openQueue.Dequeue();
            AStarNode<TNode> node = _nodeLookup.GetNodeValue(nodeId);

            ProcessNeighbours(nodeId, node);

            _nodeLookup.SetNodeValue(nodeId, new AStarNode<TNode>(node.Parent, node.G, node.H, CellStatus.Close));

            return nodeId;
        }

        private void ProcessNeighbours(Id<TNode> nodeId, AStarNode<TNode> node)
        {
            IEnumerable<Connection<TNode>> connections = _map.GetConnections(nodeId);
            foreach(Connection<TNode> connection in connections)
            {
                int gCost = node.G + connection.Cost;
                Id<TNode> neighbour = connection.Target;
                if (_nodeLookup.NodeIsVisited(neighbour))
                {
                    AStarNode<TNode> targetAstarNode = _nodeLookup.GetNodeValue(neighbour);
                    // If we already processed the neighbour in the past or we already found in the past
                    // a better path to reach this node that the current one, just skip it, else create
                    // and replace a new PathNode
                    // 만약 이웃 노드 처리가 끝났고, 현재 노드까지의 더 좋은 루트를 이전에 찾았다면 스킵
                    // 아니면 새 targetAstarNode를 찾고 새로운 PathNode로 교체
                    if (targetAstarNode.Status == CellStatus.Close || gCost >= targetAstarNode.G) //닫혀있고 gCost가 
                        continue;

                    targetAstarNode = new AStarNode<TNode>(nodeId, gCost, targetAstarNode.H, CellStatus.Open);
                    _openQueue.UpdatePriority(neighbour, targetAstarNode.F);
                    _nodeLookup.SetNodeValue(neighbour, targetAstarNode);
                }
                else
                {
                    int newHeuristic = _calculateHeuristic(neighbour);
                    AStarNode<TNode> newAStarNode = new AStarNode<TNode>(nodeId, gCost, newHeuristic, CellStatus.Open);
                    _openQueue.Enqueue(neighbour, newAStarNode.F);
                    _nodeLookup.SetNodeValue(neighbour, newAStarNode);
                }
            }
        }

        /// <summary>
		/// Reconstructs the path from the destination node with the aid
		/// of the node Lookup that stored the states of all processed nodes
		/// TODO: Maybe I should guard this with some kind of safetyGuard to prevent
		/// possible infinite loops in case of bugs, but meh...
		/// </summary>
        private Path<TNode> ReconstructPathFrom(Id<TNode> destination)
        {
            List<Id<TNode>> pathNodes = new List<Id<TNode>>();
            int pathCost = _nodeLookup.GetNodeValue(destination).F;
            Id<TNode> currentNode = destination;
            while(_nodeLookup.GetNodeValue(currentNode).Parent != currentNode)
            {
                pathNodes.Add(currentNode);
                currentNode = _nodeLookup.GetNodeValue(currentNode).Parent;
            }

            pathNodes.Add(currentNode);
            pathNodes.Reverse();

            return new Path<TNode>(pathNodes, pathCost);
        }
    }
}
