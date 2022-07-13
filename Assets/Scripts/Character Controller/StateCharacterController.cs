using System.Collections;
using UnityEngine;


public abstract class StateCharacterController
{
    protected FPController controller;

    public StateCharacterController(FPController controller)
    {
        this.controller = controller;
    }

    public abstract void UpdateController();
    public abstract void FixedUpdateController();
    public abstract void EnterState();
    public abstract void ExitState();
}
