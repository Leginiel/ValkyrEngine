namespace ValkyrEngine.Rendering.Resources;

public record AccessedBufferResource(RenderResource Buffer, PipelineStageFlags2 Stages = 0, AccessFlags2 Access = 0, ImageLayout Layout = ImageLayout.Undefined);