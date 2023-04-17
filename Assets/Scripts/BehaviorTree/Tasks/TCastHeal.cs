using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCastHeal : Task
{
    private string TaskName = "Casting Heal";

    private string SelfKey = null;
    private string TargetCharacterKey = null;
    private string HealSpellCostKey = null;
    private string HealSpellRateKey = null;


    private Character Self = null;
    private Character TargetCharacter = null;

    private float HealSpellCost = 0.0f;
    private float HealSpellRate = 0.0f;

    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("Null SelfKey key set at TCastHeal");
            return false;
        }
        if (TargetCharacterKey == null)
        {
            Debug.LogError("Null TargetCharacterKey key set at TCastHeal");
            return false;
        }
        if (HealSpellCostKey == null)
        {
            Debug.LogError("Null HealSpellCostKey key set at TCastHeal");
            return false;
        }
        if (HealSpellRateKey == null)
        {
            Debug.LogError("Null HealSpellRateKey key set at TCastHeal");
            return false;
        }

        return true;
    }

    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTargetCharacterKey(string key)
    {
        TargetCharacterKey = key;
    }
    public void SetHealSpellCostKey(string key)
    {
        HealSpellCostKey = key;
    }
    public void SetHealSpellRateKey(string key)
    {
        HealSpellRateKey = key;
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

        bt.SetCurrentTaskName(TaskName);
        bt.SetCurrentNode(this);


        Blackboard BB = bt.GetBlackboard();
        Self = BB.GetValue<Character>(SelfKey);
        TargetCharacter = BB.GetValue<Character>(TargetCharacterKey);
        HealSpellCost = BB.GetValue<float>(HealSpellCostKey);
        HealSpellRate = BB.GetValue<float>(HealSpellRateKey);

        TargetCharacter.Heal(HealSpellRate);
        Self.DecreaseMana(HealSpellCost);
        Self.PerformedAction();

        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
