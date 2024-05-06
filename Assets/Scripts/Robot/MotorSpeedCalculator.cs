using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorSpeedCalculator : MonoBehaviour
{
    public float maxVoltage = 12.0f;
    public float maxRPM = 176.0f; 

    public float CalculateSpeed(float voltage)
    {
        float rpmPerVolt = maxRPM / maxVoltage;
        return voltage * rpmPerVolt;
    }
}
