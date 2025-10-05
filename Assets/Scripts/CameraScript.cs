using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public float distance;
    public float sensitivity;
    public float scrollSensitivity;
    public Vector2 zoomRange;
    float horizontalAngle = -Mathf.PI / 2;
    float verticalAngle = Mathf.PI / 5;
    Vector3 lookAt, vLookAt;

    private void Update() {
        // Input.
        distance *= Mathf.Pow(scrollSensitivity, Input.mouseScrollDelta.y);
        distance = Mathf.Clamp(distance, zoomRange.x, zoomRange.y);
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            horizontalAngle -= Input.GetAxis("Mouse X") * sensitivity;
            verticalAngle -= Input.GetAxis("Mouse Y") * sensitivity;
            verticalAngle = Mathf.Clamp(verticalAngle, Mathf.PI * .1f, Mathf.PI * .49f);
        }

        // Look at.
        int maxPuzzleDimension = 0;
        Puzzle puzzle = PuzzleScript.instance?.puzzle;
        if (puzzle != null) {
            maxPuzzleDimension = Mathf.Max(puzzle.width, puzzle.height);
        }
        Vector3 targetLookAt = new Vector3(0, Mathf.InverseLerp(3, 10, maxPuzzleDimension) * -1f, 0);
        lookAt = Vector3.SmoothDamp(lookAt, targetLookAt, ref vLookAt, 0.25f);

        // Set position.
        float xzDistance = distance * Mathf.Cos(verticalAngle);
        float x = Mathf.Cos(horizontalAngle) * xzDistance;
        float y = Mathf.Sin(verticalAngle) * distance;
        float z = Mathf.Sin(horizontalAngle) * xzDistance;
        transform.localPosition = new Vector3(x, y, z);
        transform.LookAt(lookAt);
    }
}
