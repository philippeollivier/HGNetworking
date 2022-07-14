using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum MoveState
{
    Grounded,
    Airborne,
    WallRun,
    WallClimb,
    WallVault,
    Grapple,
    Slide,
    Ragdoll
}

public class FPController : MonoBehaviour
{
    public FirstPersonCameraController FPCameraController;

    #region Variables

    [Header("State Settings (DO NOT MODIFY DIRECTLY)")]
    public MoveState moveState;
    [HideInInspector] public Vector3 desiredMotion;
    [HideInInspector] public Vector3 rotatedMotion;
    [HideInInspector] public bool sprinting = false;
    [HideInInspector] public bool crouching = false;
    [HideInInspector] public bool grounded = false;
    [HideInInspector] public bool wallClimbRaycast = false;
    [HideInInspector] public bool canWallClimb = true;
    [HideInInspector] public Vector3 wallVaultNormal = Vector3.zero;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider capsuleCollider;
    [HideInInspector] public Transform cameraTransform;
    private Dictionary<MoveState, StateCharacterController> controllers = new Dictionary<MoveState, StateCharacterController>();

    [Header("Walking Settings")]
    [Range(0f, 90f)] public float maxGroundAngle = 25f;
    [HideInInspector] public Vector3 contactNormal;
    [HideInInspector] public float minGroundDotProduct;
    [HideInInspector] public int stepsSinceLastGrounded = 0;
    [HideInInspector] public Rigidbody contactObject;
    [HideInInspector] public Rigidbody previousContactObject;
    [HideInInspector] public Vector3 contactVelocity;
    [HideInInspector] public Vector3 contactTranslation;
    public float crouchSpeedModifier = 0.5f;
    public float moveSpeedWalkForward = 1150f;
    public float moveSpeedWalkStrafe = 850f;
    public float moveSpeedWalkBackwards = 750f;
    public float moveSpeedRunForward = 2000f;
    public float moveSpeedRunStrafe = 1250f;
    public float moveSpeedRunBackwards = 1100f;
    public float moveStopTime = 10f;
    public float frictionStopForce = 3000f;
    public int snappingFrames = 2;
    public float snappingDist = 0.2f;
    [Range(0f, 10f)] public float maxSnapSpeed = 2f;
    private Vector3 contactWorldPosition;

    [Header("Airborne Settings")]
    public float airborneMaxSpeed = 1f;
    public float airMovementForce = 20f;
    public float impactAnimationForce = 50f;
    [HideInInspector] public float averageYVelocity = 0;

    [Header("Sliding Settings")]
    public float minSlideSpeed = 1.0f;
    public float slideForce = 1.0f;
    public float slideFrictionStopForce = 1.0f;

    [Header("Jump Settings")]
    public float jumpForce = 200f;
    public float jumpHoldTime = 0.15f;
    public float jumpHoldForce = 200f;
    public float jumpActiveTurningTimeWindow = 0.6f;
    public float jumpActiveTurningAngle = 35f;
    public float jumpActiveStoppingAngle = 190.0f;
    public float jumpActiveTurningStopSpeed = 0.8f;
    public float wallJumpForce = 100f;
    public float wallJumpHorizontalForce = 100f;
    public float airJumpForce = 100f;
    public float airJumpHorizontalForce = 100f;

    [Header("Wall Run Settings")]
    [SerializeField, Range(90, 180)] float maxClimbAngle = 140f;
    [HideInInspector] public float minClimbDotProduct;
    [HideInInspector] public Vector3 climbNormal;
    [HideInInspector] public Vector3 wallDir;
    [HideInInspector] public int climbContactCount;
    public float wallRunSpeed = 10.0f;
    public float maxVelocityChange = 1.0f;


    #endregion


    #region Unity Functions

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        //Initialize and store all states
        controllers.Add(MoveState.Grounded, new GroundedController(this));
        controllers.Add(MoveState.Airborne, new AirborneController(this));
        //controllers.Add(MoveState.WallRun, new WallRunController(this));
        controllers.Add(MoveState.Slide, new SlideController(this));

