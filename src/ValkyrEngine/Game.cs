using ValkyrEngine.Rendering;
using ValkyrEngine.Rendering.Middlewares;

namespace ValkyrEngine;

public class Game : IDisposable
{
  private bool _disposedValue;
  private RenderingContext? _context;

  public void Init(ValkyrEngineOptions options)
  {
    _context = new GameBuilder()
                    .Use<WindowRenderMiddleware>()
                    .Use<VulkanInstanceMiddleware>()
                    .Use<DebugMessengerMiddleware>()
                    .Use<SurfaceMiddleware>()
                    .Use<PhysicalDeviceMiddleware>()
                    .Use<LogicalDeviceMiddleware>()
                    .Use<SwapchainMiddleware>()
                    .Use<ImageViewsMiddleware>()
                    .Use<RenderPassMiddleware>()
                    .Use<GraphicPipelineMiddleware>()
                    .Build(options);
  }
  public void Run()
  {

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























// private int _currentFrame = 0;

// public void Run()
// {
//   if (_window is null)
//     return;

//   _window.Run();
//   _vk!.DeviceWaitIdle(_device);
// }

// // protected virtual void Dispose(bool disposing)
// // {
// //   if (!_disposedValue)
// //   {
// //     if (disposing)
// //     {
// //       DestroySyncObjects();
// //       DestoryCommandPool();
// //       DestroyFramebuffers();
// //       DestroyGraphicsPipeline();
// //       DestroyRenderPass();
// //       DestroyImageViews();
// //       DestroySwapchain();
// //       DestroyLogicalDevice();



// //     }
// //     _disposedValue = true;
// //   }
// // }
// private void InitVulkan()
// {
//   CreateLogicalDevice();
//   CreateSwapchain();
//   CreateImageViews();
//   CreateRenderPass();
//   CreateGraphicsPipeline();
//   CreateFramebuffers();
//   CreateCommandPool();
//   CreateCommandBuffers();
//   CreateSyncObjects();
// }


// private bool CheckValidationLayerSupport()
// {
//   uint layerCount = 0;
//   _vk!.EnumerateInstanceLayerProperties(ref layerCount, null);
//   var availableLayers = new LayerProperties[layerCount];
//   fixed (LayerProperties* availableLayersPtr = availableLayers)
//   {
//     _vk!.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
//   }

//   var availableLayerNames = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();

//   return _validationLayers.All(availableLayerNames.Contains);
// }
// private void DrawFrame(double delta)
// {
//   _vk!.WaitForFences(_device, 1, inFlightFences![_currentFrame], true, ulong.MaxValue);

//   uint imageIndex = 0;
//   _khrSwapchain!.AcquireNextImage(_device, _swapchain, ulong.MaxValue, imageAvailableSemaphores![_currentFrame], default, ref imageIndex);

//   if (imagesInFlight![imageIndex].Handle != default)
//   {
//     _vk!.WaitForFences(_device, 1, imagesInFlight[imageIndex], true, ulong.MaxValue);
//   }
//   imagesInFlight[imageIndex] = inFlightFences[_currentFrame];

//   SubmitInfo submitInfo = new()
//   {
//     SType = StructureType.SubmitInfo,
//   };

//   var waitSemaphores = stackalloc[] { imageAvailableSemaphores[_currentFrame] };
//   var waitStages = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };

//   var buffer = _commandBuffers![imageIndex];

//   submitInfo = submitInfo with
//   {
//     WaitSemaphoreCount = 1,
//     PWaitSemaphores = waitSemaphores,
//     PWaitDstStageMask = waitStages,

//     CommandBufferCount = 1,
//     PCommandBuffers = &buffer
//   };

//   var signalSemaphores = stackalloc[] { renderFinishedSemaphores![_currentFrame] };
//   submitInfo = submitInfo with
//   {
//     SignalSemaphoreCount = 1,
//     PSignalSemaphores = signalSemaphores,
//   };

//   _vk!.ResetFences(_device, 1, inFlightFences[_currentFrame]);

//   if (_vk!.QueueSubmit(_graphicsQueue, 1, submitInfo, inFlightFences[_currentFrame]) != Result.Success)
//   {
//     throw new Exception("failed to submit draw command buffer!");
//   }

//   var swapChains = stackalloc[] { _swapchain };
//   PresentInfoKHR presentInfo = new()
//   {
//     SType = StructureType.PresentInfoKhr,

//     WaitSemaphoreCount = 1,
//     PWaitSemaphores = signalSemaphores,

//     SwapchainCount = 1,
//     PSwapchains = swapChains,

//     PImageIndices = &imageIndex
//   };

//   _khrSwapchain.QueuePresent(_presentQueue, presentInfo);

//   _currentFrame = (_currentFrame + 1) % MaxFramesInFlight;
// }
// }
