using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class FramebufferMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    ImageView[] swapchainImageViews = context.SwapchainImageViews!;
    RenderPass renderPass = context.RenderPass.GetValueOrDefault();
    Extent2D swapchainExtent = context.SwapchainExtent.GetValueOrDefault();
    Framebuffer[] swapchainFramebuffer = new Framebuffer[swapchainImageViews.Length];

    for (int i = 0; i < swapchainImageViews.Length; i++)
    {
      ImageView attachment = swapchainImageViews[i];

      FramebufferCreateInfo framebufferInfo = new()
      {
        SType = StructureType.FramebufferCreateInfo,
        RenderPass = renderPass,
        AttachmentCount = 1,
        PAttachments = &attachment,
        Width = swapchainExtent.Width,
        Height = swapchainExtent.Height,
        Layers = 1,
      };

      if (vk.CreateFramebuffer(device, framebufferInfo, null, out swapchainFramebuffer[i]) != Result.Success)
      {
        throw new Exception("failed to create framebuffer!");
      }
    }

    context.SwapchainFramebuffer = swapchainFramebuffer;
  }

  public void CleanUp(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();

    foreach (Framebuffer framebuffer in context.SwapchainFramebuffer!)
    {
      vk.DestroyFramebuffer(device, framebuffer, null);
    }
    context.SwapchainFramebuffer = null;
  }
}
