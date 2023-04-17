using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCompareKeyWithKeyFloat : Decorator 
{
    public enum CompareType
    {
        A_EQUAL_OR_LESS_THAN_B,
        A_EQUAL_OR_HIGHER_THAN_B,
        A_EQUALS_B
    }
    private CompareType Type = CompareType.A_EQUAL_OR_LESS_THAN_B;
    private string AValueKey = null;
    private string BValueKey = null;

    float A = 0;
    float B = 0;


    private bool AreKeysValid()
    {
        if (AValueKey == null)
        {
            Debug.LogError("Null AValueKey set at DCompareKeyWithKeyFloat");
            return false;
        }
        if (BValueKey == null)
        {
            Debug.LogError("Null BValueKey set at DCompareKeyWithKeyFloat");
            return false;
        }

        return true;
    }

    public void SetCompareType(CompareType type)
    {
        Type = type;
    }
    public void SetAValueKey(string key)
    {
        AValueKey = key;
    }
    public void SetBValueKey(string key)
    {
        BValueKey = key;
    }

    //CLEAN UP

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        A = bt.GetBlackboard().GetValue<float>(AValueKey);
        B = bt.GetBlackboard().GetValue<float>(BValueKey);


        switch (Type)
        {
            case CompareType.A_EQUAL_OR_LESS_THAN_B:
                {
                    if (A <= B)
                        return ConnectedNode.Evaluate(bt);
                    else
                        return BehaviorTree.EvaluationState.FAILURE;
                }
            case CompareType.A_EQUAL_OR_HIGHER_THAN_B:
                {
                    if (A >= B)
                        return ConnectedNode.Evaluate(bt);
                    else
                        return BehaviorTree.EvaluationState.FAILURE;
                }
            case CompareType.A_EQUALS_B:
                {
                    if (A == B)
                        return ConnectedNode.Evaluate(bt);
                    else
                        return BehaviorTree.EvaluationState.FAILURE;
                }
        }

        return BehaviorTree.EvaluationState.ERROR;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        bt.SetCurrentNode(this);

        A = bt.GetBlackboard().GetValue<float>(AValueKey);
        B = bt.GetBlackboard().GetValue<float>(BValueKey);

        switch (Type)
        {
            case CompareType.A_EQUAL_OR_LESS_THAN_B:
                {
                    if (A <= B)
                        return ConnectedNode.Execute(bt);
                    else
                        return BehaviorTree.ExecutionState.FAILURE;
                }
            case CompareType.A_EQUAL_OR_HIGHER_THAN_B:
                {
                    if (A >= B)
                        return ConnectedNode.Execute(bt);
                    else
                        return BehaviorTree.ExecutionState.FAILURE;
                }
            case CompareType.A_EQUALS_B:
                {
                    if (A == B)
                        return ConnectedNode.Execute(bt);
                    else
                        return BehaviorTree.ExecutionState.FAILURE;
                }
        }

        return BehaviorTree.ExecutionState.ERROR;
    }
}
