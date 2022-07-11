using System.Collections;
using UnityEngine;

//enum MoveState
//{
//    Grounded,
//    Airborne,
//    WallRun,
//    WallClimb,
//    WallVault,
//    Grappling,
//    Slide,
//    Ragdoll
//}

public class FirstPersonCharacterController : MonoBehaviour
{
    #region Singleton Design
    private static FirstPersonCharacterController _instance;

    public static FirstPersonCharacterController Instance { get { return _instance; } }


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

    [Header("Misc Settings")]
    [SerializeField]
    private MoveState moveState;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Transform cameraTransform;


    [Header("Walking Settings")]
    public float moveSpeedWalkForward = 20.0f;
    public float moveSpeedWalkStrafe = 20.0f;
    public float moveSpeedWalkBackwards = 20.0f;
    public float moveSpeedRunForward = 20.0f;
    public float moveSpeedRunStrafe = 20.0f;
    public float moveSpeedRunBackwards = 20.0f;
    public float moveStopTime = 0.01f;
    public float frictionStopForce = 2.0f;

    public bool sprinting = false;
    public bool crouching = false;
    private bool grounded = false;


    [Header("Jump Settings")]
    public float jumpForce = 600.0f;
    public float jumpHoldTime = 1.0f;
    public float jumpHoldForce = 1.0f;
    public float jumpActiveTurningTimeWindow = 1.0f;
    public float jumpActiveTurningAngle = 120.0f;
    public float jumpActiveStoppingAngle = 160.0f;
    public float jumpActiveTurningStopSpeed = 2.0f;
    public float wallJumpForce = 100f;
    public float wallJumpHorizontalForce = 100f;
    public float airMovementForce = 50.0f;
    public float impactAnimationForce = 20.0f;
    public float airborneMaxSpeed = 10.0f;

    private float averageYVelocity = 0;


    [Header("Wall Climb Settings")]
    public float wallClimbRange = 1.0f;
    public float wallClimbAngle = 20.0f;
    public float wallClimbAccelerationTime = 2.3f;
    public float wallClimbAccelerationForce = 5.0f;
    public float wallClimbAccelerationFriction = 5.0f;
    public float wallClimbDurationTime = 3.0f;

    public float wallVaultOffset = 0.2f;
    public float wallVaultForce = 1f;
    public float wallVaultHorizontalForce = 1f;
    private Vector3 wallVaultNormal;

    private bool canWallClimb = false;

    [Header("Grappling Gun Settings")]
    public float grappleForce;
    public float grappleForwardForce;
    public float grappleMaxAngle = 90f;
    public float grappleRange = float.PositiveInfinity;
    private Vector3 grappleTarget;

    [Header("Wall Run Settings")]
    public float wallRunRange = 1.0f;
    public float wallRunAngle = 20.0f;
    public float wallRunAccelerationForce = 5.0f;
    public float wallRunDurationTime = 3.0f;
    public float wallRunGravityFactor = 0.9f;

    private bool wallRunLeft = false;
    private bool canWallRun = false;

    private Vector3 desiredMotion;
    private Vector3 rotatedMotion;

