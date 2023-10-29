using ValkyrEngine.Core.Tree;

namespace ValkyrEngine.Core.Graph;

public class GraphNode<T> : Node<T>
  where T : IEquatable<T>, IComparable<T>
{
  private List<int>? _costs;

  public GraphNode(T value)
    : base(value) { }

  public GraphNode(T value, NodeList<T> edges)
    : base(value, edges) { }

  public List<int> Costs => _costs ??= new List<int>();
}