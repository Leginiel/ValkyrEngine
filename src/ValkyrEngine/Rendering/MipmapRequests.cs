using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct MipmapRequests
{
  public int PhysicalResource;
  public PipelineStageFlags2 Stages;
  public AccessFlags2 Access;
  public ImageLayout Layout;
}