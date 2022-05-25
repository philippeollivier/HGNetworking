using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public int flags = 0;
    public int ghostId;
    public GhostManager.ghostType ghostType = 0;
    public Vector3 Position
    {
        get { return position; }
        set
        {
            flags = flags | GhostManager.POSFLAG;
            position = value;
        }
    }
    public Vector3 Scale
    {
        get { return scale; }
        set
        {
            flags = flags | GhostManager.SCALEFLAG;
            scale = value;
        }
    }
    public Quaternion Rotation
    {
        get { return rotation; }
        set
        {
            flags = flags | GhostManager.ROTFLAG;
            rotation = value;
        }
    }

    public void Initialize(int ghostId, GhostManager.ghostType ghostType)
    {
        this.ghostId = ghostId;
        this.ghostType = ghostType;
        flags = flags | GhostManager.NEWFLAG | GhostManager.POSFLAG | GhostManager.SCALEFLAG | GhostManager.ROTFLAG;

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
