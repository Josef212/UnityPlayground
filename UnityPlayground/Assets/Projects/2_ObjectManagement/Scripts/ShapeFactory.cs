using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject 
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;

    // ==============================================================

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Assert.IsTrue(shapeId >= 0 && shapeId < prefabs.Length, "Shape ID exceeds prefabs pool.");
        Assert.IsTrue(materialId >= 0 && materialId < materials.Length, "Material ID exceeds materials pool.");

        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeId = shapeId;
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
    }
}
