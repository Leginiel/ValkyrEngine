using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class SyncObjectMiddleware : IRenderMiddleware
{
  public const int MaxFramesInFlight = 2;

  public static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    Image[] swapchainImages = context.SwapchainImages!;
    Semaphore[] imageAvailableSemaphores = new Semaphore[MaxFramesInFlight];
    Semaphore[] renderFinishedSemaphores = new Semaphore[MaxFramesInFlight];
    Fence[] inFlightFences = new Fence[MaxFramesInFlight];
    Fence[] imagesInFlight = new Fence[swapchainImages.Length];

    SemaphoreCreateInfo semaphoreInfo = new()
    {
      SType = StructureType.SemaphoreCreateInfo,
    };

    FenceCreateInfo fenceInfo = new()
    {
      SType = StructureType.FenceCreateInfo,
      Flags = FenceCreateFlags.SignaledBit,
    };

    for (var i = 0; i < MaxFramesInFlight; i++)
    {
      if (vk.CreateSemaphore(device, semaphoreInfo, null, out imageAvailableSemaphores[i]) != Result.Success ||
          vk.CreateSemaphore(device, semaphoreInfo, null, out renderFinishedSemaphores[i]) != Result.Success ||
          vk.CreateFence(device, fenceInfo, null, out inFlightFences[i]) != Result.Success)
      {
        throw new Exception("failed to create synchronization objects for a frame!");
      }
    }

    context.ImageAvailableSemaphores = imageAvailableSemaphores;
    context.RenderFinishedSemaphores = renderFinishedSemaphores;
    context.InFlightFences = inFlightFences;
    context.ImagesInFlightFences = imagesInFlight;
  }

  public static void CleanUp(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    Semaphore[] imageAvailableSemaphores = context.ImageAvailableSemaphores!;
    Semaphore[] renderFinishedSemaphores = context.RenderFinishedSemaphores!;
    Fence[] inFlightFences = context.InFlightFences!;

    for (int i = 0; i < MaxFramesInFlight; i++)
    {
      vk.DestroySemaphore(device, renderFinishedSemaphores![i], null);
      vk.DestroySemaphore(device, imageAvailableSemaphores![i], null);
      vk.DestroyFence(device, inFlightFences![i], null);
    }

    context.ImageAvailableSemaphores = null;
    context.RenderFinishedSemaphores = null;
    context.InFlightFences = null;
    context.ImagesInFlightFences = null;
  }
}
