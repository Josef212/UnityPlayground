using UnityEngine;
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

    [SerializeField] private HexUnit unitPrefab;

    // ----

    HexCellShaderData cellShaderData;

    HexCell[] cells;
    HexGridChunk[] chunks;
    
    int chunkCountX, chunkCountZ;

    HexCellPriorityQueue searchFrontier;
    
    int searchFrontierPhase;
    HexCell currentPathFrom, currentPathTo;
    bool currentPathExists;

    List<HexUnit> units = new List<HexUnit>();

    public bool HasPath { get { return currentPathExists; } }

    //========================================================

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        HexUnit.unitPrefab = unitPrefab;
    }

    void Awake () 
	{
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        HexUnit.unitPrefab = unitPrefab;
        cellShaderData = gameObject.AddComponent<HexCellShaderData>();

        CreateMap(cellCountX, cellCountZ);
	}

    public bool CreateMap(int x, int z)
    {
        ClearPath();
        ClearUnits();

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

        cellShaderData.Initialize(cellCountX, cellCountZ);

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
        cell.Index = i;
        cell.ShaderData = cellShaderData;

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

    public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, unit);
        ShowPath(unit.Speed);
    }

    bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        int speed = unit.Speed;
        searchFrontierPhase += 2;

        if (searchFrontier == null)
            searchFrontier = new HexCellPriorityQueue();
        else
            searchFrontier.Clear();
        
        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        searchFrontier.Enqueue(fromCell);

        while(searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;

            if (current == toCell)
            {
                return true;
            }

            int currentTurn = (current.Distance - 1)/ speed;

            for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; ++d)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if(neighbor == null || neighbor.SearchPhase > searchFrontierPhase) continue;

                if (!unit.IsValidDestination(neighbor))
                {
                    continue;
                }
                int moveCost = unit.GetMoveCost(current, neighbor, d);
                if (moveCost < 0)
                {
                    continue;
                }

                int distance = current.Distance + moveCost;
                int turn = (distance - 1) / speed;
                if(turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                if(neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if(distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }

        return false;
    }

    void ShowPath(int speed)
    {
        if(currentPathExists)
        {
            HexCell current = currentPathTo;

            while(current != currentPathFrom)
            {
                int turn = (current.Distance - 1) / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }

        currentPathFrom.EnableHighlight(Color.blue);
        currentPathTo.EnableHighlight(Color.red);
    }

    public void ClearPath()
    {
        if(currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        currentPathFrom = currentPathTo = null;
    }

    void ClearUnits()
    {
        for(int i = 0; i < units.Count; ++i)
        {
            units[i].Die();
        }

        units.Clear();
    }

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.Grid = this;
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }

        return null;
    }

    public List<HexCell> Getpath()
    {
        if (!currentPathExists) return null;

        List<HexCell> path = ListPool<HexCell>.Get();

        for(HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();

        return path;
    }

    List<HexCell> GetVisibleCells(HexCell fromCell, int range)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();

        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        searchFrontier.Enqueue(fromCell);
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
            visibleCells.Add(current);

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }

                int distance = current.Distance + 1;
                if (distance > range)
                {
                    continue;
                }

                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.SearchHeuristic = 0;
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }

        return visibleCells;
    }

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].IncreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DecreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
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

        writer.Write(units.Count);
        for(int i = 0; i < units.Count; ++i)
        {
            units[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        ClearPath();
        ClearUnits();

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
            cells[i].Load(reader, header);
        }

        for (int i = 0; i < chunks.Length; ++i)
        {
            chunks[i].Refresh();
        }

        if(header >= 2)
        {
            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; ++i)
            {
                HexUnit.Load(reader, this);
            }
        }
    }
}
