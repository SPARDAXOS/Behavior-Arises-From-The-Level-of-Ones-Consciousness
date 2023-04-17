using UnityEngine;

public class TShootProjectileAtCharacter : Task
{
    private enum ConditionResult
    {
        SUCCESS,
        FAILURE,
        RUNNING,
        ERROR
    }

    private bool Running = false;

    private string TaskName = "Casting Fireball";

    private float SuccessRadius = 2.0f; //Default

    private string SelfKey = null;
    private string TargetCharacterKey = null;
    private string SpawnPositionKey = null;
    private string ProjectileKey = null;
    private string ProjectileCostKey = null;
    private string ProjectileSpeedKey = null;
    private string ProjectileDamageKey = null;
    private string AggroIncreaseRateKey = null;

    private Character Self = null;
    private Character TargetCharacter = null;
    private GameObject Projectile = null;
    private float ProjectileCost = 0.0f;
    private float ProjectileSpeed = 0.0f;
    private float ProjectileDamage = 0.0f;
    private float AggroIncreaseRate = 0.0f;


    private Vector3 SpawnPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);


    private bool AreKeysValid()
    {
        if (SelfKey == null)
        {
            Debug.LogError("Null Self key set at TShootProjectileAtCharacter");
            return false;
        }
        if (TargetCharacterKey == null)
        {
            Debug.LogError("Null TargetCharacterKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (SpawnPositionKey == null)
        {
            Debug.LogError("Null SpawnPositionKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (ProjectileKey == null)
        {
            Debug.LogError("Null TargetCharacterKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (ProjectileCostKey == null)
        {
            Debug.LogError("Null ProjectileCostKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (ProjectileSpeedKey == null)
        {
            Debug.LogError("Null TargetCharacterKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (ProjectileDamageKey == null)
        {
            Debug.LogError("Null TargetCharacterKey key set at TShootProjectileAtCharacter");
            return false;
        }
        if (AggroIncreaseRateKey == null)
        {
            Debug.LogError("Null AggroIncreaseRateKey key set at TShootProjectileAtCharacter");
            return false;
        }
        return true;
    }

    public void SetSuccessRadius(float radius)
    {
        SuccessRadius = radius;
    }


    public void SetSelfKey(string key)
    {
        SelfKey = key;
    }
    public void SetTargetCharacterKey(string key)
    {
        TargetCharacterKey = key;
    }
    public void SetSpawnPositionKey(string key)
    {
        SpawnPositionKey = key;
    }
    public void SetProjectileKey(string key)
    {
        ProjectileKey = key;
    }
    public void SetProjectileCostKey(string key)
    {
        ProjectileCostKey = key;
    }
    public void SetProjectileSpeedKey(string key)
    {
        ProjectileSpeedKey = key;
    }
    public void SetProjectileDamageKey(string key)
    {
        ProjectileDamageKey = key;
    }
    public void SetAggroIncreaseRateKey(string key)
    {
        AggroIncreaseRateKey = key;
    }


    private void CalculateDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Direction = Vector3.Normalize(targetPos - currentPos);
    }
    private void CalculateVelocity(float speed)
    {
        Velocity = Direction * speed * Time.fixedDeltaTime;
    }

    private ConditionResult QuickBailRadiusCheck(Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);
        if (Length <= SuccessRadius && Projectile.activeInHierarchy)
            return ConditionResult.SUCCESS;
        return ConditionResult.RUNNING;
    }
    private ConditionResult CheckRadius(Vector3 currentPos, Vector3 targetPos)
    {
        float Length = Vector3.Magnitude(targetPos - currentPos);
        if(Length <= SuccessRadius)
        {
            Projectile.SetActive(false);
            TargetCharacter.TakeDamage(ProjectileDamage);
            Self.PerformedAction();
            Self.DecreaseMana(ProjectileCost);
            Self.IncreaseAggro(AggroIncreaseRate);

            Status = RunningStatus.NOT_RUNNING;
            Running = false;
            return ConditionResult.SUCCESS;
        }
        
        return ConditionResult.RUNNING;
    }
    private ConditionResult ShootTo(BehaviorTree bt)
    {
        Blackboard bb = bt.GetBlackboard();
        Self = bb.GetValue<Character>(SelfKey);
        TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
        Projectile = bb.GetValue<GameObject>(ProjectileKey);
        ProjectileCost = bb.GetValue<float>(ProjectileCostKey);
        ProjectileSpeed = bb.GetValue<float>(ProjectileSpeedKey);
        ProjectileDamage = bb.GetValue<float>(ProjectileDamageKey);
        AggroIncreaseRate = bb.GetValue<float>(AggroIncreaseRateKey);

        TargetPosition = TargetCharacter.transform.position;

        CalculateDirection(Projectile.transform.position, TargetPosition);
        CalculateVelocity(ProjectileSpeed);
        Projectile.transform.position += Velocity;
        ConditionResult State = CheckRadius(Projectile.transform.position, TargetPosition);

        return State;
    }


    public override void Interrupt()
    {
        Status = RunningStatus.NOT_RUNNING;
        if (!Projectile)
            Debug.LogError("Attempted interrupting while Projectile is null - Was Not Running");
        Projectile.SetActive(false);
        Running = false;
    }
    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (!AreKeysValid())
            return BehaviorTree.EvaluationState.ERROR;

        bt.SetCurrentNode(this);

        Blackboard bb = bt.GetBlackboard();
        
        TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
        TargetPosition = TargetCharacter.transform.position;

        //Quick bail out.
        ConditionResult State = QuickBailRadiusCheck(Projectile.transform.position, TargetPosition);

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

        Blackboard bb = bt.GetBlackboard();
        Projectile = bb.GetValue<GameObject>(ProjectileKey);
        Self = bb.GetValue<Character>(SelfKey);
        TargetCharacter = bb.GetValue<Character>(TargetCharacterKey);
        TargetPosition = TargetCharacter.transform.position;

        //Quick bail out.
        ConditionResult State = QuickBailRadiusCheck(Projectile.transform.position, TargetPosition);
        if (State == ConditionResult.SUCCESS)
            return BehaviorTree.ExecutionState.SUCCESS;

        if (!Running)
        {
            SpawnPosition = bt.GetBlackboard().GetValue<Vector3>(SpawnPositionKey);
            Projectile.transform.position = SpawnPosition;
            Projectile.SetActive(true);

            Running = true;
            Status = RunningStatus.RUNNING;
        }
        bt.SetCurrentTaskName(TaskName);
        bt.SetCurrentNode(this);

        State = ShootTo(bt);
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
