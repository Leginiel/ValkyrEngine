using System.Runtime.CompilerServices;
using Throw;
using ValkyrEngine.Rendering.Resources;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace ValkyrEngine.Rendering;

public class RenderGraph
{
  private readonly Dictionary<string, int> _resourceToIndex = [];
  private readonly Dictionary<string, int> _renderPassesToIndex = [];
  private readonly List<RenderResource> _resources = [];
  private readonly List<RenderPass> _renderPasses = [];

  private Dictionary<string, RenderPassExternalLockInterface> _externalLockInterfaces;
  private List<ResourceDimensions> _physicalDimensions;
  private List<ImageView> _physicalAttachments;
  private List<Buffer> _physicalBuffers;
  private List<Image> _physicalImageAttachments;
  private List<Image> _physicalHistoryImageAttachments;
  private List<PipelineEvent> _physicalEvents;
  private List<PipelineEvent> _physicalHistoryEvents;
  private List<bool> _physicalItemHasHistory;
  private List<int> _physicalAliases;
  private List<Barriers> _passBarriers;
  private ImageView _swapchainAttachment = Unsafe.NullRef<ImageView>();
  private ResourceDimensions _swapchainDimensions;
  private int _swapchainPhysicalIndex;
  private List<PhysicalPass> _physicalPasses;
  private List<HashSet<int>> _passDependecies;
  private List<HashSet<int>> _passMergeDependencies;
  private List<PassSubmissionState> _passSumissionState;

  public Device Device { get; set; }
  public ResourceDimensions BackBufferDimensions { get; set; }
  public bool EnableTimestamps { get; set; } = false;
  public string BackBufferSource { get; set; }

  public void AddExternaLockInterface(string name, RenderPassExternalLockInterface @interface)
  {
    _externalLockInterfaces.Add(name, @interface);
  }
  public RenderPassExternalLockInterface? FindExternalLockInterface(string name)
  {
    if (_externalLockInterfaces.TryGetValue(name, out RenderPassExternalLockInterface? result))
      return result;

    return null;
  }

  public RenderPass AddRenderPass(string name, RenderGraphQueueFlags queue)
  {
    if (!_renderPassesToIndex.TryGetValue(name, out int index))
    {
      index = _renderPasses.Count;
      _renderPasses.Add(new RenderPass(this, index, queue)
      {
        Name = name
      });
      _resourceToIndex.Add(name, index);
    }

    return _renderPasses[index];
  }

  public RenderPass? FindRenderPass(string name)
  {
    if (_renderPassesToIndex.TryGetValue(name, out int index))
      return _renderPasses[index];

    return null;
  }
  public ResourceDimensions GetResourceDimensions(RenderResource? resource)
  {
    throw new NotImplementedException();
  }

  public void Bake() { }

  public void Reset()
  {
    _renderPasses.Clear();
    _resources.Clear();
    _renderPassesToIndex.Clear();
    _resourceToIndex.Clear();
    _externalLockInterfaces.Clear();
    _physicalPasses.Clear();
    _physicalDimensions.Clear();
    _physicalAttachments.Clear();
    _physicalBuffers.Clear();
    _physicalImageAttachments.Clear();
    _physicalEvents.Clear();
    _physicalHistoryEvents.Clear();
    _physicalHistoryImageAttachments.Clear();
  }
  public void Log() { }
  public void SetupAttachments(Device device, ImageView imageView)
  {
    int capacity = _physicalDimensions.Count;

    _physicalAttachments.Clear();
    _physicalAttachments.Capacity = capacity;
    _physicalBuffers.Capacity = capacity;
    _physicalImageAttachments.Capacity = capacity;
    _physicalHistoryImageAttachments.Capacity = capacity;
    _physicalEvents.Capacity = capacity;
    _physicalHistoryEvents.Capacity = capacity;

    //_swapchainAttachment = swapchain;
    for (int i = 0; i < capacity; i++)
    {
      if (_physicalItemHasHistory[i])
      {
        (_physicalHistoryImageAttachments[i], _physicalImageAttachments[i]) = (_physicalImageAttachments[i], _physicalHistoryImageAttachments[i]);
        (_physicalHistoryEvents[i], _physicalEvents[i]) = (_physicalEvents[i], _physicalHistoryEvents[i]);
      }

      ResourceDimensions dimension = 
    }
  }

