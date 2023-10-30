namespace ValkyrEngine.Rendering;

internal interface IRenderMiddleware
{
  void Init(RenderingContext context, ValkyrEngineOptions options);
  void CleanUp(RenderingContext context);
}