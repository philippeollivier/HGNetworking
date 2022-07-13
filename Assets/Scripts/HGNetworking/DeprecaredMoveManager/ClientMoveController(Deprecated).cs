//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ClientMoveController : MonoBehaviour
//{
//    public Dictionary<int, MoveObject> controlObjects;
//    public SlidingWindow window = new SlidingWindow(64, true);
//    public Dictionary<int, List<KeybindingActions>> moveLists = new Dictionary<int, List<KeybindingActions>>();
//    public Dictionary<int, List<KeybindingActions>>[] moveFrames = new Dictionary<int, List<KeybindingActions>>[2*64];
//    public int[] sendsRemaining = new int[2 * 64];
//    /* #MoveLists
//     * moveId
//     * #object
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     * moveId
//     * #object
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     * moveId
//     * #object
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     * object Id
//     * numMoves
//     * move1
//     * move2
//     * move3
//     */

//    private InputManager inputManager;
//    private void Start()
//    {
//        inputManager = InputManager.Instance;
//    }
//    // Update is called once per frame
//    void Update()
//    {   
//        if(inputManager.GetKeyDown(KeybindingActions.Forward))
//        {
//            foreach(MoveObject controlObject in controlObjects.Values)
//            {
//                if(controlObject.selected)
//                {
//                    controlObject.KeyDown(KeybindingActions.Forward);
//                }
//            }
//        }
//    }

//    public void GiveControlOfGhost(int ghostId, int moveId)
//    {
//        MoveObject controller = GhostManager.ghosts[ghostId].gameObject.AddComponent<MoveObject>();
//        controller.Initialize(ghostId, moveId, true);
//        //This should pass by reference
//        moveLists[moveId] = controller.GetMoves();
//        controlObjects[moveId] = controller;
//    }

//    public int WriteToPacket(int remainingBytes, Packet packet)
//    {
//        int frameId = window.AdvancePointer();
//        if (HasMoreDataToWrite() && remainingBytes > 1 && frameId != -1)
//        {
//            int size = 1;
//            List<int> movesToWrite = new List<int>();

//            moveFrames[frameId] = new Dictionary<int, List<KeybindingActions>>();
//            foreach (int moveIndex in moveLists.Keys)
//            {
//                if(moveLists[moveIndex].Count > 0)
//                {
//                    moveFrames[frameId][moveIndex] = moveLists[moveIndex].ConvertAll(move => move);
//                }
//            }


//            for (int i = 0; i < moveFrames.Length; i++)
//            {
//                if(sendsRemaining[i] > 0)
//                {

//                }
//            }
//            packet.Write(movesToWrite.Count);
//            packet.Write(frameId);
//            foreach(int moveIndex in movesToWrite)
//            {
//                packet.Write(moveIndex);
//                packet.Write(moveLists[moveIndex].Count);
//                foreach(int move in moveLists[moveIndex])
//                {
//                    packet.Write(move);
//                }

                
//            }
//            foreach (MoveObject controlObject in controlObjects.Values)
//            {
//                controlObject.ClearMoves();
//            }
//            return size;
//        } else
//        {
//            packet.Write(0);
//            return 1;
//        }


//    }

//    public bool HasMoreDataToWrite()
//    {
//        foreach(List<KeybindingActions> moveList in moveLists.Values)
//        {
//            if(moveList.Count > 0)
//            {
//                return true;
//            }
//        }
//        for(int i = 0; i < sendsRemaining.Length; i++)
//        {
//            if(sendsRemaining[i] > 0)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public int GetMoveListSize(int frameId)
//    {
//        int size = 0;
//        foreach(int moveId in moveFrames[frameId].Keys)
//        {
//            size += 3 * sizeof(int) + moveFrames[frameId][moveId].Count * sizeof(int);
//        }
//        //MoveId + #Objects + #Move/Object + Moves/Object
//        return size;
//    }
//}
