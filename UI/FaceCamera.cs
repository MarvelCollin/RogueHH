using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            Vector3 lookAt = mainCamera.transform.position - transform.position;
            lookAt.y = 0; 
            transform.rotation = Quaternion.LookRotation(lookAt);
        }
    }
}
