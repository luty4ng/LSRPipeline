using UnityEngine;
using UnityEngine.Rendering;

public partial class LSRPCamera
{
    private const string bufferName = "CameraRender Event";
    private CommandBuffer buffer = new CommandBuffer() { name = bufferName };
    private ScriptableRenderContext m_CachedContext;
    private Camera m_CachedCamera;
    private CullingResults m_CachedCullingResults;
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
    {
        m_CachedContext = context;
        m_CachedCamera = camera;


        if (m_CachedCamera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
            m_CachedCullingResults = m_CachedContext.Cull(ref cullingParameters);
        else
            return;

        m_CachedContext.SetupCameraProperties(m_CachedCamera);
        buffer.ClearRenderTarget(clearDepth: true, clearColor: true, backgroundColor: Color.clear);

        // FrameDebugger采样
        buffer.BeginSample(bufferName);
        // buffer只有在context Execute之后才会生效
        m_CachedContext.ExecuteCommandBuffer(buffer);
        buffer.Clear();

        DrawVisibleGeometry();
        m_CachedContext.DrawSkybox(m_CachedCamera);

        buffer.EndSample(bufferName);
        m_CachedContext.ExecuteCommandBuffer(buffer);
        buffer.Clear();

        m_CachedContext.Submit();
    }

    private void DrawVisibleGeometry()
    {
        // Filter settings for ScriptableRenderContext.DrawRenderers.
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
        // This struct describes the methods to sort objects during rendering.
        SortingSettings sortingSettings = new SortingSettings();
        //  Settings for ScriptableRenderContext.DrawRenderers.
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        m_CachedContext.DrawRenderers(m_CachedCullingResults, ref drawingSettings, ref filteringSettings);
    }
}