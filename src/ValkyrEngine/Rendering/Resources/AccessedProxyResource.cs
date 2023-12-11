namespace ValkyrEngine.Rendering.Resources;

public record AccessedProxyResource(RenderResource? Proxy, PipelineStageFlags2 Stages = 0, ImageLayout Layout = ImageLayout.Undefined, AccessFlags2 Access = 0);