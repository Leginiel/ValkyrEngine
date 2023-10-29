using Silk.NET.Windowing;

namespace ValkyrEngine;

public class Game : IDisposable
{
  private bool _disposedValue;
  private IWindow? _window;


  public void Init(WindowOptions windowOptions)
  {
    InitWindow(windowOptions);
    InitVulkan();
  }
  public void Run()
  {
    if (_window is null)
      return;

    _window.Run();
  }

  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _window?.Dispose();
      }
      _disposedValue = true;
    }
  }
  private void InitWindow(WindowOptions windowOptions)
  {
    _window = Window.Create(windowOptions);
    _window.Initialize();
  }
  private void InitVulkan()
  {
  }
}
