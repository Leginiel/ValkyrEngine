using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;
using ValkyrEngine.Rendering.Middlewares;
using Semaphore = Silk.NET.Vulkan.Semaphore;


namespace ValkyrEngine.Rendering;

internal unsafe sealed class RenderingContext : IDisposable
{
  private int _currentFrame = 0;
  private bool _disposedValue;
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
  public Semaphore[]? ImageAvailableSemaphores { get; internal set; }
  public Semaphore[]? RenderFinishedSemaphores { get; internal set; }
  public Fence[]? InFlightFences { get; internal set; }
  public Fence[]? ImagesInFlightFences { get; internal set; }

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

  public void DrawFrame(double delta)
  {
    Device device = Device.GetValueOrDefault();
    SwapchainKHR swapchain = Swapchain.GetValueOrDefault();
    Queue graphicQueue = GraphicQueue.GetValueOrDefault();
    Queue presentQueue = PresentQueue.GetValueOrDefault();
    uint imageIndex = 0;

    Vk!.WaitForFences(device, 1, InFlightFences![_currentFrame], true, ulong.MaxValue);
    KhrSwapchain!.AcquireNextImage(device, swapchain, ulong.MaxValue, ImageAvailableSemaphores![_currentFrame], default, ref imageIndex);

    if (ImagesInFlightFences![imageIndex].Handle != default)
    {
      Vk!.WaitForFences(device, 1, ImagesInFlightFences[imageIndex], true, ulong.MaxValue);
    }
    ImagesInFlightFences[imageIndex] = ImagesInFlightFences[_currentFrame];

    Semaphore* waitSemaphores = stackalloc[] { ImageAvailableSemaphores[_currentFrame] };
    PipelineStageFlags* waitStages = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
    Semaphore* signalSemaphores = stackalloc[] { RenderFinishedSemaphores![_currentFrame] };
    CommandBuffer buffer = CommandBuffer![imageIndex];
    SubmitInfo submitInfo = new()
    {
      SType = StructureType.SubmitInfo,
      WaitSemaphoreCount = 1,
      PWaitSemaphores = waitSemaphores,
      PWaitDstStageMask = waitStages,

      CommandBufferCount = 1,
      PCommandBuffers = &buffer,
      SignalSemaphoreCount = 1,
      PSignalSemaphores = signalSemaphores,
    };

    Vk!.ResetFences(device, 1, InFlightFences[_currentFrame]);

    if (Vk!.QueueSubmit(graphicQueue, 1, submitInfo, InFlightFences[_currentFrame]) != Result.Success)
    {
      throw new Exception("failed to submit draw command buffer!");
    }

    SwapchainKHR* swapChains = stackalloc[] { swapchain };
    PresentInfoKHR presentInfo = new()
    {
      SType = StructureType.PresentInfoKhr,
      WaitSemaphoreCount = 1,
      PWaitSemaphores = signalSemaphores,
      SwapchainCount = 1,
      PSwapchains = swapChains,
      PImageIndices = &imageIndex
    };

    KhrSwapchain.QueuePresent(presentQueue, presentInfo);

    _currentFrame = (_currentFrame + 1) % SyncObjectMiddleware.MaxFramesInFlight;
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
    if (!_disposedValue)
    {
      if (disposing)
      {
        while (_cleanUpJobs.TryPop(out Action<RenderingContext>? job))
        {
          job.Invoke(this);
        }
      }
      _disposedValue = true;
    }
  }
}