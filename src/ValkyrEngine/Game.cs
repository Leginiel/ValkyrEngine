using Silk.NET.Windowing;
using ValkyrEngine.Rendering;
using ValkyrEngine.Rendering.Middlewares;

namespace ValkyrEngine;

public class Game : IDisposable
{
  private bool _disposedValue;
  private RenderingContext? _context;

  public void Init(ValkyrEngineOptions options)
  {
    _context = new RenderingContext(options)
                .Add<WindowRenderMiddleware>()
                .Add<VulkanInstanceMiddleware>()
                .Add<DebugMessengerMiddleware>()
                .Add<SurfaceMiddleware>()
                .Add<PhysicalDeviceMiddleware>()
                .Add<LogicalDeviceMiddleware>()
                .Add<SwapchainMiddleware>()
                .Add<ImageViewsMiddleware>()
                .Add<RenderPassMiddleware>()
                .Add<GraphicPipelineMiddleware>()
                .Add<FramebufferMiddleware>()
                .Add<CommandPoolMiddleware>()
                .Add<CommandBufferMiddleware>()
                .Add<SyncObjectMiddleware>();
  }
  public void Run()
  {
    if (_context is null)
      return;

    IWindow window = _context.Window!;
    window.Render += _context.DrawFrame;
    window.Resize += _context.Resize;
    window.Run();
    _context.Vk!.DeviceWaitIdle(_context.Device.GetValueOrDefault());
  }
  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _context?.Dispose();
        _context = null;
      }
      _disposedValue = true;
    }
  }
}