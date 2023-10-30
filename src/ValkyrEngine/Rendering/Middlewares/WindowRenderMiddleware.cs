using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering.Middlewares;

internal class WindowRenderMiddleware : IRenderMiddleware
{
  public void CleanUp(RenderingContext context)
  {
    context.Window?.Dispose();
    context.Window = null;
  }

  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    IWindow window = Window.Create(options.WindowOptions);

    window.Initialize();

    if (window.VkSurface is null)
    {
      throw new Exception("Windowing platform doesn't support Vulkan.");
    }
    context.Window = window;
  }
}