namespace ValkyrEngine.Core.Tree;

public class Node<T> : IComparable<Node<T>>
  where T : IEquatable<T>, IComparable<T>
{
  public T Value { get; set; }
  public NodeList<T> Edges { get; }

  public Node(T value)
      : this(value, new NodeList<T>()) { }
  public Node(T value, NodeList<T> edges)
  {
    Value = value;
    Edges = edges;
  }
  public int CompareTo(Node<T>? other)
  {
    T? otherValue = default;

    if (other is not null)
      otherValue = other.Value;

    return Value.CompareTo(otherValue);
  }
}