        //Update references to components 
        cameraTransform = Camera.main.transform;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        //Set default state
        SetMoveState(MoveState.Airborne);
    }

    private void Update()
    {
        UpdateSharedValues();
        controllers[moveState].UpdateController();
    }

    private void FixedUpdate()
    {
        UpdateSharedValues();
        UpdateFixedState();
        controllers[moveState].FixedUpdateController();

        ClearFixedState();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }
    #endregion


    #region State Machine Helper Functions

    public void SetMoveState(MoveState moveState)
    {
        //Break out early if we are not transitioning states 
        if (moveState == this.moveState)
        {
            return;
        }
        if (!controllers.ContainsKey(moveState))
        {
            Debug.LogError($"MoveState {moveState} has not been added to controllers yet");
            return;
        }

        //Transition FSM state
        controllers[this.moveState].ExitState();
        this.moveState = moveState;
        controllers[this.moveState].EnterState();
    }

    #endregion


    #region Update State Value Functions

    private void UpdateSharedValues()
    {
        UpdateDesiredMotion();
        UpdateSprintValues();
        UpdateCrouchValues();
    }

    private void UpdateFixedState()
    {
        UpdateImpactAnimationValues();
        stepsSinceLastGrounded++;

        if (contactObject)
        {
            if (contactObject.isKinematic || contactObject.mass >= rb.mass)
            {
                UpdateContactState();
            }
        }
    }

    private Vector3 UpdateDesiredMotion()
    {
        desiredMotion = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rotatedMotion = (Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * desiredMotion).normalized;
        return desiredMotion;
    }

    private void UpdateImpactAnimationValues()
    {
        averageYVelocity = (averageYVelocity + rb.velocity.y) / 2f;
    }

    private bool UpdateSprintValues()
    {
        sprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        return sprinting;
    }

    private bool UpdateCrouchValues()
    {
        crouching = (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl));
        return crouching;
    }

    private void UpdateContactState()
    {
        if (contactObject == previousContactObject)
        {
            contactTranslation = contactObject.position - contactWorldPosition;
            contactVelocity = contactTranslation / Time.fixedDeltaTime;
        }
        contactWorldPosition = contactObject.position;
    }

    private void ClearFixedState()
    {
        grounded = false;
        climbContactCount = 0;
        previousContactObject = contactObject;
        contactNormal = contactVelocity = climbNormal = Vector3.zero;
        contactObject = null;
    }

    #endregion


    #region Helper Functions

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            //TODO: We don't need to do it everytime, we can precompute it, however for testing we have it here atm
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);

            if (normal.y >= minGroundDotProduct)
            {
                grounded = true;
                contactNormal = (contactNormal + normal).normalized;
                contactObject = collision.rigidbody;
                stepsSinceLastGrounded = 0;
            }
            else if (normal.y >= minClimbDotProduct)
            {
                climbContactCount += 1;
                climbNormal = normal;
                wallDir = Vector3.Cross(normal, Vector3.up);
            }
        }
    }

    public bool canWallRun()
    {

        //controller.rb.AddForce()

        //We know walls normal
        //We know forward on camera,

        //We want to move forward in camera direction until desired velocity is 0 or

        //Wallrun entrance requirements
        //Speed > val
        //Wall
        //movement is into wall + at least n degrees away from it

        //rotatedMotion


        //From player to wall we want to project the vector



        return climbContactCount > 0;
    }

    #endregion


    #region Shared Functions
    public void StartJumpCoroutines()
    {
        StopCoroutine(JumpForceCoroutine());
        StopCoroutine(JumpActiveTurningCoroutine());
        StartCoroutine(JumpForceCoroutine());
        StartCoroutine(JumpActiveTurningCoroutine());
    }

    public IEnumerator JumpForceCoroutine()
    {
        float activeTimer = 0.0f;
        while (activeTimer <= jumpHoldTime)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                activeTimer += Time.fixedDeltaTime;
                rb.AddForce(Vector3.up * jumpHoldForce * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield break;
            }
        }
    }

    public IEnumerator JumpActiveTurningCoroutine()
    {
        float activeTimer = 0.0f;
        while (activeTimer <= jumpActiveTurningTimeWindow)
        {
            //Get angle between current velocity and desired angle
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            float magnitude = velocity.magnitude;
            velocity.Normalize();

            float angle = Vector3.Angle(rotatedMotion, velocity);

            if (desiredMotion == Vector3.zero)
            {

            }
            else if (angle < jumpActiveTurningAngle && magnitude > 0.1f && angle > 0.01f)
            {
                //Why is small deviation happening
                Debug.Log($"Rot {rotatedMotion} Vel {velocity} Angle {Vector3.Angle(rotatedMotion, velocity)} Mag {magnitude} Force {magnitude * (rotatedMotion - velocity)}");
                rb.AddForce(magnitude * (rotatedMotion - velocity), ForceMode.Impulse);
            }
            //else if (Vector3.Angle(rotatedMotion, velocity) > jumpActiveStoppingAngle)
            //{
            //    rb.AddForce(-velocity * magnitude * jumpActiveTurningStopSpeed, ForceMode.Impulse);
            //    yield break;
            //}

            activeTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}