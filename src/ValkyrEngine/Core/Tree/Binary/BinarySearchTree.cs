using System.Buffers;

namespace ValkyrEngine.Core.Tree.Binary;

public class BinarySearchTree<T> : BinaryTree<T>
    where T : IEquatable<T>, IComparable<T>
{
  public int Count { get; private set; }


  public bool Contains(T data)
  {
    BinaryNode<T>? current = Root;

    while (current is not null)
    {
      switch (current.Value.CompareTo(data))
      {
        case 0:
          return true;
        case > 0:
          current = current.Left;
          break;
        case < 0:
          current = current.Right;
          break;
      }
    }
    return false;
  }
  public virtual void Add(T data)
  {
    BinaryNode<T> newNode = new(data);
    int result;

    BinaryNode<T>? current = Root;
    BinaryNode<T>? parent = null;

    while (current is not null)
    {
      result = current.Value.CompareTo(data);
      switch (result)
      {
        case 0:
          return;
        case > 0:
          parent = current;
          current = current.Left;
          break;
        case < 0:
          parent = current;
          current = current.Right;
          break;
      }
    }

    Count++;

    if (parent is null)
    {
      Root = newNode;
    }
    else
    {
      result = parent.Value.CompareTo(data);
      if (result > 0)
        parent.Left = newNode;
      else
        parent.Right = newNode;
    }
  }

  public bool Remove(T data)
  {
    BinaryNode<T>? current = Root;
    BinaryNode<T>? parent = null;

    if (current is null)
      return false;

    int result = current.Value.CompareTo(data);

    while (result != 0)
    {
      parent = current;
      switch (result)
      {
        case > 0:
          current = current.Left;
          break;
        case < 0:
          current = current.Right;
          break;
      }

      if (current is null)
        return false;

      result = current.Value.CompareTo(data);
    }

    Count--;

    if (current.Right is null)
    {
      PromoteChild(current.Value, current.Left, parent);
    }
    else if (current.Right.Left is null)
    {
      current.Right.Left = current.Left;
      PromoteChild(current.Value, current.Right, parent);
    }
    else
    {
      BinaryNode<T> leftmost = current.Right.Left;
      BinaryNode<T> lmParent = current.Right;

      while (leftmost.Left is not null)
      {
        lmParent = leftmost;
        leftmost = leftmost.Left;
      }
      lmParent.Left = leftmost.Right;
      leftmost.Left = current.Left;
      leftmost.Right = current.Right;

      PromoteChild(current.Value, leftmost, parent);
    }

    return true;
  }

  private void PromoteChild(T currentValue, BinaryNode<T>? childToPromote, BinaryNode<T>? parent)
  {
    if (parent is null)
    {
      Root = childToPromote;
      return;
    }
    switch (parent.Value.CompareTo(currentValue))
    {
      case > 0:
        parent.Left = childToPromote;
        break;
      case < 0:
        parent.Right = childToPromote;
        break;
    }
  }
}