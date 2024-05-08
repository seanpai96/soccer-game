using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWheelSpeed : MonoBehaviour
{
    public float maxLinearSpeed = 2;        // m/s
    public float maxRotationalSpeed = 1;
    public float wheelRadius = 0.07f;       // meters
    public float trackWidth = 0.288f;       // meters Distance between tyres
    public float forceLimit = 100;
    public float damping = 10;

    public ArticulationBody wA1;
    public ArticulationBody wA2;
    public ArticulationBody wA3;
    public ArticulationBody wA4;
    public PlayerController playerController;
    private RotationDirection direction;

    // Start is called before the first frame update
    void Start()
    {
        SetParameters(wA1);
        SetParameters(wA2);
        SetParameters(wA3);
        SetParameters(wA4);
    }

    private void Update()
    {
        //Debug.Log(playerController.wheelSpeed1 + " " + playerController.wheelSpeed2 + " " + playerController.wheelSpeed3 + " " + playerController.wheelSpeed4);
        //SetSpeed(wA1, playerController.wheelSpeed1);
        //SetSpeed(wA2, playerController.wheelSpeed2);
        //SetSpeed(wA3, playerController.wheelSpeed3);
        //SetSpeed(wA4, playerController.wheelSpeed4);
        //SetSpeed(wA1, 1000);
        //SetSpeed(wA2, 1000);
        //SetSpeed(wA3, 1000);
        //SetSpeed(wA4, 1000);
    }

    private void SetParameters(ArticulationBody joint)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.forceLimit = forceLimit;
        drive.damping = damping;
        joint.xDrive = drive;
    }

    private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
    {
        ArticulationDrive drive = joint.xDrive;
        if (float.IsNaN(wheelSpeed))
        {
            drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
        }
        else
        {
            drive.targetVelocity = wheelSpeed;
        }
        drive.targetVelocity = wheelSpeed;
        joint.xDrive = drive;
    }
}
