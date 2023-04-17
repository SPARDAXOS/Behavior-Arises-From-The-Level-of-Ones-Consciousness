using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTimerTest : Task
{

    //Only tasks send themselves to the root and set the running task stuff
    public int Num = 0;
    private BehaviorTree.ExecutionState  state;

    public float TimerLimit = 0.5f;
    private float Timer = 0.0f;

    private BehaviorTree.ExecutionState RunTimer()
    {
        Timer += Time.deltaTime;
        Debug.Log("Timer " + Num + " is " + Timer);
        if(Timer >= TimerLimit)
        {
            Timer = TimerLimit;
            return BehaviorTree.ExecutionState.SUCCESS;
        }
        return BehaviorTree.ExecutionState.RUNNING;
    }

    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        Debug.Log("Test task " + Num + " run!");

        state = RunTimer();
        //if (state == BehaviorTree.ExecutionState.RUNNING)
        //{
        //    bt.SetRunningTask(this); // It will keep on being set...
        //    return BehaviorTree.ExecutionState.RUNNING;
        //}

        return state;
    }
}
