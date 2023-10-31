using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class ImageViewsMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    Vk vk = context.Vk!;
    Image[] swapchainImages = context.SwapchainImages!;
    Format swapchainImageFormat = context.SwapchainImageFormat.GetValueOrDefault();
    Device device = context.Device.GetValueOrDefault();
    int swapchainImageCount = swapchainImages.Length;
    ImageView[] swapchainImageViews = new ImageView[swapchainImageCount];

    for (int i = 0; i < swapchainImageCount; i++)
    {
      ImageViewCreateInfo createInfo = new()
      {
        SType = StructureType.ImageViewCreateInfo,
        Image = swapchainImages[i],
        ViewType = ImageViewType.Type2D,
        Format = swapchainImageFormat,
        Components =
          {
            R = ComponentSwizzle.Identity,
            G = ComponentSwizzle.Identity,
            B = ComponentSwizzle.Identity,
            A = ComponentSwizzle.Identity,
          },
        SubresourceRange =
          {
            AspectMask = ImageAspectFlags.ColorBit,
            BaseMipLevel = 0,
            LevelCount = 1,
            BaseArrayLayer = 0,
            LayerCount = 1,
          }
      };

      if (vk.CreateImageView(device, createInfo, null, out swapchainImageViews[i]) != Result.Success)
      {
        throw new Exception("failed to create image views!");
      }
    }

    context.SwapchainImageViews = swapchainImageViews;
  }

  public void CleanUp(RenderingContext context)
  {
    if (context.SwapchainImageViews is null)
      return;

    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();

    foreach (ImageView imageView in context.SwapchainImageViews)
    {
      vk.DestroyImageView(device, imageView, null);
    }
  }
}
