using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class SurfaceMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    Instance instance = context.Instance.GetValueOrDefault();

    if (!context.Vk!.TryGetInstanceExtension(instance, out KhrSurface khrSurface))
    {
      throw new NotSupportedException("KHR_surface extension not found.");
    }

    context.Surface = context.Window!.VkSurface!.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();
    context.KhrSurface = khrSurface;
  }

  public void CleanUp(RenderingContext context)
  {
    context.KhrSurface!.DestroySurface(context.Instance.GetValueOrDefault(), context.Surface.GetValueOrDefault(), null);
  }
}