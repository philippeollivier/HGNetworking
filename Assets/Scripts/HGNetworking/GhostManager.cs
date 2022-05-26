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
        public bool hasMoreDataToWrite = false;
        public bool active;
        public SlidingWindow window = new SlidingWindow(64, true);
        public Dictionary<int, List<GhostState>> ghostStates = new Dictionary<int, List<GhostState>>();
        public void Connect()
        {

        }
        public void Disconnect()
        {

        }
        public int WriteToPacket(Packet packet, int packetId, int remainingBytes)
        {
            int numGhosts = 0;
            //Add latest ghost data to lists
            foreach (int ghostId in ghostStates.Keys)
            {
                AddState(ghostId);
                if (ghostStates[ghostId].Count > 0)
                {
                    numGhosts++;
                }
            }
            packet.Write(numGhosts);
            if (numGhosts > 0)
            {
                int size = 0;
                //Go through each ghost's list
                foreach (int ghostId in ghostStates.Keys)
                {
                    //If it has unACKED changes
                    if (ghostStates[ghostId].Count > 0)
                    {
                        int flags = GetFlags(ghostId);
                        size += GetPacketSize(flags);
                        //Write the ghost if there is space
                        if (remainingBytes - size >= 0)
                        {
                            remainingBytes -= size;
                            WriteGhostToPacket(ghostId, flags, packet);
                        }
                        else
                        {
                            hasMoreDataToWrite = true;
                        }
                    }
                }
                return size;
            } else
            {
                hasMoreDataToWrite = false;
                return 0;
            }
            
        }

        public void AddState(int ghostId)
        {
            if (ghosts[ghostId].flags > 0)
            {
                ghostStates[ghostId].Add(new GhostState(ghosts[ghostId]));
            }
        }
        public void AddState(int ghostId, int flags)
        {
            GhostState state = new GhostState(ghosts[ghostId]);
            state.flags = flags;
            ghostStates[ghostId].Add(state);
        }


        private int GetFlags(int ghostId)
        {
            int flags = 0; //00000000
            ghostStates[ghostId].ForEach((GhostState ghost) =>
            {
                //10100100
                flags = flags | ghost.flags;
            });
            return flags;
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
        private void WriteGhostToPacket(int ghostId, int flags, Packet packet)
        {
            GhostState mostRecentState = ghostStates[ghostId][ghostStates[ghostId].Count-1];
            packet.Write(ghostId);
            packet.Write((byte)flags);
            if ((flags & NEWFLAG) > 0)
            {
                packet.Write((int)ghosts[ghostId].ghostType);
            }
            if ((flags & DELFLAG) > 0)
            {
                //Delete doesn't add any data
                return;
            }
            if ((flags & POSFLAG) > 0)
            {
                packet.Write(mostRecentState.position);
            }
            if ((flags & SCALEFLAG) > 0)
            {
                packet.Write(mostRecentState.scale);
            }
            if ((flags & ROTFLAG) > 0)
            {
                packet.Write(mostRecentState.rotation);
            }
        }


    }

    public class GhostState
    {
        public int ghostId;
        public int flags = 0;
        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);
        public Quaternion rotation = new Quaternion(0, 0, 0, 0);
        public GhostState(Ghost ghost)
        {
            ghostId = ghost.ghostId;
            flags = ghost.flags;
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

        ghostConnections[connectionId].Connect();
        foreach (int ghostId in ghostConnections[connectionId].ghostStates.Keys)
        {
            ghostConnections[connectionId].AddState(ghostId, 0 | NEWFLAG | POSFLAG | SCALEFLAG | ROTFLAG);
        }
    }

    public static void Disconnect(int connectionid)
    {
        ghostConnections[connectionid].Disconnect();
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, int packetId, Packet packet)
    {
        return ghostConnections[connectionId].WriteToPacket(packet, packetId, remainingBytes);
    }

    public static bool HasMoreDataToWrite(int connectionId)
    {
        return ghostConnections[connectionId].hasMoreDataToWrite;
    }

    public static Ghost NewGhost(ghostType ghostType)
    {
        Ghost ghost = ObjectManager.CreateObject(objectAssociation[ghostType.TestGhost]).GetComponent<Ghost>();
        ghost.Initialize(ghostIndex, ghostType);
        ghosts[ghostIndex] = ghost;
        foreach(GhostConnection connection in ghostConnections.Values)
        {
            connection.ghostStates[ghostIndex] = new List<GhostState>();
            connection.ghostStates[ghostIndex].Add(new GhostState(ghost));
        }
        ghostIndex++;
        return ghost;
    }

    public static void ReadFromPacket(int connectionId, int packetId, Packet packet)
    {
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

    public static void Initialize()
    {
        objectAssociation[ghostType.TestGhost] = objectType.TestGhost;
    }
}
