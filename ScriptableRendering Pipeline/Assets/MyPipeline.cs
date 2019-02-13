using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using Conditional = System.Diagnostics.ConditionalAttribute;

public class MyPipeline : RenderPipeline
{
    //protected override IRenderPipeline InternalCreatePipeline()
    //{
    //    return new MyPipeline();
    //}

    public MyPipeline()
    {
        m_shaderPassNames["ForwardBase"] = new ShaderPassName("ForwardBase");
        m_shaderPassNames["PrepassBase"] = new ShaderPassName("PrepassBase");
        m_shaderPassNames["Always"] = new ShaderPassName("Always");
        m_shaderPassNames["Vertex"] = new ShaderPassName("Vertex");
        m_shaderPassNames["VertexLMRGBM"] = new ShaderPassName("VertexLMRGBM");
        m_shaderPassNames["VertexLM"] = new ShaderPassName("VertexLM");

        if(m_errorMaterial == null)
        {
            Shader errorShader = Shader.Find("Hidden/InternalErrorShader");
            m_errorMaterial = new Material(errorShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }

    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        base.Render(renderContext, cameras);

        foreach (var camera in cameras)
        {
            Render(renderContext, camera);
        }
    }

    void Render(ScriptableRenderContext context, Camera camera)
    {
        ScriptableCullingParameters cullingParameters;
        if (!CullResults.GetCullingParameters(camera, out cullingParameters))
        {
            return;
        }

#if UNITY_EDITOR
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
#endif

        CullResults.Cull(ref cullingParameters, context, ref m_cull);

        context.SetupCameraProperties(camera);

        CameraClearFlags clearFlags = camera.clearFlags;
        m_cameraBuffer.ClearRenderTarget(
            (clearFlags & CameraClearFlags.Depth) != 0,
            (clearFlags & CameraClearFlags.Color) != 0,
            camera.backgroundColor
        );
        //cameraBuffer.ClearRenderTarget(true, false, Color.clear);
        m_cameraBuffer.BeginSample("Render Camera");
        context.ExecuteCommandBuffer(m_cameraBuffer);
        m_cameraBuffer.Clear();

        var drawSettings = new DrawRendererSettings(
            camera, new ShaderPassName("SRPDefaultUnlit")
        );
        drawSettings.sorting.flags = SortFlags.CommonOpaque;

        var filterSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.opaque
        };

        context.DrawRenderers(
            m_cull.visibleRenderers, ref drawSettings, filterSettings
        );

        context.DrawSkybox(camera);

        drawSettings.sorting.flags = SortFlags.CommonTransparent;
        filterSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(
            m_cull.visibleRenderers, ref drawSettings, filterSettings
        );

        DrawDefaultPipeline(context, camera);

        m_cameraBuffer.EndSample("Render Camera");
        context.ExecuteCommandBuffer(m_cameraBuffer);
        m_cameraBuffer.Clear();

        context.Submit();
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    private void DrawDefaultPipeline(ScriptableRenderContext context, Camera camera)
    {
        var drawSettings = new DrawRendererSettings(camera, new ShaderPassName("ForwardBase"));
        drawSettings.SetShaderPassName(1, new ShaderPassName("PrepassBase"));
        drawSettings.SetShaderPassName(2, new ShaderPassName("Always"));
        drawSettings.SetShaderPassName(3, new ShaderPassName("Vertex"));
        drawSettings.SetShaderPassName(4, new ShaderPassName("VertexLMRGBM"));
        drawSettings.SetShaderPassName(5, new ShaderPassName("VertexLM"));
        drawSettings.SetOverrideMaterial(m_errorMaterial, 0);

        var filterSettings = new FilterRenderersSettings(true);

        context.DrawRenderers(m_cull.visibleRenderers, ref drawSettings, filterSettings);
    }

    CullResults m_cull;
    CommandBuffer m_cameraBuffer = new CommandBuffer { name = "Render Camera" };
    private Dictionary<string, ShaderPassName> m_shaderPassNames = new Dictionary<string, ShaderPassName>();
    Material m_errorMaterial;
}
