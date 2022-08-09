using System.Collections.Generic;
using UnityEngine;

public class PhysicsGhostComponent
{
    public Dictionary<int, PhysicsState> historicalState = new Dictionary<int, PhysicsState>();
}
