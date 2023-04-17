using UnityEngine;

public class DCompareSelfStatWithParty : Decorator
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        ERROR
    }
    public enum SuccessConditionType
    {
        HIGHEST_IN_PARTY,
        LOWEST_IN_PARTY,
        NOT_HIGHEST_IN_PARTY,
        NOT_LOWEST_IN_PARTY
    }
    public enum CompareStatType
    {
        HEALTH,
        MANA,
        AGGRO
    }

    private SuccessConditionType CurrentSuccessConditionType = SuccessConditionType.HIGHEST_IN_PARTY;
    private CompareStatType CurrentCompareStatType = CompareStatType.HEALTH;

    private string SelfKey = null;
    private string TargetPartyKey = null;

    private string HealthKey = null;
    private string ManaKey = null;
    private string AggroKey = null;


    private Character Self = null;
    private Party TargetParty = null;

    private float OwnStat = 0.0f;

    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey is null - DCompareSelfStatWithParty");
            return false;
        }
        if (TargetPartyKey == null)
        {
            Debug.LogError("PartyKey is null - DCompareSelfStatWithParty");
            return false;
        }

        switch (CurrentCompareStatType)
        {
            case CompareStatType.HEALTH:
                {
                    if (HealthKey == null)
                    {
                        Debug.LogError("HealthKey is null - DCompareSelfStatWithParty");
                        return false;
                    }
                }break;
            case CompareStatType.MANA:
                {
                    if (ManaKey == null)
                    {
                        Debug.LogError("ManaKey is null - DCompareSelfStatWithParty");
                        return false;
                    }
                }break;
            case CompareStatType.AGGRO:
                {
                    if (AggroKey == null)
                    {
                        Debug.LogError("AggroKey is null - DCompareSelfStatWithParty");
                        return false;
                    }
                }break;
        }

        return true;
    }

    public void SetConditionType(SuccessConditionType type)
    {
        CurrentSuccessConditionType = type;
    }
    public void SetStatType(CompareStatType type)
    {
        CurrentCompareStatType = type;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTargetPartyKey(string key)
    {
        TargetPartyKey = key;
    }
    public void SetHealthKey(string key)
    {
        HealthKey = key;
    }
    public void SetManaKey(string key)
    {
        ManaKey = key;
    }
    public void SetAggroKey(string key)
    {
        AggroKey = key;
    }

    private ConditionResult Compare(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetParty = bb.GetValue<Party>(TargetPartyKey);

        switch (CurrentCompareStatType)
        {
            case CompareStatType.HEALTH:
                {
                    OwnStat = bb.GetValue<float>(HealthKey);
                }
                break;
            case CompareStatType.MANA:
                {
                    OwnStat = bb.GetValue<float>(ManaKey);
                }
                break;
            case CompareStatType.AGGRO:
                {
                    OwnStat = bb.GetValue<float>(AggroKey);
                }
                break;
        }

        return Check(bt);
    }
    private ConditionResult Check(BehaviorTree bt)
    {
        Character[] AllCharacters = TargetParty.GetCharactersLeft();
        bool Results;

        switch (CurrentSuccessConditionType)
        {
            case SuccessConditionType.HIGHEST_IN_PARTY:
                {
                    Results = IsHighest(AllCharacters);
                    if (Results)
                        return ConditionResult.SUCCESS;
                    else
                        return ConditionResult.FAILURE;
                }
            case SuccessConditionType.LOWEST_IN_PARTY:
                {
                    Results = IsLowest(AllCharacters);
                    if (Results)
                        return ConditionResult.SUCCESS;
                    else
                        return ConditionResult.FAILURE;
                }
            case SuccessConditionType.NOT_HIGHEST_IN_PARTY:
                {
                    Results = IsHighest(AllCharacters);
                    if (!Results)
                        return ConditionResult.SUCCESS;
                    else
                        return ConditionResult.FAILURE;
                }
            case SuccessConditionType.NOT_LOWEST_IN_PARTY:
                {
                    Results = IsLowest(AllCharacters);
                    if (!Results)
                        return ConditionResult.SUCCESS;
                    else
                        return ConditionResult.FAILURE;
                }
        }

        return ConditionResult.ERROR;
    }

    private bool IsHighest(Character[] characters)
    {
        float ComparedStat = 0.0f;
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] && characters[i] != Self)
            {
                switch (CurrentCompareStatType)
                {
                    case CompareStatType.HEALTH:
                        {
                            ComparedStat = characters[i].GetCurrentHealth();
                        }
                        break;
                    case CompareStatType.MANA:
                        {
                            ComparedStat = characters[i].GetCurrentMana();
                        }
                        break;
                    case CompareStatType.AGGRO:
                        {
                            ComparedStat = characters[i].GetCurrentAggro();
                        }
                        break;
                }

                if (ComparedStat >= OwnStat) //If is as much or more.
                    return false;
            }
        }
        return true;
    }
    private bool IsLowest(Character[] characters)
    {
        float ComparedStat = 0.0f;
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] && characters[i] != Self)
            {
                switch (CurrentCompareStatType)
                {
                    case CompareStatType.HEALTH:
                        {
                            ComparedStat = characters[i].GetCurrentHealth();
                        }
                        break;
                    case CompareStatType.MANA:
                        {
                            ComparedStat = characters[i].GetCurrentMana();
                        }
                        break;
                    case CompareStatType.AGGRO:
                        {
                            ComparedStat = characters[i].GetCurrentAggro();
                        }
                        break;
                }

                if (ComparedStat <= OwnStat) //If is as much or less.
                    return false;
            }
        }

        return true;
    }

    // s there even a point of the evaluation states
    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        //This needs cleaning
        ConditionResult State = Compare(bt);
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
        ConditionResult State = Compare(bt);
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
