using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : Node
{
    public enum RunningStatus
    {
        RUNNING,
        NOT_RUNNING 
    }
    protected RunningStatus Status = RunningStatus.NOT_RUNNING; // for running tasks.
    //?? Context free? not even implementaion for execute?

    public RunningStatus GetStatus()
    {
        return Status;
    } 
    public virtual void Interrupt()
    {
    }
}
