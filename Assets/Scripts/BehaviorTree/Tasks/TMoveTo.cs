using UnityEngine;

public class TMoveTo : Task
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        RUNNING,
        ERROR
    }
    public enum TargetType
    {
        POSITION,
        CHARACTER
    }

    private TargetType Type = TargetType.POSITION;

    private bool Running = false;

    private string TargetPositionKey = null;
    private string TargetCharacterKey = null;
    private string SelfKey = null;
    private string MovementSpeedKey = null;

    private string TaskName = "Moving";

    private float SuccessRadius = 2.0f;

    private Character Self = null;
    private Character TargetCharacter = null;
    private Vector3 TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);

    private float MovementSpeed = 0.0f;

    private Vector3 FinalPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);

    private bool AreKeysValid()
    {
        switch (Type)
        {
            case TargetType.POSITION:
                {
                    if (TargetPositionKey == null)
                    {
                        Debug.LogError("Null TargetPositionKey set at TMoveTo");
                        return false;
                    }
                }
                break;
            case TargetType.CHARACTER:
                {
                    if (TargetCharacterKey == null)
                    {
                        Debug.LogError("Null TargetCharacterKey set at TMoveTo");
                        return false;
                    }
                }
                break;
        }
        if (SelfKey == null)
        {
            Debug.LogError("Null SelfKey set at TMoveTo");
            return false;
        }
        if (MovementSpeedKey == null)
        {
            Debug.LogError("Null MovementSpeed key set at TMoveTo");
            return false;
        }
        return true;
    }
    public void SetTargetType(TargetType type)
    {
        Type = type;
    }
    public void SetTargetPositionKey(string key)
    {
        TargetPositionKey = key;
    }
    public void SetTargetCharacterKey(string key)
    {
        TargetCharacterKey = key;
    }
    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetMovementSpeedKey(string key)
    {
        MovementSpeedKey = key;
    }
    public void SetSuccessRadius(float radius)
    {
        SuccessRadius = radius;
    }



    private void CalculateDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Direction = Vector3.Normalize(targetPos - currentPos);
    }
    private void CalculateVelocity(float speed)
    {
        Velocity = Direction * speed * Time.fixedDeltaTime;
    }

    private ConditionResult EvaluationRadiusCheck(Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);
        if (Length <= SuccessRadius)
            return ConditionResult.SUCCESS;

        return ConditionResult.RUNNING;
    }
    private ConditionResult CheckRadius(Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);
        if(Length <= SuccessRadius)
        {
            Status = RunningStatus.NOT_RUNNING;
            Running = false;
            return ConditionResult.SUCCESS;
        }
        
        return ConditionResult.RUNNING;
    }
    private ConditionResult MoveTo(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        MovementSpeed = bb.GetValue<float>(MovementSpeedKey);

        switch (Type)
        {
            case TargetType.POSITION:
                {
                    TargetPosition = bb.GetValue<Vector3>(TargetPositionKey);
                    FinalPosition = TargetPosition;
                }
                break;
            case TargetType.CHARACTER:
                {
                    TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
                    FinalPosition = TargetCharacter.transform.position;
                }
                break;
        }

        //Quick bail out.
        ConditionResult State = CheckRadius(Self.transform.position, FinalPosition);
        if (State == ConditionResult.SUCCESS)
            return State;

        CalculateDirection(Self.transform.position, FinalPosition);
        CalculateVelocity(MovementSpeed);
        Self.transform.position += Velocity;
        State = CheckRadius(Self.transform.position, FinalPosition);

        return State;
    }


    public override void Interrupt()
    {
        Status = RunningStatus.NOT_RUNNING;
        Running = false;
    }


    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        Blackboard bb = bt.GetBlackboard();
        Character Self = bb.GetValue<Character>(SelfKey);

        switch (Type)
        {
            case TargetType.POSITION:
                {
                    TargetPosition = bb.GetValue<Vector3>(TargetPositionKey);
                    FinalPosition = TargetPosition;
                }
                break;
            case TargetType.CHARACTER:
                {
                    TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
                    FinalPosition = TargetCharacter.transform.position;
                }
                break;
        }

        //Quick bail out.
        ConditionResult State = EvaluationRadiusCheck(Self.transform.position, FinalPosition);

        switch (State)
        {
            case ConditionResult.SUCCESS:
                return BehaviorTree.EvaluationState.SUCCESS;
            case ConditionResult.FAILURE:
                return BehaviorTree.EvaluationState.FAILURE;
            case ConditionResult.RUNNING:
                return BehaviorTree.EvaluationState.RUNNING;
            case ConditionResult.ERROR:
                return BehaviorTree.EvaluationState.ERROR;
        }

        return BehaviorTree.EvaluationState.ERROR;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt) // Maybe send the unique id for each object? 
    {
        if (!AreKeysValid())
            return BehaviorTree.ExecutionState.ERROR;

        if (!Running)
        {
            Running = true;
            Status = RunningStatus.RUNNING;
            bt.SetCurrentTaskName(TaskName);
        }
        bt.SetCurrentNode(this);

        ConditionResult State = MoveTo(bt);

        switch (State)
        {
            case ConditionResult.SUCCESS:
                return BehaviorTree.ExecutionState.SUCCESS;
            case ConditionResult.FAILURE:
                return BehaviorTree.ExecutionState.FAILURE;
            case ConditionResult.RUNNING:
                return BehaviorTree.ExecutionState.RUNNING;
            case ConditionResult.ERROR:
                return BehaviorTree.ExecutionState.ERROR;
        }

        return BehaviorTree.ExecutionState.ERROR;
    }
}
