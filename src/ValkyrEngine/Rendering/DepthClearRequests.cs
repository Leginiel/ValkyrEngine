using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct DepthClearRequests
{
  public RenderPass Pass;
  public ClearDepthStencilValue Target;
}