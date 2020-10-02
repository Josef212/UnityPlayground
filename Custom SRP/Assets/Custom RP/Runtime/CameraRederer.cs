using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRederer
{
	public void Render(ScriptableRenderContext ctx, Camera cam)
	{
		_context = ctx;
		_camera = cam;

		PrepareBuffer();
		PrepareForSceneWindow();
		if(!Cull())
		{
			return;
		}

		SetUp();
		DrawVisibleGeometry();
		DrawUnsupportedShaders();
		DrawGizmos();
		Submit();
	}

	private void SetUp()
	{
		_context.SetupCameraProperties(_camera);
		CameraClearFlags flags = _camera.clearFlags;
		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth,
			flags == CameraClearFlags.Color,
			flags == CameraClearFlags.Color ?
				_camera.backgroundColor.linear : Color.clear
		);
		buffer.BeginSample(SampleName);
		ExecuteBuffer();
	}

	private void Submit()
	{
		buffer.EndSample(SampleName);
		ExecuteBuffer();
		_context.Submit();
	}

	private void ExecuteBuffer()
	{
		_context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}

	private bool Cull()
	{
		if(_camera.TryGetCullingParameters(out var parameters))
		{
			_cullingResults = _context.Cull(ref parameters);
			return true;
		}

		return false;
	}
	
	private void DrawVisibleGeometry()
	{
		var sortingSettings = new SortingSettings(_camera) { criteria = SortingCriteria.CommonOpaque }; 
		var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
		
		_context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
		
		_context.DrawSkybox(_camera);

		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;
		
		_context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
	}
	
	private ScriptableRenderContext _context;
	private Camera _camera;

	private const string bufferName = "Rener Camera";
	private CommandBuffer buffer = new CommandBuffer() { name = bufferName };

	private CullingResults _cullingResults;

	private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
}
