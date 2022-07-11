using System.Collections;
using UnityEngine;

public class SlideController : StateCharacterController
{
    #region State Character Controller 
    public SlideController(FPController controller) : base(controller)
    {

    }

    public override void UpdateController() {

    }

    public override void FixedUpdateController() {
        //Apply friction if sliding on the ground
        if (controller.grounded)
        {
            float pow = Mathf.Pow(controller.slideFrictionStopForce, Time.fixedDeltaTime);
            Vector3 desiredVelocity = new Vector3(controller.rb.velocity.x / pow, controller.rb.velocity.y / pow, controller.rb.velocity.z / pow);
            controller.rb.AddForce(desiredVelocity - controller.rb.velocity, ForceMode.VelocityChange);
        }

        if (controller.rb.velocity.magnitude < controller.minSlideSpeed || !controller.crouching)
        {
            controller.SetMoveState(MoveState.Airborne);
        }
    }

    public override void EnterState() {
        //Gain burst of speed when sliding
        controller.rb.AddForce(controller.rotatedMotion.normalized * controller.slideForce, ForceMode.VelocityChange);
    }

    public override void ExitState() {

    }

    #endregion

    #region Helper Functions



    #endregion
}
