using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering;

internal sealed class RenderingContext : IDisposable
{
  private bool disposedValue;
  private readonly Stack<Action<RenderingContext>> _cleanUpJobs = new();

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