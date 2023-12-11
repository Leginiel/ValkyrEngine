using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace ValkyrEngine.Rendering;

public struct PipelineEvent()
{
  public PipelineStageFlags2 PipelineBarrierSourceStages = 0;
  public Semaphore WaitGraphicSemaphore;
  public Semaphore WaitComputeSemaphore;

  public AccessFlags2 ToFlushAccess = 0;
  public AccessFlags2[] InvalidatedInStage = new AccessFlags2[64];
  public ImageLayout Layout = ImageLayout.Undefined;
}