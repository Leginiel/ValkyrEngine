using ValkyrEngine.Rendering;
using ValkyrEngine.Rendering.Middlewares;
using ValkyrEngine.Rendering.Steps;

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























// private ExtDebugUtils? debugUtils;
// private DebugUtilsMessengerEXT debugMessenger;
// private int _currentFrame = 0;

// // public void Init(WindowOptions windowOptions, string applicationName)
// // {
// //   InitWindow(windowOptions);
// //   InitVulkan(applicationName);
// // }
// public void Run()
// {
//   if (_window is null)
//     return;

//   _window.Run();
//   _vk!.DeviceWaitIdle(_device);
// }

// // public void Dispose()
// // {
// //   // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
// //   Dispose(disposing: true);
// //   GC.SuppressFinalize(this);
// // }public void Dispose()
// // {
// //   // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
// //   Dispose(disposing: true);
// //   GC.SuppressFinalize(this);
// // }

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

// //       if (ValidationLayersEnabled)
// //       {
// //         debugUtils?.DestroyDebugUtilsMessenger(_instance, debugMessenger, null);
// //       }

// //       DestroyWindowSurface();

// //       _vk?.DestroyInstance(_instance, null);
// //       _vk?.Dispose();

// //       _window?.Dispose();
// //     }
// //     _disposedValue = true;
// //   }
// // }

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

// //       if (ValidationLayersEnabled)
// //       {
// //         debugUtils?.DestroyDebugUtilsMessenger(_instance, debugMessenger, null);
// //       }

// //       DestroyWindowSurface();

// //     }
// //     _disposedValue = true;
// //   }
// // }
// private void InitVulkan()
// {
//   SetupDebugMessenger();
//   CreateSurface();
//   PickPhysicalDevice();
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

// private void SetupDebugMessenger()
// {
//   if (!ValidationLayersEnabled)
//     return;

//   //TryGetInstanceExtension equivilant to method CreateDebugUtilsMessengerEXT from original tutorial.
//   if (!_vk!.TryGetInstanceExtension(_instance, out debugUtils))
//     return;

//   DebugUtilsMessengerCreateInfoEXT createInfo = new();
//   PopulateDebugMessengerCreateInfo(ref createInfo);

//   if (debugUtils!.CreateDebugUtilsMessenger(_instance, in createInfo, null, out debugMessenger) != Result.Success)
//   {
//     throw new Exception("failed to set up debug messenger!");
//   }
// }
// private void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo)
// {
//   createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
//   createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
//                                DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
//                                DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
//   createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
//                            DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
//                            DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
//   createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
// }
// private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
// {
//   Console.WriteLine($"validation layer:" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

//   return Vk.False;
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
