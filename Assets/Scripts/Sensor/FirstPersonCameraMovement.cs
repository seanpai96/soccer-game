using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraMovement : MonoBehaviour
{
    public Camera firstPersonCamera;

    float movementSpeed = 5.0f; 
    float rotationSpeed = 200.0f;
    bool activeMovement;
    // Start is called before the first frame update
    void Start()
    {
        activeMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeMovement)
        {
            float moveZ = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
            firstPersonCamera.transform.Translate(0, 0, moveZ);

            float moveX = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            firstPersonCamera.transform.Translate(moveX, 0, 0);

            if (Input.GetKey(KeyCode.Q))
            {
                firstPersonCamera.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.E))
            {
                firstPersonCamera.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }
        }
    }

    public void SetActiveMovement(bool decision)
    {
        activeMovement = decision;
    }
}
