using ValkyrEngine;
using Silk.NET.Windowing;
using Silk.NET.Maths;

namespace TestApp;

internal class Program
{
  private static void Main(string[] args)
  {
    WindowOptions windowOptions = WindowOptions.DefaultVulkan with
    {
      Size = new Vector2D<int>(800, 600),
      Title = "TestApp"
    };
    ValkyrEngineOptions options = new()
    {
      WindowOptions = windowOptions,
      ApplicationName = "TestApp",
      VertexShaderCode = File.ReadAllBytes("bin/Debug/net8.0/shaders/vert.spv"),
      FragmentShaderCode = File.ReadAllBytes("bin/Debug/net8.0/shaders/frag.spv")
    };
    Game game = new();

    game.Init(options);
    game.Run();
  }
}