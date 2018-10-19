using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour 
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField] [Range(10, 200)] private int resolution = 10;
    [SerializeField] private Function function;

    private bool changed = false;
    private Transform[] points;


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
        
        if(function.animated)
        {
            for (int i = 0; i < points.Length; ++i)
            {
                Transform point = points[i];
                Vector3 position = point.position;

                position.y = function.GetY(position.x);

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
        points = new Transform[resolution];

        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        Vector3 position = Vector3.zero;

        for (int i = 0; i < resolution; i++)
        {
            Transform point = Instantiate(pointPrefab);

            position.x = (i + 0.5f) * step - 1f;

            position.y = function.GetY(position.x);

            point.localPosition = position;
            point.localScale = scale;

            point.SetParent(transform, false);

            points[i] = point;
        }
    }
}
