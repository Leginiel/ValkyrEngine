using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class PhysicalDeviceMiddleware : IRenderMiddleware
{
  public static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Instance instance = context.Instance.GetValueOrDefault();
    KhrSurface khrSurface = context.KhrSurface!;
    SurfaceKHR surface = context.Surface.GetValueOrDefault();
    PhysicalDevice? physicalDevice = null;
    uint devicedCount = 0;

    vk.EnumeratePhysicalDevices(instance, ref devicedCount, null);

    if (devicedCount == 0)
    {
      throw new Exception("failed to find GPUs with Vulkan support!");
    }

    var devices = new PhysicalDevice[devicedCount];
    fixed (PhysicalDevice* devicesPtr = devices)
    {
      vk.EnumeratePhysicalDevices(instance, ref devicedCount, devicesPtr);
    }

    foreach (var device in devices)
    {
      if (IsDeviceSuitable(context, vk, device, khrSurface, surface))
      {
        physicalDevice = device;
        break;
      }
    }

    if (physicalDevice?.Handle == 0)
    {
      throw new Exception("failed to find a suitable GPU!");
    }
    context.PhysicalDevice = physicalDevice;
  }

  public static void CleanUp(RenderingContext context)
  {
    context.PhysicalDevice = null;
  }

  private static bool IsDeviceSuitable(RenderingContext context, Vk vk, PhysicalDevice device, KhrSurface khrSurface, SurfaceKHR surface)
  {
    QueueFamilyIndices indices = context.FindQueueFamilies(device);
    bool extensionsSupported = CheckDeviceExtensionsSupport(vk, device);
    bool swapChainAdequate = false;

    if (extensionsSupported)
    {
      var swapChainSupport = QuerySwapChainSupport(device, khrSurface, surface);
      swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
    }

    return indices.IsComplete && extensionsSupported && swapChainAdequate;
  }

  private static bool CheckDeviceExtensionsSupport(Vk vk, PhysicalDevice device)
  {
    uint extentionsCount = 0;
    vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, null);

    var availableExtensions = new ExtensionProperties[extentionsCount];
    fixed (ExtensionProperties* availableExtensionsPtr = availableExtensions)
    {
      vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, availableExtensionsPtr);
    }

    var availableExtensionNames = availableExtensions.Select(extension => Marshal.PtrToStringAnsi((IntPtr)extension.ExtensionName)).ToHashSet();

    return RenderingContext.DeviceExtensions.All(availableExtensionNames.Contains);
  }

  private static SwapchainSupportDetails QuerySwapChainSupport(PhysicalDevice device, KhrSurface khrSurface, SurfaceKHR surface)
  {
    var details = new SwapchainSupportDetails();

    khrSurface.GetPhysicalDeviceSurfaceCapabilities(device, surface, out details.Capabilities);

    uint formatCount = 0;
    khrSurface.GetPhysicalDeviceSurfaceFormats(device, surface, ref formatCount, null);

    if (formatCount != 0)
    {
      details.Formats = new SurfaceFormatKHR[formatCount];
      fixed (SurfaceFormatKHR* formatsPtr = details.Formats)
      {
        khrSurface.GetPhysicalDeviceSurfaceFormats(device, surface, ref formatCount, formatsPtr);
      }
    }
    else
    {
      details.Formats = Array.Empty<SurfaceFormatKHR>();
    }

    uint presentModeCount = 0;
    khrSurface.GetPhysicalDeviceSurfacePresentModes(device, surface, ref presentModeCount, null);

    if (presentModeCount != 0)
    {
      details.PresentModes = new PresentModeKHR[presentModeCount];
      fixed (PresentModeKHR* formatsPtr = details.PresentModes)
      {
        khrSurface.GetPhysicalDeviceSurfacePresentModes(device, surface, ref presentModeCount, formatsPtr);
      }
    }
    else
    {
      details.PresentModes = [];
    }

    return details;
  }
}