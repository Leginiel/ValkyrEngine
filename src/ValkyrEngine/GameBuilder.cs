using ValkyrEngine.Rendering;

namespace ValkyrEngine;

internal class GameBuilder : IGameBuilder
{
  private readonly List<IRenderMiddleware> _middlewares = [];
  public RenderingContext Build(ValkyrEngineOptions options)
  {
    RenderingContext context = new();

    foreach (IRenderMiddleware middleware in _middlewares)
    {
      middleware.Init(context, options);
      context.AddCleanUpJob(middleware.CleanUp);
    }

    return context;
  }

  public IGameBuilder Use<T>()
    where T : IRenderMiddleware, new()
  {
    _middlewares.Add(new T());

    return this;
  }
}