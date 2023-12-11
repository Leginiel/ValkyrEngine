using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public struct Barrier
{
  public uint ResourceIndex;
  public ImageLayout Layout;
  public AccessFlags2 Access;
  public PipelineStageFlags2 Stages;
  public bool History;
}