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
    public float steerSpeed;
    public float rotateSpeed;

    private float[] data = new float[4];
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = 0;

        if (Input.GetKey(front))
        {
            data[0] += steerSpeed; 
            data[1] += steerSpeed; 
            data[2] += steerSpeed; 
            data[3] += steerSpeed; 
        }

        if (Input.GetKey(back))
        {
            data[0] += -steerSpeed; 
            data[1] += -steerSpeed; 
            data[2] += -steerSpeed; 
            data[3] += -steerSpeed; 
        }

        if (Input.GetKey(left))
        {
            if (data[0] > 0)
            {
                data[0] += 0f;
                data[1] += rotateSpeed;
                data[2] += 0f;
                data[3] += rotateSpeed;
            }
            else if (data[0] == 0)
            {
                data[0] += -rotateSpeed;
                data[1] += rotateSpeed;
                data[2] += -rotateSpeed;
                data[3] += rotateSpeed;
            }
            else
            {
                data[0] += 0f;
                data[1] += -rotateSpeed;
                data[2] += 0f;
                data[3] += -rotateSpeed;
            }
        }

        if (Input.GetKey(right))
        {
            if (data[0] > 0)
            {
                data[0] += rotateSpeed;
                data[1] += 0f;
                data[2] += rotateSpeed;
                data[3] += 0f;
            }
            else if (data[0] == 0)
            {
                data[0] += rotateSpeed;
                data[1] += -rotateSpeed;
                data[2] += rotateSpeed;
                data[3] += -rotateSpeed;
            }
            else
            {
                data[0] += -rotateSpeed;
                data[1] += 0f;
                data[2] += -rotateSpeed;
                data[3] += 0f;
            }
        }

        connectRos.PublishFloat32MultiArray(topicName, data);
    }
}
