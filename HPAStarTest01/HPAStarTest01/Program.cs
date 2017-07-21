using HPAStarTest01.Factories;
using HPAStarTest01.Passabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01
{
    public partial class Program
    {
        //private static readonly int Height = 16;
        //private static readonly int Width = 16;

        //private static readonly Position StartPosition = new Position(1, 0);
        //private static readonly Position EndPosition = new Position(15, 15);

        static void Main(string[] args)
        {
            const int clusterSize = 8;
            const int maxLevel = 2;
            const int height = 128; //맵의 세로 크기
            const int width = 128; //맵의 가로 크기

            Position startPosition = new Position(1, 0);
            Position endPosition = new Position(69, 69);

            // Prepare the abstract graph beforehand
            IPassability passability = new FakePassability(width, height);

            ConcreteMap concreteMap = ConcreteMapFactory.CreateConcreteMap(width, height, passability);
            HierarchicalMapFactory abstractMapFactory = new HierarchicalMapFactory();
            HierarchicalMap absTilting = abstractMapFactory.CreateHierarchicalMap(concreteMap, clusterSize, maxLevel, EntranceStyle.EndEntrance);
            //var edges = absTiling.AbstractGraph.Nodes.SelectMany(x => x.Edges.Values)
            //    .GroupBy(x => x.Info.Level)
            //    .ToDictionary(x => x.Key, x => x.Count());

            Func<Position, Position, List<IPathNode>> doHierarchicalSearch = (startPosition, endPosition)
                => HierarchicalSearch(absTilting, maxLevel, concreteMap, startPosition, endPosition);


        }

        private static List<IPathNode> HierarchicalSearch(HierarchicalMap hierarchicalMap, int maxLeve, ConcreteMap concreteMap, )
    }
}
