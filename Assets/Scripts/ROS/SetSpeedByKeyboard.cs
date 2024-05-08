using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeedByKeyboard : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName;
    public KeyCode front;
    public KeyCode back;
    public KeyCode left;
    public KeyCode right;

    private float[] data = new float[3];
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = 0;

        if (Input.GetKey(front))
        {          
            data[2] += -1; 
        }

        if (Input.GetKey(back))
        {
            data[2] += 1; 
        }

        if (Input.GetKey(left))
        {
            data[0] += 1;
        }

        if (Input.GetKey(right))
        {
            data[0] += -1;
        }

        connectRos.PublishFloat32MultiArray(topicName, data);
    }
}
