﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour 
{
    public int cellCountX = 20;
    public int cellCountZ = 15;

    [SerializeField] private HexCell cellPrefab;
    [SerializeField] private Text cellLabelPrefab;
    [SerializeField] private HexGridChunk chunkPrefab;

    [SerializeField] private Texture2D noiseSource;

    [SerializeField] private int seed;

    // ----

    HexCell[] cells;
    HexGridChunk[] chunks;
    
    int chunkCountX, chunkCountZ;

    //========================================================

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
    }

    void Awake () 
	{
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);

        CreateMap(cellCountX, cellCountZ);
	}

    public bool CreateMap(int x, int z)
    {
        if(x <= 0 || x % HexMetrics.chunckSizeX != 0 ||
            z <= 0 || z % HexMetrics.chunckSizeZ != 0)
        {
            Debug.LogError("Unsopported map size.");
            return false; 
        }

        if(chunks != null)
        {
            for(int i = 0; i < chunks.Length; ++i)
            {
                Destroy(chunks[i].gameObject);
            }
        }

        cellCountX = x;
        cellCountZ = z;

        chunkCountX = cellCountX / HexMetrics.chunckSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunckSizeZ;

        CreateChunks();
        CreateCells();

        return true;
    }

	//========================================================

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for(int z = 0, i = 0; z < chunkCountZ; ++z)
        {
            for(int x = 0; x < chunkCountX; ++x)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];

        for (int z = 0, i = 0; z < cellCountZ; ++z)
        {
            for (int x = 0; x < cellCountX; ++x)
            {
                CreateCell(x, z, i++);
            }
        }
    }

	void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        if(x > 0)
        {
            cell.SetNeighbour(HexDirection.W, cells[i - 1]);
        }

        if(z > 0)
        {
            if((z & 1) == 0)
            {
                cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX]);
                if(x > 0)
                {
                    cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX]);
                if(x < cellCountX - 1)
                {
                    cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);

        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunckSizeX;
        int chunkZ = z / HexMetrics.chunckSizeZ;

        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunckSizeX;
        int localZ = z - chunkZ * HexMetrics.chunckSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunckSizeX, cell);
    }
    
    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ) return null;

        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX) return null;

        return cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i < chunks.Length; ++i)
        {
            chunks[i].ShowUI(visible);
        }
    }

    public void FindDistanceTo(HexCell cell)
    {
        StopAllCoroutines();
        StartCoroutine(Search(cell));
    }

    IEnumerator Search(HexCell cell)
    {
        for(int i = 0; i < cells.Length; ++i)
        {
            cells[i].Distance = int.MaxValue;
        }

        WaitForSeconds delay = new WaitForSeconds(1 / 60f); 
        List<HexCell> frontier = new List<HexCell>();
        cell.Distance = 0;

        frontier.Add(cell);

        while(frontier.Count > 0)
        {
            yield return delay;
            HexCell current = frontier[0];
            frontier.RemoveAt(0);

            for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; ++d)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if(neighbor == null) continue;
                if (neighbor.IsUnderwater) continue;

                HexEdgeType edgeType = current.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff) continue;

                int dst = current.Distance;
                if (current.HasRoadThroughEdge(d))
                {
                    dst += 1;
                }
                else if(current.Walled != neighbor.Walled)
                {
                    continue;
                }
                else
                {
                    dst += edgeType == HexEdgeType.Flat ? 5 : 10;
                    dst += neighbor.UrbanLevel + neighbor.FarmLevel + neighbor.PlantLevel;
                }

                if(neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = dst;
                    frontier.Add(neighbor);
                }
                else if(dst< neighbor.Distance)
                {
                    neighbor.Distance = dst;
                }
                
                frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            }
        }
    }

    // ==========================================================
    // ==========================================================

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);

        for(int i = 0; i < cells.Length; ++i)
        {
            cells[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        StopAllCoroutines();

        int x = 20, z = 15;
        if(header >= 1)
        {
            x = reader.ReadInt32();
            z = reader.ReadInt32();
        }

        if(x != cellCountX || z != cellCountZ)
        {
            if (!CreateMap(x, z)) return;
        }

        for (int i = 0; i < cells.Length; ++i)
        {
            cells[i].Load(reader);
        }

        for (int i = 0; i < chunks.Length; ++i)
        {
            chunks[i].Refresh();
        }
    }
}
