using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public int ghostId;
    public int moveId;
    public bool onClient;
    public void Initialize(int ghostId, int moveId, bool onClient)
    {
        this.ghostId = ghostId;
        this.moveId = moveId;
        this.onClient = onClient;
    }



}
