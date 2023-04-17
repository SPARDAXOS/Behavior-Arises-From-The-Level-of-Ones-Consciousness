using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DComparePartyStatWithValue : Decorator 
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        ERROR
    }

    public enum CompareType
    {
        HIGHER_THAN_VALUE,
        HIGHER_OR_EQUALS_VALUE,
        EQUALS_VALUE,
        LOWER_THAN_VALUE,
        LOWER_OR_EQUALS_VALUE
    }
    public enum StatType
    {
        HEALTH,
        MANA,
        AGGRO
    }
    private enum CheckResult
    {
        HIGHER,
        EQUALS,
        LOWER,
    }
    private CompareType CurrentCompareType = CompareType.HIGHER_OR_EQUALS_VALUE;
    private StatType CurrentStatType = StatType.MANA;

    private string TargetPartyKey = null;

    private Party TargetParty = null;
    float Value = 0;


    private bool AreKeysValid()
    {
        if (TargetPartyKey == null)
        {
            Debug.LogError("Null TargetPartyKey set at DComparePartyStatWithValue");
            return false;
        }

        return true;
    }

    public void SetCompareType(CompareType type)
    {
        CurrentCompareType = type;
    }
    public void SetStatType(StatType type)
    {
        CurrentStatType = type;
    }

    public void SetTargetPartyKey(string key)
    {
        TargetPartyKey = key;
    }
    public void SetValue(float value)
    {
        Value = value;
    }

    //CLEAN UP


    private ConditionResult Compare(BehaviorTree bt)
    {
        TargetParty = bt.GetBlackboard().GetValue<Party>(TargetPartyKey);
        Character[] Characters = TargetParty.GetCharactersLeft();
        CheckResult Results;

        switch (CurrentCompareType)
        {
            case CompareType.HIGHER_THAN_VALUE:
                {
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (Characters[i])
                        {
                            Results = Check(Characters[i]);
                            if (Results == CheckResult.HIGHER)
                                return ConditionResult.SUCCESS;
                        }
                    }
                    return ConditionResult.FAILURE;
                }
            case CompareType.HIGHER_OR_EQUALS_VALUE:
                {
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (Characters[i])
                        {
                            Results = Check(Characters[i]);
                            if (Results == CheckResult.HIGHER || Results == CheckResult.EQUALS)
                                return ConditionResult.SUCCESS;
                        }
                    }
                    return ConditionResult.FAILURE;
                }
            case CompareType.EQUALS_VALUE:
                {
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (Characters[i])
                        {
                            Results = Check(Characters[i]);
                            if (Results == CheckResult.EQUALS)
                                return ConditionResult.SUCCESS;
                        }
                    }
                    return ConditionResult.FAILURE;
                }
            case CompareType.LOWER_OR_EQUALS_VALUE:
                {
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (Characters[i])
                        {
                            Results = Check(Characters[i]);
                            if (Results == CheckResult.LOWER || Results == CheckResult.EQUALS)
                                return ConditionResult.SUCCESS;
                        }
                    }
                    return ConditionResult.FAILURE;
                }
            case CompareType.LOWER_THAN_VALUE:
                {
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (Characters[i])
                        {
                            Results = Check(Characters[i]);
                            if (Results == CheckResult.LOWER)
                                return ConditionResult.SUCCESS;
                        }
                    }
                    return ConditionResult.FAILURE;
                }
        }

        return ConditionResult.ERROR;
    }
    private CheckResult Check(Character character)
    {
        float CharacterValue = 0.0f;
        switch (CurrentStatType)
        {
            case StatType.HEALTH:
                {
                    CharacterValue = character.GetCurrentHealth();
                }
                break;
            case StatType.MANA:
                {
                    CharacterValue = character.GetCurrentMana();
                }
                break;
            case StatType.AGGRO:
                {
                    CharacterValue = character.GetCurrentAggro();
                }
                break;
        }

        if (CharacterValue > Value)
            return CheckResult.HIGHER;
        else if (CharacterValue < Value)
            return CheckResult.LOWER;
        else
            return CheckResult.EQUALS;
    }

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        ConditionResult state = Compare(bt);

        switch (state)
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

        ConditionResult state = Compare(bt);

        switch (state)
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
