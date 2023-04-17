using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TApplyStatusAilments : Task
{
    public enum StatusAilmentType
    {
        BUFF,
        DEBUFF
    } 
    public enum TargetType
    {
        CHARACTER,
        PARTY
    }

    private string TaskName = "Casting Attack Buff";

    private StatusAilmentType CurrentStatusAilmentType = StatusAilmentType.BUFF;
    private TargetType CurrentTargetType = TargetType.CHARACTER;


    private string SelfKey = null;
    private string SpellCostKey = null;
    private string StatusAilmentRateKey = null;
    private string StatusAilmentTurnsKey = null;

    private string BuffTypeKey = null;
    private string DebuffTypeKey = null;

    private string TargetCharacterKey = null;
    private string TargetPartyKey = null;


    private Character Self = null;
    private float SpellCost = 0.0f;
    private int StatusAilmentTurns = 0;
    private float StatusAilmentRate = 0.0f;

    private StatusAilments.Buffs BuffType = StatusAilments.Buffs.ATTACK; //Default
    private StatusAilments.Debuffs DebuffType = StatusAilments.Debuffs.ATTACK; //Default

    private Character TargetCharacter = null;
    private Party TargetParty = null;


    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey is null - TApplyStatusAilments");
            return false;
        }
        if (SpellCostKey == null)
        {
            Debug.LogError("SpellCostKey is null - TApplyStatusAilments");
            return false;
        }
        if (StatusAilmentRateKey == null)
        {
            Debug.LogError("StatusAilmentRateKey is null - TApplyStatusAilments");
            return false;
        }
        if (StatusAilmentTurnsKey == null)
        {
            Debug.LogError("StatusAilmentTurnsKey is null - TApplyStatusAilments");
            return false;
        }
        

        switch (CurrentStatusAilmentType)
        {
            case StatusAilmentType.BUFF:
                {
                    if (BuffTypeKey == null)
                    {
                        Debug.LogError("BuffTypeKey is null - TApplyStatusAilments");
                        return false;
                    }
                }
                break;
            case StatusAilmentType.DEBUFF:
                {
                    if (DebuffTypeKey == null)
                    {
                        Debug.LogError("DebuffTypeKey is null - TApplyStatusAilments");
                        return false;
                    }
                }
                break;
        }
        switch (CurrentTargetType)
        {
            case TargetType.CHARACTER:
                {
                    if (TargetCharacterKey == null)
                    {
                        Debug.LogError("TargetCharacterKey is null - TApplyStatusAilments");
                        return false;
                    }
                }
                break;
            case TargetType.PARTY:
                {
                    if (TargetPartyKey == null)
                    {
                        Debug.LogError("TargetPartyKey is null - TApplyStatusAilments");
                        return false;
                    }
                }
                break;
        }
        return true;
    }


    public void SetCurrentStatusAilmentType(StatusAilmentType type)
    {
        CurrentStatusAilmentType = type;
    }
    public void SetCurrentTargetType(TargetType type)
    {
        CurrentTargetType = type;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetSpellCostKey(string key)
    {
        SpellCostKey = key;
    }
    public void SetStatusAilmentRateKey(string key)
    {
        StatusAilmentRateKey = key;
    }
    public void SetStatusAilmentTurnsKey(string key)
    {
        StatusAilmentTurnsKey = key;
    }


    public void SetBuffTypeKey(string key)
    {
        BuffTypeKey = key;
    }
    public void SetDebuffTypeKey(string key)
    {
        DebuffTypeKey = key;
    }

    public void SetTargetCharacterKey(string key)
    {
        TargetCharacterKey = key;
    }
    public void SetTargetPartyKey(string key)
    {
        TargetPartyKey = key;
    }


    private bool ApplyStatusAilment(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        SpellCost = bb.GetValue<float>(SpellCostKey);
        StatusAilmentRate = bb.GetValue<float>(StatusAilmentRateKey);
        StatusAilmentTurns = bb.GetValue<int>(StatusAilmentTurnsKey);

        switch (CurrentTargetType)
        {
            case TargetType.CHARACTER:
                {
                    TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
                    if (TargetCharacter == null)
                    {
                        Debug.LogError("Attempted to apply status ailment to null character - TApplyStatusAilments");
                        return false;
                    }

                    if (CurrentStatusAilmentType == StatusAilmentType.BUFF)
                    {
                        BuffType = bb.GetValue<StatusAilments.Buffs>(BuffTypeKey);
                        TargetCharacter.ApplyBuff(BuffType, StatusAilmentRate, StatusAilmentTurns);
                    }
                    else if (CurrentStatusAilmentType == StatusAilmentType.DEBUFF)
                    {
                        DebuffType = bb.GetValue<StatusAilments.Debuffs>(DebuffTypeKey);
                        TargetCharacter.ApplyDebuff(DebuffType, StatusAilmentRate, StatusAilmentTurns);
                    }

                    Self.PerformedAction();
                    Self.DecreaseMana(SpellCost);
                    return true;
                }
            case TargetType.PARTY:
                {
                    TargetParty = bb.GetValue<Party>(TargetPartyKey);
                    if(TargetParty == null)
                    {
                        Debug.LogError("Attempted to apply status ailment to null party - TApplyStatusAilments");
                        return false;
                    }

                    Character[] AvailableCharacters = TargetParty.GetCharactersLeft();
                    if(AvailableCharacters.Length == 0)
                    {
                        Debug.LogError("Attempted to apply status ailment to empty party - TApplyStatusAilments");
                        return false;
                    }

                    if (CurrentStatusAilmentType == StatusAilmentType.BUFF)
                    {
                        BuffType = bb.GetValue<StatusAilments.Buffs>(BuffTypeKey);
                        for(int i = 0; i < AvailableCharacters.Length; i++)
                            if (AvailableCharacters[i])
                                AvailableCharacters[i].ApplyBuff(BuffType, StatusAilmentRate, StatusAilmentTurns);
                        Self.PerformedAction();
                        Self.DecreaseMana(SpellCost);
                        return true;
                    }
                    else if (CurrentStatusAilmentType == StatusAilmentType.DEBUFF)
                    {
                        DebuffType = bb.GetValue<StatusAilments.Debuffs>(DebuffTypeKey);
                        for (int i = 0; i < AvailableCharacters.Length; i++)
                            if (AvailableCharacters[i])
                                AvailableCharacters[i].ApplyDebuff(DebuffType, StatusAilmentRate, StatusAilmentTurns);
                        Self.PerformedAction();
                        Self.DecreaseMana(SpellCost);
                        return true;
                    }
                }
                break;
        }

        return false;
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

        bool Results = ApplyStatusAilment(bt);

        if (!Results)
            return BehaviorTree.ExecutionState.FAILURE;
        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
