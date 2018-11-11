using UnityEngine;

public class NewMapMenu : MonoBehaviour 
{

    [SerializeField] private HexGrid hexGrid;
    
	//========================================================

	public void Open()
    {
        gameObject.SetActive(true);
        HexMapCamera.Loocked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Loocked = false;
    }

    public void CreateSmallMap()
    {
        CreateMap(20, 15);
    }

    public void CreateMediumMap()
    {
        CreateMap(40, 30);
    }

    public void CreateLargeMap()
    {
        CreateMap(80, 60);
    }

    // ------------------------------------------------------------

    void CreateMap(int x, int z)
    {
        hexGrid.CreateMap(x, z);
        HexMapCamera.ValidatePosition();
        Close();
    }
}
