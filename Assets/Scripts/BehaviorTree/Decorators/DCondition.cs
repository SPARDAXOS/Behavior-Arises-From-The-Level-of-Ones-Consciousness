using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCondition : Decorator
{
    public enum ConditionType
    {
        BOOL,
        CHARACTER
    }
    public enum SuccessCriteria
    {
        IF_TRUE,
        IF_FALSE
    }

    ConditionType CurrentConditionType = ConditionType.BOOL;
    SuccessCriteria CurrentSuccessCriteria = SuccessCriteria.IF_TRUE;


    private string ConditionKey = null;
    private string CharacterKey = null;

    private bool Condition = false;
    private Character Character = null;

    private bool AreKeysValid()
    {
        switch (CurrentConditionType)
        {
            case ConditionType.BOOL:
                {
                    if (ConditionKey == null)
                    {
                        Debug.LogError("Null ConditionKey set at DCondition");
                        return false;
                    }
                }
                break;
            case ConditionType.CHARACTER:
                {
                    if (CharacterKey == null)
                    {
                        Debug.LogError("Null CharacterKey set at DCondition");
                        return false;
                    }
                }
                break;
        }
        return true;
    }

    public void SetConditionType(ConditionType type)
    {
        CurrentConditionType = type;
    }
    public void SetSuccessCriteriaType(SuccessCriteria type)
    {
        CurrentSuccessCriteria = type;
    }


    public void SetConditionKey(string key)
    {
        ConditionKey = key;
    }
    public void SetCharacterKey(string key)
    {
        CharacterKey = key;
    }

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        switch (CurrentConditionType)
        {
            case ConditionType.BOOL:
                {
                    Condition = bt.GetBlackboard().GetValue<bool>(ConditionKey);

                    if (CurrentSuccessCriteria == SuccessCriteria.IF_TRUE)
                    {
                        if (Condition)
                            return ConnectedNode.Evaluate(bt);
                        else
                            return BehaviorTree.EvaluationState.FAILURE;
                    }
                    else
                    {
                        if (Condition)
                            return BehaviorTree.EvaluationState.FAILURE;
                        else
                            return ConnectedNode.Evaluate(bt);
                    }
                }
            case ConditionType.CHARACTER:
                {
                    Character = bt.GetBlackboard().GetValue<Character>(CharacterKey);

                    if (CurrentSuccessCriteria == SuccessCriteria.IF_TRUE)
                    {
                        if (Character)
                            return ConnectedNode.Evaluate(bt);
                        else
                            return BehaviorTree.EvaluationState.FAILURE;
                    }
                    else
                    {
                        if (Character)
                            return BehaviorTree.EvaluationState.FAILURE;
                        else
                            return ConnectedNode.Evaluate(bt);
                    }
                }
        }

        return BehaviorTree.EvaluationState.ERROR;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        bt.SetCurrentNode(this);

        switch (CurrentConditionType)
        {
            case ConditionType.BOOL:
                {
                    Condition = bt.GetBlackboard().GetValue<bool>(ConditionKey);

                    if (CurrentSuccessCriteria == SuccessCriteria.IF_TRUE)
                    {
                        if (Condition)
                            return ConnectedNode.Execute(bt);
                        else
                            return BehaviorTree.ExecutionState.FAILURE;
                    }
                    else
                    {
                        if (Condition)
                            return BehaviorTree.ExecutionState.FAILURE;
                        else
                            return ConnectedNode.Execute(bt);
                    }
                }
            case ConditionType.CHARACTER:
                {
                    Character = bt.GetBlackboard().GetValue<Character>(CharacterKey);

                    if (CurrentSuccessCriteria == SuccessCriteria.IF_TRUE)
                    {
                        if (Character)
                            return ConnectedNode.Execute(bt);
                        else
                            return BehaviorTree.ExecutionState.FAILURE;
                    }
                    else
                    {
                        if (Character)
                            return BehaviorTree.ExecutionState.FAILURE;
                        else
                            return ConnectedNode.Execute(bt);
                    }
                }
        }

        return BehaviorTree.ExecutionState.ERROR;
    }
}
