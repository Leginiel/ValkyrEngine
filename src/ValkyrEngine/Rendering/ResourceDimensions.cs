namespace ValkyrEngine.Rendering;

public class ResourceDimensions(string name)
{
  public string Name => name;
  public BufferInfo BufferInfo { get; set; }
  public Format Format { get; set; } = Format.Undefined;
  public uint Width { get; set; } = 0;
  public uint Height { get; set; } = 0;
  public uint Depth { get; set; } = 1;
  public uint Layers { get; set; } = 1;
  public uint Levels { get; set; } = 1;
  public uint Samples { get; set; } = 1;
  public AttachmentInfoFlags Flags { get; set; } = AttachmentInfoFlags.PersistantBit;
  public SurfaceTransformFlagsKHR Transform { get; set; } = SurfaceTransformFlagsKHR.IdentityBitKhr;
  public RenderGraphQueueFlags Queues { get; set; } = RenderGraphQueueFlags.GraphicBit;
  public ImageUsageFlags ImageUsage { get; set; } = 0;

  public bool UsesSemaphore()
  {
    if (Flags.HasFlag(AttachmentInfoFlags.InternalProxyBit))
      return true;

    RenderGraphQueueFlags physicalQueues = Queues;

    if (physicalQueues.HasFlag(RenderGraphQueueFlags.ComputeBit))
    {
      physicalQueues |= RenderGraphQueueFlags.GraphicBit;
    }

    physicalQueues &= RenderGraphQueueFlags.ComputeBit;

    return (physicalQueues & (physicalQueues - 1)) != 0;
  }

  public bool IsStorageImage => ImageUsage.HasFlag(ImageUsageFlags.StorageBit);
  public bool IsBufferLike => IsStorageImage || (BufferInfo.Size != 0) || ImageUsage.HasFlag(ImageUsageFlags.StorageBit);
}
