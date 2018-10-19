using UnityEngine;

public delegate float GraphFunction (float x, float z, float t);

public enum GraphFunctionName
{
    Sine,
    MultiSine,
    Sine2D,
    MultiSine2D,
    Ripple
}