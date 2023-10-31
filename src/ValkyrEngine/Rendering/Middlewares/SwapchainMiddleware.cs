using System.Net.Mime;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class SwapchainMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    PhysicalDevice physicalDevice = context.PhysicalDevice.GetValueOrDefault();
    KhrSurface khrSurface = context.KhrSurface!;
    SurfaceKHR surface = context.Surface.GetValueOrDefault();
    Instance instance = context.Instance.GetValueOrDefault();
    Device device = context.Device.GetValueOrDefault();
    IWindow window = context.Window!;
    Vk vk = context.Vk!;
    SwapchainSupportDetails swapChainSupport = QuerySwapChainSupport(khrSurface, surface, physicalDevice);
    SurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
    PresentModeKHR presentMode = ChoosePresentMode(swapChainSupport.PresentModes);
    Extent2D extent = ChooseSwapExtent(swapChainSupport.Capabilities, window);
    SurfaceCapabilitiesKHR capabilities = swapChainSupport.Capabilities;
    uint imageCount = Math.Max(capabilities.MinImageCount + 1, capabilities.MaxImageCount);
    SwapchainCreateInfoKHR createInfo = new()
    {
      SType = StructureType.SwapchainCreateInfoKhr,
      Surface = surface,
      MinImageCount = imageCount,
      ImageFormat = surfaceFormat.Format,
      ImageColorSpace = surfaceFormat.ColorSpace,
      ImageExtent = extent,
      ImageArrayLayers = 1,
      ImageUsage = ImageUsageFlags.ColorAttachmentBit,
      ImageSharingMode = SharingMode.Exclusive,
      PreTransform = swapChainSupport.Capabilities.CurrentTransform,
      CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
      PresentMode = presentMode,
      Clipped = true,
      OldSwapchain = default
    };
    QueueFamilyIndices indices = context.FindQueueFamilies();
    uint* queueFamilyIndices = stackalloc[] { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };

    if (indices.GraphicsFamily != indices.PresentFamily)
    {
      createInfo = createInfo with
      {
        ImageSharingMode = SharingMode.Concurrent,
        QueueFamilyIndexCount = 2,
        PQueueFamilyIndices = queueFamilyIndices,
      };
    }

    if (!vk.TryGetDeviceExtension(instance, device, out KhrSwapchain khrSwapchain))
    {
      throw new NotSupportedException("VK_KHR_swapchain extension not found.");
    }
    if (khrSwapchain!.CreateSwapchain(device, createInfo, null, out SwapchainKHR swapchain) != Result.Success)
    {
      throw new Exception("failed to create swap chain!");
    }

    khrSwapchain.GetSwapchainImages(device, swapchain, ref imageCount, null);
    Image[] swapchainImages = new Image[imageCount];
    fixed (Image* swapChainImagesPtr = swapchainImages)
    {
      khrSwapchain.GetSwapchainImages(device, swapchain, ref imageCount, swapChainImagesPtr);
    }
    context.KhrSwapchain = khrSwapchain;
    context.Swapchain = swapchain;
    context.SwapchainImageFormat = surfaceFormat.Format;
    context.SwapchainExtent = extent;
    context.SwapchainImages = swapchainImages;
  }
  public void CleanUp(RenderingContext context)
  {
    context.KhrSwapchain?.DestroySwapchain(context.Device.GetValueOrDefault(), context.Swapchain.GetValueOrDefault(), null);
    context.KhrSwapchain = null;
    context.Swapchain = null;
    context.SwapchainImageFormat = null;
    context.SwapchainExtent = null;
    context.SwapchainImages = null;
  }

  private static SwapchainSupportDetails QuerySwapChainSupport(KhrSurface khrSurface, SurfaceKHR surface, PhysicalDevice physicalDevice)
  {
    SwapchainSupportDetails details = new()
    {
      Formats = [],
      PresentModes = []
    };
    uint formatCount = 0;
    uint presentModeCount = 0;

    khrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out details.Capabilities);
    khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, null);

    if (formatCount != 0)
    {
      details.Formats = new SurfaceFormatKHR[formatCount];
      fixed (SurfaceFormatKHR* formatsPtr = details.Formats)
      {
        khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, formatsPtr);
      }
    }

    khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, null);

    if (presentModeCount != 0)
    {
      details.PresentModes = new PresentModeKHR[presentModeCount];
      fixed (PresentModeKHR* formatsPtr = details.PresentModes)
      {
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, formatsPtr);
      }
    }

    return details;
  }

  private static SurfaceFormatKHR ChooseSwapSurfaceFormat(IReadOnlyList<SurfaceFormatKHR> availableFormats)
  {
    foreach (SurfaceFormatKHR availableFormat in availableFormats)
    {
      if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
      {
        return availableFormat;
      }
    }

    return availableFormats[0];
  }

  private static PresentModeKHR ChoosePresentMode(IReadOnlyList<PresentModeKHR> availablePresentModes)
  {
    foreach (PresentModeKHR availablePresentMode in availablePresentModes)
    {
      if (availablePresentMode == PresentModeKHR.MailboxKhr)
      {
        return availablePresentMode;
      }
    }

    return PresentModeKHR.FifoKhr;
  }

  private static Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities, IWindow window)
  {
    if (capabilities.CurrentExtent.Width != uint.MaxValue)
      return capabilities.CurrentExtent;

    Vector2D<int> framebufferSize = window.FramebufferSize;

    uint extendWidth = Math.Clamp((uint)framebufferSize.X, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
    uint extendHeight = Math.Clamp((uint)framebufferSize.Y, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);

    return new()
    {
      Width = extendWidth,
      Height = extendHeight
    };
  }
}