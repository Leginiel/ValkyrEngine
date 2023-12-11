using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace ValkyrEngine.Rendering;

public class PassSubmissionState
{
  public List<BufferMemoryBarrier2> BufferBarriers { get; } = [];
  public List<ImageMemoryBarrier2> ImageBarriers { get; } = [];
  public List<SubpassContents> SubpassContents { get; } = [];
  public List<Semaphore> WaitSemaphores { get; } = [];
  public List<PipelineStageFlags2> WaitSemaphoreStages { get; } = [];
  public List<AccessedExternalLockInterface> ExternalLocks { get; } = [];
  public Semaphore[] ProxySemaphore = new Semaphore[2];
  public bool NeedSubmissionSemaphore { get; set; } = false;
  public CommandBuffer CommandBuffer { get; set; }
  public CommandBufferType QueueType { get; set; } = CommandBufferType.Count;
  public bool Graphics { get; set; } = false;
  public bool Active { get; set; } = false;
  public TaskGroup RenderingDependency { get; set; }
  public void EmitPrePassBarriers()
  {

  }

  public void Submit()
  {

  }
}