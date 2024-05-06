using UnityEngine;
using WebSocketSharp;
using System;
using TMPro;
public class ArmTransfer : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public MotorSpeedCalculator motorSpeedCalculator;
    public float[] jointPositions;
    public float axis1CorrectionFactor = 90.0f;
    public float axis2CorrectionFactor = 60.0f;
    public float axis3CorrectionFactor = 60.0f;
    public float axis4CorrectionFactor = 60.0f;
    public float axis5CorrectionFactor = 60.0f;

    string inputTopic = "/joint_trajectory_point";
    string inputMsgType = "trajectory_msgs/msg/JointTrajectoryPoint";
    string outputTopic = "/arm_angle";
    float[] data = new float[6];
    bool manual;
    
    void Start()
    {
        setManual(true);
        connectRos.ws.OnMessage += OnWebSocketMessage;
        connectRos.SubscribeToTopic(inputTopic, inputMsgType);
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        var genericMessage = JsonUtility.FromJson<GenericRosMessage>(jsonString);
        if (genericMessage.topic == inputTopic && !manual)
        {
            RobotNewsMessageJointTrajectory message = JsonUtility.FromJson<RobotNewsMessageJointTrajectory>(jsonString);
            HandleJointTrajectoryMessage(message);
        }
    }

    private void HandleJointTrajectoryMessage(RobotNewsMessageJointTrajectory message)
    {
        jointPositions = message.msg.positions;
        for (int i = 0; i < jointPositions.Length; i++)
        {
            jointPositions[i] = jointPositions[i] * Mathf.Rad2Deg;
        }

        data[0] = 180.0f - jointPositions[4] - axis5CorrectionFactor;
        data[1] = 180.0f - jointPositions[4] - axis5CorrectionFactor;
        data[2] = jointPositions[3] - axis4CorrectionFactor;
        data[3] = jointPositions[2] - axis3CorrectionFactor;
        data[4] = jointPositions[1] - axis2CorrectionFactor;
        data[5] = jointPositions[0] - axis1CorrectionFactor;

        connectRos.PublishFloat32MultiArray(outputTopic, data);
    }
    
    [System.Serializable]
    public class GenericRosMessage
    {
        public string op;
        public string topic;
    }
    
    [System.Serializable]
    public class RobotNewsMessageJointTrajectory
    {
        public string op;
        public string topic;
        public JointTrajectoryPointMessage msg;
    }

    [System.Serializable]
    public class JointTrajectoryPointMessage
    {
        public float[] positions;
        public float[] velocities;
        public float[] accelerations;
        public float[] effort;
        public TimeFromStart time_from_start;
    }

    [System.Serializable]
    public class TimeFromStart
    {
        public int sec;
        public int nanosec;
    }

    public void setManual(bool decision)
    {
        manual = decision;
    }
}
