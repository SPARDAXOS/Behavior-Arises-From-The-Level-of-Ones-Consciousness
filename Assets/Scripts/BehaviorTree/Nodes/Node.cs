using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public virtual BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {


        return BehaviorTree.EvaluationState.ERROR;
    }
    public virtual BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {


        return BehaviorTree.ExecutionState.ERROR;
    }
}
