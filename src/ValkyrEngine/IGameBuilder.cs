using ValkyrEngine.Rendering;

namespace ValkyrEngine;

internal interface IGameBuilder
{
  IGameBuilder Use<T>()
    where T : IRenderMiddleware, new();
  RenderingContext Build(ValkyrEngineOptions options);
}