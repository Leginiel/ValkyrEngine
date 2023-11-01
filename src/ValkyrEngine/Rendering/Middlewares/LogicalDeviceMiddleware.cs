using System.Runtime.CompilerServices;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class LogicalDeviceMiddleware : IRenderMiddleware
{
  public static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    ValkyrEngineOptions options = context.Options;
    PhysicalDevice physicalDevice = context.PhysicalDevice.GetValueOrDefault();
    PhysicalDeviceFeatures deviceFeatures = new();
    QueueFamilyIndices indices = context.FindQueueFamilies();
    string[] deviceExtensions = RenderingContext.DeviceExtensions;
    string[] validationLayers = RenderingContext.ValidationLayers;
    float queuePriority = 1.0f;
    uint[] uniqueQueueFamilies = [indices.GraphicsFamily!.Value, indices.PresentFamily!.Value];

    uniqueQueueFamilies = uniqueQueueFamilies.Distinct().ToArray();

    using GlobalMemory mem = GlobalMemory.Allocate(uniqueQueueFamilies.Length * sizeof(DeviceQueueCreateInfo));
    DeviceQueueCreateInfo* queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref mem.GetPinnableReference());

    for (int i = 0; i < uniqueQueueFamilies.Length; i++)
    {
      queueCreateInfos[i] = new()
      {
        SType = StructureType.DeviceQueueCreateInfo,
        QueueFamilyIndex = uniqueQueueFamilies[i],
        QueueCount = 1,
        PQueuePriorities = &queuePriority
      };
    }

    DeviceCreateInfo createInfo = new()
    {
      SType = StructureType.DeviceCreateInfo,
      QueueCreateInfoCount = (uint)uniqueQueueFamilies.Length,
      PQueueCreateInfos = queueCreateInfos,

      PEnabledFeatures = &deviceFeatures,

      EnabledExtensionCount = (uint)deviceExtensions.Length,
      PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(deviceExtensions),

      EnabledLayerCount = 0
    };

    if (options.ActivateValidationLayers)
    {
      createInfo.EnabledLayerCount = (uint)validationLayers.Length;
      createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(validationLayers);
    }

    if (vk.CreateDevice(physicalDevice, in createInfo, null, out Device device) != Result.Success)
    {
      throw new Exception("failed to create logical device!");
    }

    vk.GetDeviceQueue(device, indices.GraphicsFamily!.Value, 0, out Queue graphicsQueue);
    vk.GetDeviceQueue(device, indices.PresentFamily!.Value, 0, out Queue presentQueue);

    if (options.ActivateValidationLayers)
    {
      SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
    }

    SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);
    context.Device = device;
    context.GraphicQueue = graphicsQueue;
    context.PresentQueue = presentQueue;
  }

  public static void CleanUp(RenderingContext context)
  {
    context.Vk?.DestroyDevice(context.Device.GetValueOrDefault(), null);
  }
}