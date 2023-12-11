namespace ValkyrEngine.Rendering;

public class PhysicalPass()
{
  public List<uint> Passes { get; } = [];
  public List<uint> Discards { get; } = [];
  public List<Barrier> Invalidate { get; } = [];
  public List<Barrier> Flush { get; } = [];
  public List<Barrier> History { get; } = [];
  public List<(uint, uint)> AliasTransfer { get; } = [];
  public RenderPassInfo RenderPassInfo { get; set; }
  public List<Subpass> Subpasses { get; } = [];
  public List<ColorClearRequests> ColorClearRequests { get; } = [];
  public DepthClearRequests DepthClearRequest { get; set; }
  public List<List<ScaledClearRequests>> ScaledClearRequests { get; } = [];
  public List<MipmapRequests> MipmapRequests { get; } = [];
  public List<uint> PhysicalColorAttachments { get; } = [];
  public uint PhysicalDepthStencilAttachment { get; set; } = 0;
  public uint Layers { get; set; } = 1;
}