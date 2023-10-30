namespace ValkyrEngine.Rendering;

internal sealed class RenderingContext : IDisposable
{
  private bool disposedValue;
  private readonly Stack<Action<RenderingContext>> _cleanUpJobs = new();

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  public void AddCleanUpJob(Action<RenderingContext> job)
  {
    _cleanUpJobs.Push(job);
  }
  private void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        while (_cleanUpJobs.TryPop(out Action<RenderingContext>? job))
        {
          job.Invoke(this);
        }
      }
      disposedValue = true;
    }
  }
}