using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFindTarget : Task
{
    public enum SearchType // Can easily add more
    {
        LOWEST_HEALTH,
        HIGHEST_HEALTH,
        LOWEST_AGGRO,
        HIGHEST_AGGRO
    }

    private string TaskName = "Looking For Target";

    private SearchType Type = SearchType.LOWEST_HEALTH;
    private string TargetPartyKey = null;
    private string ResultCharacterKey = null;

    private float LowestHealth = 0.0f;
    private float HighestHealth = 0.0f;

    private float LowestAggro = 0.0f;
    private float HighestAggro = 0.0f;
    private Character TargetCharacter = null;


    private bool AreKeysValid()
    {
        if (TargetPartyKey == null)
        {
            Debug.LogError("TargetPartyKey null at TFindTarget");
            return false;
        }
        if (ResultCharacterKey == null)
        {
            Debug.LogError("ResultCharacterKey null at TFindTarget");
            return false;
        }

        return true;
    }

    public void SetSearchType(SearchType type)
    {
        Type = type;
    }
    public void SetEnemiesPartyKey(string key)
    {
        TargetPartyKey = key;
    }
    public void SetResultCharacterKey(string key)
    {
        ResultCharacterKey = key;
    }

    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        Blackboard BB = bt.GetBlackboard();
        Party EnemiesGroup = BB.GetValue<Party>(TargetPartyKey);
        Character[] Enemies = EnemiesGroup.GetCharactersLeft();
        if (Enemies == null)
        {
            Debug.LogWarning("No enemies were found - Evaluation");
            return BehaviorTree.EvaluationState.FAILURE;
        }

        return BehaviorTree.EvaluationState.SUCCESS;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        bt.SetCurrentTaskName(TaskName);
        bt.SetCurrentNode(this);

        Blackboard BB = bt.GetBlackboard();
        Party EnemiesGroup = BB.GetValue<Party>(TargetPartyKey);
        Character[] Enemies = EnemiesGroup.GetCharactersLeft();
        if (Enemies == null)
        {
            Debug.LogWarning("No enemies were found");
            return BehaviorTree.ExecutionState.FAILURE;
        }
        switch (Type)
        {
            case SearchType.LOWEST_HEALTH:
                {
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (!Enemies[i]) // In case entry is empty
                            continue;
                        if (i == 0)
                        {
                            LowestHealth = Enemies[i].GetCurrentHealth();
                            TargetCharacter = Enemies[i];
                            continue;
                        }
                        if(Enemies[i].GetCurrentHealth() < LowestHealth)
                        {
                            LowestHealth = Enemies[i].GetCurrentHealth();
                            TargetCharacter = Enemies[i];
                        }
                    }
                    BB.UpdateValue<Character>(ResultCharacterKey, TargetCharacter);
                    return BehaviorTree.ExecutionState.SUCCESS;
                }
            case SearchType.HIGHEST_HEALTH:
                {
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (!Enemies[i]) // In case entry is empty
                            continue;
                        if (i == 0)
                        {
                            HighestHealth = Enemies[i].GetCurrentHealth();
                            TargetCharacter = Enemies[i];
                            continue;
                        }
                        if (Enemies[i].GetCurrentHealth() > HighestHealth)
                        {
                            HighestHealth = Enemies[i].GetCurrentHealth();
                            TargetCharacter = Enemies[i];
                        }
                    }
                    BB.UpdateValue<Character>(ResultCharacterKey, TargetCharacter);
                    return BehaviorTree.ExecutionState.SUCCESS;
                }
            case SearchType.LOWEST_AGGRO:
                {
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (!Enemies[i]) // In case entry is empty
                            continue;
                        if (i == 0)
                        {
                            LowestAggro = Enemies[i].GetCurrentAggro();
                            TargetCharacter = Enemies[i];
                            continue;
                        }
                        if (Enemies[i].GetCurrentAggro() < LowestAggro)
                        {
                            LowestAggro = Enemies[i].GetCurrentAggro();
                            TargetCharacter = Enemies[i];
                        }
                    }
                    BB.UpdateValue<Character>(ResultCharacterKey, TargetCharacter);
                    return BehaviorTree.ExecutionState.SUCCESS;
                }
            case SearchType.HIGHEST_AGGRO:
                {
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (Enemies[i])
                        {
                            if (i == 0)
                            {
                                HighestAggro = Enemies[i].GetCurrentAggro();
                                TargetCharacter = Enemies[i];
                                continue;
                            }
                            if (Enemies[i].GetCurrentAggro() > HighestAggro)
                            {
                                HighestAggro = Enemies[i].GetCurrentAggro();
                                TargetCharacter = Enemies[i];
                            }
                        }
                    }
                    BB.UpdateValue<Character>(ResultCharacterKey, TargetCharacter);
                    return BehaviorTree.ExecutionState.SUCCESS;
                }
        }

        return BehaviorTree.ExecutionState.ERROR;
    }

}
