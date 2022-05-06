using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class TestScript : MonoBehaviour
{
    public int frameID;
    public SlidingWindow sw = new SlidingWindow(10, false);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sw.FillFrame(frameID);
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            sw.AdvancePointer();
        }
    }
}
