using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject 
{
    [SerializeField] private ShapeFactory shapeFactory;

    [SerializeField] private KeyCode createKey = KeyCode.C;
    [SerializeField] private KeyCode newGameKey = KeyCode.N;
    [SerializeField] private KeyCode saveKey = KeyCode.S;
    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private PersistentStorage storage;

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

    private void BeginNewGame()
    {
        for(int i = 0; i < shapes.Count; ++i)
        {
            Destroy(shapes[i].gameObject);
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
