using System.Collections;
using UnityEngine;

public class AirborneController : StateCharacterController
{
    #region State Character Controller
    public AirborneController(FPController controller) : base(controller)
    {

    }

    public override void UpdateController() {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AirJump();
        }
    }

    public override void FixedUpdateController() {
        controller.rb.AddForce(AirAccelerate(controller.rotatedMotion.normalized, controller.airMovementForce), ForceMode.VelocityChange);

        if (controller.climbContactCount > 0) //todo also add ortho movement is > val
        {
            controller.SetMoveState(MoveState.WallRun);
        }
        else if (controller.grounded)
        {
            controller.SetMoveState(MoveState.Grounded);
        }
    }

    public override void EnterState() {

    }

    public override void ExitState() {
        //Debug.Log("Exiting airborne");
        //ImpactTransition();
    }
    #endregion


    #region Helper Functions

    private Vector3 AirAccelerate(Vector3 accelDir, float accelerate)
    {
        Vector3 prevVelocity = controller.rb.velocity;
        prevVelocity.y = 0;
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.deltaTime;

        if (projVel + accelVel > controller.airborneMaxSpeed)
        {
            accelVel = Mathf.Clamp(controller.airborneMaxSpeed - projVel, 0, controller.airborneMaxSpeed);
        }

        return accelDir * accelVel;
    }

    private void AirJump()
    {
        controller.rb.velocity = Vector3.zero;
        controller.rb.AddForce(Vector3.up * controller.airJumpForce, ForceMode.VelocityChange);
        controller.rb.AddForce(controller.rotatedMotion * controller.airJumpHorizontalForce, ForceMode.VelocityChange);
        controller.StartJumpCoroutines();
    }

    private void ImpactTransition()
    {
        FirstPersonCameraController.Instance.PlayImpactAnimation(Mathf.Abs(controller.averageYVelocity) / controller.impactAnimationForce);
    }

    #endregion
}