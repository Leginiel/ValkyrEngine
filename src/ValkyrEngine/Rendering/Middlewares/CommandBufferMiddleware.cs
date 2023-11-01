using Silk.NET.Vulkan;

namespace ValkyrEngine.Rendering.Middlewares;

internal unsafe class CommandBufferMiddleware : IRenderMiddleware
{
  public static bool Recreatable { get; } = true;

  public static void Init(RenderingContext context)
  {
    Vk vk = context.Vk!;
    Device device = context.Device.GetValueOrDefault();
    CommandPool commandPool = context.CommandPool.GetValueOrDefault();
    Pipeline graphicsPipeline = context.GraphicsPipeline.GetValueOrDefault();
    RenderPass renderPass = context.RenderPass.GetValueOrDefault();
    Extent2D swapchainExtent = context.SwapchainExtent.GetValueOrDefault();
    Framebuffer[] swapchainFramebuffer = context.SwapchainFramebuffer!;
    CommandBuffer[] commandBuffer = new CommandBuffer[swapchainFramebuffer.Length];

    CommandBufferAllocateInfo allocInfo = new()
    {
      SType = StructureType.CommandBufferAllocateInfo,
      CommandPool = commandPool,
      Level = CommandBufferLevel.Primary,
      CommandBufferCount = (uint)commandBuffer.Length,
    };

    fixed (CommandBuffer* commandBuffersPtr = commandBuffer)
    {
      if (vk.AllocateCommandBuffers(device, allocInfo, commandBuffersPtr) != Result.Success)
      {
        throw new Exception("failed to allocate command buffers!");
      }
    }

    for (int i = 0; i < commandBuffer.Length; i++)
    {
      CommandBufferBeginInfo beginInfo = new()
      {
        SType = StructureType.CommandBufferBeginInfo,
      };

      if (vk.BeginCommandBuffer(commandBuffer[i], beginInfo) != Result.Success)
      {
        throw new Exception("failed to begin recording command buffer!");
      }

      RenderPassBeginInfo renderPassInfo = new()
      {
        SType = StructureType.RenderPassBeginInfo,
        RenderPass = renderPass,
        Framebuffer = swapchainFramebuffer[i],
        RenderArea =
          {
            Offset = { X = 0, Y = 0 },
            Extent = swapchainExtent,
          }
      };

      ClearValue clearColor = new()
      {
        Color = new() { Float32_0 = 0, Float32_1 = 0, Float32_2 = 0, Float32_3 = 1 },
      };

      renderPassInfo.ClearValueCount = 1;
      renderPassInfo.PClearValues = &clearColor;

      vk.CmdBeginRenderPass(commandBuffer[i], &renderPassInfo, SubpassContents.Inline);
      vk.CmdBindPipeline(commandBuffer[i], PipelineBindPoint.Graphics, graphicsPipeline);
      vk.CmdDraw(commandBuffer[i], 3, 1, 0, 0);
      vk.CmdEndRenderPass(commandBuffer[i]);

      if (vk!.EndCommandBuffer(commandBuffer[i]) != Result.Success)
      {
        throw new Exception("failed to record command buffer!");
      }

      context.CommandBuffer = commandBuffer;
    }
  }
  public static void CleanUp(RenderingContext context)
  {
    context.CommandBuffer = null;
  }
}
