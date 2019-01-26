using UnityEngine;

public delegate Vector3 GraphFunction (float x, float z, float t);

public enum GraphFunctionName
{
    Sine,
    MultiSine,
    Sine2D,
    MultiSine2D,
    Ripple,
    Cylinder,
    Sphere,
    Torus,
    Heart
}