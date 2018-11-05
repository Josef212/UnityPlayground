using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour 
{
    public HexCoordinates coordinates;

    public Color color;

    [SerializeField]  HexCell[] neighbours;

	//========================================================

	void Start () 
	{
		
	}
	

	void Update () 
	{
		
	}

	//========================================================

	public HexCell GetNeighbour(HexDirection direction)
    {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell)
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }
}
