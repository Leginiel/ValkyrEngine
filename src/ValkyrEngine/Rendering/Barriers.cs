using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct Barriers
{
  public List<Barrier> Invalidate;
  public List<Barrier> Flush;
}