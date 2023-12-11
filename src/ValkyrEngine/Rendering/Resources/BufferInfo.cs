using ValkyrEngine.Rendering.Resources;

namespace ValkyrEngine.Rendering;

public record BufferInfo(ulong Size = 0, BufferUsageFlags Usage = 0, AttachmentInfoFlags Flags = AttachmentInfoFlags.PersistantBit) : IRenderResourceInfo;