using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;
using WebSocketSharp;

public class CarROSbridgeSubscriber : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/wheel_speed";
    public string msgType = "std_msgs/msg/Float32MultiArray";
    public float maxLinearSpeed = 2;        // m/s
    public float maxRotationalSpeed = 1;
    public float wheelRadius = 0.07f;       // meters
    public float trackWidth = 0.288f;       // meters Distance between tyres
    public float forceLimit = 100;
    public float damping = 10;

    public float wA1Value = 0.0f;
    public float wA2Value = 0.0f;
    public float wA3Value = 0.0f;
    public float wA4Value = 0.0f;


    private RotationDirection direction;

    void Start()
    {
        connectRos.ws.OnMessage += OnWebSocketMessage;
        connectRos.SubscribeToTopic(topicName, msgType);
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        if (jsonString.Contains("\"topic\": \"" + topicName + "\""))
        {
            RobotNewsMessage message = JsonUtility.FromJson<RobotNewsMessage>(jsonString);
            float[] data = message.msg.data;
            wA1Value = data[0];
            wA2Value = data[1];
            wA3Value = data[2];
            wA4Value = data[3];
        }
    }

    [System.Serializable]
    public class RobotNewsMessage
    {
        public string op;
        public string topic;
        public MessageData msg;
    }

    [System.Serializable]
    public class MessageData
    {
        public LayoutData layout;
        public float[] data;
    }

    [System.Serializable]
    public class LayoutData
    {
        public int[] dim;
        public int data_offset;
    }

}


