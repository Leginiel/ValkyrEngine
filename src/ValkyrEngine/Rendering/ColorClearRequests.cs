using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct ColorClearRequests
{
  public RenderPass Pass;
  public ClearColorValue Target;
  public int Index;
}