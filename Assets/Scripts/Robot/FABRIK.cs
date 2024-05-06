using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{
    public Transform[] joints;
    public Transform[] jointsTarget;
    public Transform target;
    public Transform baseCurrent;
    public Transform carCurrent;
    public ConnectROSBridge connectRos;
    public float threshold = 0.001f;
    public int maxIterations = 10;

    private float[] jointsLength;
    private float armLength;
    private float baseTargetAngle;
    private float shoulderTargetAngle;
    private float elbowTargetAngle;
    private Vector3[] jointsPosition;

    void Update()
    {
        Initialize();
        ResolveFABRIK();
    }

    void Initialize()
    {
        jointsPosition = new Vector3[joints.Length];
        jointsLength = new float[joints.Length - 1];
        armLength = 0;

        jointsTarget[0].rotation = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < joints.Length; i++)
        {
            jointsTarget[i].position = joints[i].position;
            jointsPosition[i] = joints[i].position;
            if (i < joints.Length - 1)
            {
                jointsLength[i] = (joints[i + 1].position - joints[i].position).magnitude;
                armLength += jointsLength[i];
            }
        }
    }

    void ResolveFABRIK()
    {
        CalculateBaseRotation();

        if ((target.position - joints[1].position).magnitude >= armLength)
        {
            TargetOutOfRange();
        }
        else
        {
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                jointsTarget[jointsPosition.Length - 1].position = target.position;
                for (int i = jointsTarget.Length - 2; i >= 1; i--)
                {
                    Vector3 direction = (jointsTarget[i].position - jointsTarget[i + 1].position).normalized;
                    jointsTarget[i].position = jointsTarget[i + 1].position + direction * jointsLength[i];
                }

                jointsTarget[1].position = joints[1].position;
                for (int i = 1; i < jointsTarget.Length - 1; i++)
                {
                    Vector3 direction = (jointsTarget[i + 1].position - jointsTarget[i].position).normalized;
                    jointsTarget[i + 1].position = jointsTarget[i].position + direction * jointsLength[i];
                }

                CalculateArmRotation();
                if ((jointsTarget[jointsPosition.Length - 1].position - target.position).sqrMagnitude < threshold * threshold)
                    break;
            }
        }
        PublishAngleToROS();
    }

    void CalculateBaseRotation()
    {
        Vector3 targetVector = target.position - joints[0].position;
        float tempAngle = Mathf.Atan2(targetVector.z, targetVector.x) - Mathf.Atan2(0, -1);

        tempAngle *= Mathf.Rad2Deg;
        tempAngle -= 90 - carCurrent.eulerAngles.y;
        tempAngle = (tempAngle + 360) % 360;

        baseTargetAngle = tempAngle;

        tempAngle = 360 - baseCurrent.localEulerAngles.y - tempAngle;
        jointsTarget[0].rotation = Quaternion.Euler(0, tempAngle, 0);
    }

    void TargetOutOfRange()
    {
        Vector3 wrist2Target = target.position - jointsTarget[3].position;
        Vector3 base2Shoulder = jointsTarget[1].position - jointsTarget[0].position;
        elbowTargetAngle = 0;
        shoulderTargetAngle = Vector3.Angle(base2Shoulder, wrist2Target);
    }

    void CalculateArmRotation()
    {
        Vector3 elbow2Wrist = jointsTarget[3].position - jointsTarget[2].position;
        Vector3 shoulder2Elbow = jointsTarget[2].position - jointsTarget[1].position;
        Vector3 base2Shoulder = jointsTarget[1].position - jointsTarget[0].position;

        elbowTargetAngle = Vector3.Angle(elbow2Wrist, shoulder2Elbow);
        shoulderTargetAngle = Vector3.Angle(shoulder2Elbow, base2Shoulder);
    }

    void PublishAngleToROS()
    {
        float[] data = new float[6];
        data[3] = elbowTargetAngle;
        data[4] = shoulderTargetAngle;
        data[5] = baseTargetAngle;
        connectRos.PublishFloat32MultiArray("/arm_angle", data);
    }
}
