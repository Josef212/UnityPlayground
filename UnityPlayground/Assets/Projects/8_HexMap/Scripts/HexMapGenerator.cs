using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour 
{
    [SerializeField] private HexGrid grid;

    [SerializeField, Range(0f, 0.5f)] private float jitterProbability = 0.25f;
    [SerializeField, Range(20, 200)] private int chunkSizeMin = 30;
    [SerializeField, Range(20, 200)] private int chunkSizeMax = 100;
    [SerializeField, Range(5, 95)] private int landPercentage = 50;

    private int cellCount;

    private HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    //========================================================

    private void Start()
    {
        GenerateMap(20, 15);
    }

    //========================================================

    public void GenerateMap(int x, int z)
    {
        cellCount = x * z;
        grid.CreateMap(x, z);

        if (searchFrontier == null) searchFrontier = new HexCellPriorityQueue();

        CreateLand();

        for(int i = 0; i < cellCount; ++i)
        {
            grid.GetCell(i).SearchPhase = 0;
        }
    }

    // -------------------------------------

    HexCell GetRandomCell()
    {
        return grid.GetCell(Random.Range(0, cellCount));
    }

    void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        while(landBudget > 0)
        {
            landBudget = RaiseTerrain(Random.Range(chunkSizeMin, chunkSizeMax + 1), landBudget);
        }
    }

    int RaiseTerrain(int chunkSize, int budget)
    {
        searchFrontierPhase += 1;

        HexCell firstCell = GetRandomCell();
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.coordinates;

        int size = 0; 
        while(size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.Elevation += 1;
            if(current.TerrainTypeIndex == 0)
            {
                current.TerrainTypeIndex = 1;
                if (--budget == 0) break;
            }
            size += 1;

            for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; ++d)
            {
                HexCell neighbor = current.GetNeighbor(d);

                if(neighbor && neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }

        searchFrontier.Clear();

        return budget;
    }
}
