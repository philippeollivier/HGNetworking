using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private List<int> currentMoves = new List<int>();
    public int ghostId;
    public int moveId;
    public bool onClient;
    public bool selected;
    public void Initialize(int ghostId, int moveId, bool onClient)
    {
        this.ghostId = ghostId;
        this.moveId = moveId;
        this.onClient = onClient;
        selected = true;
    }

    public List<int> GetMoves()
    {
        return currentMoves;
    }

    public void ClearMoves()
    {
        currentMoves.Clear();
    }

    public void KeyDown(KeybindingActions key)
    {

    }

    public void KeyUp(KeybindingActions key)
    {

    }

    public void Key(KeybindingActions key)
    {
        if (key == KeybindingActions.Forward)
        {
            transform.Translate(transform.forward);
            currentMoves.Add((int)KeybindingActions.Forward);
        }
    }
}