  public void EnqueueRenderPasses(ref Device device, TaskComposer composer)
  {

  }
  public ImageView GetPhysicalTextureResource(int index)
  {
    return _physicalAttachments[index];
  }
  public ImageView GetPhysicalTextureResource(ref RenderTextureResource resource)
  {
    return GetPhysicalTextureResource(resource.PhysicalIndex);
  }
  public bool TryGetPhysicalTextureResource(ref RenderTextureResource resource, out ImageView result)
  {
    bool isUsed = resource.PhysicalIndex != RenderResource.Unused;

    result = Unsafe.NullRef<ImageView>();
    if (isUsed)
      result = GetPhysicalTextureResource(resource.PhysicalIndex);
    return isUsed;
  }
  public ImageView GetPhysicalHistoryTextureResource(int index)
  {
    return _physicalHistoryImageAttachments[index].View;
  }
  public ImageView GetPhysicalHistoryTextureResource(RenderResource resource)
  {
    return GetPhysicalHistoryTextureResource(resource.PhysicalIndex);
  }
  public Buffer GetPhysicalBufferResource(int index)
  {
    return _physicalBuffers[index];
  }
  public Buffer GetPhysicalBufferResource(ref RenderBufferResource resource)
  {
    return GetPhysicalBufferResource(resource.PhysicalIndex);
  }
  public bool TryGetPhysicalBufferResource(ref RenderBufferResource resource, out Buffer result)
  {
    bool isUsed = resource.PhysicalIndex != RenderResource.Unused;

    result = Unsafe.NullRef<Buffer>();
    if (isUsed)
      result = GetPhysicalBufferResource(resource.PhysicalIndex);
    return isUsed;
  }
  public Buffer ConsumePersistentPhysicalBufferResource(int index)
  {
    throw new NotImplementedException();
  }
  public void InstallPersistentPhysicalBufferResource(int index, Buffer buffer)
  {
  }
  public List<Buffer> ConsumePersistentPhysicalBufferResource()
  {
    throw new NotImplementedException();
  }
  public void InstallPersistentPhysicalBufferResource(List<Buffer> buffer)
  {

  }

  private void FilterPasses(List<int> list)
  {

  }
  private void ValidatePasses()
  {
    foreach (RenderPass renderPass in _renderPasses)
      renderPass.Validate();
  }
  private void BuildBarriers()
  {

  }

