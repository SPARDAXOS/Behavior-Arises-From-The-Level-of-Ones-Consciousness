using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Core Attributes
    protected bool CanPerformAction = false;

    protected float CurrentSenseTickRate = 0.5f;
    protected float CurrentEvaluationTickRate = 0.5f;
    protected float CurrentExecutionTickRate = 0.5f;

    protected float CurrentMovementSpeed = 40.0f;

    protected float CurrentHealthCap = 100.0f;
    protected float CurrentHealth = 100.0f;

    protected float CurrentManaCap = 100.0f;
    public float CurrentMana = 100.0f;
    protected float CurrentManaIncreaseRate = 10.0f;

    protected float CurrentAggro = 10.0f;
    protected float CurrentAggroIncreaseARate = 5.0f;

    protected float CurrentAttackDamage = 10.0f;

    protected float CurrentSuperCost = 100.0f;

    protected float CurrentFHealthDangerThreshold = 60.0f;
    protected float CurrentEHealthDangerThreshold = 40.0f;


    //Color Effects Related
    protected float DamagedCEDuration = 0.3f;
    protected float HealedCEDuration = 1.2f;
    protected float BuffedCEDuration = 1.0f;
    protected float DebuffedCEDuration = 1.0f;
    protected float SuperCEDuration = 1.5f;

    protected Color DamagedCEColor = Color.red;
    protected Color HealedCEColor = new Color(212.0f, 175.0f, 55.0f, 255.0f);
    protected Color BuffedCEColor = new Color(0.0f, 255.0f, 24.0f, 255.0f);
    protected Color DebuffedCEColor = new Color(255.0f, 0.0f, 255.0f, 255.0f);
    protected Color SuperCEColor = Color.blue;


    //Blackboard Keys
    protected string CanPerformActionKey = "CanPerformAction";
    protected string MovementSpeedKey = "MovementSpeed";
    protected string HealthKey = "Health";
    protected string ManaKey = "Mana";
    protected string AggroKey = "Aggro";
    protected string AttackDamageKey = "AttackDamage";
    protected string SuperCostKey = "SuperCost";
    protected string SelfKey = "Self";
    protected string FriendsPartyKey = "FriendsParty";
    protected string EnemiesPartyKey = "EnemiesParty";
    protected string EnemiesCountKey = "EnemiesCount";
    protected string TargetCharacterKey = "TargetCharacter";
    protected string SlotPositionKey = "SlotPosition";
    protected string AggroIncreaseRateAKey = "AggroIncreaseRate_Attack";
    protected string LowestCostSpellKey = "LowestCostSpell";


    //Related Systems References
    protected Sprite Portrait = null;
    protected HUD HUDScript = null;
    protected BehaviorTree ControllingBT = null;


    //Buffs/Debuffs Collections
    protected List<StatusAilments.Buffs> AppliedBuffsTypes = new List<StatusAilments.Buffs>();
    protected List<StatusAilments.Debuffs> AppliedDebuffsTypes = new List<StatusAilments.Debuffs>();

    protected Dictionary<StatusAilments.Buffs, float> CurrentBuffs = new Dictionary<StatusAilments.Buffs, float>();
    protected Dictionary<StatusAilments.Debuffs, float> CurrentDebuffs = new Dictionary<StatusAilments.Debuffs, float>();

    protected Dictionary<StatusAilments.Buffs, int> CurrentBuffsTurns = new Dictionary<StatusAilments.Buffs, int>();
    protected Dictionary<StatusAilments.Debuffs, int> CurrentDebuffsTurns = new Dictionary<StatusAilments.Debuffs, int>();


    //Timers
    protected float SenseTimer = 0.0f;
    protected float EvaluationTimer = 0.0f;
    protected float ExecutionTimer = 0.0f;

    protected float ColorEffectTimer = 0.0f;


    //Initilization Related
    protected virtual void LoadResources()
    {
    }
    protected virtual void SetupStartState()
    {
    }
    protected virtual void SetupHUDElements()
    {
    }
    protected virtual void SetupPosition()
    {
    }
    protected virtual void SetupBlackboardEntries()
    {
        Blackboard BB = ControllingBT.GetBlackboard();

        BB.AddValue<Character>(SelfKey, this);
        BB.AddValue<Character>(TargetCharacterKey, null);



        BB.AddValue<Vector3>(SlotPositionKey, transform.position);
        BB.AddValue<bool>(CanPerformActionKey, CanPerformAction);

        BB.AddValue<float>(MovementSpeedKey, CurrentMovementSpeed);

        BB.AddValue<float>(HealthKey, CurrentHealth);
        BB.AddValue<float>(ManaKey, CurrentMana);

        BB.AddValue<float>(AttackDamageKey, CurrentAttackDamage);

        BB.AddValue<float>(SuperCostKey, CurrentSuperCost);

        BB.AddValue<float>(AggroKey, CurrentAggro);
        BB.AddValue<float>(AggroIncreaseRateAKey, CurrentAggroIncreaseARate);
    }
    public virtual void Init()
    {
    }


    //Systems References Sets
    public void SetHUDScriptReference(HUD hud)
    {
        if (!hud)
        {
            Debug.LogError("Null hud reference sent to " + name);
        }
        HUDScript = hud;
    }
    public void SetBehaviorTree(BehaviorTree bt)
    {
        if (bt == null)
        {
            Debug.LogError("Invalid behavior tree reference sent to " + name);
            return;
        }
        else
            ControllingBT = bt;
    }


    //Not Used As Keys Anywhere
    public float GetFHealthDangerThreshold()
    {
        return CurrentFHealthDangerThreshold;
    }
    public float GetEHealthDangerThreshold()
    {
        return CurrentEHealthDangerThreshold;
    }


    //For getting these stats when the target is not self
    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }
    public float GetCurrentMana()
    {
        return CurrentMana;
    }
    public float GetCurrentAggro()
    {
        return CurrentAggro;
    }


    //Blackboard Keys Gets
    public string GetCanPerformActionKey()
    {
        return CanPerformActionKey;
    }
    public string GetMovementSpeedKey()
    {
        return MovementSpeedKey;
    }
    public string GetHealthKey()
    {
        return HealthKey;
    }
    public string GetManaKey()
    {
        return ManaKey;
    }
    public string GetAggroKey()
    {
        return AggroKey;
    }
    public string GetAttackDamageKey()
    {
        return AttackDamageKey;
    }
    public string GetSuperCostKey()
    {
        return SuperCostKey;
    }
    public string GetSelfKey()
    {
        return SelfKey;
    }
    public string GetFriendsPartyKey()
    {
        return FriendsPartyKey;
    }
    public string GetEnemiesPartyKey()
    {
        return EnemiesPartyKey;
    }
    public string GetEnemiesCountKey()
    {
        return EnemiesCountKey;
    }
    public string GetTargetCharacterKey()
    {
        return TargetCharacterKey;
    }
    public string GetSlotPositionKey()
    {
        return SlotPositionKey;
    }
    public string GetAggroIncreaseRateAKey()
    {
        return AggroIncreaseRateAKey;
    }
    public string GetLowestCostSpellKey()
    {
        return LowestCostSpellKey;
    }


    //Turn Related
    public virtual void EndTurn()
    {
        //Reset timers
        SenseTimer = CurrentSenseTickRate;
        EvaluationTimer = CurrentEvaluationTickRate;
        ExecutionTimer = CurrentExecutionTickRate;

        //Turn related
        IncreaseMana(CurrentManaIncreaseRate);
        DegradeCurrentBuffs();
        DegradeCurrentDebuffs();
        ResetColorEffects();

        //Reset BT
        ControllingBT.Reset();
    }
    public void PerformedAction()
    {
        CanPerformAction = false;
        ControllingBT.GetBlackboard().UpdateValue<bool>(CanPerformActionKey, CanPerformAction);
    }
    public void ResetCanPerformAction()
    {
        CanPerformAction = true;
        ControllingBT.GetBlackboard().UpdateValue<bool>(CanPerformActionKey, CanPerformAction);
    }


    //Buffs/Debuffs Related
    public bool IsBuffApplied(StatusAilments.Buffs buff)
    {
        if (CurrentBuffs.Count == 0)
            return false;
        if (CurrentBuffs.ContainsKey(buff))
            return true;
        if (CurrentBuffsTurns.ContainsKey(buff))
        {
            Debug.LogError("buff " + buff.ToString() + " does not exist in CurrentBuffs but is in CurrentBuffsTurns");
            return true;
        } //To catch inconsistency errors
        return false;
    }
    public bool IsDebuffApplied(StatusAilments.Debuffs debuff)
    {
        if (CurrentDebuffs.Count == 0)
            return false;
        if (CurrentDebuffs.ContainsKey(debuff))
            return true;
        if (CurrentDebuffsTurns.ContainsKey(debuff)) //To catch inconsistency errors
        {
            Debug.LogError("debuff " + debuff.ToString() + " does not exist in CurrentDebuffs but is in CurrentDebuffsTurns");
            return true;
        }
        return false;
    }

    public void ApplyBuff(StatusAilments.Buffs buff, float rate, int turns)
    {
        if (IsBuffApplied(buff))
        {
            Debug.LogWarning("Attempted applying " + buff.ToString() + " buff but it was already applied!");
            return;
        }
        CurrentBuffs.Add(buff, rate);
        CurrentBuffsTurns.Add(buff, turns);
        AppliedBuffsTypes.Add(buff);
        ApplyBuffEffects(buff);
        ApplyColorEffect(BuffedCEColor, BuffedCEDuration);
    }
    public void ApplyDebuff(StatusAilments.Debuffs debuff, float rate, int turns)
    {
        if (IsDebuffApplied(debuff))
        {
            Debug.LogWarning("Attempted applying " + debuff.ToString() + " debuff but it was already applied!");
            return;
        }
        CurrentDebuffs.Add(debuff, rate);
        CurrentDebuffsTurns.Add(debuff, turns);
        AppliedDebuffsTypes.Add(debuff);
        ApplyDebuffEffects(debuff);
        ApplyColorEffect(DebuffedCEColor, DebuffedCEDuration);
    }

    public void UnapplyBuff(StatusAilments.Buffs buff)
    {
        if (!IsBuffApplied(buff))
        {
            Debug.LogError("Attempted to unapply buff " + buff.ToString() + " but it was not applied");
            return;
        }
        UapplyBuffEffects(buff);
        CurrentBuffs.Remove(buff);
        CurrentBuffsTurns.Remove(buff);
        AppliedBuffsTypes.Remove(buff);
    }
    public void UnapplyDebuff(StatusAilments.Debuffs debuff)
    {
        if (!IsDebuffApplied(debuff))
        {
            Debug.LogError("Attempted to unapply debuff " + debuff.ToString() + " but it was not applied");
            return;
        }
        UapplyDebuffEffects(debuff);
        CurrentDebuffs.Remove(debuff);
        CurrentDebuffsTurns.Remove(debuff);
        AppliedDebuffsTypes.Remove(debuff);
    }

    protected float GetAppliedBuffRate(StatusAilments.Buffs buff)
    {
        if (!IsBuffApplied(buff))
        {
            Debug.LogError("Attempted to get buff rate for a not applied buff " + buff + " - GetAppliedBuffRate");
            return 0.0f;
        }
        return CurrentBuffs[buff];
    }
    protected float GetAppliedDebuffRate(StatusAilments.Debuffs debuff)
    {
        if (!IsDebuffApplied(debuff))
        {
            Debug.LogError("Attempted to get debuff rate for a not applied debuff " + debuff + " - GetAppliedDebuffRate");
            return 0.0f;
        }
        return CurrentDebuffs[debuff];
    }

    private void DegradeCurrentBuffs()
    {
        if (CurrentBuffs.Count == 0)
            return;

        int Num = -1;

        for (int i = 0; i < AppliedBuffsTypes.Count; i++)
        {
            if (CurrentBuffsTurns.ContainsKey(AppliedBuffsTypes[i]))
            {
                CurrentBuffsTurns[AppliedBuffsTypes[i]]--;
                Num = CurrentBuffsTurns[AppliedBuffsTypes[i]];
                if (Num == 0)
                    UnapplyBuff(AppliedBuffsTypes[i]);
            }
            else
                Debug.LogError("CurrentBuffsTurns did not contain a key from AppliedBuffsTypes - Inconsistency");
        }
    }
    private void DegradeCurrentDebuffs()
    {
        if (CurrentDebuffs.Count == 0)
            return;

        int Num = -1;

        for (int i = 0; i < AppliedDebuffsTypes.Count; i++)
        {
            if (CurrentDebuffsTurns.ContainsKey(AppliedDebuffsTypes[i]))
            {
                CurrentDebuffsTurns[AppliedDebuffsTypes[i]]--;
                Num = CurrentDebuffsTurns[AppliedDebuffsTypes[i]];
                if (Num == 0)
                    UnapplyDebuff(AppliedDebuffsTypes[i]);
            }
            else
                Debug.LogError("CurrentDebuffsTurns did not contain a key from AppliedDebuffsTypes - Inconsistency");
        }
    }


    //Requires Unique Character Index
    protected virtual void ApplyBuffEffects(StatusAilments.Buffs buff)
    {
    }
    protected virtual void ApplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
    }
    protected virtual void UapplyBuffEffects(StatusAilments.Buffs buff)
    {
    }
    protected virtual void UapplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
    }


    //Visual Feedback Related
    protected void ApplyColorEffect(Color color, float duration)
    {
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        SR.color = color;
        ColorEffectTimer = duration;
    }
    private void UpdateColorEffectsTimers()
    {
        if (ColorEffectTimer > 0.0f)
        {
            ColorEffectTimer -= Time.deltaTime;
            if(ColorEffectTimer < 0.0f)
            {
                ColorEffectTimer = 0.0f;
                ResetColorEffects();
            }
        }
    }
    protected void ResetColorEffects()
    {
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        SR.color = Color.white;
    }


    //Attributes Related
    public virtual void IncreaseMana(float amount)
    {
        CurrentMana += amount;
        if (CurrentMana > CurrentManaCap)
            CurrentMana = CurrentManaCap;
        ControllingBT.GetBlackboard().UpdateValue<float>(ManaKey, CurrentMana);
    }
    public virtual void DecreaseMana(float amount)
    {
        CurrentMana -= amount;
        if (CurrentMana < 0.0f)
            CurrentMana = 0.0f;
        ControllingBT.GetBlackboard().UpdateValue<float>(ManaKey, CurrentMana);
    }
    public virtual void IncreaseAggro(float amount)
    {
        CurrentAggro += amount;
        ControllingBT.GetBlackboard().UpdateValue<float>(AggroKey, CurrentAggro);
    }


    //Currrently Only Player Characters (Could Be Used For Enemy Characters Special Attacks)
    public virtual void UseSuper(float cost)
    {
    }


    //Health Related
    public virtual void TakeDamage(float damage)
    {
        if (IsBuffApplied(StatusAilments.Buffs.DEFENSE))
        {
            damage -= GetAppliedBuffRate(StatusAilments.Buffs.DEFENSE);
            if (damage < 0.0f)
                damage = 0.0f;
        }
        if (IsDebuffApplied(StatusAilments.Debuffs.DEFENSE))
            damage += GetAppliedDebuffRate(StatusAilments.Debuffs.DEFENSE);

        CurrentHealth -= damage;
        ControllingBT.GetBlackboard().UpdateValue<float>(HealthKey, CurrentHealth);

        ApplyColorEffect(DamagedCEColor, DamagedCEDuration);
    }
    public virtual void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > CurrentHealthCap)
            CurrentHealth = CurrentHealthCap;
        ControllingBT.GetBlackboard().UpdateValue<float>(HealthKey, CurrentHealth);

        ApplyColorEffect(HealedCEColor, HealedCEDuration);
    }


    //Main Update Loops
    public virtual void Sense()
    {
    }
    public void Evaluate()
    {
        EvaluationTimer -= Time.deltaTime;
        if (EvaluationTimer <= 0.0f)
        {
            EvaluationTimer = CurrentEvaluationTickRate;
            ControllingBT.Evaluate();
        }
    }
    public void Execute()
    {
        ExecutionTimer -= Time.deltaTime;
        if (ExecutionTimer <= 0.0f)
        {
            ExecutionTimer = CurrentExecutionTickRate;
            ControllingBT.Execute();
        }
    }

    private void Update()
    {
        UpdateColorEffectsTimers();
    }
}
