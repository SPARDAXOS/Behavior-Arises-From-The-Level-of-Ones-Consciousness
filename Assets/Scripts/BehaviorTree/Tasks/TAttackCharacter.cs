using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAttackCharacter : Task
{
    private string SelfKey = null;
    private string TargetCharacterKey = null;
    private string AttackDamageKey = null;
    private string AggroIncreaseRateAKey = null;

    private string TaskName = "Attacking";

    private Character SelfCharacter = null;
    private Character TargetCharacter = null;
    private float AttackDamage = 0.0f;
    private float AggroIncreaseRateA = 0.0f;

    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("Null SelfKey set at TAttackCharacter");
            return false;
        }
        if (TargetCharacterKey == null)
        {
            Debug.LogError("Null TargetCharacterKey set at TAttackCharacter");
            return false;
        }
        if (AttackDamageKey == null)
        {
            Debug.LogError("Null AttackDamageKey set at TAttackCharacter");
            return false;
        }
        if (AggroIncreaseRateAKey == null)
        {
            Debug.LogError("Null AggroIncreaseRateAKey set at TAttackCharacter");
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
    public void SetAttackDamageKey(string key)
    {
        AttackDamageKey = key;
    }
    public void SetAggroIncreaseRateAKey(string key)
    {
        AggroIncreaseRateAKey = key;
    }

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        return BehaviorTree.EvaluationState.SUCCESS;
    }

    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt) // Maybe send the unique id for each object? 
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        bt.SetCurrentTaskName(TaskName);
        bt.SetCurrentNode(this);

        Blackboard BB = bt.GetBlackboard();
        TargetCharacter = BB.GetValue<Character>(TargetCharacterKey);
        SelfCharacter = BB.GetValue<Character>(SelfKey);
        AttackDamage = BB.GetValue<float>(AttackDamageKey);
        AggroIncreaseRateA = BB.GetValue<float>(AggroIncreaseRateAKey);

        TargetCharacter.TakeDamage(AttackDamage);
        SelfCharacter.IncreaseAggro(AggroIncreaseRateA);
        SelfCharacter.PerformedAction();
        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
