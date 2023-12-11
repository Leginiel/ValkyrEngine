namespace ValkyrEngine.Rendering;


[Flags]
public enum RenderGraphQueueFlags : byte
{
  GraphicBit = 1 << 0,
  ComputeBit = 1 << 1,
  AsyncComputeBit = 1 << 2,
  AsyncGraphicBit = 1 << 3
}