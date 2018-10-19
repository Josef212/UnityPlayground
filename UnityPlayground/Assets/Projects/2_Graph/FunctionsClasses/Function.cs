using UnityEngine;

[CreateAssetMenu(fileName = "Diagonal", menuName = "Graph/Diagonal", order = 51)]
public class Function : ScriptableObject 
{
    public bool animated = false;

    public virtual float GetY(float x)
    {
        return x;
    }   
}
