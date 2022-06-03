using System;
using UnityEngine;
using System.Collections.Generic;


[Serializable]
public class SlidingWindow : ISerializationCallbackReceiver
{
    public enum WindowStatus { Success, Duplicate, OutOfOrder, OutofBounds, NonActiveFrame }
    [SerializeField]
    public int LeftBound { get { return leftBound; } }
    public int RightBound { get { return RightBound; } }
    public int CurrentPointer { get { return currentPointer; } }
    public bool IsFull { get => currentPointer == rightBound-1; }
    public int MaxSize { get; private set; }
    [SerializeField] private bool[] window;
    readonly bool acceptOutOfOrder;


    [SerializeField] private int leftBound;
    [SerializeField] private int rightBound;
    [SerializeField] private int currentPointer;
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {

    }
    public SlidingWindow(int maxSize, bool acceptOutOfOrder)
    {
        MaxSize = maxSize;
        leftBound = 0;
        rightBound = maxSize;
        currentPointer = 0;
        window = new bool[2 * maxSize];
        for(int i = 0; i < 2*maxSize; i++)
        {
            window[i] = false;
        }
        this.acceptOutOfOrder = acceptOutOfOrder;
    }

    public WindowStatus FillFrame(int frameId)
    {
        if (frameId >= (2 * MaxSize))
        {
            throw new System.Exception($"Frame Id {frameId} out of bounds. Max frame is {2 * MaxSize - 1}");
        }
        else if (!InWindow(frameId))
        {
            return WindowStatus.OutofBounds;
        }
        else if (window[frameId] == true)
        {
            return WindowStatus.Duplicate;
        }
        else
        {
            if (ActiveFrames(frameId))
            {
                WindowStatus ret;
                if (frameId != LeftBound)
                {
                    window[frameId] = acceptOutOfOrder;
                    ret = WindowStatus.OutOfOrder;
                } else
                {
                    window[frameId] = true;
                    ret = WindowStatus.Success;
                }

                while (window[leftBound] == true)
                {
                    window[leftBound] = false;
                    leftBound = loopAdvance(leftBound);
                    rightBound = loopAdvance(rightBound);
                }
                return ret;
            }
            else
            {
                return WindowStatus.NonActiveFrame;
            }
        }
    }




    public bool InWindow(int frameId)
    {
        if (leftBound < rightBound)
        {
            return frameId >= leftBound && frameId < rightBound;
        }
        else
        {
            return (frameId >= 0 && frameId < rightBound) || (frameId >= leftBound && frameId < 2 * MaxSize);
        }
    }

    public bool ActiveFrames(int frameId)
    {
        if (leftBound <= currentPointer)
        {
            return frameId < currentPointer && frameId >= leftBound;
        }
        else
        {
            return (frameId >= 0 && frameId < currentPointer) || (frameId >= leftBound && frameId < 2 * MaxSize);
        }
    }

    public int AdvancePointer()
    {
        int currentValue = currentPointer;
        int nextValue = loopAdvance(currentPointer);
        if (InWindow(nextValue))
        {
            currentPointer = nextValue;
            return currentValue;
        }
        else
        {
            return -1;
        }
        
    }

    private int loopAdvance(int pointer)
    {
        int nextValue = pointer + 1;
        if (nextValue > (2 * MaxSize - 1))
        {
            nextValue = 0;
        }
        return nextValue;
    }
}
