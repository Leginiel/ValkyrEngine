using Throw;
using ValkyrEngine.Rendering.Resources;

namespace ValkyrEngine.Rendering;

public class RenderPass(RenderGraph graph, int index, RenderGraphQueueFlags queue)
{
  private const RenderGraphQueueFlags ComputeQueues = RenderGraphQueueFlags.AsyncComputeBit | RenderGraphQueueFlags.ComputeBit;

  public RenderGraphQueueFlags Queue { get; } = queue;
  public RenderGraph Graph { get; } = graph;
  public int Index { get; } = index;
  public PooledList<RenderResource?> ColorOutputs { get; } = [];
  public PooledList<RenderResource?> ColorInputs { get; } = [];
  public PooledList<RenderResource?> ColorScaleInputs { get; } = [];
  public PooledList<RenderResource?> ResolveOutputs { get; } = [];
  public PooledList<RenderResource?> StorageTextureInputs { get; } = [];
  public PooledList<RenderResource?> StorageTextureOutputs { get; } = [];
  public PooledList<RenderResource?> BlitTextureInputs { get; } = [];
  public PooledList<RenderResource?> BlitTextureOutputs { get; } = [];
  public PooledList<RenderResource?> AttachmentsInputs { get; } = [];
  public PooledList<RenderResource?> HistoryInputs { get; } = [];
  public PooledList<RenderResource?> StorageOutputs { get; } = [];
  public PooledList<RenderResource?> StorageInputs { get; } = [];
  public PooledList<RenderResource?> TransferOutputs { get; } = [];
  public PooledList<AccessedTextureResource?> GenericTexture { get; } = [];
  public PooledList<AccessedBufferResource?> GenericBuffer { get; } = [];
  public List<AccessedProxyResource> ProxyInputs { get; } = [];
  public List<AccessedProxyResource> ProxyOutputs { get; } = [];
  public PooledList<AccessedExternalLockInterface> LockInterfaces { get; } = [];
  public RenderResource? DepthStencilInput { get; private set; }
  public RenderResource? DepthStencilOutput { get; private set; }
  public TryGetClearColorValueDelegate? GetClearColorValueFunc { get; set; }
  public TryGetClearDepthStencilDelegate? GetClearDepthStencilFunc { get; set; }
  public BuildRenderPassDelegate? BuildRenderPassAction { get; set; }

  public RenderPassInterface? RenderPassInterface { get; set; }
  public uint PhysicalPassIndex { get; set; }

  public string Name { get; set; }
  public bool IsNeeded => (RenderPassInterface?.NeedRenderPass) ?? true;
  public bool IsMultiview => (RenderPassInterface?.RenderPassIsSeperateLayered) ?? true;
  public bool IsConditional => (RenderPassInterface?.RenderPassIsConditional) ?? false;

