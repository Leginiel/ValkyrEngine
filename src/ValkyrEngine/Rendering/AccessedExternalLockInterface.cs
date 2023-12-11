using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct AccessedExternalLockInterface()
{
  public RenderPassExternalLockInterface Iface;
  public PipelineStageFlags2 Stages;
}