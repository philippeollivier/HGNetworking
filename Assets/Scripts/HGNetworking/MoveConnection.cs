using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveConnection
{
    public Dictionary<int, MoveObject> moveObjects = new Dictionary<int, MoveObject>();
    public int objectId = 0;
    public int WriteToPacket(int remainingBytes, Packet packet)
    {
        int size = sizeof(int);
        packet.Write(moveObjects.Count);
        foreach (int moveId in moveObjects.Keys)
        {
            if(size + sizeof(int) + 3*sizeof(int) + 4*sizeof(int) < remainingBytes)
            {
                packet.Write(moveId);
                packet.Write(moveObjects[moveId].gameObject.transform.position);
                packet.Write(moveObjects[moveId].gameObject.transform.rotation);
                size += sizeof(int) + 3 * sizeof(int) + 4 * sizeof(int);
            } else
            {
                Debug.Log("Holy moly you're writing a lot of information with the move manager. Packet is full didn't send enough");
            }

        }
        return size;
    }
}
