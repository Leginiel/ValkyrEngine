using Collections.Pooled;

namespace ValkyrEngine.Core.Tree;

public class NodeList<T> : PooledList<Node<T>>
  where T : IEquatable<T>, IComparable<T>
{
  private const int BaseSize = 2;

  public NodeList()
      : base(BaseSize, ClearMode.Never) { }
  public NodeList(params Node<T>[] nodes)
      : base(nodes, ClearMode.Never) { }

  public Node<T>? FindByValue(T? value)
  {
    for (int i = 0; i < Span.Length; i++)
    {
      Node<T> node = Span[i];
      T? nodeValue = node.Value;

      if ((nodeValue is null && value is null)
          || (nodeValue is not null && nodeValue.Equals(value)))
        return node;
    }

    return null;
  }
}