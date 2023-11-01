using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering.Middlewares;

internal class WindowRenderMiddleware : IRenderMiddleware
{
  public static void Init(RenderingContext context)
  {
    ValkyrEngineOptions options = context.Options;
    IWindow window = Window.Create(options.WindowOptions);

    window.Initialize();

    if (window.VkSurface is null)
    {
      throw new Exception("Windowing platform doesn't support Vulkan.");
    }
    context.Window = window;
  }

  public static void CleanUp(RenderingContext context)
  {
    context.Window?.Dispose();
    context.Window = null;
  }
}