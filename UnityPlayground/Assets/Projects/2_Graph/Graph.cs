using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour 
{
    [SerializeField] private GraphFunctionName function;
    [SerializeField] private Transform pointPrefab;
    [SerializeField] [Range(10, 200)] private int resolution = 10;

    [SerializeField] private bool animated = false;

    private bool changed = false;
    private Transform[] points;

    // ---

    GraphFunction[] functions =
    {
        SineFunction, MultiSineFunction, Sine2DFunction, MultiSine2DFunction, RippleFunction
    };


	//========================================================

	void Awake () 
	{
        SetPoints();
	}
	

	void Update () 
	{
		if(changed)
        {
            DestroyChilds();
            SetPoints();
            changed = false;
        }
        
        if(animated)
        {
            float t = Time.time;
            GraphFunction f = functions[(int)function];

            for (int i = 0; i < points.Length; ++i)
            {
                Transform point = points[i];
                Vector3 position = point.position;

                position.y = f(position.x, position.z, t);

                point.localPosition = position;
            }
        }
	}

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            changed = true;
    }
#endif

    //========================================================

    void DestroyChilds()
    {
        Transform[] childs = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            childs[i] = transform.GetChild(i);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(childs[i].gameObject);
        }
    }
    // -----

    void SetPoints()
    {
        points = new Transform[resolution * resolution];

        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        Vector3 position = Vector3.zero;

        GraphFunction f = functions[(int)function];

        for(int i = 0, z = 0; z < resolution; ++z)
        {
            position.z = (z + 0.5f) * step - 1f;

            for (int x = 0; x < resolution; ++x, ++i)
            {
                Transform point = Instantiate(pointPrefab);

                position.x = (x + 0.5f) * step - 1f;

                position.y = f(position.x, position.z, 0f);

                point.localPosition = position;
                point.localScale = scale;

                point.SetParent(transform, false);

                points[i] = point;
            }
        }
    }

    // ----

    static float SineFunction(float x, float z, float t)
    {
        return Mathf.Sin((x + t) * Mathf.PI);
    }

    static float MultiSineFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (x + 2f * t)) / 2f;
        y *= 2f / 3f;

        return y;
    }

    static float Sine2DFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t)) + Mathf.Sin(Mathf.PI * (z + t));
        y *= 0.5f;

        return y;
    }

    static float MultiSine2DFunction(float x, float z, float t)
    {
        float y = 4f * Mathf.Sin(Mathf.PI * (x + z + t * 0.5f));
        y += Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (z + 2f * t)) * 0.5f;
        y *= 1f / 5.5f;

        return y;
    }

    static float RippleFunction(float x, float z, float t)
    {
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(Mathf.PI * (4f * d - t));
        y /= 1f + 10f * d;

        return y;
    }
}
