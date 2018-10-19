using UnityEngine;

[CreateAssetMenu(fileName = "CustomCurve", menuName = "Graph/CustomCurve", order = 51)]
public class AnimationCurveFunction : Function 
{
    public AnimationCurve curve;
    public float rangeMin = -1, rangeMax = 1;

    public override float GetY(float x)
    {
        float slope = 1f / (rangeMax - rangeMin);

        return curve.Evaluate(slope * (x - rangeMin));
    }
}
