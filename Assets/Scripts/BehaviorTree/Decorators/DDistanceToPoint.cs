using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDistanceToPoint : Decorator
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        ERROR
    }
    public enum SuccessCondition
    {
        FAR_FROM_TARGET,
        CLOSE_TO_TARGET
    }

    private SuccessCondition CurrentSuccessCondition = SuccessCondition.FAR_FROM_TARGET;

    private string SelfKey = null;
    private string TargetPointKey = null;

    private Character Self = null;
    private Vector3 TargetPoint = new Vector3();

    private float SuccessDistanceThreshold = 1.0f;


    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey null at DDistanceToPoint");
            return false;
        }
        if (TargetPointKey == null)
        {
            Debug.LogError("TargetPointKey null at DDistanceToPoint");
            return false;
        }
        return true;
    }

    public void SetSuccessCondition(SuccessCondition condition)
    {
        CurrentSuccessCondition = condition;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTargetPointKey(string key)
    {
        TargetPointKey = key;
    }
    public void SetSuccessDistanceThreshold(float threshold)
    {
        SuccessDistanceThreshold = threshold;
    }

    private ConditionResult CheckRadius(BehaviorTree bt, Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);

        switch (CurrentSuccessCondition)
        {
            case SuccessCondition.FAR_FROM_TARGET:
                {
                    if (Length >= SuccessDistanceThreshold)
                        return ConditionResult.SUCCESS;
                }break;
            case SuccessCondition.CLOSE_TO_TARGET:
                {
                    if (Length <= SuccessDistanceThreshold)
                        return ConditionResult.SUCCESS;
                }break;
        }

        return ConditionResult.FAILURE;
    }


    //THIS NEEDS MAJOR REWORK AND REFACTOR IMPORTANTLY


    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;
        bt.SetCurrentNode(this);

        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetPoint = bb.GetValue<Vector3>(TargetPointKey);

        ConditionResult State = CheckRadius(bt, Self.transform.position, TargetPoint);

        switch (State)
        {
            case ConditionResult.SUCCESS:
                return ConnectedNode.Evaluate(bt);
            case ConditionResult.FAILURE:
                return BehaviorTree.EvaluationState.FAILURE;
            case ConditionResult.ERROR:
                return BehaviorTree.EvaluationState.ERROR;
        }

        return BehaviorTree.EvaluationState.ERROR;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;


        bt.SetCurrentNode(this);

        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetPoint = bb.GetValue<Vector3>(TargetPointKey);

        ConditionResult State = CheckRadius(bt, Self.transform.position, TargetPoint);
        switch (State)
        {
            case ConditionResult.SUCCESS:
                return ConnectedNode.Execute(bt);
            case ConditionResult.FAILURE:
                return BehaviorTree.ExecutionState.FAILURE;
            case ConditionResult.ERROR:
                return BehaviorTree.ExecutionState.ERROR;
        }

        return BehaviorTree.ExecutionState.ERROR;
    }
}
