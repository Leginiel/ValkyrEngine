using Silk.NET.Vulkan;

namespace ValkyrEngine;

internal struct SwapchainSupportDetails
{
  public SurfaceCapabilitiesKHR Capabilities;
  public SurfaceFormatKHR[] Formats;
  public PresentModeKHR[] PresentModes;
}