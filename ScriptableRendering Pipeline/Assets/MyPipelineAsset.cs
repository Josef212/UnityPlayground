using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(menuName = "Rendering/My pipeline")]
public class MyPipelineAsset : RenderPipelineAsset
{
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new MyPipeline();
    }
}
