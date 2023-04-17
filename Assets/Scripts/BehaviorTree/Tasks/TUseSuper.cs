using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TUseSuper : Task
{
    public enum SuperType
    {
        BUFF,
        DAMAGE,
        HEAL
    }

    private string TaskName = "Using Super";
    private SuperType CurrentSuperType = SuperType.BUFF;

    private string SelfKey = null;
    private string TargetPartyKey = null;
    private string SuperCostKey = null;

    //Buff specific
    private string SuperBuffTypeKey = null;
    private string SuperBuffRateKey = null;
    private string SuperBuffTurnsKey = null;
    //Damage specific
    private string SuperDamageRateKey = null;

    //Heal specific
    private string SuperHealRateKey = null;


    private Character Self = null;
    private Party TargetParty = null;
    private float SuperCost = 0.0f;
    //Buff specific
    private StatusAilments.Buffs SuperBuffType = StatusAilments.Buffs.DEFENSE; // Default
    private float SuperBuffRate = 0.0f;
    private int SuperBuffTurns = 0;
    //Damage specific
    private float SuperDamageRate = 0.0f;
    //Heal specific
    private float SuperHealRate = 0.0f;


    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey null at TUseSuper");
            return false;
        }
        if (TargetPartyKey == null)
        {
            Debug.LogError("TargetPartyKey null at TUseSuper");
            return false;
        }
        if (SuperCostKey == null)
        {
            Debug.LogError("SuperCostKey null at TUseSuper");
            return false;
        }

        switch (CurrentSuperType)
        {
            case SuperType.BUFF:
                {
                    if (SuperBuffTypeKey == null)
                    {
                        Debug.LogError("SuperBuffTypeKey null at TUseSuper");
                        return false;
                    }
                    if (SuperBuffRateKey == null)
                    {
                        Debug.LogError("SuperDefenseBuffRateKey null at TUseSuper");
                        return false;
                    }
                    if (SuperBuffTurnsKey == null)
                    {
                        Debug.LogError("SuperDefenseBuffTurnsKey null at TUseSuper");
                        return false;
                    }
                }
                break;
            case SuperType.DAMAGE:
                {
                    if (SuperDamageRateKey == null)
                    {
                        Debug.LogError("SuperDamageRateKey null at TUseSuper");
                        return false;
                    }
                }
                break;
            case SuperType.HEAL:
                {
                    if (SuperHealRateKey == null)
                    {
                        Debug.LogError("SuperHealRateKey null at TUseSuper");
                        return false;
                    }
                }
                break;
        }

        return true;
    }


    public void SetCurrentSuperType(SuperType type)
    {
        CurrentSuperType = type;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTargetPartyKey(string key)
    {
        TargetPartyKey = key;
    }
    public void SetSuperCostKey(string key)
    {
        SuperCostKey = key;
    }
    public void SetSuperBuffTypeKey(string key)
    {
        SuperBuffTypeKey = key;
    }
    public void SetSuperBuffRateKey(string key)
    {
        SuperBuffRateKey = key;
    }
    public void SetSuperBuffTurnsKey(string key)
    {
        SuperBuffTurnsKey = key;
    }
    public void SetSuperDamageRateKey(string key)
    {
        SuperDamageRateKey = key;
    }
    public void SetSuperHealRateKey(string key)
    {
        SuperHealRateKey = key;
    }
    

    private void UseSuper(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetParty = bb.GetValue<Party>(TargetPartyKey);
        SuperCost = bb.GetValue<float>(SuperCostKey);

        Character[] TargetCharacters = TargetParty.GetCharactersLeft();

        switch (CurrentSuperType)
        {
            case SuperType.BUFF:
                {
                    SuperBuffType = bb.GetValue<StatusAilments.Buffs>(SuperBuffTypeKey);
                    SuperBuffRate = bb.GetValue<float>(SuperBuffRateKey);
                    SuperBuffTurns = bb.GetValue<int>(SuperBuffTurnsKey);
                    SuperBuffTurns++; //To compensate for TEndTurn that comes afterwards.
                    for (int i = 0; i < TargetCharacters.Length; i++)
                        if (TargetCharacters[i])
                            TargetCharacters[i].ApplyBuff(SuperBuffType, SuperBuffRate, SuperBuffTurns);
                }
                break;
            case SuperType.DAMAGE:
                {
                    SuperDamageRate = bb.GetValue<float>(SuperDamageRateKey);
                    for (int i = 0; i < TargetCharacters.Length; i++)
                        if (TargetCharacters[i])
                            TargetCharacters[i].TakeDamage(SuperDamageRate);
                }
                break;
            case SuperType.HEAL:
                {
                    SuperHealRate = bb.GetValue<float>(SuperHealRateKey);
                    for (int i = 0; i < TargetCharacters.Length; i++)
                        if (TargetCharacters[i])
                            TargetCharacters[i].Heal(SuperHealRate);
                }
                break;
        }

        Self.PerformedAction();
        Self.UseSuper(SuperCost);
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

        UseSuper(bt);

        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
