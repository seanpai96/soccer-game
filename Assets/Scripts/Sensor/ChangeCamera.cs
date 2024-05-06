using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Camera godCamera;
    public Camera carCamera;
    public Camera firstPersonCamera;

    float baseDepth;
    private void Start()
    {
        baseDepth = godCamera.depth;        
    }

    public void UseGodCamera()
    {
        carCamera.depth = baseDepth - 1;
        firstPersonCamera.depth = baseDepth - 2;
    }

    public void UseCarCamera()
    {
        carCamera.depth = baseDepth + 2;
        firstPersonCamera.depth = baseDepth + 1;
    }

    public void UseFirstPersonCamera()
    {
        carCamera.depth = baseDepth + 1;
        firstPersonCamera.depth = baseDepth + 2;
    }
}
