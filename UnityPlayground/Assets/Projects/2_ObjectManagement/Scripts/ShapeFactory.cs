using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject 
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;
    [SerializeField] private bool recycle;

    List<Shape>[] pools;

    // ==============================================================

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Assert.IsTrue(shapeId >= 0 && shapeId < prefabs.Length, "Shape ID exceeds prefabs pool.");
        Assert.IsTrue(materialId >= 0 && materialId < materials.Length, "Material ID exceeds materials pool.");

        Shape instance;

        if (recycle)
        {
            if(pools == null)
            {
                CreatePools();
            }

            List<Shape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if(lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.ShapeId = shapeId;
            }
        }
        else
        {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }
        
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public void Reclaim(Shape shape)
    {
        if(recycle)
        {
            if(pools == null)
            {
                CreatePools();
            }

            pools[shape.ShapeId].Add(shape);
            shape.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shape.gameObject);
        }
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
    }

    void CreatePools()
    {
        pools = new List<Shape>[prefabs.Length];
        for(int i = 0; i < pools.Length; ++i)
        {
            pools[i] = new List<Shape>();
        }
    }
}
