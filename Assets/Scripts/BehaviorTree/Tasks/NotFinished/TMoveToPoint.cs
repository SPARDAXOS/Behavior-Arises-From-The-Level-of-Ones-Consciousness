using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMoveToPoint : Task
{
    private string TargetPointKey = null;
    private string SelfKey = null;
    private string MovementSpeedKey = null;

    private string TaskName = "Moving To Point";

    private bool Running = false;

    private Character Self = null;

    private Vector3 TargetPoint = new Vector3(0.0f, 0.0f, 0.0f);
    private float MovementSpeed = 0.0f;

    private float SuccessAdjustmentRadius = 0.5f;

    private Vector3 Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);

    public void SetTargetPointKey(string key)
    {
        TargetPointKey = key;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetMovementSpeedKey(string key)
    {
        MovementSpeedKey = key;
    }
    public void SetSuccessAdjustmentRadius(float radius)
    {
        SuccessAdjustmentRadius = radius;
    }

    private void CalculateDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Direction = Vector3.Normalize(targetPos - currentPos);
    }
    private void CalculateVelocity(float speed)
    {
        Velocity = Direction * speed * Time.deltaTime;
    }

    private BehaviorTree.ExecutionState CheckRadius(BehaviorTree bt, Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);

        if (Length <= SuccessAdjustmentRadius)
        {
            Self.transform.position = targetPos;
            Running = false;

            return BehaviorTree.ExecutionState.SUCCESS;
        }

        return BehaviorTree.ExecutionState.RUNNING;
    }
    private BehaviorTree.ExecutionState MoveToPosition(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetPoint = bb.GetValue<Vector3>(TargetPointKey);
        float Speed = bb.GetValue<float>(MovementSpeedKey);


        BehaviorTree.ExecutionState State = CheckRadius(bt, Self.transform.position, TargetPoint); // Dont move at all if in success range
        if (State == BehaviorTree.ExecutionState.SUCCESS) // Its this quick bale probably
        {
            Self.transform.position = TargetPoint;
            Running = false;
            return State;
        }

        CalculateDirection(Self.transform.position, TargetPoint);
        CalculateVelocity(Speed);
        Self.transform.position += Velocity;
        State = CheckRadius(bt, Self.transform.position, TargetPoint);

        return State;
    }

    public override void Interrupt()
    {
        Running = false;
    }

    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt) // Maybe send the unique id for each object? 
    {
        if (TargetPointKey == null)
        {
            Debug.LogError("Null TargetPointKey set at TMoveToPoint");
            return BehaviorTree.ExecutionState.ERROR;
        }
        if (SelfKey == null)
        {
            Debug.LogError("Null SelfKey set at TMoveToPoint");
            return BehaviorTree.ExecutionState.ERROR;
        }
        if (MovementSpeedKey == null)
        {
            Debug.LogError("Null MovementSpeed key set at TMoveToPoint");
            return BehaviorTree.ExecutionState.ERROR;
        }

        if (!Running)
        {
            Status = RunningStatus.RUNNING; //On first tick gets set to this.
            Running = true;
            bt.SetCurrentTaskName(TaskName);
        }
        bt.SetCurrentNode(this);

        BehaviorTree.ExecutionState State = MoveToPosition(bt);

        return State;
    }
}
