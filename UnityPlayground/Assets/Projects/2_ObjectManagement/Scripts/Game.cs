﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject 
{
    [SerializeField] private PersistableObject prefab;

    [SerializeField] private KeyCode createKey = KeyCode.C;
    [SerializeField] private KeyCode newGameKey = KeyCode.N;
    [SerializeField] private KeyCode saveKey = KeyCode.S;
    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private PersistentStorage storage;

    List<PersistableObject> objects;

    private string savePath;

    //========================================================

    void Awake () 
	{
        objects = new List<PersistableObject>();
	}
	

	void Update () 
	{
		if(Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if(Input.GetKey(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKey(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKey(loadKey))
        {
            storage.Load(this);
        }

    }

	//========================================================

	private void CreateObject()
    {
        PersistableObject o = Instantiate(prefab);
        Transform t = o.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        objects.Add(o);
    }

    private void BeginNewGame()
    {
        for(int i = 0; i < objects.Count; ++i)
        {
            Destroy(objects[i].gameObject);
        }

        objects.Clear();
    }

    // ---------------

    public override void Save(GameDataWriter writer)
    {
        writer.Write(objects.Count);

        for(int i = 0; i < objects.Count; ++i)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();

        for(int i = 0; i < count; ++i)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }
}
