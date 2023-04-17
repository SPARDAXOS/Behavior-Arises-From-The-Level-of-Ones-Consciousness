using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTaunt : Task
{
    private string TaskName = "Taunt";

    private string SelfKey = null;
    private string TauntRateKey = null;
    private string SpellCostKey = null;

    private Character Self = null;
    private float TauntRate = 0.0f;
    private float SpellCost = 0.0f;

    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("SelfKey is null - TIncreaseSelfAggro");
            return false;
        }
        if (TauntRateKey == null)
        {
            Debug.LogError("TauntRateKey is null - TIncreaseSelfAggro");
            return false;
        }
        if (SpellCostKey == null)
        {
            Debug.LogError("SpellCostKey is null - TIncreaseSelfAggro");
            return false;
        }

        return true;
    }

    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTauntRateKey(string key)
    {
        TauntRateKey = key;
    }
    public void SetSpellCostKey(string key)
    {
        SpellCostKey = key;
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

        TauntRate = bb.GetValue<float>(TauntRateKey);
        SpellCost = bb.GetValue<float>(SpellCostKey);
        Self = bb.GetValue<Character>(SelfKey);

        Self.IncreaseAggro(TauntRate);
        Self.DecreaseMana(SpellCost);
        Self.PerformedAction();

        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
