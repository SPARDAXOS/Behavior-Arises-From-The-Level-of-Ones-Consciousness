using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCooldown : Decorator
{
    private float CooldownDuration = 0.0f;
    private float Timer = 0.0f;


    public void SetCooldownDuration(float duration)
    {
        CooldownDuration = duration;
    }


    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt) // NOT FINISHED DONT USE
    {
        Timer += Time.deltaTime;
        if(Timer >= CooldownDuration)
        {
            Timer = 0.0f;
            return ConnectedNode.Execute(bt);
        }

        return BehaviorTree.ExecutionState.SUCCESS;
    } // I really iam getting confused now on how this thing returns what and behaves
}
