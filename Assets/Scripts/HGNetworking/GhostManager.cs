using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GhostManager
{
    private static int ghostIndex = 0;
    public enum ghostType
    {
        TestGhost
    }

    public const int NEWFLAG = 1; //1
    public const int DELFLAG = 1 << 1; //10
    public const int POSFLAG = 1 << 2; //100
    public const int SCALEFLAG = 1 << 3; //1000
    public const int ROTFLAG = 1 << 4; //10000

    public class GhostConnection
    {
        public int connectionId;
        public bool hasMoreDataToWrite = false;
        public bool active = false;
        public Dictionary<int, List<GhostState>> ghostStates = new Dictionary<int, List<GhostState>>();
        public void Connect(int connectionId)
        {
            Debug.Log($"Connection: {connectionId} connected to GhostManager");
            this.active = true;
            this.connectionId = connectionId;
            foreach (Ghost ghost in ghosts.Values)
            {
                ghost.NewPlayer(connectionId);
                
            }
        }
        public void Disconnect()
        {
            this.active = false;
            this.connectionId = -1;
        }
        public int WriteToPacket(Packet packet, int remainingBytes)
        {
            int numGhosts = 0;
            //Add latest ghost data to lists
            foreach (Ghost ghost in ghosts.Values)
            {
                if(ghost.flags[connectionId] > 0)
                {
                    numGhosts++;
                }
            }
            Debug.Log($"Writing data for {numGhosts} ghosts");
            packet.Write(numGhosts);
            if (numGhosts > 0)
            {
                int size = 0;
                //Go through each ghost's list
                foreach (Ghost ghost in ghosts.Values)
                {
                    //If it has unACKED changes
                    if (ghost.flags[connectionId] > 0)
                    {
                        size += GetPacketSize(ghost.flags[connectionId]);
                        //Write the ghost if there is space
                        if (remainingBytes - size >= 0)
                        {
                            remainingBytes -= size; 
                            WriteGhostToPacket(AddState(packet.PacketHeader.packetId, ghost), packet);
                        }
                        else
                        {
                            hasMoreDataToWrite = true;
                        }
                    }
                }
                Debug.Log($"Wrote: {numGhosts} to packet: {packet.PacketHeader.packetId}");
                return size;
            } else
            {
                hasMoreDataToWrite = false;
                return 0;
            }
            
        }

        public GhostState AddState(int packetId, Ghost ghost)
        {
            GhostState state = new GhostState(ghost, connectionId);
            if(ghostStates[packetId] == null)
            {
                ghostStates[packetId] = new List<GhostState>();
            }
            ghostStates[packetId].Add(state);
            ghost.flags[connectionId] = 0;
            return state;
        }

        private int GetPacketSize(int flags)
        {
            int size = 2 * sizeof(int);
            if ((flags & NEWFLAG) > 0)
            {
                size += sizeof(int);
            }
            if((flags & DELFLAG) > 0)
            {
                //Delete doesn't add any data
            }
            if ((flags & POSFLAG) > 0)
            {
                size += 3 * sizeof(float);
            }
            if ((flags & SCALEFLAG) > 0)
            {
                size += 3 * sizeof(float);
            }
            if ((flags & ROTFLAG) > 0)
            {
                size += 4 * sizeof(float);
            }
            return size;
        }
        private void WriteGhostToPacket(GhostState ghost, Packet packet)
        {
            packet.Write(ghost.ghostId);
            Debug.Log($"Packet: {packet.PacketHeader.packetId} | Writing {ghost.ghostId} with flags: {ghost.flags}");
            packet.Write((byte)ghost.flags);
            if ((ghost.flags & NEWFLAG) > 0)
            {
                packet.Write((int)ghost.ghostType);
            }
            if ((ghost.flags & DELFLAG) > 0)
            {
                //Delete doesn't add any data
                return;
            }
            if ((ghost.flags & POSFLAG) > 0)
            {
                packet.Write(ghost.position);
            }
            if ((ghost.flags & SCALEFLAG) > 0)
            {
                packet.Write(ghost.scale);
            }
            if ((ghost.flags & ROTFLAG) > 0)
            {
                packet.Write(ghost.rotation);
            }
        }

        public void ProcessNotification(bool success, int packetId)
        {
            if(success)
            {

                ghostStates[packetId] = null;
            } else if(ghostStates.ContainsKey(packetId))
            {
                foreach(GhostState state in ghostStates[packetId])
                {
                    ghosts[state.ghostId].flags[connectionId] = ghosts[state.ghostId].flags[connectionId] | state.flags;
                }

            }
        }

    }

    public class GhostState
    {
        public int ghostId;
        public int flags = 0;
        public ghostType ghostType = ghostType.TestGhost;
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


    public static Dictionary<int, GhostConnection> ghostConnections = new Dictionary<int, GhostConnection>();
    public static Dictionary<int, Ghost> ghosts = new Dictionary<int, Ghost>();
    static Dictionary<ghostType, objectType> objectAssociation = new Dictionary<ghostType, objectType>();
    public static GameObject[] prefabs;
    public static void Connect(int connectionId)
    {
        ghostConnections[connectionId].Connect(connectionId);
    }

    public static void Disconnect(int connectionid)
    {
        ghostConnections[connectionid].Disconnect();
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, Packet packet)
    {
        return ghostConnections[connectionId].WriteToPacket(packet, remainingBytes);
    }

    public static bool HasMoreDataToWrite(int connectionId)
    {
        if(ghostConnections[connectionId].hasMoreDataToWrite)
        {
            Debug.Log($"Connection {connectionId} has more data to write");
        }
        return ghostConnections[connectionId].hasMoreDataToWrite;
    }

    public static Ghost NewGhost(ghostType ghostType)
    {
        Debug.Log($"Creating ghost with ghostId: {ghostIndex}");
        Ghost ghost = ObjectManager.CreateObject(objectAssociation[ghostType.TestGhost]).GetComponent<Ghost>();
        ghost.Initialize(ghostIndex, ghostType);
        ghosts[ghostIndex] = ghost;
        ghostIndex++;
        return ghost;
    }

    public static void ReadFromPacket(int connectionId, Packet packet)
    {
        Debug.Log($"Reading information about ghost");
        int numGhosts = packet.ReadInt();
        for(int i = 0; i < numGhosts; i++)
        {
            int ghostId = packet.ReadInt();
            int flags = packet.ReadByte();
            if ((flags & NEWFLAG) > 0)
            {
                NewGhost((ghostType)packet.ReadInt());
            }
            if ((flags & DELFLAG) > 0)
            {
                ObjectManager.Destroy(ghosts[ghostId]);
                return;
            }
            if ((flags & POSFLAG) > 0)
            {
                ghosts[ghostId].gameObject.transform.position = packet.ReadVector3();
            }
            if ((flags & SCALEFLAG) > 0)
            {
                ghosts[ghostId].gameObject.transform.localScale = packet.ReadVector3();
            }
            if ((flags & ROTFLAG) > 0)
            {
                ghosts[ghostId].gameObject.transform.rotation = packet.ReadQuaternion();
            }
        } 
    }

    public static void ProcessNotification(bool success, int packetId, int connectionId)
    {
        ghostConnections[connectionId].ProcessNotification(success, packetId);
    }

    public static void Initialize()
    {
        objectAssociation[ghostType.TestGhost] = objectType.TestGhost;
    }
}
