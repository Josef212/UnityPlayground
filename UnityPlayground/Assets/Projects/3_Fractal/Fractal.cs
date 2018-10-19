using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour 
{
    [SerializeField] private Mesh[] meshes;
    [SerializeField] [Range(0.0f, 1.0f)] private float spawnProbability = 0.7f;
    [SerializeField] private Material material;
    [SerializeField] private int maxDepth;

    [SerializeField] private float childScale;
    [SerializeField] private Color initialColor = Color.white;
    [SerializeField] private Color finalColor1 = Color.yellow;
    [SerializeField] private Color finalColor2 = Color.cyan;
    [SerializeField] private bool testEndColor = true;

    [SerializeField] private float maxRotationSpeed = 30f;
    [SerializeField] private float maxTwist = 20f;

    private float rotationSpeed;
    private int depth;
    private Material[,] materials;

    // ---

    private static Vector3[] childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
        Vector3.down
    };

    private static Quaternion[] childOrientations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f),
        Quaternion.Euler(180f, 0f, 0f)
    };

	//========================================================

	void Start () 
	{
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);

        if (materials == null)
            InitializeMaterials();

        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = 
            materials[depth, Random.Range(0, materials.GetLength(1))];

        if(depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
	}

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    //========================================================

    IEnumerator CreateChildren()
    {
        for(int i = 0; i < childDirections.Length; ++i)
        {
            if(Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
            }
        }
    }

    private void Initialize(Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        spawnProbability = parent.spawnProbability;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.localRotation = childOrientations[childIndex];
    }

    void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 2];
        for(int i = 0; i <= maxDepth; ++i)
        {
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(initialColor, finalColor1, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(initialColor, finalColor2, t);
        }

        if(testEndColor)
        {
            materials[maxDepth, 0].color = Color.magenta;
            materials[maxDepth, 1].color = Color.red;
        }
    }
}
