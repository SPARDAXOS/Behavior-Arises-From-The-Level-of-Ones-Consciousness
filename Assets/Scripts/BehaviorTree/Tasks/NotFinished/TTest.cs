using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTest : Task
{

    //Only tasks send themselves to the root and set the running task stuff
    public int Num = 0;
    public BehaviorTree.ExecutionState  state;

    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        Debug.Log("Test task " + Num + " run!");

        return state;
    }
}
