using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class ConnectROSBridge : MonoBehaviour
{
    public string rosbridgeServerAddress = "localhost:9090";
    public WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        string protocol = "ws://";
        ws = new WebSocket(protocol + rosbridgeServerAddress);
        ws.Connect();
    }

    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }

    public void SubscribeToTopic(string topic, string type)
    {
        string subscribeMessage = $"{{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"{topic}\",\"type\":\"{type}\"}}";
        ws.Send(subscribeMessage);
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

        ws.Send(jsonMessage);
    }
}
