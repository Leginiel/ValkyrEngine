
namespace ValkyrEngine.Rendering.Vulkan;

public unsafe class Image(VkImage image, ImageCreateInfo info, VkImageView defaultView, ImageViewType viewType) : NativeVulkanObject<VkImage, ImageCreateInfo>(image, info)
{
  private const ImageUsageFlags SafeUsageFlags = ImageUsageFlags.TransientAttachmentBit
                                                  | ImageUsageFlags.ColorAttachmentBit
                                                  | ImageUsageFlags.DepthStencilAttachmentBit
                                                  | ImageUsageFlags.InputAttachmentBit;
  private SurfaceTransformFlagsKHR _surfaceTransform;

  public Format Format => Info.Format;
  public LayoutType LayoutType { get; set; }
  public VkImageView View { get; } = defaultView;
  public ImageViewType ViewType { get; } = viewType;
  public ImageLayout SwapchainLayout { get; } = ImageLayout.Undefined;
  public bool IsSwapchainImage => SwapchainLayout != ImageLayout.Undefined;
  public SurfaceTransformFlagsKHR SurfaceTransform
  {
    get => _surfaceTransform;
    set
    {
      _surfaceTransform = value;
      if (value != SurfaceTransformFlagsKHR.IdentityBitKhr && !Info.Usage.HasFlag(SafeUsageFlags))
      {
        //LogWarning "Using surface transform for non-pure render target image (usage: %u). This can lead to weird results.\n
      }
    }
  }

  public uint GetWidth(int lod = 0)
  {
    return Math.Max(1, Info.Extent.Width >> lod);
  }
  public uint GetHeight(int lod = 0)
  {
    return Math.Max(1, Info.Extent.Height >> lod);
  }
  public uint GetDepth(int lod = 0)
  {
    return Math.Max(1, Info.Extent.Depth >> lod);
  }
  public ImageLayout GetLayout(ImageLayout optimal)
  {
    return LayoutType == LayoutType.Optimal ? optimal : ImageLayout.General;
  }
  public override void CleanUp(ref Vk vk, ref Device device)
  {
    vk.DestroyImage(device, Native, null);
  }
}