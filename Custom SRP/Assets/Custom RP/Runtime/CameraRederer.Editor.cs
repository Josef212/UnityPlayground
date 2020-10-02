using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using UnityEditor;

partial class CameraRederer
{
	partial void DrawGizmos();
	partial void DrawUnsupportedShaders();
	partial void PrepareForSceneWindow();
	partial void PrepareBuffer();
	
#if UNITY_EDITOR
	private string SampleName { get; set; }
	
	private static Material ErrorMaterial
	{
		get
		{
			if(errorMaterial == null)
			{
				errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
			}

			return errorMaterial;
		}
	}
	
	partial void DrawUnsupportedShaders()
	{
		var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(_camera)){overrideMaterial = ErrorMaterial};
		for (int i = 1; i < legacyShaderTagIds.Length; ++i)
		{
			drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
		}
		var filteringSettings = FilteringSettings.defaultValue;
		_context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
	}
	
	partial void DrawGizmos()
	{
		if(Handles.ShouldRenderGizmos())
		{
			_context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
			_context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
		}
	}

	partial void PrepareForSceneWindow()
	{
		if(_camera.cameraType == CameraType.SceneView)
		{
			ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
		}
	}

	partial void PrepareBuffer()
	{
		Profiler.BeginSample("Editor Only");
		buffer.name = SampleName = _camera.name;
		Profiler.EndSample();
	}
	

	private static Material errorMaterial = null;
	private static ShaderTagId[] legacyShaderTagIds = 
	{
		new ShaderTagId("Always"),
		new ShaderTagId("ForwardBase"),
		new ShaderTagId("PrepassBase"),
		new ShaderTagId("Vertex"),
		new ShaderTagId("VertexLMRGBM"),
		new ShaderTagId("VertexLM")
	};
	
#else
	const string SampleName = bufferName;
#endif
}
