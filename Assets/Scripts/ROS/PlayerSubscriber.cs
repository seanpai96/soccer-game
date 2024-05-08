using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PlayerSubscriber : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/player";
    public string msgType = "std_msgs/msg/Float32MultiArray";

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;

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
            x = data[0];
            y = data[1];
            z = data[2];
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
