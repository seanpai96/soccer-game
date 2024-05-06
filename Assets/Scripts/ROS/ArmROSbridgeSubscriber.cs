using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WebSocketSharp;

public class ArmROSbridgeSubscriber : MonoBehaviour
{
    public ConnectROSBridge connectRos;
    public string topicName = "/arm_angle";
    public string msgType = "std_msgs/msg/Float32MultiArray";
    public GameObject robot;
    public float[] data = new float[6];

    ArmController armController;
    const int ROTATION_THRESHOLD = 3;

    void Start()
    {
        connectRos.ws.OnMessage += OnWebSocketMessage;
        connectRos.SubscribeToTopic(topicName, msgType);
        armController = robot.GetComponent<ArmController>();
    }

    void Update()
    {
        for (int i = armController.joints.Length - 1; i >= 0; i--)
        {
            float inputVal = CountInputVal(i);
            RotationDirection direction = GetRotationDirection(inputVal);
            armController.RotateJoint(i, direction);
        }
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        if (jsonString.Contains("\"topic\": \"/arm_angle\""))
        {
            RobotNewsMessage message = JsonUtility.FromJson<RobotNewsMessage>(jsonString);
            data = message.msg.data;
        }
    }

    float CountInputVal(int index)
    {
        ArticulationJointController jointController = armController.joints[index].robotPart.GetComponent<ArticulationJointController>();
        float currentAngle = jointController.CurrentPrimaryAxisRotation();
        float targetAngle = data[index];
        float mappedTargetAngle = MapTargetAngle(currentAngle, targetAngle);
        
        if (Math.Abs(mappedTargetAngle - currentAngle) >= ROTATION_THRESHOLD)
        {
            if (mappedTargetAngle > currentAngle) return 1;
            if (currentAngle > mappedTargetAngle) return -1;
        }
        return 0;
    }

    float MapTargetAngle(float currentRotation, float targetAngle)
    {
        float normalizedCurrentRotation = currentRotation % 360f;
        if (normalizedCurrentRotation < 0) normalizedCurrentRotation += 360f;

        int cycleOffset = Mathf.FloorToInt(currentRotation / 360f);
        float adjustedTargetAngle = targetAngle + (cycleOffset * 360f);

        if (Mathf.Abs(adjustedTargetAngle - currentRotation) > 180f)
        {
            if (adjustedTargetAngle > currentRotation)
                adjustedTargetAngle -= 360f;
            else
                adjustedTargetAngle += 360f;
        }

        return adjustedTargetAngle;
    }

    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
            return RotationDirection.Positive;
        else if (inputVal < 0)
            return RotationDirection.Negative;
        else
            return RotationDirection.None;
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
