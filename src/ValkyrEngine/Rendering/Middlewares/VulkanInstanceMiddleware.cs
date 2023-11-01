using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Windowing;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class VulkanInstanceMiddleware : IRenderMiddleware
{
  public static void Init(RenderingContext context)
  {
    Vk vk = Vk.GetApi();
    ValkyrEngineOptions options = context.Options;
    bool activateValidationLayers = options.ActivateValidationLayers;

    if (activateValidationLayers && !CheckValidationLayerSupport(vk))
    {
      throw new Exception("validation layers requested, but not available!");
    }

    ApplicationInfo appInfo = new()
    {
      SType = StructureType.ApplicationInfo,
      PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(options.ApplicationName),
      ApplicationVersion = new Version32(1, 0, 0),
      PEngineName = (byte*)Marshal.StringToHGlobalAnsi("ValkyrEngine"),
      EngineVersion = new Version32(1, 0, 0),
      ApiVersion = Vk.Version11
    };

    InstanceCreateInfo createInfo = new()
    {
      SType = StructureType.InstanceCreateInfo,
      PApplicationInfo = &appInfo
    };

    var glfwExtensions = GetRequiredExtensions(context.Window!, activateValidationLayers);
    createInfo.EnabledExtensionCount = (uint)glfwExtensions.Length;
    createInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(glfwExtensions);
    createInfo.EnabledLayerCount = 0;

    if (vk.CreateInstance(createInfo, null, out Instance instance) != Result.Success)
    {
      throw new Exception("failed to create instance!");
    }

    Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
    Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);

    context.Vk = vk;
    context.Instance = instance;
  }
  public static void CleanUp(RenderingContext context)
  {
    Vk? vk = context.Vk;
    if (context.Instance is not null)
      vk?.DestroyInstance(context.Instance.Value, null);

    vk?.Dispose();

    context.Vk = null;
    context.Instance = null;
  }

  private static string[] GetRequiredExtensions(IWindow window, bool activateValidationLayers)
  {
    var glfwExtensions = window.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);
    var extensions = SilkMarshal.PtrToStringArray((nint)glfwExtensions, (int)glfwExtensionCount);
    extensions = extensions.Append("VK_KHR_portability_enumeration").ToArray();

    if (activateValidationLayers)
    {
      return [.. extensions, ExtDebugUtils.ExtensionName];
    }

    return extensions;
  }

  private static bool CheckValidationLayerSupport(Vk vk)
  {
    uint layerCount = 0;
    vk.EnumerateInstanceLayerProperties(ref layerCount, null);
    var availableLayers = new LayerProperties[layerCount];

    fixed (LayerProperties* availableLayersPtr = availableLayers)
    {
      vk.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
    }

    var availableLayerNames = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();

    return RenderingContext.ValidationLayers.All(availableLayerNames.Contains);
  }
}
