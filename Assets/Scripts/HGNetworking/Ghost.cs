using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Dictionary<int, int> flags = new Dictionary<int, int>();
    public int ghostId;
    public GhostManager.ghostType ghostType = 0;
    public Vector3 Position
    {
        get { return position; }
        set
        {
            List<int> connectionIds = new List<int>(flags.Keys);
            foreach(int key in connectionIds)
            {
                flags[key] = flags[key] | GhostManager.POSFLAG;
            }
            position = value;
        }
    }
    public Vector3 Scale
    {
        get { return scale; }
        set
        {
            List<int> connectionIds = new List<int>(flags.Keys);
            foreach (int key in connectionIds)
            {
                flags[key] = flags[key] | GhostManager.SCALEFLAG;
            }
            scale = value;
        }
    }
    public Quaternion Rotation
    {
        get { return rotation; }
        set
        {
            List<int> connectionIds = new List<int>(flags.Keys);
            foreach (int key in connectionIds)
            {
                flags[key] = flags[key] | GhostManager.ROTFLAG;
            }
            rotation = value;
        }
    }

    public void Initialize(int ghostId, GhostManager.ghostType ghostType)
    {
        this.ghostId = ghostId;
        this.ghostType = ghostType;
        foreach (int connectionId in GhostManager.ghostConnections.Keys)
        {
            flags[connectionId] = 0 | GhostManager.NEWFLAG | GhostManager.POSFLAG | GhostManager.SCALEFLAG | GhostManager.ROTFLAG;

        }
    }

    public void NewPlayer(int connectionId)
    {
        flags[connectionId] = 0 | GhostManager.NEWFLAG | GhostManager.POSFLAG | GhostManager.SCALEFLAG | GhostManager.ROTFLAG;
    }

    private Vector3 position = new Vector3(0, 0, 0);
    private Vector3 scale = new Vector3(1, 1, 1);
    private Quaternion rotation = new Quaternion(0, 0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Position = transform.position;
        Scale = transform.localScale;
        Rotation = transform.rotation;
    }


}
