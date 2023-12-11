namespace ValkyrEngine.Rendering.Resources;

public interface IFlaggable { }

public interface IFlaggable<TFlags> : IFlaggable
  where TFlags : unmanaged, Enum
{
  TFlags Flags { get; }
  void SetFlag(TFlags flags);
  void ClearFlag(TFlags flags);
  bool HasFlag(TFlags flags);
}