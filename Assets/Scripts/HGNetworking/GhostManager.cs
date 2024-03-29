using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager
{
    public const int NEWFLAG = 1; //1
    public const int DELFLAG = 1 << 1; //10
    public const int POSFLAG = 1 << 2; //100
    public const int SCALEFLAG = 1 << 3; //1000
    public const int ROTFLAG = 1 << 4; //10000

    private int ghostIndex = 0;

    public enum GhostType
    {
        CubeGhost,
        Player,
        Ball
    }

    public Dictionary<int, List<GhostState>> ghostStates = new Dictionary<int, List<GhostState>>();
    
    public void Connect()
    {
        //foreach (Ghost ghost in ghosts.Values)
        //{
        //    ghost.NewPlayer(connectionId);
        //}
    }



    public class GhostConnection
    {
        public int WriteToPacket(Packet packet, int remainingBytes)
        {
            //if (HasMoreDataToWrite(connectionId) && active && remainingBytes > 1)
            //{
            //    int size = 1;
            //    List<GhostState> ghostsToWrite = new List<GhostState>();
            //    //Go through each ghost's list
            //    foreach (Ghost ghost in ghosts.Values)
            //    {
            //        //If it has unACKED changes
            //        if (ghost.flags[connectionId] > 0)
            //        {
            //            int tempGhostSize = GetPacketSize(ghost.flags[connectionId]);
            //            //Write the ghost if there is space
            //            if (remainingBytes >= size + tempGhostSize)
            //            {
            //                ghostsToWrite.Add(AddState(packet.PacketHeader.packetId, ghost));
            //                size += tempGhostSize;
            //            }
            //        }
            //    }
            //    packet.Write(ghostsToWrite.Count);
            //    foreach(GhostState ghost in ghostsToWrite)
            //    {
            //        WriteGhostToPacket(ghost, packet);
            //    }
            //    return size;
            //} else
            //{
            //    packet.Write(0);
            //    return 1;
            //}
            return 0;
        }

        public GhostState AddState(int packetId, Ghost ghost)
        {
            //GhostState state = new GhostState(ghost, connectionId);
            //if(!ghostStates.ContainsKey(packetId) || ghostStates[packetId] == null)
            //{
            //    ghostStates[packetId] = new List<GhostState>();
            //}
            //ghostStates[packetId].Add(state);
            //ghost.flags[connectionId] = 0;
            //return state;
            return null;
        }

        private int GetPacketSize(int flags)
        {
            ////Ghost Id and Flags cast to Byte
            //int size = sizeof(int) + sizeof(byte);
            //if ((flags & NEWFLAG) > 0)
            //{
            //    size += sizeof(int);
            //}
            //if((flags & DELFLAG) > 0)
            //{
            //    //Delete doesn't add any data
            //}
            //if ((flags & POSFLAG) > 0)
            //{
            //    size += 3 * sizeof(float);
            //}
            //if ((flags & SCALEFLAG) > 0)
            //{
            //    size += 3 * sizeof(float);
            //}
            //if ((flags & ROTFLAG) > 0)
            //{
            //    size += 4 * sizeof(float);
            //}
            //return size;
            return 0;
        }
        private void WriteGhostToPacket(GhostState ghost, Packet packet)
        {
            //packet.Write(ghost.ghostId);
            //packet.Write((byte)ghost.flags);
            //if ((ghost.flags & NEWFLAG) > 0)
            //{
            //    packet.Write((int)ghost.ghostType);
            //}
            //if ((ghost.flags & DELFLAG) > 0)
            //{
            //    //Delete doesn't add any data
            //    return;
            //}
            //if ((ghost.flags & POSFLAG) > 0)
            //{
            //    packet.Write(ghost.position);
            //}
            //if ((ghost.flags & SCALEFLAG) > 0)
            //{
            //    packet.Write(ghost.scale);
            //}
            //if ((ghost.flags & ROTFLAG) > 0)
            //{
            //    packet.Write(ghost.rotation);
            //}
        }

        public void ProcessNotification(bool success, int packetId)
        {
            //if(success)
            //{
            //    ghostStates.Remove(packetId);
            //} 
            //else if(ghostStates.ContainsKey(packetId))
            //{
            //    foreach(GhostState state in ghostStates[packetId])
            //    {
            //        ghosts[state.ghostId].flags[connectionId] = ghosts[state.ghostId].flags[connectionId] | state.flags;
            //    }
            //}
        }

    }

    public class GhostState
    {
        public int ghostId;
        public int flags = 0;
        public GhostType ghostType = GhostType.CubeGhost;
        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);
        public Quaternion rotation = new Quaternion(0, 0, 0, 0);
        public GhostState(Ghost ghost, int connectionId)
        {
            ghostId = ghost.ghostId;
            flags = ghost.flags[connectionId];
            ghostType = ghost.ghostType;
            position = ghost.Position;
            scale = ghost.Scale;
            rotation = ghost.Rotation;
        }
    }


    public Dictionary<int, GhostConnection> ghostConnections = new Dictionary<int, GhostConnection>();
    public Dictionary<int, Ghost> ghosts = new Dictionary<int, Ghost>();
    Dictionary<GhostType, objectType> objectAssociation = new Dictionary<GhostType, objectType>();
    Dictionary<GhostType, objectType> clientObjectAssociation = new Dictionary<GhostType, objectType>();

    public Dictionary<int, Ghost> localGhosts = new Dictionary<int, Ghost>();
    public GameObject[] prefabs;

   

    public bool HasMoreDataToWrite()
    {
        //foreach (Ghost ghost in ghosts.Values)
        //{
        //    if (ghost.flags.ContainsKey(connectionId) && ghost.flags[connectionId] > 0)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }

    public Ghost NewGhost(GhostType ghostType, Vector3 position)
    {
        Ghost ghost = ObjectManager.Instance.CreateObject(objectAssociation[ghostType]).GetComponent<Ghost>();
        ghost.Initialize(ghostIndex, ghostType);
        ghost.gameObject.transform.position = position;
        ghosts[ghostIndex] = ghost;
        ghostIndex++;
        return ghost;
    }

    public Ghost ApplyGhostToObject(GameObject newGhost, GhostType ghostType)
    {
        Ghost ghost = newGhost.AddComponent<Ghost>();
        ghost.Initialize(ghostIndex, ghostType);
        ghosts[ghostIndex] = ghost;
        ghostIndex++;
        return ghost;
    }
      
    public Ghost NewGhostClient(GhostType ghostType, int ghostId)
    {
        Ghost ghost = ObjectManager.Instance.CreateObject(clientObjectAssociation[ghostType]).GetComponent<Ghost>();
        ghost.Initialize(ghostId);
        localGhosts[ghostId] = ghost;
        ghost.onClient = true;
        return localGhosts[ghostId];
    }
    public void ReadFromPacket(int connectionId, Packet packet)
    {
        int numGhosts = packet.ReadInt();
        for (int i = 0; i < numGhosts; i++)
        {
            int ghostId = packet.ReadInt();
            int flags = packet.ReadByte();
            if ((flags & NEWFLAG) > 0)
            {
                NewGhostClient((GhostType)packet.ReadInt(), ghostId);
            }
            if ((flags & DELFLAG) > 0)
            {
                ObjectManager.Destroy(localGhosts[ghostId].gameObject);
                return;
            }
            if ((flags & POSFLAG) > 0)
            {
                Vector3 postiion = packet.ReadVector3();
                if (!localGhosts[ghostId].isControlled) {
                    localGhosts[ghostId].gameObject.transform.position = postiion;
                }
            }
            if ((flags & SCALEFLAG) > 0)
            {
                Vector3 scale = packet.ReadVector3();
                if (!localGhosts[ghostId].isControlled)
                {

                    localGhosts[ghostId].gameObject.transform.localScale = scale;
                }
            }
            if ((flags & ROTFLAG) > 0)
            {
                Quaternion rotation = packet.ReadQuaternion();
                if (!localGhosts[ghostId].isControlled)
                {
                    localGhosts[ghostId].gameObject.transform.rotation = rotation;
                }
            }
        } 
    }

    public void ProcessNotification(bool success, int packetId, int connectionId)
    {
        ghostConnections[connectionId].ProcessNotification(success, packetId);
    }

    public void Initialize()
    {
        objectAssociation[GhostType.CubeGhost] = objectType.CubeGhost;
        clientObjectAssociation[GhostType.CubeGhost] = objectType.CubeGhost;
        objectAssociation[GhostType.Player] = objectType.ServerPlayer;
        clientObjectAssociation[GhostType.Player] = objectType.ClientPlayer;
        objectAssociation[GhostType.Ball] = objectType.ServerBall;
        clientObjectAssociation[GhostType.Ball] = objectType.ClientBall;
    }
}
