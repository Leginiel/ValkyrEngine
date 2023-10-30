using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class DebugMessengerMiddleware : IRenderMiddleware
{
  public void Init(RenderingContext context, ValkyrEngineOptions options)
  {
    if (!options.ActivateValidationLayers)
      return;

    Instance instance = context.Instance.GetValueOrDefault();
    Vk vk = context.Vk!;

    if (!vk.TryGetInstanceExtension(instance, out ExtDebugUtils? debugUtils))
      return;

    DebugUtilsMessengerCreateInfoEXT createInfo = new();
    PopulateDebugMessengerCreateInfo(ref createInfo);

    if (debugUtils!.CreateDebugUtilsMessenger(instance, in createInfo, null, out DebugUtilsMessengerEXT debugMessenger) != Result.Success)
    {
      throw new Exception("failed to set up debug messenger!");
    }

    context.DebugMessenger = debugMessenger;
    context.DebugUtils = debugUtils;
  }

  public void CleanUp(RenderingContext context)
  {
    if (context.DebugMessenger is not null)
      context.DebugUtils?.DestroyDebugUtilsMessenger(context.Instance.GetValueOrDefault(), context.DebugMessenger.Value, null);
  }

  private static void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo)
  {
    createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
    createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                 DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                 DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
    createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                             DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                             DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
    createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
  }

  private static uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
  {
    Console.WriteLine($"validation layer:" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

    return Vk.False;
  }
}