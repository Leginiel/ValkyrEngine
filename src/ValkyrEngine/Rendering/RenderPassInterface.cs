using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering;

public class RenderPassInterface
{
  public virtual bool RenderPassIsConditional { get; } = false;
  public virtual bool RenderPassIsSeperateLayered { get; } = false;

  public virtual bool NeedRenderPass { get; } = true;
  public virtual bool TryGetClearDepthStecil(out ClearDepthStencilValue? value)
  {
    value = new ClearDepthStencilValue(1, 0u);
    return true;
  }
  public virtual bool TryGetClearColor(uint attachment, out ClearColorValue? value)
  {
    value = new();
    return true;
  }
  public virtual void SetupDependencies(RenderPass self, RenderGraph graph) { }
  public virtual void Setup(ref Device device) { }
  public virtual void EnqueuePrepareRenderPass(RenderGraph graph, TaskComposer composer) { }
  public virtual void BuildRenderPass(ref CommandBuffer commandBuffer) { }
  public virtual void BuildRenderPassSeparateLayer(ref CommandBuffer commandBuffer, uint layer) { }
}