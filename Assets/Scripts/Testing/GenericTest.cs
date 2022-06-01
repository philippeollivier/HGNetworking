using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTest : MonoBehaviour
{
    public SlidingWindow sw = new SlidingWindow(64, true);
    public int valueToAdd = 0;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)){
            sw.FillFrame(valueToAdd);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            sw.AdvancePointer();
        }
    }
}
