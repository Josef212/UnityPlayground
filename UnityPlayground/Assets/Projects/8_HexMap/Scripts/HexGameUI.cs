using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour 
{
    [SerializeField] private HexGrid grid;

    private HexCell currentCell;
    private HexUnit selectedUnit;

    //========================================================

    void Start () 
	{
		
	}
	

	void Update () 
	{
		if(!EventSystem.current.IsPointerOverGameObject())
        {
            if(Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if(selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                    DoMove();
                else
                    DoPathfinding();
            }
        }
	}

	//========================================================

	public void SetEditMode(bool toogle)
    {
        enabled = !toogle;
        grid.ShowUI(!toogle);
        grid.ClearPath();
        if (toogle) Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        else Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

        if(cell != currentCell)
        {
            currentCell = cell;
            return true;
        }

        return false;
    }

    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();

        if(currentCell)
        {
            selectedUnit = currentCell.Unit;
        }
    }

    void DoPathfinding()
    {
        if(UpdateCurrentCell())
        {
            if(currentCell && selectedUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoMove()
    {
        if(grid.HasPath)
        {
            selectedUnit.Travel(grid.Getpath());
            grid.ClearPath();
        }
    }
}
