using UnityEngine;

[CreateAssetMenu(fileName = "SineWave", menuName = "Graph/SineWave", order = 51)]
public class SineWave : Function 
{
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;

    public override float GetY(float x)
    {
        return Mathf.Sin((x * frequency + Time.time) * Mathf.PI) * amplitude;
    }
}