  public void AddExternalLock(string name, PipelineStageFlags2 stages)
  {
    RenderPassExternalLockInterface iface = Graph.FindExternalLockInterface(name);

    if (iface is not null)
    {
      foreach (ref AccessedExternalLockInterface @lock in LockInterfaces.Span)
      {
        if (@lock.Iface.Equals(iface))
        {
          @lock.Stages |= stages;
          return;
        }
      }
      LockInterfaces.Add(new() { Iface = iface, Stages = stages });
    }
  }
  public RenderResource? SetDepthStencilInput(string name)
  {
    DepthStencilInput = PrepareInputResource(RenderResourceType.Texture, name, ImageUsageFlags.DepthStencilAttachmentBit);
    return DepthStencilInput;
  }
  public RenderResource? SetDepthStencilOutput(string name, AttachmentInfo info)
  {
    DepthStencilOutput = PrepareOutputResource(RenderResourceType.Texture, name, ImageUsageFlags.DepthStencilAttachmentBit, info);
    return DepthStencilOutput;
  }
  public RenderResource? AddColorOutput(string name, AttachmentInfo info, string input = "")
  {
    RenderResource? resource = PrepareOutputResource(RenderResourceType.Texture, name, ImageUsageFlags.ColorAttachmentBit, info, ColorOutputs);

    if (resource is not null)
    {
      PrepareInputResource(RenderResourceType.Texture, input, ImageUsageFlags.ColorAttachmentBit, ColorInputs);

      if (info.Levels != 1)
        resource.SetFlag(ImageUsageFlags.TransferDstBit | ImageUsageFlags.TransferSrcBit);

      ColorScaleInputs.Add(null);
    }

    return resource;
  }
  public RenderResource? AddResolveOutput(string name, AttachmentInfo info)
  {
    return PrepareOutputResource(RenderResourceType.Texture, name, ImageUsageFlags.ColorAttachmentBit, info, ResolveOutputs);
  }
  public RenderResource? AddAttachmentInput(string name)
  {
    return PrepareInputResource(RenderResourceType.Texture, name, ImageUsageFlags.InputAttachmentBit, AttachmentsInputs);
  }
  public RenderResource? AddHistoryInput(string name)
  {
    return PrepareInputResource(RenderResourceType.Texture, name, ImageUsageFlags.SampledBit, HistoryInputs);
  }
  public RenderResource? AddTextureInput(string name, PipelineStageFlags2 stages = 0)
  {
    RenderResource? resource = PrepareInputResource(RenderResourceType.Texture, name, ImageUsageFlags.SampledBit);
    AccessedTextureResource? texture = GenericTexture.FirstOrDefault((AccessedTextureResource acc) => acc.Texture == resource);

    if (texture is not null)
      return texture.Texture;

    if (resource is null)
      return resource;

    if (stages == 0)
      stages = Queue.HasFlag(ComputeQueues) ? PipelineStageFlags2.ComputeShaderBit : PipelineStageFlags2.FragmentShaderBit;

    GenericTexture.Add(new(resource, ImageLayout.ShaderReadOnlyOptimal, AccessFlags2.ShaderSampledReadBit, stages));
    return resource;
  }
  public RenderResource? AddBlitTextureReadOnlyInput(string name)
  {
    RenderResource? resource = PrepareInputResource(RenderResourceType.Texture, name, ImageUsageFlags.TransferSrcBit);

    if (resource is null)
      return resource;

    GenericTexture.Add(new(resource, ImageLayout.TransferSrcOptimal, AccessFlags2.TransferReadBit, PipelineStageFlags2.BlitBit));
    return resource;
  }
  public RenderResource? AddUniformInput(string name, PipelineStageFlags2 stages = 0)
  {
    if (stages == 0)
      stages = Queue.HasFlag(ComputeQueues) ? PipelineStageFlags2.ComputeShaderBit : PipelineStageFlags2.FragmentShaderBit;

    return AddGenericBufferInput(name, stages, AccessFlags2.UniformReadBit, BufferUsageFlags.UniformBufferBit);
  }
  public RenderResource? AddStorageReadOnlyInput(string name, PipelineStageFlags2 stages = 0)
  {
    if (stages == 0)
      stages = Queue.HasFlag(ComputeQueues) ? PipelineStageFlags2.ComputeShaderBit : PipelineStageFlags2.FragmentShaderBit;

    return AddGenericBufferInput(name, stages, AccessFlags2.ShaderStorageReadBit, BufferUsageFlags.StorageBufferBit);
  }
  public RenderResource? AddStorageOutput(string name, BufferInfo info, string input = "")
  {
    RenderResource? resource = PrepareOutputResource(RenderResourceType.Buffer, name, BufferUsageFlags.StorageBufferBit, info, StorageOutputs);

    if (resource is not null)
      PrepareInputResource(RenderResourceType.Buffer, input, BufferUsageFlags.StorageBufferBit, StorageInputs);
    return resource;
  }
  public RenderResource? AddTransferOutput(string name, BufferInfo info)
  {
    return PrepareOutputResource(RenderResourceType.Buffer, name, BufferUsageFlags.TransferDstBit, info, TransferOutputs);
  }
  public RenderResource? AddStorageTextureOutput(string name, AttachmentInfo info, string input = "")
  {
    RenderResource? resource = PrepareOutputResource(RenderResourceType.Texture, name, ImageUsageFlags.StorageBit, info, StorageTextureOutputs);

    if (resource is not null)
      PrepareInputResource(RenderResourceType.Texture, input, ImageUsageFlags.StorageBit, StorageTextureInputs);
    return resource;
  }
  public RenderResource? AddBlitTextureOutput(string name, AttachmentInfo info, string input = "")
  {
    RenderResource? resource = PrepareOutputResource(RenderResourceType.Texture, name, ImageUsageFlags.TransferDstBit, info, BlitTextureOutputs);

    if (resource is not null)
      PrepareInputResource(RenderResourceType.Texture, input, ImageUsageFlags.TransferDstBit, BlitTextureInputs);
    return resource;
  }
  public RenderResource? AddVertexBufferInput(string name)
  {
    return AddGenericBufferInput(name, PipelineStageFlags2.VertexInputBit, AccessFlags2.VertexAttributeReadBit, BufferUsageFlags.VertexBufferBit);
  }
  public RenderResource? AddIndexBufferInput(string name)
  {
    return AddGenericBufferInput(name, PipelineStageFlags2.VertexInputBit, AccessFlags2.IndexReadBit, BufferUsageFlags.IndexBufferBit);
  }
  public RenderResource? AddIndirectBufferInput(string name)
  {
    return AddGenericBufferInput(name, PipelineStageFlags2.DrawIndirectBit, AccessFlags2.IndirectCommandReadBit, BufferUsageFlags.IndirectBufferBit);
  }
  public void AddProxyOutput(string name, PipelineStageFlags2 stages)
  {
    RenderResource? resource = PrepareOutputResource(RenderResourceType.Proxy, name);

    if (resource is null)
      return;

    ProxyOutputs.Add(new(resource, stages, ImageLayout.General));
  }
  public void AddProxyInput(string name, PipelineStageFlags2 stages)
  {
    RenderResource? resource = PrepareInputResource(RenderResourceType.Proxy, name);
    ProxyInputs.Add(new(resource, stages, ImageLayout.General));
  }
  public void MakeColorInputScaled(int index)
  {
    (ColorInputs[index], ColorScaleInputs[index]) = (ColorScaleInputs[index], ColorInputs[index]);
  }
  public bool TryGetClearColor(uint index, out ClearColorValue? value)
  {
    value = null;

    if (RenderPassInterface is not null)
      return RenderPassInterface.TryGetClearColor(index, out value);

    if (GetClearColorValueFunc is not null)
      return GetClearColorValueFunc.Invoke(index, out value);

    return false;
  }
  public bool TryGetClearDepthStecil(out ClearDepthStencilValue? value)
  {
    value = null;

    if (RenderPassInterface is not null)
      return RenderPassInterface.TryGetClearDepthStecil(out value);

    if (GetClearDepthStencilFunc is not null)
      return GetClearDepthStencilFunc.Invoke(out value);

    return false;
  }
  public void PrepareRenderPass(TaskComposer composer)
  {
    RenderPassInterface?.EnqueuePrepareRenderPass(Graph, composer);
  }
  public void Setup(ref Device device)
  {
    RenderPassInterface?.Setup(ref device);
  }
  public void SetupDependencies()
  {
    RenderPassInterface?.SetupDependencies(this, Graph);
  }
  public void BuildRenderPass(ref CommandBuffer cmd, uint layer)
  {
    if (RenderPassInterface is not null)
    {
      if (!RenderPassInterface.RenderPassIsSeperateLayered)
        RenderPassInterface.BuildRenderPassSeparateLayer(ref cmd, layer);
      else
        RenderPassInterface.BuildRenderPass(ref cmd);
    }
    else
    {
      BuildRenderPassAction?.Invoke(ref cmd);
    }
  }
  public void Validate()
  {
    ColorInputs.Throw().IfCountNotEquals(ColorOutputs.Count);
    StorageInputs.Throw().IfCountNotEquals(StorageOutputs.Count);
    BlitTextureInputs.Throw().IfCountNotEquals(BlitTextureOutputs.Count);
    StorageTextureInputs.Throw().IfCountNotEquals(StorageTextureOutputs.Count);
    ResolveOutputs.Throw()
                  .IfCountNotEquals(0)
                  .IfCountNotEquals(ColorOutputs.Count);

    for (int i = 0; i < ColorInputs.Count; i++)
    {
      RenderResource? input = ColorInputs[i];
      if (input is null)
        continue;

      ResourceDimensions inputDimension = Graph.GetResourceDimensions(input);
      ResourceDimensions outputDimension = Graph.GetResourceDimensions(ColorOutputs[i]);

      if (inputDimension != outputDimension)
        MakeColorInputScaled(i);
    }

    ValidateObject(StorageInputs,
                   StorageOutputs,
                   (RenderResource? resource) => resource!.Info.ThrowIfNull(),
                   "Doing RMW on a storage buffer, but usage and sizes do not match.");
    ValidateObject(BlitTextureInputs,
                   BlitTextureOutputs,
                   Graph.GetResourceDimensions,
                   "Doing RMW on a blit image, but usage and sizes do not match.");
    ValidateObject(StorageTextureInputs,
                   StorageTextureOutputs,
                   Graph.GetResourceDimensions,
                   "Doing RMW on a storage texture image, but sizes do not match.");

    if (DepthStencilInput is not null && DepthStencilOutput is not null)
    {
      ResourceDimensions input = Graph.GetResourceDimensions(DepthStencilInput);
      ResourceDimensions output = Graph.GetResourceDimensions(DepthStencilOutput);

      input.Throw().IfNotEquals(output);
    }
  }
  private static void ValidateObject<T>(PooledList<RenderResource?> inputs, PooledList<RenderResource?> outputs, Func<RenderResource?, T> accessor, string message)
    where T : notnull
  {
    if (inputs.Count == 0)
      return;

    for (int i = 0; i < inputs.Count; i++)
    {
      if (inputs[i] is null)
        continue;

      T input = accessor.Invoke(inputs[i]);
      T output = accessor.Invoke(outputs[i]);

      input.Throw(message).IfNotEquals(output);
    }
  }
  private RenderResource? AddGenericBufferInput(string name, PipelineStageFlags2 stages, AccessFlags2 access, BufferUsageFlags usage)
  {
    RenderResource? resource = PrepareInputResource(RenderResourceType.Buffer, name, usage);

    if (resource is not null)
      GenericBuffer.Add(new(resource, stages, access, ImageLayout.General));
    return resource;
  }
  private RenderResource? PrepareInputResource<TFlags>(RenderResourceType type, string name, TFlags flags, params PooledList<RenderResource?>[] storages)
    where TFlags : unmanaged, Enum
  {
    RenderResource? resource = PrepareInputResource(type, name);

    resource?.SetFlag(flags);
    return resource;
  }
  private RenderResource? PrepareInputResource(RenderResourceType type, string name, params PooledList<RenderResource?>[] storages)
  {
    RenderResource? resource = PrepareResource(type, name, isRead: true);

    if (resource is not null)
    {
      foreach (PooledList<RenderResource?> storage in storages)
        storage.Add(resource);
    }

    return resource;
  }
  private RenderResource? PrepareOutputResource<TFlags, TInfo>(RenderResourceType type,
                                                               string name,
                                                               TFlags flags,
                                                               TInfo info,
                                                               params PooledList<RenderResource?>[] storages)
    where TInfo : class, IRenderResourceInfo
    where TFlags : unmanaged, Enum
  {
    RenderResource? resource = PrepareOutputResource(type, name, storages);

    if (resource is not null)
    {
      resource.SetFlag(flags);
      resource.Info = info;
    }

    return resource;
  }
  private RenderResource? PrepareOutputResource(RenderResourceType type, string name, params PooledList<RenderResource?>[] storages)
  {
    RenderResource? resource = PrepareResource(type, name, isWritten: true);

    if (resource is not null)
    {
      foreach (PooledList<RenderResource?> storage in storages)
        storage.Add(resource);
    }

    return resource;
  }
  private RenderResource? PrepareResource(RenderResourceType type, string name, bool isRead = false, bool isWritten = false)
  {
    if (!string.IsNullOrEmpty(name))
      return null;

    RenderResource resource = Graph.GetResource(type, name);
    resource.SetFlag(Queue);

    if (isRead)
      resource.ReadInPass(Index);

    if (isWritten)
      resource.WrittenInPass(Index);

    return resource;
  }
}