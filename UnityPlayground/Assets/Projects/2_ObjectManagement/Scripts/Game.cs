using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject 
{
    [SerializeField] private ShapeFactory shapeFactory;

    [SerializeField] private KeyCode createKey = KeyCode.C;
    [SerializeField] private KeyCode destroyKey = KeyCode.X;
    [SerializeField] private KeyCode newGameKey = KeyCode.N;
    [SerializeField] private KeyCode saveKey = KeyCode.S;
    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private PersistentStorage storage;



    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    float creationProgress, destructionProgress;


    List<Shape> shapes;

    const int saveVersion = 3;
    
    //========================================================

    void Awake () 
	{
        shapes = new List<Shape>();
	}
	

	void Update () 
	{
		if(Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if(Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else if(Input.GetKey(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKey(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKey(loadKey))
        {
            storage.Load(this);
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

	//========================================================

	private void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));

        shapes.Add(instance);
    }

    private void DestroyShape()
    {
        if(shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);

            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    private void BeginNewGame()
    {
        for(int i = 0; i < shapes.Count; ++i)
        {
            shapeFactory.Reclaim(shapes[i]);
        }

        shapes.Clear();
    }

    // ---------------

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);

        for(int i = 0; i < shapes.Count; ++i)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version <= 0 ? -version : reader.ReadInt();

        for(int i = 0; i < count; ++i)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}
