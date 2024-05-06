using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualSetSpeed : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/wheel_speed";
    public Slider leftFrontWheel;
    public Slider rightFrontWheel;
    public Slider leftBackWheel;
    public Slider rightBackWheel;
    public bool manual;

    private float[] data = new float[4];

    // Update is called once per frame
    void Update()
    {
        if (manual)
        {
            data[0] = leftFrontWheel.value;
            data[1] = rightFrontWheel.value;
            data[2] = leftBackWheel.value;
            data[3] = rightBackWheel.value;
            connectRos.PublishFloat32MultiArray(topicName, data);
        }
    }

    public void publishZeroData()
    {
        data[0] = 0;
        data[1] = 0;
        data[2] = 0;
        data[3] = 0;
        connectRos.PublishFloat32MultiArray(topicName, data);
    }

    public void setManual(bool decision)
    {
        manual = decision;
    }
}