  private void BuildPhyicalPasses()
  {

  }
  private void BuildTransients()
  {

  }
  private void BuildPhysicalResources()
  {
    int physicalIndex = 0;

    foreach (RenderPass renderPass in _renderPasses)
    {
      BuildPhysicalDimensions(ref physicalIndex, renderPass.GenericTexture, resourceAccess: (AccessedTextureResource input) => input.Texture!);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.GenericBuffer, resourceAccess: (AccessedBufferResource input) => input.Buffer!);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.ColorScaleInputs, action: (ResourceDimensions dimension) => dimension.ImageUsage |= ImageUsageFlags.SampledBit);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.ColorInputs, renderPass.ColorOutputs);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.StorageInputs, renderPass.StorageOutputs);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.BlitTextureInputs, renderPass.BlitTextureOutputs);
      BuildPhysicalDimensions(ref physicalIndex, renderPass.StorageTextureInputs, renderPass.StorageTextureOutputs);
    }

  }
  private void BuildPhysicalDimensions<T>(ref int physicalIndex,
                                          PooledList<T?> inputs,
                                          PooledList<T?>? outputs = null,
                                          Func<T, RenderResource>? resourceAccess = null,
                                          Action<ResourceDimensions>? action = null)
    where T : class
  {
    for (int i = 0; i < inputs.Count; i++)
    {
      T? input = inputs[i];
      if (input is null)
        continue;

      RenderResource? resource = (resourceAccess is not null) ? resourceAccess.Invoke(input) : input as RenderResource;

      if (resource is not null)
      {
        BuildPhysicalDimension(physicalIndex++, resource, action);

        if (outputs is not null && outputs[i] is RenderResource outputResource)
        {
          outputResource.PhysicalIndex = (outputResource.PhysicalIndex == RenderResource.Unused)
                                          ? resource.PhysicalIndex
                                          : outputResource.PhysicalIndex.Throw("Cannot alias resources. Index already claimed.").IfNotEquals(resource.PhysicalIndex);
        }
      }
    }
  }
  private void BuildPhysicalDimension(int physicalIndex, RenderResource resource, Action<ResourceDimensions>? action)
  {
    ResourceDimensions dimension;

    if (resource.PhysicalIndex == RenderResource.Unused)
    {
      dimension = GetResourceDimensions(resource);
      _physicalDimensions.Add(dimension);
      resource.PhysicalIndex = physicalIndex++;
      action?.Invoke(dimension);
      return;
    }

    dimension = _physicalDimensions[resource.PhysicalIndex];
    dimension.Queues |= resource.GetFlag<RenderGraphQueueFlags>();
    if (resource.SupportsFlag<ImageUsageFlags>())
      dimension.ImageUsage |= resource.GetFlag<ImageUsageFlags>();

    action?.Invoke(dimension);
  }
  private void BuildPhysicalBarriers()
  {

  }
  private void BuildRenderPassInfo()
  {

  }
  private void BuildAliases()
  {

  }

  private void EnqueueScaledRequests(ref CommandBuffer cmd, List<ScaledClearRequests> requests)
  {

  }
  private void EnqueueMipmapRequests(ref CommandBuffer cmd, List<MipmapRequests> requests)
  {
    if (requests.Count == 0)
      return;

    foreach (MipmapRequests request in requests)
    {
      Image image = _physicalAttachments[request.PhysicalResource].Image;

    }
  }
  // void on_swapchain_changed(const Vulkan::SwapchainParameterEvent &e);
  // void on_swapchain_destroyed(const Vulkan::SwapchainParameterEvent &e);
  // void on_device_created(const Vulkan::DeviceCreatedEvent &e);
  // void on_device_destroyed(const Vulkan::DeviceCreatedEvent &e);

  private void SetupPhysicalBuffer(ref Device device, int attachment)
  {

  }
  private void SetupPhysicalImage(ref Device device, int attachment)
  {

  }
  private void DependPassesRecursive(RenderPass pass, HashSet<int> passes, int stackCount, bool noCheck, bool ignoreSelf, bool mergeDependency)
  {

  }
  private void TraverseDependencies(RenderPass pass, int stackCount)
  {

  }
  private bool DependsOnPass(int destination, int source)
  {
    throw new NotImplementedException();
  }
  private void ReorderPasses(List<int> passes)
  {

  }
  private bool NeedInvalidate(Barrier barrier, PipelineEvent pipelineEvent)
  {
    throw new NotImplementedException();
  }
  private void EnqueueRenderPass(ref Device device, PhysicalPass physicalPass, PassSubmissionState state, TaskComposer composer)
  {

  }
  private void EnqueueSwapchainScalePass(ref Device device)
  {

  }
  private bool PhysicalPassRequiresWork(PhysicalPass pass)
  {
    throw new NotImplementedException();
  }
  private void PhyiscalPassTransferOwnership(PhysicalPass pass)
  {

  }
  private void PhysicalPassInvalidateAttachments(PhysicalPass pass)
  {

  }
  private void PhysicalPassEnqueueGraphicCommands(PhysicalPass pass, PassSubmissionState state)
  {

  }
  private void PhysicalPassEnqueueComputeCommands(PhysicalPass pass, PassSubmissionState state)
  {

  }
  private void PhysicalPassHandleInvalidateBarrier(Barrier barrier, PassSubmissionState state, bool physicalGraphicsQueue)
  {

  }
  private void PhysicalPassHandleExternalAcquire(PhysicalPass pass, PassSubmissionState state)
  {

  }
  private void PhysicalPassHandleSignal(ref Device device, PhysicalPass pass, PassSubmissionState state)
  {

  }
  private void PhysicalPassFlushBarrier(Barrier barrier, PassSubmissionState state)
  {

  }
  private void PhysicalPassHandleCPUTimeline(ref Device device, PhysicalPass pass, PassSubmissionState state, TaskComposer composer)
  {

  }
  private void PhysicalPassHandleGPUTimeline(ref Device device, PhysicalPass pass, PassSubmissionState state) //ThreadGroup group
  {

  }

  public RenderResource GetResource(RenderResourceType type, string name)
  {
    if (!_resourceToIndex.TryGetValue(name, out int index))
    {
      index = _resources.Count;
      _resources.Add(new(type, index)
      {
        Name = name
      });
      _resourceToIndex.Add(name, index);
    }

    return _resources[index];
  }

  // static inline RenderGraphQueueFlagBits get_default_post_graphics_queue()
  // {
  //   if (Vulkan::ImplementationQuirks::get().use_async_compute_post &&
  //       !Vulkan::ImplementationQuirks::get().render_graph_force_single_queue)
  //   {
  //     return RENDER_GRAPH_QUEUE_ASYNC_GRAPHICS_BIT;
  //   }
  //   else
  //   {
  //     return RENDER_GRAPH_QUEUE_GRAPHICS_BIT;
  //   }
  // }

  // static inline RenderGraphQueueFlagBits get_default_compute_queue()
  // {
  //   if (Vulkan::ImplementationQuirks::get().render_graph_force_single_queue)
  //     return RENDER_GRAPH_QUEUE_COMPUTE_BIT;
  //   else
  //     return RENDER_GRAPH_QUEUE_ASYNC_COMPUTE_BIT;
  // }

}

