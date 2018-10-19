using System;
using UnityEngine;

public class Clock : MonoBehaviour 
{
    [SerializeField] private bool continous = true;
    [SerializeField] private Transform hoursTransform, minutesTransform, secondsTransform;
    [SerializeField] private float degreesPerHour, degreesPerMinute, degreesPerSecond;

	//========================================================

	void Start () 
	{
        DateTime time = DateTime.Now;

        hoursTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerSecond, 0f);

    }
	

	void Update () 
	{
        if (continous)
            AdjustArmsContinous();
        else
            AdjustArmsDiscrete();
    }

	//========================================================

	private void AdjustArmsContinous()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;

        hoursTransform.localRotation = Quaternion.Euler(0f, (float)time.TotalHours * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, (float)time.TotalMinutes * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, (float)time.TotalSeconds * degreesPerSecond, 0f);
    }

    private void AdjustArmsDiscrete()
    {
        DateTime time = DateTime.Now;

        hoursTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, time.Minute * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, time.Second * degreesPerSecond, 0f);
    }
}
