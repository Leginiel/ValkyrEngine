using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class CommandPoolMiddleware : IRenderMiddleware
{
  public static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    QueueFamilyIndices queueFamiliyIndicies = context.FindQueueFamilies();
    CommandPoolCreateInfo poolInfo = new()
    {
      SType = StructureType.CommandPoolCreateInfo,
      QueueFamilyIndex = queueFamiliyIndicies.GraphicsFamily!.Value,
    };

    if (vk.CreateCommandPool(device, poolInfo, null, out CommandPool commandPool) != Result.Success)
    {
      throw new Exception("failed to create command pool!");
    }
    context.CommandPool = commandPool;
  }

  public static void CleanUp(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    CommandPool commandPool = context.CommandPool.GetValueOrDefault();

    vk.DestroyCommandPool(device, commandPool, null);

    context.CommandPool = null;
  }
}
