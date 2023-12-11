namespace ValkyrEngine.Rendering.Vulkan;

internal unsafe class ImageView(VkImageView imageView, ImageViewCreateInfo info, Image image) : NativeVulkanObject<VkImageView, ImageViewCreateInfo>(imageView, info)
{
  public VkImageView? DepthView { get; set; }
  public VkImageView? StencilView { get; set; }
  public VkImageView? UnormView { get; set; }
  public VkImageView? SrgbView { get; set; }
  public VkImageView[]? RenderTargetViews { private get; set; }
  public Format Format => Info.Format;
  public Image Image => image;

  public int ViewWidth { get; }
  public int ViewHeight { get; }
  public int ViewDepth { get; }

  public VkImageView FloatView => DepthView is not null ? DepthView.Value : Native;
  public VkImageView IntegerView => StencilView is not null ? StencilView.Value : Native;

  public VkImageView GetRenderTargetView(int layer)
  {
    if (RenderTargetViews?.Length >= layer)
      return RenderTargetViews[layer];

    return Native;
  }

  public override void CleanUp(ref Vk vk, ref Device device)
  {
    vk.DestroyImageView(device, Native, null);

    if (DepthView is not null)
      vk.DestroyImageView(device, DepthView.Value, null);
    if (StencilView is not null)
      vk.DestroyImageView(device, StencilView.Value, null);
    if (UnormView is not null)
      vk.DestroyImageView(device, UnormView.Value, null);
    if (SrgbView is not null)
      vk.DestroyImageView(device, SrgbView.Value, null);

    if (RenderTargetViews is not null)
    {
      foreach (VkImageView view in RenderTargetViews)
        vk.DestroyImageView(device, view, null);
    }
  }
}