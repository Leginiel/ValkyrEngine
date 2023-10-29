using ValkyrEngine.Core.Tree;

namespace ValkyrEngine.Core.Graph;

public class Graph<T>
  where T : IEquatable<T>, IComparable<T>
{
  public NodeList<T> Nodes { get; }
  public int Count => Nodes.Count;

  public Graph(NodeList<T>? nodes = null)
  {
    Nodes = nodes ?? new();
  }

  public void AddNode(GraphNode<T> node)
  {
    Nodes.Add(node);
  }

  public void AddNode(T value)
  {
    Nodes.Add(new GraphNode<T>(value));
  }
  public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
  {
    from.Edges.Add(to);
    from.Costs.Add(cost);
  }

  public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
  {
    from.Edges.Add(to);
    from.Costs.Add(cost);

    to.Edges.Add(from);
    to.Costs.Add(cost);
  }
  public bool Contains(T value)
  {
    return Nodes.FindByValue(value) != null;
  }

  public bool Remove(T value)
  {
    GraphNode<T>? nodeToRemove = (GraphNode<T>?)Nodes.FindByValue(value);
    if (nodeToRemove is null)
      return false;

    Nodes.Remove(nodeToRemove);

    foreach (GraphNode<T> node in Nodes.Cast<GraphNode<T>>())
    {
      int index = node.Edges.IndexOf(nodeToRemove);
      if (index != -1)
      {
        node.Edges.RemoveAt(index);
        node.Costs.RemoveAt(index);
      }
    }

    return true;
  }
}