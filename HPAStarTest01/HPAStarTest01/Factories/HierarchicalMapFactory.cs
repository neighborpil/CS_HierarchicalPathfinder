using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;

namespace HPAStarTest01.Factories
{
    public class HierarchicalMapFactory
    {
        private const int MAX_ENTERANCE_WIDTH = 6;

        private HierarchicalMap _hierarchicalMap;
        private ConcreteMap _concreteMap;
        private EntranceStyle _entranceStyle;
        private int _clusterSize;
        private int _maxLevel;

        readonly Dictionary<Id<AbstractNode>, NodeBackup> nodeBackups = new Dictionary<Id<AbstractNode>, NodeBackup>();

        public HierarchicalMap CreateHierarchicalMap(ConcreteMap concreteMap, int clusterSize, int maxLevel, EntranceStyle style)
        {
            _clusterSize = clusterSize;
            _entranceStyle = style;
            _maxLevel = maxLevel;
            _concreteMap = concreteMap;
            _hierarchicalMap = new HierarchicalMap(concreteMap, clusterSize, maxLevel);

            List<Entrance> entrances;
            List<Cluster> clusters;
            CreateEntrancesAndClusters(out entrances, out clusters);
            _hierarchicalMap.Clusters = clusters;
        }

        private class NodeBackup
        {
            public int Level { get; private set; }
            public List<AbstractEdge> Edges { get; private set; }

            public NodeBackup(int level, List<AbstractEdge> edges)
            {
                Level = level;
                Edges = edges;
            }
        }
    }
}
