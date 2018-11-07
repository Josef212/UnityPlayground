﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour 
{
    [SerializeField] private Color[] colors;
    [SerializeField] private HexGrid hexGrid;

    private Color activeColor;
    private int activeElevation;

    private bool applyColor;
    private bool applyElevation = true;

    private int brushSize = 0;

	//========================================================

	void Awake () 
	{
        SelectColor(0);
	}
	

	void Update () 
	{
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    //========================================================

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    // ------------

    public void SelectColor(int index)
    {
        applyColor = index >= 0;

        if(applyColor)
        {
            activeColor = colors[index];
        }
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    // --------

    void EditCell(HexCell cell)
    {
        if (cell == null) return;

        if(applyColor)
        {
            cell.Color = activeColor;
        }

        if(applyElevation)
        {
            cell.Elevation = activeElevation;
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; ++z, ++r)
        {
            for (int x = centerX - r; x <= centerX + brushSize; ++x)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; --z, ++r)
        {
            for (int x = centerX - brushSize; x <= centerX + r; ++x)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }
}
