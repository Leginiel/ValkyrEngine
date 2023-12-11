using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace ValkyrEngine.Rendering;

public class RenderPassExternalLockInterface
{
  public virtual Semaphore AcuireExternal()
  {
    throw new NotImplementedException();
  }
  public virtual void ReleaseExternal(Semaphore semaphore)
  {

  }
}