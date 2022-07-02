using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Free,
    Locked
}

public class FirstPersonCameraController : MonoBehaviour
{
    //CUSTOM SETTINGS
    [Header("Cursor Settings")]
    public bool lockCursor = false;

    [Header("Camera Settings")]
    public CameraState cameraState = CameraState.Free;
    public Rigidbody cameraBaseRB;
    private Transform cameraDollyTransform;
    private Transform cameraTransform;

    [Header("Free Camera Settings")]
    public float mouseSensitivityX = 3.0f;
    public float mouseSensitivityY = 3.0f;
    public float minPitch = -85.0f;
    public float maxPitch = 85.0f;

    [Header("Dolly Settings")]
    public float tiltSmoothTime = 0.5f;

    //CAMERA INTERNAL VARIABLES
    private float yaw;
    private float pitch;
    private Vector3 offset;
    private Vector3 currentRotation;

    //DOLLY INTERNAL VARIABLES
    public Vector3 currentTilt = Vector3.zero;
    public Vector3 desiredTilt = Vector3.zero;
    private Vector3 tiltVelocity = Vector3.zero;


    private void Start()
    {
        cameraTransform = Camera.main.transform;
        cameraDollyTransform = Camera.main.transform.parent;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (CameraState.Free == cameraState)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            currentRotation = new Vector3(pitch, yaw);
            cameraTransform.eulerAngles = currentRotation;
        }
        else if (CameraState.Locked == cameraState)
        {
            //TODO
        }

        //Tilt 
        currentTilt = Vector3.SmoothDamp(currentTilt, desiredTilt, ref tiltVelocity, tiltSmoothTime);
        cameraDollyTransform.rotation = Quaternion.Euler(currentTilt);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredTilt = new Vector3(-15, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredTilt = new Vector3(15, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            desiredTilt = new Vector3(0, 0, 15);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            desiredTilt = new Vector3(0, 0, -15);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            desiredTilt = Vector3.zero;
        }
    }

    public Vector3 GetForward()
    {
        //TODO
        return Vector3.zero;
    }
}
