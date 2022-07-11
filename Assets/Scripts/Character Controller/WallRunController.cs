using System.Collections;
using UnityEngine;

public class WallRunController : StateCharacterController
{
    private bool addedWallYForce = false, onWall = true, newWall = false;
    private Vector3 prevWallDir;

    #region State Character Controller 
    public WallRunController(FPController controller) : base(controller)
    {

    }

    public override void UpdateController() {

    }

    public override void FixedUpdateController() {
        //Code yoinked from https://github.com/Faboover/Unity-FP-Platformer-Project/blob/master/FP%20TF%20TT%20Platformer/Assets/Scripts/CharacterControls.cs

        float angleFacing = Vector3.Angle(controller.wallDir, controller.transform.forward);
        float angleDif = Vector3.Angle(controller.wallDir, controller.rotatedMotion);

        // To allow movement away from the wall in the given range based on player's intended direction of movement
        if (angleDif > 45 && angleDif < 135 && controller.desiredMotion != Vector3.zero)
        {
            controller.SetMoveState(MoveState.Airborne);
            return;
        }

        // Calculate how fast we should be moving in along the wall
        Vector3 targetVelocity = Vector3.zero;
        // How should we use the players input based on the angle of the character from the wall.
        if (angleFacing <= 45 || angleFacing >= 135)
        {
            targetVelocity = Vector3.Scale(controller.wallDir, new Vector3(controller.desiredMotion.z, 0, controller.desiredMotion.z));
        }

        targetVelocity *= controller.wallRunSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = controller.rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -controller.maxVelocityChange, controller.maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -controller.maxVelocityChange, controller.maxVelocityChange);
        velocityChange.y = 0;

        // If there is a new wall found while moving along a wall, adjust player movement direction to meet the new wall's direction
        // Else Move player along the current wall
        if (newWall && !Input.GetButtonDown("Jump"))
        {
            controller.rb.AddForce(velocityChange, ForceMode.VelocityChange);
            newWall = false;
        }
        else
        {
            controller.rb.AddForce(velocityChange * controller.wallRunSpeed);

            // Have it so a upwards or downwards force is added when player first connects to a wall
            // Once done, don't add this force again until player is off a wall and on a new wall
            if (!addedWallYForce)
            {
                float yForce = 2500f;

                // If the player's y velocity is high/low enough add a upward/downward force to compensate
                if (controller.rb.velocity.y > 5)
                {
                    if (controller.rb.velocity.y > 10)
                    {
                        controller.rb.AddForce(new Vector3(0, -yForce * 2, 0));
                    }
                    else
                    {
                        controller.rb.AddForce(new Vector3(0, -yForce, 0));
                    }
                }
                else if (controller.rb.velocity.y < -2)
                {
                    if (controller.rb.velocity.y < -10)
                    {
                        controller.rb.AddForce(new Vector3(0, yForce * 2, 0));
                    }
                    else
                    {
                        controller.rb.AddForce(new Vector3(0, yForce, 0));
                    }
                }

                addedWallYForce = true;
            }
        }


        //TODO If the player is not turning the camera left or right and the player is moving, have the camera roation adjusted
        //if (controller.desiredMotion != Vector3.zero)
        //{
        //    AdjustRotation();
        //}

        if (controller.climbContactCount == 0 || controller.desiredMotion == Vector3.zero)
        {
            if (controller.grounded)
            {
                controller.SetMoveState(MoveState.Grounded);
                return;
            }

            controller.SetMoveState(MoveState.Airborne);
            return;
        }
    }

    public override void EnterState() {
        controller.rb.useGravity = false;
    }

    public override void ExitState() {
        controller.rb.useGravity = true;
    }

    #endregion

    #region Helper Functions

    #endregion
}
