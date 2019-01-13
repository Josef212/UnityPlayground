using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class MyPipeline : RenderPipeline
{
    //protected override IRenderPipeline InternalCreatePipeline()
    //{
    //    return new MyPipeline();
    //}

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
        if(!CullResults.GetCullingParameters(camera, out cullingParameters))
        {
            return;
        }

        CullResults.Cull(ref cullingParameters, context, ref cull);

        context.SetupCameraProperties(camera);
        
        CameraClearFlags clearFlags = camera.clearFlags;
        cameraBuffer.ClearRenderTarget((clearFlags & CameraClearFlags.Depth) != 0, (clearFlags & CameraClearFlags.Color) != 0, camera.backgroundColor);
        cameraBuffer.BeginSample("Render Camera");
        context.ExecuteCommandBuffer(cameraBuffer);
        cameraBuffer.Clear();

        var drawSettings = new DrawRendererSettings(camera, new ShaderPassName("SRPDefaultUnlit"));
        drawSettings.sorting.flags = SortFlags.CommonOpaque;

        var filterSettings = new FilterRenderersSettings(true) { renderQueueRange = RenderQueueRange.opaque };
        context.DrawRenderers(cull.visibleRenderers, ref drawSettings, filterSettings);

        context.DrawSkybox(camera);

        drawSettings.sorting.flags = SortFlags.CommonTransparent;
        filterSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cull.visibleRenderers, ref drawSettings, filterSettings);

        cameraBuffer.EndSample("Render Camera");
        context.ExecuteCommandBuffer(cameraBuffer);
        cameraBuffer.Clear();

        context.Submit();
    }

    CullResults cull;
    CommandBuffer cameraBuffer = new CommandBuffer { name = "Render Camera" };
}
