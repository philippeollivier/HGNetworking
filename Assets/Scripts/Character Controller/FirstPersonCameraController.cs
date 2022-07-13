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
    #region Singleton Design
    private static FirstPersonCameraController _instance;

    public static FirstPersonCameraController Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    //CUSTOM SETTINGS
    [Header("Cursor Settings")]
    public bool lockCursor = false;

    [Header("Camera Settings")]
    public CameraState cameraState = CameraState.Free;

    public float runFov = 100.0f;
    public float walkFov = 90.0f;
    public float fovSmoothing = 0.05f;
    private float fovVelocity;
    private float currentFov = 0.0f;

    public Vector3 crouchHeight = new Vector3(0, 0.7f, 0);
    public Vector3 standingHeight = new Vector3(0, 1.7f, 0);
    public float crouchSmoothing = 0.05f;
    private Vector3 crouchVelocity;
    private Vector3 currentOffset;

    private Transform cameraBaseTransform;
    private Transform cameraTransform;
    public Camera cameraCam;
    private Animator cameraAnimator;

    [Header("Free Camera Settings")]
    public float mouseSensitivityX = 3.0f;
    public float mouseSensitivityY = 3.0f;
    public float minPitch = -85.0f;
    public float maxPitch = 85.0f;

    [Header("Animation Settings")]


    //CAMERA INTERNAL VARIABLES
    private float yaw;
    private float pitch;
    private Vector3 currentRotation;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        cameraAnimator = Camera.main.transform.parent.GetComponent<Animator>();
        cameraBaseTransform = Camera.main.transform.parent.parent;
        cameraCam = Camera.main;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        if (CameraState.Free == cameraState)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            currentRotation = new Vector3(pitch, 0);

            cameraTransform.localEulerAngles = currentRotation;
            cameraBaseTransform.eulerAngles = new Vector3(0, yaw, 0);

            currentFov = Mathf.SmoothDamp(currentFov, (FPController.Instance.sprinting) ? (runFov) : (walkFov), ref fovVelocity, fovSmoothing);
            currentOffset = Vector3.SmoothDamp(currentOffset, (FPController.Instance.crouching || FPController.Instance.moveState == MoveState.Slide) ? (crouchHeight) : (standingHeight), ref crouchVelocity, crouchSmoothing);

            cameraCam.fieldOfView = currentFov;
            cameraTransform.localPosition = currentOffset;
        }
        else if (CameraState.Locked == cameraState)
        {
            //Not sure if we even need this
        }


    }

    public Vector3 GetForward()
    {
        return Vector3.zero;
    }


    #region Camera Animation
    public void PlayImpactAnimation(float impact)
    {
        cameraAnimator.SetLayerWeight(cameraAnimator.GetLayerIndex("LandingImpact"), Mathf.Clamp01(impact));
        cameraAnimator.SetTrigger("Landed");
    }


    public void WallRunL()
    {
        cameraAnimator.SetTrigger("WallRunL");
    }

    public void WallRunR()
    {
        cameraAnimator.SetTrigger("WallRunR");
    }

    public void WallRunIdle()
    {
        cameraAnimator.SetTrigger("WallRunIdle");
    }

    #endregion
}
