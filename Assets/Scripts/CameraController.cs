using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSensitivity;
    public float maxRotationY; // Adjust as needed

    private void Update()
    {
        if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            float inputX = Input.GetTouch(0).deltaPosition.x * rotationSensitivity;

            if (Mathf.Abs(inputX) > 0.1f) // Adjust threshold as needed
            {
                RotateCamera(inputX);
            }
        }
    }

    void RotateCamera(float rotationAmount)
    {
        transform.Rotate(0f, rotationAmount, 0f);

        // Clamp the rotation around the y-axis to avoid over-rotation
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float clampedY = Mathf.Clamp(NormalizeAngle(currentRotation.y), -maxRotationY, maxRotationY);
        Debug.Log(NormalizeAngle(currentRotation.y));
        transform.rotation = Quaternion.Euler(currentRotation.x, clampedY, currentRotation.z);
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }

}
