using Silk.NET.Windowing;

namespace ValkyrEngine;

public struct ValkyrEngineOptions
{
  public WindowOptions WindowOptions;
  public string ApplicationName;
  public bool ActivateValidationLayers;

  public byte[] VertexShaderCode;
  public byte[] FragmentShaderCode;
}