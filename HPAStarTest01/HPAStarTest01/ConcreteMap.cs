using HPAStarTest01.Factories;
using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;

namespace HPAStarTest01
{
    public enum TileType
    {
        Hex,
        Octile, /** Octiles with cost 1 to adjacent and sqrt(2) to diagonal. */
        OctileUnicost, /** Octiles with uniform cost 1 to adjacent and diagonal. */
        Tile
    }

    public class ConcreteMap : IMap<ConcreteNode>
    {
        public IPassability Passability { get; set; }
        
        public TileType TileType { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int MaxEdges { get; set; }

        public ConcreteGraph Graph { get; set; }

        public int NrNodes => Width * Height;

        public ConcreteMap(TileType tileType, int width, int height, IPassability passability)
        {
            Passability = passability;
            TileType = tileType;
            MaxEdges = Helpers.GetMaxEdges(tileType);
            Width = width;
            Height = height;
            Graph = GraphFactory.CreateGraph(width, height, Passability);
        }

        // Create a new concreteMap as a copy of another concreteMap (just copying obstacles)
        public ConcreteMap Slice(int horizOrigin, int vertOrigin, int width, int height, IPassability passability)
        {
            ConcreteMap slicedConcreteMap = new ConcreteMap(this.TileType, width, height, passability);

            foreach(ConcreteNode slicedMapNode in slicedConcreteMap.Graph.Nodes)
            {
                ConcreteNode globalConcreteNode = Graph.GetNode(GetNodeIdFromPos(horizOrigin + slicedMapNode.Info.Position.X,
                    vertOrigin + slicedMapNode.Info.Position.Y));
                slicedMapNode.Info.IsObstacle = globalConcreteNode.Info.IsObstacle;
                slicedMapNode.Info.Cost = globalConcreteNode.Info.Cost;
            }

            return slicedConcreteMap;
        }

        /// <summary>
        /// x, y좌표를 통해 ID를 반환
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Id<ConcreteNode> GetNodeIdFromPos(int x, int y)
        {
            return Id<ConcreteNode>.From(y * Width + x);
        }

        public IEnumerable<Connection<ConcreteNode>> GetConnections(Id<ConcreteNode> nodeId)
        {
            List<Connection<ConcreteNode>> result = new List<Connection<ConcreteNode>>();
            ConcreteNode node = Graph.GetNode(nodeId);
            ConcreteNodeInfo nodeInfo = node.Info;

            foreach(ConcreteEdge edge in node.Edges.Values)
            {
                Id<ConcreteNode> targetNodeId = edge.TargetNodeId;
                ConcreteNodeInfo targetNodeInfo = Graph.GetNodeInfo(targetNodeId);
                if(CanJump(targetNodeInfo.Position, nodeInfo.Position) && !targetNodeInfo.IsObstacle)
                {
                    result.Add(new Connection<ConcreteNode>(targetNodeId, edge.Info.Cost));
                }
            }

            return result;
        }

        /// <summary>
        /// Tells whether we can move from p1 to p2 in line. Bear in mind
        /// this function does not consider intermediate points (it is
        /// assumed you can jump between intermediate points)
        /// 현재 노드로부터 타겟 노드까지의 중간에 루트를 무시한 최단 거리를 구한다
        /// </summary>
        public bool CanJump(Position p1, Position p2)
        {
            if (TileType != TileType.Octile && this.TileType != TileType.OctileUnicost)
                return true;
            if (Helpers.AreAligned(p1, p2))
                return true;

            // The following piece of code existed in the original implementation.
            // It basically checks that you do not forcefully cross a blocked diagonal.
            // Honestly, this is weird, bad designed and supposes that each position is adjacent to each other.
            ConcreteNodeInfo nodeInfo12 = Graph.GetNode(GetNodeIdFromPos(p2.X, p1.Y)).Info;
            ConcreteNodeInfo nodeInfo21 = Graph.GetNode(GetNodeIdFromPos(p1.X, p2.Y)).Info;
            return !(nodeInfo12.IsObstacle && nodeInfo21.IsObstacle);
        }

        /// <summary>
        /// 시작노드와 타겟 노드 사이의 휴리스틱을 결정한다
        /// TileType에 따라 다른 방식이 적용된다 Hex, OctileUnicost, Octile, Tile이 있다
        /// </summary>
        /// <param name="startNodeId"></param>
        /// <param name="targetNodeId"></param>
        /// <returns></returns>
        public int GetHeuristic(Id<ConcreteNode> startNodeId, Id<ConcreteNode> targetNodeId)
        {
            Position startPosition = Graph.GetNodeInfo(startNodeId).Position;
            Position targetPosition = Graph.GetNodeInfo(targetNodeId).Position;

            int startX = startPosition.X;
            int targetX = targetPosition.X;
            int startY = startPosition.Y;
            int targetY = targetPosition.Y;
            int diffX = Math.Abs(targetX - startX);
            int diffY = Math.Abs(targetY - startY);
            switch (TileType)
            {
                case TileType.Hex:
                    // Vancouver distance
                    // See P.Yap: Grid-based Path-Finding (LNAI 2338 pp.44-55)
                    {
                        int correction = 0;
                        if(diffX % 2 != 0)
                        {
                            if (targetY < startY)
                                correction = targetX % 2;
                            else if (targetY > startY)
                                correction = startX % 2;
                        }

                        // Note: formula in paper is wrong, corrected below.
                        int dist = Math.Max(0, diffY - diffX / 2 - correction) + diffX;
                        return dist * 1;
                    }
                case TileType.OctileUnicost:
                    return Math.Max(diffX, diffY) * Constants.COST_ONE;
                case TileType.Octile:
                    int maxDiff;
                    int minDiff;
                    if(diffX > diffY)
                    {
                        maxDiff = diffX;
                        minDiff = diffY;
                    }
                    else
                    {
                        maxDiff = diffY;
                        minDiff = diffX;
                    }

                    return (minDiff * Constants.COST_ONE * 34) / 24 + (maxDiff - minDiff) * Constants.COST_ONE;
                case TileType.Tile:
                    return (diffX + diffY) * Constants.COST_ONE;
                default:
                    return 0;
            }
        }
    }
}
