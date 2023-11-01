namespace ValkyrEngine.Rendering;

internal interface IRenderMiddleware
{
  static virtual bool Recreatable { get; } = false;
  static abstract void Init(RenderingContext context);
  static abstract void CleanUp(RenderingContext context);
}