namespace ValkyrEngine.Rendering.Resources;

public sealed class RenderResource(RenderResourceType type, int index)
{
  private readonly Dictionary<Type, IFlaggable> _flagStorage = [];
  private readonly HashSet<int> _writtenInPasses = [];
  private readonly HashSet<int> _readInPasses = [];
  public const int Unused = -1;

  public RenderResourceType Type => type;
  public int Index => index;
  public int PhysicalIndex { get; set; } = Unused;
  public IRenderResourceInfo? Info { get; set; }
  public bool TransientState { get; set; } = false;
  public string? Name { get; set; }
  public IReadOnlySet<int> WrittenInPasses => _writtenInPasses;
  public IReadOnlySet<int> ReadInPasses => _readInPasses;

  public void WrittenInPass(int index)
  {
    _writtenInPasses.Add(index);
  }
  public void ReadInPass(int index)
  {
    _readInPasses.Add(index);
  }
  public void SetFlag<TFlags>(TFlags flag)
    where TFlags : unmanaged, Enum
  {
    GetFlagObject<TFlags>().SetFlag(flag);
  }
  public void ClearFlag<TFlags>(TFlags flag)
    where TFlags : unmanaged, Enum
  {
    GetFlagObject<TFlags>().ClearFlag(flag);
  }
  public bool HasFlag<TFlags>(TFlags flag)
    where TFlags : unmanaged, Enum
  {
    return GetFlagObject<TFlags>().HasFlag(flag);
  }
  public bool SupportsFlag<TFlags>()
  {
    return _flagStorage.TryGetValue(typeof(TFlags), out _);
  }
  public TFlags GetFlag<TFlags>()
    where TFlags : unmanaged, Enum
  {
    return GetFlagObject<TFlags>().Flags;
  }
  private IFlaggable<TFlags> GetFlagObject<TFlags>()
    where TFlags : unmanaged, Enum
  {
    Type type = typeof(TFlags);

    if (_flagStorage.TryGetValue(type, out IFlaggable? entry))
      return (IFlaggable<TFlags>)entry;

    ResourceFlags<TFlags> newEntry = new();

    _flagStorage.Add(type, newEntry);
    return newEntry;
  }
}