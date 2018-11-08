﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour 
{
    public HexCoordinates coordinates;
    
    [SerializeField]  HexCell[] neighbours;

    public HexGridChunk chunk;

    [SerializeField] private bool[] roads;

    [HideInInspector] public RectTransform uiRect;

    private int elevation = int.MinValue;
    private Color color;

    private bool hasIncomingRiver, hasOutgoingRiver;
    private HexDirection incomingRiver, outgoingRiver;

    public int Elevation
    {
        get { return elevation; }
        set
        {
            if (elevation == value) return;

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            if (hasOutgoingRiver && elevation < GetNeighbour(outgoingRiver).elevation)
                RemoveOutgoingRiver();
            if (hasIncomingRiver && elevation > GetNeighbour(incomingRiver).elevation)
                RemoveIncomingRiver();

            for(int i = 0; i < roads.Length; ++i)
            {
                if(roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
        }
    }

    public Color Color
    {
        get { return color; }
        set
        {
            if (color == value) return;

            color = value;
            Refresh();
        }
    }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public bool HasIncomingRiver {  get { return hasIncomingRiver; } }
    public bool HasOutgoingRiver { get { return hasOutgoingRiver; } }
    public HexDirection IncomingRiver { get { return incomingRiver; } }
    public HexDirection OutgoingRiver { get { return outgoingRiver; } }
    public bool HasRiver { get { return hasIncomingRiver || hasOutgoingRiver; } }
    public bool HasRiverBeginOrEnd { get { return hasIncomingRiver != hasOutgoingRiver; } }

    public float StreamBedY { get { return (elevation + HexMetrics.streamBedElevationOffset) * 
                HexMetrics.elevationStep; } }

    public float RiverSurfaceY { get { return (elevation + HexMetrics.riverSurfaceElevationOffset) *
                                                HexMetrics.elevationStep; } }
    
    public HexDirection RiverBeginOrEndDirection {  get { return hasIncomingRiver ? incomingRiver : outgoingRiver; } }

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

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbours[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver) return;

        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbour(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver) return;

        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbour(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction) return;

        HexCell neighbor = GetNeighbour(direction);
        if (!neighbor || elevation < neighbor.elevation) return;

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction) RemoveIncomingRiver();

        hasOutgoingRiver = true;
        outgoingRiver = direction;

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();

        SetRoad((int)direction, false);
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public bool HasRoads
    {
        get
        {
            for(int i = 0; i < roads.Length; ++i)
            {
                if (roads[i]) return true;
            }
            return false;
        }
    }

    public void AddRoad(HexDirection direction)
    {
        if(!roads[(int)direction] && !HasRiverThroughEdge(direction) && 
            GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads()
    {
        for(int i = 0; i < neighbours.Length; ++i)
        {
            if(roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbours[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbours[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = elevation - GetNeighbour(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    //========================================================

    void Refresh()
    {
        if(chunk)
        {
            chunk.Refresh();

            for(int i = 0; i < neighbours.Length; ++i)
            {
                HexCell n = neighbours[i];
                if(n != null && n.chunk != chunk)
                {
                    n.chunk.Refresh();
                }
            }
        }
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}
