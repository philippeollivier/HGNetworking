using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    enum MoveState
    {
        Grounded,
        Air,
        WallRun,
        WallClimb,
        WallVault,
        Slide,
        Ragdoll
    }

    private MoveState moveState;
    

    void Update()
    {
        MoveStateBasedUpdate();
    }


    void MoveStateBasedUpdate()
    {
        switch (moveState)
        {
            case MoveState.Grounded:
                UpdateMoveStateGrounded();
                break;
            case MoveState.Air:
                break;
            case MoveState.WallRun:
                break;
            case MoveState.WallClimb:
                break;
            case MoveState.WallVault:
                break;
            case MoveState.Slide:
                break;
            case MoveState.Ragdoll:
                break;
        }
    }

    #region Move State Updates 
    void UpdateMoveStateGrounded()
    {

    }
    #endregion
}
