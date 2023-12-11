namespace ValkyrEngine.Rendering.Resources;

public record AccessedTextureResource(RenderResource? Texture, ImageLayout Layout = ImageLayout.Undefined, AccessFlags2 Access = 0, PipelineStageFlags2 Stages = 0);