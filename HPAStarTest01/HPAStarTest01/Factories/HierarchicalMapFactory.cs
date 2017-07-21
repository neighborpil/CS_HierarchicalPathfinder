using HPAStarTest01;
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

            CreateAbstractNodes(entrances);
            CreateEdges(entrances, clusters);

            return _hierarchicalMap;
        }

        #region Graph manipulation

        public void RemoveAbstraceNode(HierarchicalMap map, Id<AbstractNode> nodeId)
        {
            if (nodeBackups.ContainsKey(nodeId))
                RestoreNodeBackup(map, nodeId);
            else
                map.RemoveAbstractNode(nodeId);
        }

        public Id<AbstractNode> InsertAbstractNode(HierarchicalMap map, Position pos)
        {
            Id<ConcreteNode> nodeId = Id<ConcreteNode>.From(pos.Y * map.Width + pos.X);
            Id<AbstractNode> abstractNodeId = InsertNodeIntoHierarchicalMap(map, nodeId, pos);
            map.AddHierarchicalEdgesForAbstractNode(abstractNodeId);
            return abstractNodeId;
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

        // insert a new node, such as start or target, to the abstract graph and
        // returns the id of the newly created node in the abstract graph
        // x and y are the positions where I want to put the node
        // abstract그래프에 시작 또는 타겟노드와 같은 새로운 노드를 넣는다
        // 그러며 abstract 그래프 안에서 새로 생성된 노드의 아이디를 반환한다
        // x와 y는 내가 노드를 놓기를 원하는 위치이다
        private Id<AbstractNode> InsertNodeIntoHierarchicalMap(HierarchicalMap map, Id<ConcreteNode> concreteNodeId, Position pos)
        {
            // If the node already existed (for instance, it was the an entrance point already
            // existing in the graph, we need to keep track of the previous status in order
            // to be able to restore it once we delete this STAL
            // 만약 노드가 이미 존재한다면(예를들면 그래프에서 이미 존재하는 시작 지점이라면, 
            // 우리가 한번 지워버린 이 STAL을 복구하기 위하여 이전의 status를 따라갈 필요가 있다
            if (map.ConcreteNodeIdToAbstractNodeIdMap.ContainsKey(concreteNodeId))
            {
                Id<AbstractNode> existingAbstractNodeId = map.ConcreteNodeIdToAbstractNodeIdMap[concreteNodeId];
                NodeBackup nodeBackup = new NodeBackup(map.AbstractGraph.GetNodeInfo(existingAbstractNodeId).Level,
                    map.GetNodeEdges(concreteNodeId));
                nodeBackups[existingAbstractNodeId] = nodeBackup;

                return map.ConcreteNodeIdToAbstractNodeIdMap[concreteNodeId];
            }

            Cluster cluster = map.FindClusterForPosition(pos);

            // create global entrance
            Id<AbstractNode> abstractNodeId = Id<AbstractNode>.From(map.NrNodes);

            EntrancePoint entrance = cluster.AddEntrance(abstractNodeId, new Position(pos.X - cluster.Origin.X, pos.Y - cluster.Origin.Y));
            cluster.UpdatePathsForLocalEntrance(entrance);

            map.ConcreteNodeIdToAbstractNodeIdMap[concreteNodeId] = abstractNodeId;


            AbstractNodeInfo info = new AbstractNodeInfo(
                abstractNodeId,
                1,
                cluster.Id,
                pos,
                concreteNodeId
                );

            map.AbstractGraph.AddNode(abstractNodeId, info);

            foreach (var entrancePoint in cluster.EntrancePoints)
            {
                if(cluster.AreConnected(abstractNodeId, entrancePoint.AbstractNodeId))
                {
                    map.AddEdge(
                        entrancePoint.AbstractNodeId,
                        abstractNodeId,
                        cluster.GetDistance(entrancePoint.AbstractNodeId, abstractNodeId));
                    map.AddEdge(
                        abstractNodeId,
                        entrancePoint.AbstractNodeId,
                        cluster.GetDistance(abstractNodeId, entrancePoint.AbstractNodeId));

                }
            }
            return abstractNodeId;
        }

        #endregion

        //여기부터 HierarchicalSearch만들때까지
    }
}
