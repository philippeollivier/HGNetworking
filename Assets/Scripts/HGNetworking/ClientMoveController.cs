using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMoveController : MonoBehaviour
{
    public Dictionary<int, MoveObject> controlObjects;
    public Dictionary<int, List<int>> listOfMoves = new Dictionary<int, List<int>>();
    public SlidingWindow window = new SlidingWindow(64, true);
    public Dictionary<int, List<int>>[] moveFrames = new Dictionary<int, List<int>>[128];
    /*
     * #object
     * object Id
     * numMoves
     * move1
     * move2
     * move3
     * object
     * numMoves
     * move1
     * move2
     * move3
     */

    private InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager.Instance;
    }
    // Update is called once per frame
    void Update()
    {   
        if(inputManager.GetKeyDown(KeybindingActions.Forward))
        {
            foreach(MoveObject controlObject in controlObjects.Values)
            {
                if(controlObject.selected)
                {
                    controlObject.KeyDown(KeybindingActions.Forward);
                }
            }
        }
    }

    public void GiveControlOfGhost(int ghostId, int moveId)
    {
        MoveObject controller = GhostManager.ghosts[ghostId].gameObject.AddComponent<MoveObject>();
        controller.Initialize(ghostId, moveId, true);
        //This should pass by reference
        listOfMoves[controller.moveId] = controller.GetMoves();
        controlObjects[moveId] = controller;
    }

    public int WriteToPacket(int remainingBytes, Packet packet)
    {

        if (HasMoreDataToWrite() && remainingBytes > 1)
        {
            int size = 1;
            List<int> movesToWrite = new List<int>();

            foreach(int moveIndex in listOfMoves.Keys)
            {
                if(listOfMoves[moveIndex].Count > 0)
                {
                    int tempMoveSize = GetMoveListSize(moveIndex);
                    if(remainingBytes >= size + tempMoveSize)
                    {
                        movesToWrite.Add(moveIndex);
                        size += tempMoveSize;
                    }
                }
            }
            packet.Write(movesToWrite.Count);
            foreach(int moveIndex in movesToWrite)
            {
                packet.Write(moveIndex);
                packet.Write(listOfMoves[moveIndex].Count);
                foreach(int move in listOfMoves[moveIndex])
                {
                    packet.Write(move);
                }
            }
            foreach (MoveObject controlObject in controlObjects.Values)
            {
                controlObject.ClearMoves();
            }
            return size;
        } else
        {
            packet.Write(0);
            return 1;
        }


    }

    public bool HasMoreDataToWrite()
    {
        foreach(List<int> moveList in listOfMoves.Values)
        {
            if(moveList.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public int GetMoveListSize(int moveId)
    {
        return sizeof(int) + listOfMoves[moveId].Count * sizeof(int);
    }
}
