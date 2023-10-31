using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering;

internal unsafe sealed class RenderingContext : IDisposable
{
  private bool disposedValue;
  private readonly Stack<Action<RenderingContext>> _cleanUpJobs = new();

  internal static readonly string[] DeviceExtensions =
  [
    KhrSwapchain.ExtensionName
  ];

  internal static readonly string[] ValidationLayers =
  [
    "VK_LAYER_KHRONOS_validation"
  ];

  public IWindow? Window { get; set; }
  public Vk? Vk { get; set; }
  public Instance? Instance { get; set; }
  public DebugUtilsMessengerEXT? DebugMessenger { get; set; }
  public ExtDebugUtils? DebugUtils { get; set; }
  public SurfaceKHR? Surface { get; set; }
  public KhrSurface? KhrSurface { get; internal set; }
  public PhysicalDevice? PhysicalDevice { get; internal set; }
  public Device? Device { get; internal set; }
  public Queue? GraphicQueue { get; internal set; }
  public Queue? PresentQueue { get; internal set; }
  public KhrSwapchain? KhrSwapchain { get; internal set; }
  public SwapchainKHR? Swapchain { get; internal set; }
  public Format? SwapchainImageFormat { get; internal set; }
  public Extent2D? SwapchainExtent { get; internal set; }
  public Image[]? SwapchainImages { get; internal set; }
  public ImageView[]? SwapchainImageViews { get; internal set; }
  public RenderPass? RenderPass { get; internal set; }
  public PipelineLayout? PipelineLayout { get; internal set; }
  public Pipeline? GraphicsPipeline { get; internal set; }
  public Framebuffer[]? SwapchainFramebuffer { get; internal set; }
  public CommandPool? CommandPool { get; internal set; }
  public CommandBuffer[]? CommandBuffer { get; internal set; }

  public QueueFamilyIndices FindQueueFamilies(PhysicalDevice? physicalDevice = null)
  {
    PhysicalDevice device = physicalDevice ?? PhysicalDevice.GetValueOrDefault();
    KhrSurface khrSurface = KhrSurface!;
    SurfaceKHR surface = Surface.GetValueOrDefault();
    QueueFamilyIndices indices = new();

    uint queueFamilityCount = 0;
    uint i = 0;

    Vk?.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, null);
    QueueFamilyProperties[] queueFamilies = new QueueFamilyProperties[queueFamilityCount];

    fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
    {
      Vk?.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, queueFamiliesPtr);
    }

    while (!indices.IsComplete && i < queueFamilies.Length)
    {
      QueueFamilyProperties queueFamily = queueFamilies[i];

      khrSurface.GetPhysicalDeviceSurfaceSupport(device, i, surface, out Bool32 presentSupport);

      if (presentSupport)
        indices.PresentFamily = i;

      if (queueFamily.QueueFlags.HasFlag(QueueFlags.GraphicsBit))
        indices.GraphicsFamily = i;

      i++;
    }

    return indices;
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  public void AddCleanUpJob(Action<RenderingContext> job)
  {
    _cleanUpJobs.Push(job);
  }
  private void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        while (_cleanUpJobs.TryPop(out Action<RenderingContext>? job))
        {
          job.Invoke(this);
        }
      }
      disposedValue = true;
    }
  }
}