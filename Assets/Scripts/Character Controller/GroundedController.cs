using System.Collections;
using UnityEngine;

public class GroundedController : StateCharacterController
{
    private Vector3 previousPosition;
    private bool jump;

    public GroundedController(FPController controller) : base(controller)
    {

    }

    public override void UpdateController()
    {
        jump = Input.GetKeyDown(KeyCode.Space) || jump;
    }

    public override void FixedUpdateController() {
        if (jump)
        {
            Jump();
            jump = false;
        }

        if (controller.desiredMotion != Vector3.zero)
        {
            //Rotate move vector to be inline with camera direction and ground plane normal
            Vector3 rotatedMotion = ApplyNonConstantMovespeed(controller.desiredMotion);

            //Apply friction
            float pow = Mathf.Pow(controller.frictionStopForce, Time.fixedDeltaTime);
            Vector3 desiredVelocity = new Vector3(controller.rb.velocity.x / pow, controller.rb.velocity.y / pow, controller.rb.velocity.z / pow);
            controller.rb.AddForce(desiredVelocity - controller.rb.velocity, ForceMode.VelocityChange);

            //Add desired movement
            controller.rb.AddForce(rotatedMotion * Time.fixedDeltaTime);
        }
        else
        {
            //When user is not trying to move, apply stopping friction
            Vector3 oldVelocity = controller.rb.velocity;
            oldVelocity.y = 0;
            controller.rb.AddForce(-oldVelocity * controller.moveStopTime, ForceMode.Acceleration);
        }

        //Move rigidbody based on rigidbody we are currently standing on 
        if (controller.contactObject != null)
        {
            if (controller.transform.parent != controller.contactObject.transform)
            {
                controller.transform.parent = controller.contactObject.transform;
            }
        }
        else
        {
            controller.transform.parent = null;
        }

        //Update controller move state
        if (controller.climbContactCount > 0) //todo also add ortho movement is > val
        {
            controller.SetMoveState(MoveState.WallRun);
        }
        else if (controller.sprinting && controller.crouching && controller.rb.velocity.magnitude > controller.minSlideSpeed)
        {
            controller.SetMoveState(MoveState.Slide);
        }
        else if (!controller.grounded && !SnapToGround())
        {
            controller.SetMoveState(MoveState.Airborne);
        }

        previousPosition = controller.transform.position;
    }

    public override void EnterState() {

    }

    public override void ExitState() {

        controller.transform.parent = null;
        controller.rb.velocity = (controller.transform.position - previousPosition) / Time.fixedDeltaTime;
    }

    #region Helper Functions

    private Vector3 ApplyNonConstantMovespeed(Vector3 input)
    {
        Vector3 convertedVec = input;
        if (convertedVec.z > 0)
        {
            convertedVec.z *= (controller.sprinting) ? (controller.moveSpeedRunForward) : (controller.moveSpeedWalkForward);
        }
        else
        {
            convertedVec.z *= (controller.sprinting) ? (controller.moveSpeedRunBackwards) : (controller.moveSpeedWalkBackwards);
        }

        convertedVec.x *= (controller.sprinting) ? (controller.moveSpeedRunStrafe) : (controller.moveSpeedWalkStrafe);
        return AdjustVelocityDirection(convertedVec * ((controller.crouching)?(controller.crouchSpeedModifier):(1.0f)));
    }

    private void Jump()
    {
        controller.stepsSinceLastGrounded = controller.snappingFrames+1;
        controller.rb.AddForce(Vector3.up * controller.jumpForce, ForceMode.VelocityChange);
        controller.StartJumpCoroutines();
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - controller.contactNormal * Vector3.Dot(vector, controller.contactNormal);
    }

    Vector3 AdjustVelocityDirection(Vector3 rawDirection)
    {
        //Get the X and Z axis based on the grounded plane  
        Vector3 xAxis = ProjectOnContactPlane(controller.cameraTransform.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(controller.cameraTransform.forward).normalized;
        return xAxis * rawDirection.x + zAxis * rawDirection.z;
    }

    bool SnapToGround()
    {

        if (controller.stepsSinceLastGrounded > controller.snappingFrames)
        {
            return false;
        }
        if (controller.rb.velocity.y > controller.maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(controller.rb.position, Vector3.down, out RaycastHit hit, controller.snappingDist))
        {
            return false;
        }
        if (hit.normal.y < controller.minGroundDotProduct)
        {
            return false;
        }

        controller.contactNormal = hit.normal;
        controller.contactObject = hit.rigidbody;
        float speed = controller.rb.velocity.magnitude;
        float dot = Vector3.Dot(controller.rb.velocity, hit.normal);

        if (dot > 0f)
        {
            controller.rb.velocity = (controller.rb.velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }
    #endregion
}
