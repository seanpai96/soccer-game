using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class RobotManualInput : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/arm_angle";

    public Slider Finger;
    public Slider Wrist;
    public Slider Elbow;
    public Slider Shoulder;
    public Slider Base;

    public bool manual;
    private float[] data = new float[6];
    const float INITIAL_ANGLE = 90;

    private void Start()
    {
        setManual(true);
    }

    void Update()
    {
        if (manual)
        {
            data[0] = Finger.value - INITIAL_ANGLE;
            data[1] = Finger.value - INITIAL_ANGLE;
            data[2] = Wrist.value - INITIAL_ANGLE;
            data[3] = Elbow.value - INITIAL_ANGLE;
            data[4] = Shoulder.value - INITIAL_ANGLE;
            data[5] = Base.value - INITIAL_ANGLE;
            PublishFloat32MultiArray(topicName, data);
        }
    }

    public void PublishFloat32MultiArray(string topic, float[] data)
    {
        string jsonMessage = $@"{{
            ""op"": ""publish"",
            ""topic"": ""{topic}"",
            ""msg"": {{
                ""layout"": {{
                    ""dim"": [{{""size"": {data.Length}, ""stride"": 1}}],
                    ""data_offset"": 0
                }},
                ""data"": [{string.Join(", ", data)}]
            }}
        }}";

        connectRos.ws.Send(jsonMessage);
    }


    public void setManual(bool decision)
    {
        manual = decision;
    }
}
