using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    //Overrides for base class
    private float OverrideSenseTickRate = 0.01f;
    private float OverrideEvaluationTickRate = 0.1f;
    private float OverrideExecutionTickRate = 0.02f;

    private float OverrideMovementSpeed = 40.0f;

    private float OverrideHealthCap = 200.0f;
    private float OverrideHealth = 200.0f;

    private float OverrideManaCap = 75.0f;
    private float OverrideMana = 75.0f;
    private float OverrideManaIncreaseRate = 25.0f;

    private float OverrideAggro = 10.0f;
    private float OverrideAggroIncreaseARate = 5.0f;

    private float OverrideAttackDamage = 5.0f;

    private float OverrideSuperCost = 100.0f;

    private float OverrideFHealthDangerThreshold = 60.0f;
    private float OverrideEHealthDangerThreshold = 40.0f;


    //Unique Attributes
    private int ECHIndex = -1;
    private float AttackBuffSpellCost = 50.0f;
    private float AttackBuffSpellRate = 30.0f;
    private int AttackBuffSpellTurns = 3;
    private StatusAilments.Buffs AttackBuffSpellType = StatusAilments.Buffs.ATTACK;

    private float AttackDebuffSpellCost = 30.0f;
    private float AttackDebuffSpellRate = 20.0f;
    private int AttackDebuffSpellTurns = 7;
    private StatusAilments.Debuffs AttackDebuffSpellType = StatusAilments.Debuffs.ATTACK;



    //Unique Keys
    private string AttackBuffSpellCostKey = "AttackBuffSpellCost";
    private string AttackBuffSpellRateKey = "AttackBuffSpellRate";
    private string AttackBuffSpellTurnsKey = "AttackBuffSpellTurns";
    private string AttackBuffSpellTypeKey = "AttackBuffSpellType";

    private string AttackDebuffSpellCostKey = "AttackDebuffSpellCost";
    private string AttackDebuffSpellRateKey = "AttackDebuffSpellRate";
    private string AttackDebuffSpellTurnsKey = "AttackDebuffSpellTurns";
    private string AttackDebuffSpellTypeKey = "AttackDebuffSpellType";


    //Unique system references
    private EnemiesController ECScript = null;


    //Initilization Related
    protected override void LoadResources()
    {
        Portrait = Resources.Load<Sprite>("Sprites/MonsterPortrait");
        if (!Portrait)
            Debug.LogError("Monster portrait was not loaded correctly");
    }
    protected override void SetupStartState()
    {
        CurrentSenseTickRate = OverrideSenseTickRate;
        CurrentEvaluationTickRate = OverrideEvaluationTickRate;
        CurrentExecutionTickRate = OverrideExecutionTickRate;

        CurrentMovementSpeed = OverrideMovementSpeed;

        CurrentHealthCap = OverrideHealthCap;
        CurrentHealth = OverrideHealth;

        CurrentManaCap = OverrideManaCap;
        CurrentMana = OverrideMana;
        CurrentManaIncreaseRate = OverrideManaIncreaseRate;

        CurrentAggro = OverrideAggro;
        CurrentAggroIncreaseARate = OverrideAggroIncreaseARate;

        CurrentAttackDamage = OverrideAttackDamage;

        CurrentSuperCost = OverrideSuperCost;

        CurrentFHealthDangerThreshold = OverrideFHealthDangerThreshold;
        CurrentEHealthDangerThreshold = OverrideEHealthDangerThreshold;

        //SetTimers
        SenseTimer = CurrentSenseTickRate;
        EvaluationTimer = CurrentEvaluationTickRate;
        ExecutionTimer = CurrentExecutionTickRate;
    }
    private void SetupECHIndex()
    {
        ECHIndex = ECScript.RegisterECH();
        if (ECHIndex == -1)
        {
            Debug.LogError("ECHIndex for Monster is -1");
            return;
        }
    }
    protected override void SetupHUDElements()
    {
        HUDScript.SetECHElementsState(ECHIndex, true);
        HUDScript.SetECHPortrait(ECHIndex, Portrait);
        HUDScript.UpdateECHHealthBar(ECHIndex, CurrentHealthCap, CurrentHealth);
        HUDScript.UpdateECHManaBar(ECHIndex, CurrentManaCap, CurrentMana);
        HUDScript.UpdateECHActionText(ECHIndex, ControllingBT.GetRunningTaskName());
        HUDScript.UpdateECHAggroCounter(ECHIndex, CurrentAggro);
    }
    protected override void SetupPosition()
    {
        transform.position = ECScript.GetECHSpawnPosition(ECHIndex);
    }
    protected override void SetupBlackboardEntries()
    {
        Blackboard BB = ControllingBT.GetBlackboard();


        //Enemy/Friend Related
        BB.AddValue<Party>(FriendsPartyKey, ECScript.GetCurrentECHParty());
        BB.AddValue<Party>(EnemiesPartyKey, ECScript.GetCurrentPCHParty());
        BB.AddValue<int>(EnemiesCountKey, ECScript.GetCurrentPCHParty().GetCharactersCount());

        //Attack Buff Related
        BB.AddValue<float>(AttackBuffSpellCostKey, AttackBuffSpellCost);
        BB.AddValue<float>(AttackBuffSpellRateKey, AttackBuffSpellRate);
        BB.AddValue<int>(AttackBuffSpellTurnsKey, AttackBuffSpellTurns);
        BB.AddValue<StatusAilments.Buffs>(AttackBuffSpellTypeKey, AttackBuffSpellType);

        //Attack Debuff Related
        BB.AddValue<float>(AttackDebuffSpellCostKey, AttackDebuffSpellCost);
        BB.AddValue<float>(AttackDebuffSpellRateKey, AttackDebuffSpellRate);
        BB.AddValue<int>(AttackDebuffSpellTurnsKey, AttackDebuffSpellTurns);
        BB.AddValue<StatusAilments.Debuffs>(AttackDebuffSpellTypeKey, AttackDebuffSpellType);


        BB.AddValue<float>(LowestCostSpellKey, AttackDebuffSpellCost); //Depends on individual character

        base.SetupBlackboardEntries();
    }
    public override void Init()
    {
        LoadResources();
        SetupStartState();
        SetupECHIndex(); // Only For Enemy Characters
        SetupHUDElements();
        SetupPosition();
        SetupBlackboardEntries();
    }


    //Enemy Characters Only
    public void SetECScriptReference(EnemiesController controller)
    {
        if (!controller)
        {
            Debug.LogError("Null players controller reference sent to Tank");
        }
        ECScript = controller;
    }
    public int GetECHIndex()
    {
        return ECHIndex;
    }


    //Unique Keys
    public string GetAttackBuffSpellCostKey()
    {
        return AttackBuffSpellCostKey;
    }
    public string GetAttackBuffSpellRateKey()
    {
        return AttackBuffSpellRateKey;
    }
    public string GetAttackBuffSpellTurnsKey()
    {
        return AttackBuffSpellTurnsKey;
    }
    public string GetAttackBuffSpellTypeKey()
    {
        return AttackBuffSpellTypeKey;
    }
    public string GetAttackDebuffSpellCostKey()
    {
        return AttackDebuffSpellCostKey;
    }
    public string GetAttackDebuffSpellRateKey()
    {
        return AttackDebuffSpellRateKey;
    }
    public string GetAttackDebuffSpellTurnsKey()
    {
        return AttackDebuffSpellTurnsKey;
    }
    public string GetAttackDebuffSpellTypeKey()
    {
        return AttackDebuffSpellTypeKey;
    }


    //Turn Related
    public override void EndTurn()
    {
        base.EndTurn();

        HUDScript.UpdateECHActionText(ECHIndex, ControllingBT.GetRunningTaskName());
        ECScript.EndECHTurn(ECHIndex);
    }


    //Requires Unique Character Index
    protected override void ApplyBuffEffects(StatusAilments.Buffs buff)
    {
        HUDScript.ApplyECHBuffStatusAilment(ECHIndex, buff);
    }
    protected override void ApplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
        HUDScript.ApplyECHDebuffStatusAilment(ECHIndex, debuff);
    }
    protected override void UapplyBuffEffects(StatusAilments.Buffs buff)
    {
        HUDScript.UapplyECHBuffStatusAilment(ECHIndex, buff);
    }
    protected override void UapplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
        HUDScript.UapplyECHDebuffStatusAilment(ECHIndex, debuff);
    }


    //Attributes Related
    public override void IncreaseMana(float amount)
    {
        base.IncreaseMana(amount);
        HUDScript.UpdateECHManaBar(ECHIndex, CurrentManaCap, CurrentMana); //Requires Unique Character Index
    }
    public override void DecreaseMana(float amount)
    {
        base.DecreaseMana(amount);
        HUDScript.UpdateECHManaBar(ECHIndex, CurrentManaCap, CurrentMana); //Requires Unique Character Index
    }
    public override void IncreaseAggro(float amount)
    {
        base.IncreaseAggro(amount);
        HUDScript.UpdateECHAggroCounter(ECHIndex, CurrentAggro); //Requires Unique Character Index
    }


    //Health Related
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth < 0.0f)
        {
            CurrentHealth = 0.0f;
            ECScript.UnregisterECH(ECHIndex);
        }
        HUDScript.UpdateECHHealthBar(ECHIndex, CurrentHealthCap, CurrentHealth); //Requires Unique Character Index
    }
    public override void Heal(float amount)
    {
        base.Heal(amount);
        HUDScript.UpdateECHHealthBar(ECHIndex, CurrentHealthCap, CurrentHealth); //Requires Unique Character Index
    }



    //Depends On Character Type
    public override void Sense()
    {
        SenseTimer -= Time.deltaTime;
        if (SenseTimer <= 0.0f)
        {
            SenseTimer = CurrentSenseTickRate;

            Blackboard BB = ControllingBT.GetBlackboard();
            Party FriendsGroup = ECScript.GetCurrentECHParty();
            Party EnemiesGroup = ECScript.GetCurrentPCHParty();

            BB.UpdateValue<Party>(FriendsPartyKey, FriendsGroup);
            BB.UpdateValue<Party>(EnemiesPartyKey, EnemiesGroup);
            BB.UpdateValue<int>(EnemiesCountKey, EnemiesGroup.GetCharactersCount());

            //This one needs to be updated some other way that doesnt involve a tick
            HUDScript.UpdateECHActionText(ECHIndex, ControllingBT.GetRunningTaskName());
            //Make BT update this

            //for attack damage Buff/Debuff changes
            float NewDamage = CurrentAttackDamage;
            if (IsBuffApplied(StatusAilments.Buffs.ATTACK))
            {
                NewDamage += GetAppliedBuffRate(StatusAilments.Buffs.ATTACK);
            }
            if (IsDebuffApplied(StatusAilments.Debuffs.ATTACK))
            {
                NewDamage -= GetAppliedDebuffRate(StatusAilments.Debuffs.ATTACK);
                if (NewDamage < 0.0f)
                    NewDamage = 0.0f;
            }
            BB.UpdateValue<float>(AttackDamageKey, NewDamage);
        }
    }
}
