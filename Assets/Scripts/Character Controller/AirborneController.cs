using System.Collections;
using UnityEngine;

public class AirborneController : StateCharacterController
{
    #region State Character Controller
    private bool jump;
    public AirborneController(FPController controller) : base(controller)
    {

    }

    public override void UpdateController() 
    {
        jump = Input.GetKeyDown(KeyCode.Space) || jump;
    }

    public override void FixedUpdateController() {
        if(jump)
        {
            AirJump();
            jump = false;
        }

        controller.rb.AddForce(AirAccelerate(controller.rotatedMotion.normalized, controller.airMovementForce), ForceMode.VelocityChange);

        if (controller.climbContactCount > 0) //todo also add ortho movement is > val
        {
            //controller.SetMoveState(MoveState.WallRun);
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
        //Debug.Log($"1 {controller.rb.velocity.normalized}");

        controller.rb.velocity = Vector3.zero;

        //Debug.Log($"2 {controller.rb.velocity.normalized}");


        controller.rb.AddForce(Vector3.up * controller.airJumpForce, ForceMode.VelocityChange);
        controller.rb.AddForce(controller.rotatedMotion * controller.airJumpHorizontalForce, ForceMode.VelocityChange);
        //Debug.Log($"3 {controller.rotatedMotion * controller.airJumpHorizontalForce}");

        //Debug.Log($"4 {controller.rb.velocity.normalized}");


        controller.StartJumpCoroutines();
    }

    private void ImpactTransition()
    {
        controller.firstPersonCameraController.PlayImpactAnimation(Mathf.Abs(controller.averageYVelocity) / controller.impactAnimationForce);
    }

    #endregion
}