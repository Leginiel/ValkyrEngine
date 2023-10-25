namespace ValkyrEngine.Core.Tree.Binary;

public class BinaryNode<T> : Node<T>
  where T : IEquatable<T>, IComparable<T>
{
  public BinaryNode(T value, BinaryNode<T> left, BinaryNode<T> right)
   : base(value, new NodeList<T>(left, right)) { }
  public BinaryNode(T value, BinaryNode<T> left)
  : base(value, new NodeList<T>(left)) { }
  public BinaryNode(T value)
  : base(value, new NodeList<T>()) { }

  public BinaryNode<T>? Left
  {
    get
    {
      return (BinaryNode<T>)Edges[0];
    }
    set
    {
      if (value is not null)
        Edges[0] = value;
    }
  }

  public BinaryNode<T>? Right
  {
    get
    {
      return (BinaryNode<T>)Edges[1];
    }
    set
    {
      if (value is not null)
        Edges[1] = value;
    }
  }
}