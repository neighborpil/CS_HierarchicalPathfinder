using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;

namespace HPAStarTest01.Graph
{
    public interface INode<TId, TInfo, TEdge>
    {
        Id<TId> NodeId { get; set; }
        TInfo Info { get; set; }
        IDictionary<Id<TId>, TEdge> Edges { get; set; }
        void RemoveEdge(Id<TId> targetNodeId);
        void AddEdge(TEdge targetNodeId);
    }

    public interface IEdge<TNode, TEdgeInfo>
    {
        Id<TNode> TargetNodeId { get; set; }
        TEdgeInfo Info { get; set; }
    }

    /// <summary>
	/// A graph is a set of nodes connected with edges. Each node and edge can hold
	/// a certain amount of information, which is expressed in the templated parameters
	/// NODEINFO and EDGEINFO
    /// 그래프는 엣지로 연결된 노드들이다. 각각의 노드와 엣지는 
    /// 노드인포와 엣지인포라는 템플릿 파라미터들로 표현된 충분한 양의 정보를 가지고 있다
	/// </summary>
    public class Graph<TNode, TNodeInfo, TEdge, TEdgeInfo>
        where TNode : INode<TNode, TNodeInfo, TEdge>
        where TEdge : IEdge<TNode, TEdgeInfo>
    {
        // We store the nodes in a list because the main operations we use
        // in this list are additions, random accesses and very few removals (only when
        // adding or removing nodes to perform specific searches).
        // This list is implicitly indexed by the nodeId, which makes removing a random
        // Node in the list quite of a mess. We could use a dictionary to ease removals,
        // but lists and arrays are faster for random accesses, and we need performance.
        // 우리는 노드를 List에 넣어 관리한다. 왜냐하면 주된 작업이 리스트에 추가하고, 랜덤
        // 액세스, 그리고 아주 적은 삭제 이기 때문이다(특별한 검색을 위하여 노드를 더하거나 뺄때만)
        // 이 노드들은 nodeId에 의해서 무조건 인덱싱이 되어 리스트에서 삭제하는데 시간이 걸린다
        // 우리는 삭제시의 문제점을 줄이기 위하여 딕셔너리의 사용이 가능하지만, 리스트와 배열이 
        // 랜덤 액세스에서 훨신 빠르기 때문에 리스트를 사용한다.
        public List<TNode> Nodes { get; set; }

        private readonly Func<Id<TNode>, TNodeInfo, TNode> _nodeCreator;
        private readonly Func<Id<TNode>, TEdgeInfo, TEdge> _edgeCreator;

        public Graph(Func<Id<TNode>, TNodeInfo, TNode> nodeCreator, Func<Id<TNode>, TEdgeInfo, TEdge> edgeCreator)
        {
            Nodes = new List<TNode>();
            _nodeCreator = nodeCreator;
            _edgeCreator = edgeCreator;
        }

        /// <summary>
		/// Adds or updates a node with the provided info. A node is updated
		/// only if the nodeId provided previously existed.
        /// 제공된 info를 더하거나 업데이트한다
        /// 노드는 nodeId가 이전에 존재 할 때만 업데이트 된다
		/// </summary>
        public void AddNode(Id<TNode> nodeId, TNodeInfo info)
        {
            int size = nodeId.IdValue + 1;
            if (Nodes.Count < size)
                Nodes.Add(_nodeCreator(nodeId, info));
            else
                Nodes[nodeId.IdValue] = _nodeCreator(nodeId, info);
        }

        #region AbstractGraph updating

        /// <summary>
        /// 엣지 추가
        /// </summary>
        /// <param name="sourceNodeId"></param>
        /// <param name="targetNodeId"></param>
        /// <param name="info"></param>
        public void AddEdge(Id<TNode> sourceNodeId, Id<TNode> targetNodeId, TEdgeInfo info)
        {
            Nodes[sourceNodeId.IdValue].AddEdge(_edgeCreator(targetNodeId, info));
        }

        /// <summary>
        /// nodeId를 받아서 List<TNode> Nodes에서 Edges의 키가 같을 경우 삭제
        /// </summary>
        /// <param name="nodeId"></param>
        public void RemoveEdgesFromAndToNode(Id<TNode> nodeId)
        {
            foreach(Id<TNode> targetNodeId in Nodes[nodeId.IdValue].Edges.Keys)
            {
                Nodes[targetNodeId.IdValue].RemoveEdge(nodeId); 
            }

            Nodes[nodeId.IdValue].Edges.Clear();
        }

        /// <summary>
        /// 마지막 노드 삭제
        /// </summary>
        public void RemoveLastNode()
        {
            Nodes.RemoveAt(Nodes.Count - 1);
        }

        #endregion

        /// <summary>
        /// 노드 아이디로 노드 반환
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public TNode GetNode(Id<TNode> nodeId)
        {
            return Nodes[nodeId.IdValue];
        }

        /// <summary>
        /// 노드 아이디로 노드 정보 반환
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public TNodeInfo GetNodeInfo(Id<TNode> nodeId)
        {
            return GetNode(nodeId).Info;
        }

        /// <summary>
        /// 노드 아이디로 그 노드와 엣지를 반환
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public IDictionary<Id<TNode>, TEdge> GetEdges(Id<TNode> nodeId)
        {
            return Nodes[nodeId.IdValue].Edges;
        }
    }

    /// <summary>
    /// 콘크리트 그래프
    /// </summary>
    public class ConcreteGraph : Graph<ConcreteNode, ConcreteNodeInfo, ConcreteEdge, ConcreteEdgeInfo>
    {
        public ConcreteGraph() : base((nodeid, info) => new ConcreteNode(nodeid, info), (nodeid, info) => new ConcreteEdge(nodeid, info))
        {
        }
    }

    public class AbstractGraph : Graph<AbstractNode, AbstractNodeInfo, AbstractEdge, AbstractEdgeInfo>
    {
        public AbstractGraph() : base((nodeid, info) => new AbstractNode(nodeid, info), (nodeid, info) => new AbstractEdge(nodeid, info))
        {
        }
    }
}
