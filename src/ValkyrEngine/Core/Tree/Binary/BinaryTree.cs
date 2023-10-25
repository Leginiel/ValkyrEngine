namespace ValkyrEngine.Core.Tree.Binary;

public class BinaryTree<T>
    where T : IEquatable<T>, IComparable<T>
{
  public BinaryNode<T>? Root { get; set; }

  public virtual void Clear()
  {
    Root = null;
  }

  public IEnumerable<T> PreorderTraversal()
  {
    Stack<BinaryNode<T>?> stack = new();

    stack.Push(Root);

    while (stack.TryPop(out BinaryNode<T>? current))
    {
      if (current is null)
        yield break;

      yield return current.Value;

      if (current.Right is not null)
        stack.Push(current.Right);
      if (current.Left is not null)
        stack.Push(current.Left);
    }
  }
  public IEnumerable<T> PostorderTraversal()
  {
    Stack<BinaryNode<T>?> stack = new();

    stack.Push(Root);

    while (stack.TryPop(out BinaryNode<T>? current))
    {
      if (current is null)
        yield break;

      yield return current.Value;

      if (current.Left is not null)
        stack.Push(current.Left);
      if (current.Right is not null)
        stack.Push(current.Right);
    }
  }
  public IEnumerable<T> InorderTraversal()
  {
    Stack<BinaryNode<T>> stack = new();
    BinaryNode<T>? current = Root;

    if (Root is null)
      yield break;

    stack.Push(Root);

    while (stack.Count > 0 || current is not null)
    {
      if (current is not null)
      {
        stack.Push(current);
        current = current.Left;
      }

      if (stack.TryPop(out BinaryNode<T>? stackedElement))
      {
        current = stackedElement.Right;
        yield return stackedElement.Value;
      }
    }
  }
}
