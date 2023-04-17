using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEndTurn : Task
{
    private string TaskName = "Waiting For Turn";

    private string SelfKey = null;

    private Character Self = null;

    
    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey null at TEndTurn");
            return false;
        }

        return true;
    }

    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        return BehaviorTree.EvaluationState.SUCCESS;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        bt.SetCurrentNode(this);
        bt.SetCurrentTaskName(TaskName);

        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        Self.EndTurn();


        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
