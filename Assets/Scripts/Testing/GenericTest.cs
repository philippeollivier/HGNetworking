using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTest : MonoBehaviour
{
    public SlidingWindow sw = new SlidingWindow(10, true);
    public int frameToFill = 0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)){
            Debug.Log(sw.FillFrame(frameToFill));
            frameToFill++;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            sw.AdvancePointer();
        }
    }
}
