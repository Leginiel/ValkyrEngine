

namespace ValkyrEngine.Rendering.Vulkan;

public abstract class NativeVulkanObject<TObject, TCreateInfo>(TObject nativeObject, TCreateInfo info)
  where TObject : struct
  where TCreateInfo : struct
{
  public TObject Native { get; } = nativeObject;
  public TCreateInfo Info { get; } = info;

  public abstract void CleanUp(ref Vk vk, ref Device device);
}