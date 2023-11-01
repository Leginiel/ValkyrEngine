using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class GraphicPipelineMiddleware : IRenderMiddleware
{
  public static bool Recreatable { get; } = true;

  public unsafe static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    ValkyrEngineOptions options = context.Options;
    Device device = context.Device.GetValueOrDefault();
    RenderPass renderPass = context.RenderPass.GetValueOrDefault();
    Extent2D swapchainExtent = context.SwapchainExtent.GetValueOrDefault();
    ShaderModule vertShaderModule = CreateShaderModule(vk, device, options.VertexShaderCode);
    ShaderModule fragShaderModule = CreateShaderModule(vk, device, options.FragmentShaderCode);
    PipelineShaderStageCreateInfo vertShaderStageInfo = new()
    {
      SType = StructureType.PipelineShaderStageCreateInfo,
      Stage = ShaderStageFlags.VertexBit,
      Module = vertShaderModule,
      PName = (byte*)SilkMarshal.StringToPtr("main")
    };
    PipelineShaderStageCreateInfo fragShaderStageInfo = new()
    {
      SType = StructureType.PipelineShaderStageCreateInfo,
      Stage = ShaderStageFlags.FragmentBit,
      Module = fragShaderModule,
      PName = (byte*)SilkMarshal.StringToPtr("main")
    };

    PipelineShaderStageCreateInfo* shaderStages = stackalloc[]
    {
      vertShaderStageInfo,
      fragShaderStageInfo
    };
    PipelineVertexInputStateCreateInfo vertexInputInfo = new()
    {
      SType = StructureType.PipelineVertexInputStateCreateInfo,
      VertexBindingDescriptionCount = 0,
      VertexAttributeDescriptionCount = 0,
    };
    PipelineInputAssemblyStateCreateInfo inputAssembly = new()
    {
      SType = StructureType.PipelineInputAssemblyStateCreateInfo,
      Topology = PrimitiveTopology.TriangleList,
      PrimitiveRestartEnable = false,
    };
    Viewport viewport = new()
    {
      X = 0,
      Y = 0,
      Width = swapchainExtent.Width,
      Height = swapchainExtent.Height,
      MinDepth = 0,
      MaxDepth = 1,
    };
    Rect2D scissor = new()
    {
      Offset = { X = 0, Y = 0 },
      Extent = swapchainExtent,
    };
    PipelineViewportStateCreateInfo viewportState = new()
    {
      SType = StructureType.PipelineViewportStateCreateInfo,
      ViewportCount = 1,
      PViewports = &viewport,
      ScissorCount = 1,
      PScissors = &scissor,
    };
    PipelineRasterizationStateCreateInfo rasterizer = new()
    {
      SType = StructureType.PipelineRasterizationStateCreateInfo,
      DepthClampEnable = false,
      RasterizerDiscardEnable = false,
      PolygonMode = PolygonMode.Fill,
      LineWidth = 1,
      CullMode = CullModeFlags.BackBit,
      FrontFace = FrontFace.Clockwise,
      DepthBiasEnable = false,
    };
    PipelineMultisampleStateCreateInfo multisampling = new()
    {
      SType = StructureType.PipelineMultisampleStateCreateInfo,
      SampleShadingEnable = false,
      RasterizationSamples = SampleCountFlags.Count1Bit,
    };
    PipelineColorBlendAttachmentState colorBlendAttachment = new()
    {
      ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
      BlendEnable = false,
    };
    PipelineColorBlendStateCreateInfo colorBlending = new()
    {
      SType = StructureType.PipelineColorBlendStateCreateInfo,
      LogicOpEnable = false,
      LogicOp = LogicOp.Copy,
      AttachmentCount = 1,
      PAttachments = &colorBlendAttachment,
    };
    colorBlending.BlendConstants[0] = 0;
    colorBlending.BlendConstants[1] = 0;
    colorBlending.BlendConstants[2] = 0;
    colorBlending.BlendConstants[3] = 0;

    PipelineLayoutCreateInfo pipelineLayoutInfo = new()
    {
      SType = StructureType.PipelineLayoutCreateInfo,
      SetLayoutCount = 0,
      PushConstantRangeCount = 0,
    };

    if (vk.CreatePipelineLayout(device, pipelineLayoutInfo, null, out PipelineLayout pipelineLayout) != Result.Success)
    {
      throw new Exception("failed to create pipeline layout!");
    }

    GraphicsPipelineCreateInfo pipelineInfo = new()
    {
      SType = StructureType.GraphicsPipelineCreateInfo,
      StageCount = 2,
      PStages = shaderStages,
      PVertexInputState = &vertexInputInfo,
      PInputAssemblyState = &inputAssembly,
      PViewportState = &viewportState,
      PRasterizationState = &rasterizer,
      PMultisampleState = &multisampling,
      PColorBlendState = &colorBlending,
      Layout = pipelineLayout,
      RenderPass = renderPass,
      Subpass = 0,
      BasePipelineHandle = default
    };

    if (vk.CreateGraphicsPipelines(device, default, 1, pipelineInfo, null, out Pipeline graphicsPipeline) != Result.Success)
    {
      throw new Exception("failed to create graphics pipeline!");
    }
    vk.DestroyShaderModule(device, fragShaderModule, null);
    vk.DestroyShaderModule(device, vertShaderModule, null);
    SilkMarshal.Free((nint)vertShaderStageInfo.PName);
    SilkMarshal.Free((nint)fragShaderStageInfo.PName);

    context.PipelineLayout = pipelineLayout;
    context.GraphicsPipeline = graphicsPipeline;
  }

  public static void CleanUp(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();

    vk.DestroyPipeline(device, context.GraphicsPipeline.GetValueOrDefault(), null);
    vk.DestroyPipelineLayout(device, context.PipelineLayout.GetValueOrDefault(), null);

    context.PipelineLayout = null;
    context.GraphicsPipeline = null;
  }

  private static ShaderModule CreateShaderModule(Vk vk, Device device, byte[] code)
  {
    ShaderModule shaderModule;
    ShaderModuleCreateInfo createInfo = new()
    {
      SType = StructureType.ShaderModuleCreateInfo,
      CodeSize = (nuint)code.Length,
    };

    fixed (byte* codePtr = code)
    {
      createInfo.PCode = (uint*)codePtr;

      if (vk.CreateShaderModule(device, createInfo, null, out shaderModule) != Result.Success)
      {
        throw new Exception();
      }
    }

    return shaderModule;
  }
}
