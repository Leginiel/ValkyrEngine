using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class RenderPassMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    Format swapchainImageFormat = context.SwapchainImageFormat.GetValueOrDefault();

    AttachmentDescription colorAttachment = new()
    {
      Format = swapchainImageFormat,
      Samples = SampleCountFlags.Count1Bit,
      LoadOp = AttachmentLoadOp.Clear,
      StoreOp = AttachmentStoreOp.Store,
      StencilLoadOp = AttachmentLoadOp.DontCare,
      InitialLayout = ImageLayout.Undefined,
      FinalLayout = ImageLayout.PresentSrcKhr,
    };
    AttachmentReference colorAttachmentRef = new()
    {
      Attachment = 0,
      Layout = ImageLayout.ColorAttachmentOptimal,
    };
    SubpassDescription subpass = new()
    {
      PipelineBindPoint = PipelineBindPoint.Graphics,
      ColorAttachmentCount = 1,
      PColorAttachments = &colorAttachmentRef,
    };
    SubpassDependency dependency = new()
    {
      SrcSubpass = Vk.SubpassExternal,
      DstSubpass = 0,
      SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
      SrcAccessMask = 0,
      DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
      DstAccessMask = AccessFlags.ColorAttachmentWriteBit
    };
    RenderPassCreateInfo renderPassInfo = new()
    {
      SType = StructureType.RenderPassCreateInfo,
      AttachmentCount = 1,
      PAttachments = &colorAttachment,
      SubpassCount = 1,
      PSubpasses = &subpass,
      DependencyCount = 1,
      PDependencies = &dependency,
    };

    if (vk.CreateRenderPass(device, renderPassInfo, null, out RenderPass renderPass) != Result.Success)
    {
      throw new Exception("failed to create render pass!");
    }
    context.RenderPass = renderPass;
  }

  public void CleanUp(RenderingContext context)
  {
    context.Vk!.DestroyRenderPass(context.Device.GetValueOrDefault(), context.RenderPass.GetValueOrDefault(), null);
  }
}