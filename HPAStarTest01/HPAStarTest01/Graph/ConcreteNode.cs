using HPAStarTest01.Infrastructure;
using System.Collections.Generic;
using System;

namespace HPAStarTest01.Graph
{
    public class ConcreteNode : INode<ConcreteNode, ConcreteNodeInfo, ConcreteEdge>
    {
        public Id<ConcreteNode> NodeId { get; set; }
        public ConcreteNodeInfo Info { get; set; }
        public IDictionary<Id<ConcreteNode>, ConcreteEdge> Edges { get; set; }

        public ConcreteNode(Id<ConcreteNode> nodeId, ConcreteNodeInfo info)
        {
            NodeId = nodeId;
            Info = info;
            Edges = new Dictionary<Id<ConcreteNode>, ConcreteEdge>();
        }

        public void RemoveEdge(Id<ConcreteNode> targetNodeId)
        {
            Edges.Remove(targetNodeId);
        }

        public void AddEdge(ConcreteEdge edge)
        {
            Edges[edge.TargetNodeId] = edge;
        }
    }

    public class ConcreteNodeInfo
    {
        public Position Position { get; set; }
        public bool IsObstacle { get; set; }
        public int Cost { get; set; }

        public ConcreteNodeInfo(bool isObstacle, int cost, Position position)
        {
            IsObstacle = isObstacle;
            Position = position;
            Cost = cost;
        }
    }

    public class ConcreteEdge : IEdge<ConcreteNode, ConcreteEdgeInfo>
    {
        public Id<ConcreteNode> TargetNodeId { get; set; }
        public ConcreteEdgeInfo Info { get; set; }

        public ConcreteEdge(Id<ConcreteNode> targetNodeId, ConcreteEdgeInfo info)
        {
            TargetNodeId = targetNodeId;
            Info = info;
        }
    }

    public class ConcreteEdgeInfo
    {
        public int Cost { get; set; }

        public ConcreteEdgeInfo(int cost)
        {
            Cost = cost;
        }
    }
}
