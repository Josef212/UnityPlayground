using UnityEngine;

public class Shape : PersistableObject 
{
    public int MaterialId { get; private set; }
    public int ShapeId
    {
        get { return shapeId; }
        set { if(shapeId == int.MinValue && value != int.MinValue) shapeId = value; }
    }

    int shapeId = int.MinValue;
    Color color;
    MeshRenderer meshRenderer;

    static int colorPorpertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    //========================================================

    void Awake () 
	{
        meshRenderer = GetComponent<MeshRenderer>();
    }
	

	void Update () 
	{
		
	}

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);

        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);

        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }

    //========================================================

    public void SetMaterial(Material mat, int materialId)
    {
        MaterialId = materialId;
        meshRenderer.material = mat;
    }

    public void SetColor(Color color)
    {
        this.color = color;

        if(sharedPropertyBlock == null)
            sharedPropertyBlock = new MaterialPropertyBlock();

        sharedPropertyBlock.SetColor(colorPorpertyId, color);
        meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }
}
