using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour 
{
    public int AvarageFPS { get; private set; }
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }
    public int frameRange = 60;

    int[] fpsBuffer;
    int fpsBufferIndex;

	//========================================================

	void Awake () 
	{
        InitBuffer();
    }
	

	void Update () 
	{
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
            InitBuffer();

        UpdateBuffer();
        CalculateFPS();
    }

	//========================================================

	private void InitBuffer()
    {
        if (frameRange <= 0)
            frameRange = 1;

        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    private void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);

        if(fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;

        for(int i = 0; i < frameRange; ++i)
        {
            int fps = fpsBuffer[i];

            if (fps > highest) highest = fps;
            if (fps < lowest) lowest = fps;

            sum += fps;
        }

        AvarageFPS = sum / frameRange;
        HighestFPS = highest;
        LowestFPS = lowest;
    }
}
