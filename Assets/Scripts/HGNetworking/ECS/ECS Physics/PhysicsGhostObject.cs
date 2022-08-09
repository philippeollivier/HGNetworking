using System.Collections.Generic;
using UnityEngine;

public class PhysicsGhostComponent
{
    Dictionary<int, PhysicsState> historicalState;

    //Clear all forward state
    //When we want to rewind time, we go back to frame N-x.
    //From frame N-x we simulate all X physics frames. Physics frames are also affected by stuff like the player moving, or PhysicsObjects with controllers

    //For every physics frame we want to simulate physics and then other physics forces.
}