    #region Unity Functions

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        SetMoveState(MoveState.Airborne);
    }

    private void Update()
    {
        //Update Shared Values and Player Input
        UpdateDesiredMotion();
        UpdateSprintValues();
        UpdateGroundedValues();

        //Update Misc Animation values
        UpdateImpactAnimationValues();

        //Update Character Controller based on Move State
        MoveStateBasedUpdate();
    }

    void MoveStateBasedUpdate()
    {
        //Update controller based on move state
        switch (moveState)
        {
            case MoveState.Grounded:
                UpdateMoveStateGrounded();
                break;
            case MoveState.Airborne:
                UpdateMoveStateAirborne();
                break;
            case MoveState.WallRun:
                //UpdateMoveStateWallRun();
                break;
            case MoveState.WallClimb:
                UpdateMoveStateWallClimb();
                break;
            //case MoveState.Grappling:
                //UpdateMoveStateGrappling();
                //break;
            case MoveState.WallVault:
                UpdateMoveStateWallVault();
                break;
            case MoveState.Slide:
                //UpdateMoveStateSlide();
                break;
            case MoveState.Ragdoll:
                throw new System.Exception("Move state not implemented yet");
        }
    }


    private void SetMoveState(MoveState moveState)
    {
        //Break out early if we are not transitioning states 
        if (moveState == this.moveState)
        {
            return;
        }

        //Handler for any custom logic when switching move states
        switch (moveState)
        {
            case MoveState.Grounded:
                canWallClimb = true;
                canWallRun = true;
                break;
            case MoveState.Airborne:
                break;
            case MoveState.WallRun:
                canWallRun = false;
                if (wallRunLeft) { FirstPersonCameraController.Instance.WallRunR(); }
                else { FirstPersonCameraController.Instance.WallRunL(); }
                break;
            case MoveState.WallClimb:
                WallClimb();
                break;
            case MoveState.WallVault:
                rb.constraints = RigidbodyConstraints.FreezePosition;
                break;
            case MoveState.Slide:
                break;
            case MoveState.Ragdoll:
                break;
        }
        this.moveState = moveState;
    }

    #endregion

    #region Player Input And Update Temp Value Functions

    private bool UpdateSprintValues()
    {
        sprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        return sprinting;
    }

    private bool UpdateGroundedValues()
    {
        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
        grounded = (Physics.SphereCast(ray, capsuleCollider.radius - 0.001f, 1.002f - capsuleCollider.radius));
        return grounded;
    }

    private Vector3 UpdateDesiredMotion()
    {
        desiredMotion = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        return desiredMotion;
    }

    private bool UpdateWallClimbRaycast()
    {
        Vector3 rayDir = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * Vector3.forward;
        Ray ray = new Ray(transform.position + Vector3.up, rayDir);
        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rayDir * wallClimbRange, Color.blue);
        if (Physics.Raycast(ray, out hit, wallClimbRange))
        {
            wallVaultNormal = hit.normal;
            return Vector3.Angle(-hit.normal, rayDir) < wallClimbAngle && desiredMotion.z > 0;
        }
        return false;
    }


    private bool UpdateWallVaultRaycast()
    {
        Vector3 rayDir = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * Vector3.forward;
        Ray ray = new Ray(transform.position + Vector3.up * (wallVaultOffset + 1), rayDir);
        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rayDir * wallClimbRange, Color.blue);
        if (Physics.Raycast(ray, out hit, wallClimbRange))
        {
            return true;
        }
        return false;
    }


    private bool UpdateWallRunRaycast()
    {
        if (desiredMotion.x == 0)
        {
            return false;
        }
        else if (desiredMotion.x > 0)
        {
            wallRunLeft = true;
        }
        else
        {
            wallRunLeft = false;
        }


        Vector3 rayDir = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * (-desiredMotion.x * Vector3.left);
        Ray ray = new Ray(transform.position + Vector3.up, rayDir);
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rayDir, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, wallRunRange))
        {

            return Vector3.Angle(-hit.normal, rayDir) < wallRunAngle && desiredMotion.z > 0;
        }
        return false;
    }

    #endregion

    #region Animation Helper Functions

    private void UpdateImpactAnimationValues()
    {
        averageYVelocity = (averageYVelocity + rb.velocity.y) / 2f;
    }

    #endregion

    #region Move State Updates

    private void UpdateMoveStateGrounded()
    {
        if (desiredMotion != Vector3.zero)
        {
            //Rotate move vector to be inline with camera direction
            Vector3 convertedMovement = ApplyNonConstantMovespeed(desiredMotion);
            rotatedMotion = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * convertedMovement;

            //Slow all movement to give snappier feel on x-z orthogonal to movement vector
            float pow = Mathf.Pow(frictionStopForce, Time.deltaTime);
            rb.velocity = new Vector3(rb.velocity.x / pow, rb.velocity.y, rb.velocity.z / pow);

            //Add desired movement
            rb.AddForce(rotatedMotion * Time.deltaTime);
        }
        else
        {
            //Add force to slow velocity down in opposite direction of velocity
            Vector3 oldVelocity = rb.velocity;
            oldVelocity.y = 0;
            rb.AddForce(-oldVelocity * moveStopTime, ForceMode.Acceleration);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //Update controller move state
        if (UpdateWallClimbRaycast() && canWallClimb)
        {
            SetMoveState(MoveState.WallClimb);
        }
        else if (!grounded)
        {
            SetMoveState(MoveState.Airborne);
        }
    }

    private void UpdateMoveStateAirborne()
    {
        //Rotate move vector to be inline with camera direction
        rotatedMotion = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * desiredMotion;
        rb.AddForce(AirAccelerate(rotatedMotion.normalized, airMovementForce), ForceMode.VelocityChange);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AirJump();
        }

        //Update controller move state
        if (UpdateWallClimbRaycast() && canWallClimb)
        {
            SetMoveState(MoveState.WallClimb);
        }
        else if (grounded)
        {
            ImpactTransition();
            SetMoveState(MoveState.Grounded);
        }
    }

    private void UpdateMoveStateWallClimb()
    {
        //To get off wall user can either, jump off or run out of stamina or vault

        if (!UpdateWallClimbRaycast())
        {
            SetMoveState(MoveState.Airborne);
        }
        else if (!UpdateWallVaultRaycast())
        {
            SetMoveState(MoveState.WallVault);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMoveState(MoveState.Airborne);
            WallJump();
            return;
        }

        rb.AddForce(Vector3.up * (-Physics.gravity.y + wallClimbAccelerationForce), ForceMode.Acceleration);
        rb.AddForce(Vector3.up * (-rb.velocity.y * wallClimbAccelerationFriction), ForceMode.Acceleration);
    }

    private void UpdateMoveStateWallRun()
    {
        //if (UpdateWallRunRaycast())
        //{
        //    FirstPersonCameraController.Instance.WallRunIdle();
        //    SetMoveState(MoveState.Airborne);
        //}

        //rb.AddForce(-Physics.gravity * wallRunGravityFactor, ForceMode.Acceleration);
    }

    private void UpdateMoveStateWallVault()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            SetMoveState(MoveState.Airborne);
            WallVault();
            return;
        }
    }


    private void UpdateMoveStateGrappling()
    {
        //if (IsGrappleAngleExceeded())
        //{
        //    SetMoveState(MoveState.Airborne);
        //}

        //Ray ray = FirstPersonCameraController.Instance.cameraCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //rb.AddForce((grappleTarget - transform.position).normalized * grappleForce, ForceMode.Acceleration);
        //rb.AddForce(ray.direction.normalized * grappleForce, ForceMode.Acceleration);
    }

    #endregion

    #region Grounded Helper Functions
    private Vector3 ApplyNonConstantMovespeed(Vector3 input)
    {
        Vector3 convertedVec = input;
        if (convertedVec.z > 0)
        {
            convertedVec.z *= (sprinting) ? (moveSpeedRunForward) : (moveSpeedWalkForward);
        }
        else
        {
            convertedVec.z *= (sprinting) ? (moveSpeedRunBackwards) : (moveSpeedWalkBackwards);
        }

        convertedVec.x *= (sprinting) ? (moveSpeedRunStrafe) : (moveSpeedWalkStrafe);
        return convertedVec;
    }

    #endregion

    #region Airborne Helper Functions

    private void ImpactTransition()
    {
        FirstPersonCameraController.Instance.PlayImpactAnimation(Mathf.Abs(averageYVelocity) / impactAnimationForce);
    }


    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
        StartCoroutine(JumpForceCoroutine());
        StartCoroutine(JumpActiveTurningCoroutine());
    }

    private void AirJump()
    {
        Vector3 rbVel = rb.velocity;
        rbVel.y = 0;
        rb.velocity = rbVel;
        rotatedMotion = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * Vector3.back;
        rb.AddForce(Vector3.up * jumpForce + rotatedMotion * wallJumpHorizontalForce);
        StartCoroutine(JumpForceCoroutine());
        StartCoroutine(JumpActiveTurningCoroutine());
    }

    private void WallJump()
    {
        //Get angle between current velocity and desired angle
        rotatedMotion = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * Vector3.back;
        rb.AddForce(Vector3.up * wallJumpForce + rotatedMotion * wallJumpHorizontalForce);
    }

    private Vector3 AirAccelerate(Vector3 accelDir, float accelerate)
    {
        Vector3 prevVelocity = rb.velocity;
        prevVelocity.y = 0;
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.deltaTime;

        if (projVel + accelVel > airborneMaxSpeed)
        {
            accelVel = Mathf.Clamp(airborneMaxSpeed - projVel, 0, airborneMaxSpeed);
        }

        return accelDir * accelVel;
    }

    IEnumerator JumpForceCoroutine()
    {
        float activeTimer = 0.0f;
        while (activeTimer <= jumpHoldTime)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                yield break;
            }

            activeTimer += Time.deltaTime;
            rb.AddForce(Vector3.up * jumpHoldForce * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator JumpActiveTurningCoroutine()
    {
        float activeTimer = 0.0f;
        while (activeTimer <= jumpActiveTurningTimeWindow)
        {
            //Get angle between current velocity and desired angle
            rotatedMotion = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * UpdateDesiredMotion();
            rotatedMotion.Normalize();

            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            float magnitude = velocity.magnitude;
            velocity.Normalize();

            if (desiredMotion == Vector3.zero)
            {

            }
            else if (Vector3.Angle(rotatedMotion, velocity) < jumpActiveTurningAngle)
            {
                rb.AddForce(magnitude * (rotatedMotion - velocity), ForceMode.Impulse);
            }
            else if (Vector3.Angle(rotatedMotion, velocity) > jumpActiveStoppingAngle)
            {
                rb.AddForce(-velocity * magnitude * jumpActiveTurningStopSpeed, ForceMode.Impulse);
                yield break;
            }

            activeTimer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region WallClimb Helper Functions

    private void WallClimb()
    {
        StartCoroutine(WallTimerCoroutine());
    }

    IEnumerator WallTimerCoroutine()
    {
        float activeTimer = 0.0f;
        while (activeTimer <= wallClimbDurationTime && moveState == MoveState.WallClimb)
        {
            activeTimer += Time.deltaTime;
            yield return null;
        }

        if (activeTimer >= wallClimbDurationTime)
        {
            canWallClimb = false;
            SetMoveState(MoveState.Airborne);
        }

        yield break;
    }

    #endregion

    #region WallVault Helper Functions

    private void WallVault()
    {
        rb.AddForce(Vector3.up * wallVaultForce, ForceMode.VelocityChange);
        rb.AddForce(-wallVaultNormal * wallVaultHorizontalForce, ForceMode.VelocityChange);
    }

    #endregion

    #region Grappling Helper Functions

    //private bool TryGrapple()
    //{
    //    Ray ray = FirstPersonCameraController.Instance.cameraCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, grappleRange))
    //    {
    //        grappleTarget = hit.point;
    //        return true;
    //    }
    //    return false;
    //}

    //private bool IsGrappleAngleExceeded()
    //{
    //    Ray ray = FirstPersonCameraController.Instance.cameraCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //    return Vector3.Angle(ray.direction.normalized, grappleTarget - transform.position) > grappleMaxAngle;
    //}

    #endregion

}
