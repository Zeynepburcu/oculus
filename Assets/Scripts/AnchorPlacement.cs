using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnchorPlacement : MonoBehaviour
{
    public GameObject anchorPrefab;

    private void Update()
    {
        // Check if the Primary Index Trigger is pressed on the right controller
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            CreateSpatialAnchor();
        }
    }

    public void CreateSpatialAnchor()
    {
        // Get the position and rotation of the right controller
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Instantiate the prefab at the controller's position and rotation
        GameObject prefab = Instantiate(anchorPrefab, controllerPosition, controllerRotation);
    }
}

