using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class MapSubscriber : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/map";
    public MapGenerator mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        connectRos.ws.OnMessage += OnWebSocketMessage;
        SubscribeToTopic(topicName);
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        if (jsonString.Contains("\"topic\": \"/map\""))
        {
            MapMessage message = JsonUtility.FromJson<MapMessage>(jsonString);
            float resolution = message.msg.info.resolution;
            uint width = message.msg.info.width;
            float originX = message.msg.info.origin.position.x;
            float originY = message.msg.info.origin.position.y;
            int[] data = message.msg.data;
            mapGenerator.MapGenerate(resolution, width, originX, originY, data);
        }   
    }

    private void SubscribeToTopic(string topic)
    {
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"nav_msgs/OccupancyGrid\"}";
        connectRos.ws.Send(subscribeMessage);
    }

    [System.Serializable]
    public class MapMessage
    {
        public string op;
        public string topic;
        public MapData msg;
    }

    [System.Serializable]
    public class MapData
    {
        public Header header;
        public MapMetaData info;
        public int[] data;
    }

    [System.Serializable]
    public class Header
    {
        public uint seq;
        public Time stamp;
        public string frame_id;
    }

    [System.Serializable]
    public class Time
    {
        public uint secs;
        public uint nsecs;
    }

    [System.Serializable]
    public class MapMetaData
    {
        public Time map_load_time;
        public float resolution;
        public uint width;
        public uint height;
        public Pose origin;
    }

    [System.Serializable]
    public class Pose
    {
        public Point position;
        public Quaternion orientation;
    }

    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
}
