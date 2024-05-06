using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightBasedDestructor : MonoBehaviour
{
    public int heightThreshold = -10;

    void Update()
    {
        if (transform.position.y < heightThreshold)
        {
            Destroy(gameObject);
        }
    }
}
