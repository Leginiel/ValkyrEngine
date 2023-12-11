using ValkyrEngine.Rendering.Extensions;

namespace ValkyrEngine.Rendering.Resources;

public class ResourceFlags<TFlags> : IFlaggable<TFlags>
  where TFlags : unmanaged, Enum
{
  private TFlags _flags;

  public TFlags Flags => _flags;

  public void SetFlag(TFlags flags)
  {
    EnumFlagExtensions.SetFlag(ref _flags, flags);
  }
  public void ClearFlag(TFlags flags)
  {
    EnumFlagExtensions.ClearFlag(ref _flags, flags);
  }
  public bool HasFlag(TFlags flags)
  {
    return EnumFlagExtensions.HasFlag(ref _flags, flags);
  }
}