using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class LightningGenerator : MonoBehaviour
{
	private Mesh LightAreaMesh
	{
		get
		{
			if(m_lightAreaMesh.mesh == null)
			{
				m_lightAreaMesh.mesh = new Mesh();
				m_lightAreaMesh.mesh.Clear();
			}

			return m_lightAreaMesh.mesh;
		}
	}
	
	private void Awake()
	{
		m_wallsLayer = LayerMask.NameToLayer("Walls");
		m_walls = GameObject.FindGameObjectsWithTag("Wall");
	}

	private void Update()
	{
		var origin = m_playerTransform.position;
		List<Tuple<float, Vector3>> points = new List<Tuple<float, Vector3>>();
		
		foreach (var wall in m_walls)
		{
			foreach (var corner in GetObjectCorners(wall.GetComponent<BoxCollider2D>(), wall.transform))
			{
				float angle = m_angle;
				var rayDirection = corner - origin;
				Vector3[] raysDirections =
				{
					rayDirection,
					Quaternion.Euler(0, 0, -angle) * rayDirection,
					Quaternion.Euler(0, 0, angle) * rayDirection
				};

				foreach (var direction in raysDirections)
				{
					RaycastHit2D raycastHit = Physics2D.Raycast(origin, direction, 1000.0f);
					if(raycastHit.transform != null && raycastHit.transform.gameObject.layer == m_wallsLayer)
					{
						float rayAngle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
						Vector3 point = raycastHit.point;
						point.z = -0.01f;
						points.Add(new Tuple<float, Vector3>(rayAngle, point));
					}
				}
			}
		}

		var orderedPoints = points.OrderBy(x => x.Item1).ToList();
		GenerateMesh(origin, orderedPoints);
		
		if(m_drawDebug)
		{
			orderedPoints.ForEach(x => Debug.DrawLine(origin, x.Item2, Color.red));
		}
	}

	private void GenerateMesh(Vector3 center, List<Tuple<float, Vector3>> points)
	{
		if(points == null || points.Count < 2)
		{
			LightAreaMesh.Clear();
			return;
		}
		
		int vertexCount = points.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] indices = new int[(vertexCount - 1) * 3];
		
		vertices[0] = center;

		for (int i = 0; i < vertexCount - 1; ++i)
		{
			vertices[i + 1] = points[i].Item2;

			if(i < vertexCount - 2)
			{
				indices[i * 3] = 0;
				indices[i * 3 + 1] = i + 2;
				indices[i * 3 + 2] = i + 1;
			}
		}
		
		indices[indices.Length - 3] = 0;
		indices[indices.Length - 2] = 1;
		indices[indices.Length - 1] = vertexCount - 1;

		Mesh mesh = LightAreaMesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}

	private static IEnumerable<Vector3> GetObjectCorners(BoxCollider2D collider, Transform objTransform)
	{
		Vector2 offset = collider.offset;
		Vector2 size = collider.size;
         
		yield return objTransform.TransformPoint(offset + new Vector2(size.x, size.y) * 0.5f);
		yield return objTransform.TransformPoint(offset + new Vector2(size.x, -size.y) * 0.5f);
		yield return objTransform.TransformPoint(offset + new Vector2(-size.x, -size.y) * 0.5f);
		yield return objTransform.TransformPoint(offset + new Vector2(-size.x, size.y) * 0.5f);

	}

	[SerializeField] private float m_angle = 0.01f;
	[SerializeField] private bool m_drawDebug = true;
	[SerializeField] private MeshFilter m_lightAreaMesh = null;
	[SerializeField] private Transform m_playerTransform = null;

	private int m_wallsLayer = -1;
	private GameObject[] m_walls = null;
}
