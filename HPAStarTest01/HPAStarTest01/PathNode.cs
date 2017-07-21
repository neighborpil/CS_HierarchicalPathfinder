using HPAStarTest01.Graph;
using HPAStarTest01.Infrastructure;

namespace HPAStarTest01
{
    public interface IPathNode
    {
        int IdValue { get; }
    }

    public struct AbstractPathNode : IPathNode
    {
        public Id<AbstractNode> Id;
        public int Level;

        public AbstractPathNode(Id<AbstractNode> id, int lvl)
        {
            Id = id;
            Level = lvl;
        }

        public int IdValue
        {
            get { return Id.IdValue; }
        }
    }

    public struct ConcretePathNode : IPathNode
    {
        public Id<ConcreteNode> Id;

        public ConcretePathNode(Id<ConcreteNode> id)
        {
            Id = id;
        }

        public int IdValue
        {
            get { return Id.IdValue; }
        }
    }
}
