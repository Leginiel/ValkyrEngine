using Silk.NET.Maths;
using ValkyrEngine.Rendering.Resources;

namespace ValkyrEngine.Rendering;

public record AttachmentInfo(string SizeRelativeName,
                             Vector3D<float> Size,
                             SizeClass SizeClass = SizeClass.SwapchainRelative,
                             Format Format = Format.Undefined,
                             uint Samples = 1,
                             uint Levels = 1,
                             uint Layers = 1,
                             ImageUsageFlags AuxUsage = 0,
                             AttachmentInfoFlags Flags = AttachmentInfoFlags.PersistantBit) : IRenderResourceInfo;