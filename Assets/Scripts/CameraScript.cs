using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Vector3 lookAt;
    public float distance;
    public float sensitivity;
    public float scrollSensitivity;
    public Vector2 zoomRange;
    float horizontalAngle = -Mathf.PI / 2;
    float verticalAngle = Mathf.PI / 5;

    private void Update() {
        // Input.
        distance *= Mathf.Pow(scrollSensitivity, Input.mouseScrollDelta.y);
        distance = Mathf.Clamp(distance, zoomRange.x, zoomRange.y);
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            horizontalAngle -= Input.GetAxis("Mouse X") * sensitivity;
            verticalAngle -= Input.GetAxis("Mouse Y") * sensitivity;
            verticalAngle = Mathf.Clamp(verticalAngle, Mathf.PI * .1f, Mathf.PI * .49f);
        }

        // Set position.
        float xzDistance = distance * Mathf.Cos(verticalAngle);
        float x = Mathf.Cos(horizontalAngle) * xzDistance;
        float y = Mathf.Sin(verticalAngle) * distance;
        float z = Mathf.Sin(horizontalAngle) * xzDistance;
        transform.localPosition = new Vector3(x, y, z);
        transform.LookAt(lookAt);
    }
}
