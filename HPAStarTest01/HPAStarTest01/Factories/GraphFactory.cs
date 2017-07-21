using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01.Factories
{
    public class GraphFactory
    {
        public static ConcreteGraph CreateGraph(int width, int height, IPassability passability)
        {
            ConcreteGraph graph = new ConcreteGraph();

            CreateNodes(width, height, graph, passability);
            CreateEdges(graph, width, height, TileType.Octile); // We hardcode OCTILE for the time being

            return graph;
        } 

        /// <summary>
        /// 위치로 콘크리트 노드 반환
        /// </summary>
        public static ConcreteNode GetNodeByPos(ConcreteGraph graph, int x, int y, int width)
        {
            return graph.GetNode(GetNodeIdFromPos(x, y, width));
        }

        /// <summary>
        /// 위치로 노드 아이디 반환
        /// </summary>
        public static Id<ConcreteNode> GetNodeIdFromPos(int left, int top, int width)
        {
            return Id<ConcreteNode>.From(top * width + left);
        }

        private static void AddEdge(ConcreteGraph graph, Id<ConcreteNode> nodeId, int x, int y, int width, int height, bool isDiog = false)
        {
            if (y < 0 || y >= height || x < 0 || x >= width)
                return;

            ConcreteNode targetNode = GetNodeByPos(graph, x, y, width);
            int cost = targetNode.Info.Cost;
            cost = isDiog ? (cost * 34) / 24 : cost;
            graph.AddEdge(nodeId, targetNode.NodeId, new ConcreteEdgeInfo(cost));
        }

        private static void CreateEdges(ConcreteGraph graph, int width, int height, TileType tileType)
        {
            for (int top = 0; top < height; ++top)
            {
                for (int left = 0; left < width; ++left)
                {
                    Id<ConcreteNode> nodeId = GetNodeByPos(graph, left, top, width).NodeId;

                    AddEdge(graph, nodeId, left, top - 1, width, height);
                    AddEdge(graph, nodeId, left, top + 1, width, height);
                    AddEdge(graph, nodeId, left - 1, top, width, height);
                    AddEdge(graph, nodeId, left + 1, top, width, height);

                    if(tileType == TileType.Octile)
                    {
                        AddEdge(graph, nodeId, left + 1, top + 1, width, height, true);
                        AddEdge(graph, nodeId, left - 1, top + 1, width, height, true);
                        AddEdge(graph, nodeId, left + 1, top - 1, width, height, true);
                        AddEdge(graph, nodeId, left - 1, top - 1, width, height, true);
                    }else if(tileType == TileType.OctileUnicost)
                    {
                        AddEdge(graph, nodeId, left + 1, top + 1, width, height);
                        AddEdge(graph, nodeId, left - 1, top + 1, width, height);
                        AddEdge(graph, nodeId, left + 1, top - 1, width, height);
                        AddEdge(graph, nodeId, left - 1, top - 1, width, height);
                    }
                    else if(tileType == TileType.Hex)
                    {
                        if(left % 2 == 0)
                        {
                            AddEdge(graph, nodeId, left + 1, top - 1, width, height);
                            AddEdge(graph, nodeId, left - 1, top - 1, width, height);
                        }
                        else
                        {
                            AddEdge(graph, nodeId, left + 1, top + 1, width, height);
                            AddEdge(graph, nodeId, left - 1, top + 1, width, height);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 노드 생성
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="graph"></param>
        /// <param name="passability"></param>
        private static void CreateNodes(int width, int height, ConcreteGraph graph, IPassability passability)
        {
            for (int top = 0; top < height; ++top)
            {
                for (int left = 0; left < width; left++)
                {
                    Id<ConcreteNode> nodeId = GetNodeIdFromPos(left, top, width);
                    Position position = new Position(left, top);
                    int movementCost;
                    bool isObstacle = !passability.CanEnter(position, out movementCost);
                    ConcreteNodeInfo info = new ConcreteNodeInfo(isObstacle, movementCost, position);

                    graph.AddNode(nodeId, info);
                }
            }
        }
    }
}
