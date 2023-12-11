namespace ValkyrEngine.Rendering;

[Flags]
public enum AttachmentInfoFlags : uint
{
  PersistantBit = 1 << 0,
  UNormSRGBAliasBit = 1 << 1,
  SupportsPreRotateBit = 1 << 2,
  MpiGenBit = 1 << 3,

  InternalTransientBit = 1 << 16,
  InternalProxyBit = 1 << 17
}