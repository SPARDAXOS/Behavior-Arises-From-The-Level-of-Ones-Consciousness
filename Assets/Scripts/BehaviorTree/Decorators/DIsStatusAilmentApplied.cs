using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIsStatusAilmentApplied : Decorator
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        ERROR
    }

    public enum TargetType
    {
        CHARACTER,
        PARTY
    }
    public enum SuccessCondition
    {
        IF_TRUE,
        IF_FALSE
    }
    public enum StatusAilmentType
    {
        BUFF,
        DEBUFF
    }

    TargetType CurrentTargetType = TargetType.CHARACTER;
    SuccessCondition CurrentSuccessCondition = SuccessCondition.IF_TRUE;
    StatusAilmentType CurrentStatusAilmentType = StatusAilmentType.BUFF;


    StatusAilments.Buffs BuffType = StatusAilments.Buffs.DEFENSE;
    StatusAilments.Debuffs DebuffType = StatusAilments.Debuffs.DEFENSE;


    private string TargetCharacterKey = null;
    private string TargetPartyKey = null;

    private string BuffTypeKey = null;
    private string DebuffTypeKey = null;


    private Character TargetCharacter = null;
    private Party TargetParty = null;

    private bool AreKeysValid()
    {
        switch (CurrentTargetType)
        {
            case TargetType.CHARACTER:
                {
                    if (TargetCharacterKey == null)
                    {
                        Debug.LogError("TargetCharacterKey null at DIsStatusAilmentApplied");
                        return false;
                    }
                }
                break;
            case TargetType.PARTY:
                {
                    if (TargetPartyKey == null)
                    {
                        Debug.LogError("TargetPartyKey null at DIsStatusAilmentApplied");
                        return false;
                    }
                }
                break;
        }
        switch (CurrentStatusAilmentType)
        {
            case StatusAilmentType.BUFF:
                {
                    if (BuffTypeKey == null)
                    {
                        Debug.LogError("BuffTypeKey null at DIsStatusAilmentApplied");
                        return false;
                    }
                }
                break;
            case StatusAilmentType.DEBUFF:
                {
                    if (DebuffTypeKey == null)
                    {
                        Debug.LogError("DebuffTypeKey null at DIsStatusAilmentApplied");
                        return false;
                    }
                }
                break;
        }

        return true;
    }


    public void SetCurrentTargetType(TargetType type)
    {
        CurrentTargetType = type;
    }
    public void SetCurrentSuccessCondition(SuccessCondition type)
    {
        CurrentSuccessCondition = type;
    }
    public void SetCurrentStatusAilmentType(StatusAilmentType type)
    {
        CurrentStatusAilmentType = type;
    }

    public void SetTargetCharacterKey(string key)
    {
        TargetCharacterKey = key;
    }
    public void SetTargetPartyKey(string key)
    {
        TargetPartyKey = key;
    }
    public void SetBuffTypeKey(string key)
    {
        BuffTypeKey = key;
    }
    public void SetDebuffTypeKey(string key)
    {
        DebuffTypeKey = key;
    }


    private ConditionResult Check(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        bool Results = false;

        if(CurrentTargetType == TargetType.CHARACTER)
        {
            if (CurrentStatusAilmentType == StatusAilmentType.BUFF)
            {
                BuffType = bb.GetValue<StatusAilments.Buffs>(BuffTypeKey);
                Results = TargetCharacter.IsBuffApplied(BuffType);
            }
            else if (CurrentStatusAilmentType == StatusAilmentType.DEBUFF)
            {
                DebuffType = bb.GetValue<StatusAilments.Debuffs>(DebuffTypeKey);
                Results = TargetCharacter.IsDebuffApplied(DebuffType);
            }

            if (CurrentSuccessCondition == SuccessCondition.IF_TRUE && Results)
                return ConditionResult.SUCCESS;
            else if (CurrentSuccessCondition == SuccessCondition.IF_FALSE && !Results)
                return ConditionResult.SUCCESS;
            else
                return ConditionResult.FAILURE;
        }
        else if(CurrentTargetType == TargetType.PARTY)
        {
            TargetParty = bb.GetValue<Party>(TargetPartyKey);
            Character[] TargetCharacters = TargetParty.GetCharactersLeft();

            for (int i = 0; i < TargetCharacters.Length; i++)
            {
                if (TargetCharacters[i])
                {
                    if (CurrentStatusAilmentType == StatusAilmentType.BUFF)
                    {
                        BuffType = bb.GetValue<StatusAilments.Buffs>(BuffTypeKey);
                        Results = TargetCharacters[i].IsBuffApplied(BuffType);
                    }
                    else if (CurrentStatusAilmentType == StatusAilmentType.DEBUFF)
                    {
                        DebuffType = bb.GetValue<StatusAilments.Debuffs>(DebuffTypeKey);
                        Results = TargetCharacters[i].IsDebuffApplied(DebuffType);
                    }

                    if(CurrentSuccessCondition == SuccessCondition.IF_TRUE)
                    {
                        if (!Results) //If any of them does not have it
                            return ConditionResult.FAILURE;
                    }
                    else if (CurrentSuccessCondition == SuccessCondition.IF_FALSE)
                    {
                        if (Results) //If any of them does have it
                            return ConditionResult.FAILURE;
                    }
                }
            }

            //If none of the failure checks work, it means it succeded
            return ConditionResult.SUCCESS;
        }

        return ConditionResult.ERROR;
    }


    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        ConditionResult State = Check(bt);

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

        ConditionResult State = Check(bt);
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
