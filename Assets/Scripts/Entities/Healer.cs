using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Character
{
    //Overrides For Base Class
    private float OverrideSenseTickRate = 0.01f;
    private float OverrideEvaluationTickRate = 0.1f;
    private float OverrideExecutionTickRate = 0.02f;

    private float OverrideMovementSpeed = 40.0f;

    private float OverrideHealthCap = 200.0f;
    private float OverrideHealth = 200.0f;

    private float OverrideManaCap = 75.0f;
    private float OverrideMana = 75.0f;
    private float OverrideManaIncreaseRate = 10.0f;

    private float OverrideAggro = 10.0f;
    private float OverrideAggroIncreaseARate = 5.0f;

    private float OverrideAttackDamage = 5.0f; 

    private float OverrideSuperCost = 100.0f;

    private float OverrideFHealthDangerThreshold = 60.0f;
    private float OverrideEHealthDangerThreshold = 40.0f;


    //Unique Attributes
    private int PCHIndex = -1;
    private Character HealSpellTarget = null;
    private float HealSpellCost =  20.0f;
    private float HealSpellRate = 20.0f;
    private float HealSpellDangerThreshold = 150.0f;

    private GameObject FireballProjectile = null;
    private float FireballSpellCost = 52.0f;
    private float FireballSpellSpeed = 20.0f;
    private float FireballSpellDamage = 20.0f;
    private float FireballAggroRate = 30.0f;

    private float DefenseDebuffSpellCost = 40.0f;
    private float DefenseDebuffSpellRate = 12.0f;
    private int DefenseDebuffSpellTurns = 4;
    private StatusAilments.Debuffs DefenseDebuffSpellType = StatusAilments.Debuffs.DEFENSE;

    private float SuperHealRate = 20.0f;


    //Unique Keys
    private string HealSpellTargetKey = "HealSpellTarget";
    private string HealSpellCostKey = "HealSpellCost";
    private string HealSpellRateKey = "HealSpellRate";
    private string HealSpellDangerThresholdKey = "HealSpellDangerThreshold";
    private string FireballProjectileKey = "FireballProjectile";
    private string FireballSpellCostKey = "FireballCost";
    private string FireballSpellSpeedKey = "FireballSpeed";
    private string FireballSpellDamageKey = "FireballDamage";
    private string FireballAggroRateKey = "FireballAggroRate";
    private string DefenseDebuffSpellCostKey = "DefenseDebuffSpellCost";
    private string DefenseDebuffSpellRateKey = "DefenseDebuffSpellRate";
    private string DefenseDebuffSpellTurnsKey = "DefenseDebuffSpellTurns";
    private string DefenseDebuffSpellTypeKey = "DefenseDebuffSpellType";
    private string SuperMeterKey = "SuperMeter";
    private string SuperHealRateKey = "SuperHealRate";


    //Unique System References
    private PlayersController PCScript = null;


    //Unique References
    private GameObject FireballProjectileAsset = null;


    //Initilization Related
    protected override void LoadResources()
    {
        Portrait = Resources.Load<Sprite>("Sprites/HealerPortrait");
        if (!Portrait)
            Debug.LogError("Healer portrait was not loaded correctly");
        FireballProjectileAsset = Resources.Load<GameObject>("Entities/FireballProjectile");
        if (!FireballProjectileAsset)
            Debug.LogError("Wrong path trying to load FireballProjectileAsset at LoadResources - Healer");
        else
        {
            FireballProjectile = Instantiate(FireballProjectileAsset);
            FireballProjectile.SetActive(false);
        }
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
    private void SetupPCHIndex()
    {
        PCHIndex = PCScript.RegisterPCH();
        if (PCHIndex == -1)
        {
            Debug.LogError("PCHIndex for Healer is -1");
            return;
        }
    }
    protected override void SetupHUDElements()
    {
        HUDScript.SetPCHElementsState(PCHIndex, true);
        HUDScript.SetPCHPortrait(PCHIndex, Portrait);
        HUDScript.UpdatePCHHealthBar(PCHIndex, CurrentHealthCap, CurrentHealth);
        HUDScript.UpdatePCHManaBar(PCHIndex, CurrentManaCap, CurrentMana);
        HUDScript.UpdatePCHActionText(PCHIndex, ControllingBT.GetRunningTaskName());
        HUDScript.UpdatePCHAggroCounter(PCHIndex, CurrentAggro);
    }
    protected override void SetupPosition()
    {
        transform.position = PCScript.GetPCHSpawnPosition(PCHIndex);
    }
    protected override void SetupBlackboardEntries()
    {
        Blackboard BB = ControllingBT.GetBlackboard();


        //Super Related - Only Player Characters
        BB.AddValue<float>(SuperMeterKey, PCScript.GetCurrentSuper());

        //Healer Only - Unique Super
        BB.AddValue<float>(SuperHealRateKey, SuperHealRate);

        //Enemy/Friend Related
        BB.AddValue<Party>(FriendsPartyKey, PCScript.GetCurrentPCHParty());
        BB.AddValue<Party>(EnemiesPartyKey, PCScript.GetCurrentECHParty());
        BB.AddValue<int>(EnemiesCountKey, PCScript.GetCurrentECHParty().GetCharactersCount());

        //Heal Spell Related
        BB.AddValue<Character>(HealSpellTargetKey, HealSpellTarget);
        BB.AddValue<float>(HealSpellCostKey, HealSpellCost);
        BB.AddValue<float>(HealSpellRateKey, HealSpellRate);
        BB.AddValue<float>(HealSpellDangerThresholdKey, HealSpellDangerThreshold);

        //Fireball Spell Related
        BB.AddValue<GameObject>(FireballProjectileKey, FireballProjectile);
        BB.AddValue<float>(FireballSpellCostKey, FireballSpellCost);
        BB.AddValue<float>(FireballSpellSpeedKey, FireballSpellSpeed);
        BB.AddValue<float>(FireballSpellDamageKey, FireballSpellDamage);
        BB.AddValue<float>(FireballAggroRateKey, FireballAggroRate);

        //Defense Debuff Spell Related
        BB.AddValue<float>(DefenseDebuffSpellCostKey, DefenseDebuffSpellCost);
        BB.AddValue<float>(DefenseDebuffSpellRateKey, DefenseDebuffSpellRate);
        BB.AddValue<int>(DefenseDebuffSpellTurnsKey, DefenseDebuffSpellTurns);
        BB.AddValue<StatusAilments.Debuffs>(DefenseDebuffSpellTypeKey, DefenseDebuffSpellType);


        BB.AddValue<float>(LowestCostSpellKey, DefenseDebuffSpellCost); //Depends on individual character

        base.SetupBlackboardEntries();
    }
    public override void Init()
    {
        LoadResources();
        SetupStartState();
        SetupPCHIndex(); // Only Player Characters
        SetupHUDElements();
        SetupPosition();
        SetupBlackboardEntries();
    }


    //Player Characters Only
    public void SetPCScriptReference(PlayersController controller)
    {
        if (!controller)
        {
            Debug.LogError("Null players controller reference sent to healer");
        }
        PCScript = controller;
    }
    public int GetPCHIndex()
    {
        return PCHIndex;
    }


    //Unique Keys Gets
    public string GetHealSpellTargetKey()
    {
        return HealSpellTargetKey;
    }
    public string GetHealSpellCostKey()
    {
        return HealSpellCostKey;
    }
    public string GetHealSpellRateKey()
    {
        return HealSpellRateKey;
    }
    public string GetHealSpellDangerThresholdKey()
    {
        return HealSpellDangerThresholdKey;
    }
    public string GetFireballProjectileKey()
    {
        return FireballProjectileKey;
    }
    public string GetFireballSpellCostKey()
    {
        return FireballSpellCostKey;
    }
    public string GetFireballSpellSpeedKey()
    {
        return FireballSpellSpeedKey;
    }
    public string GetFireballSpellDamageKey()
    {
        return FireballSpellDamageKey;
    }
    public string GetFireballAggroRateKey()
    {
        return FireballAggroRateKey;
    }
    public string GetDefenseDebuffSpellCostKey()
    {
        return DefenseDebuffSpellCostKey;
    }
    public string GetDefenseDebuffSpellRateKey()
    {
        return DefenseDebuffSpellRateKey;
    }
    public string GetDefenseDebuffSpellTurnsKey()
    {
        return DefenseDebuffSpellTurnsKey;
    }
    public string GetDefenseDebuffSpellTypeKey()
    {
        return DefenseDebuffSpellTypeKey;
    }
    public string GetSuperMeterKey()
    {
        return SuperMeterKey;
    }
    public string GetSuperHealRateKey()
    {
        return SuperHealRateKey;
    }


    //Turn Related
    public override void EndTurn()
    {
        base.EndTurn();

        HUDScript.UpdatePCHActionText(PCHIndex, ControllingBT.GetRunningTaskName());
        PCScript.EndPCHTurn(PCHIndex);
    }


    //Requires Unique Character Index
    protected override void ApplyBuffEffects(StatusAilments.Buffs buff)
    {
        HUDScript.ApplyPCHBuffStatusAilment(PCHIndex, buff);
    }
    protected override void ApplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
        HUDScript.ApplyPCHDebuffStatusAilment(PCHIndex, debuff);
    }
    protected override void UapplyBuffEffects(StatusAilments.Buffs buff)
    {
        HUDScript.UapplyPCHBuffStatusAilment(PCHIndex, buff);
    }
    protected override void UapplyDebuffEffects(StatusAilments.Debuffs debuff)
    {
        HUDScript.UapplyPCHDebuffStatusAilment(PCHIndex, debuff);
    }


    //Attributes Related
    public override void IncreaseMana(float amount)
    {
        base.IncreaseMana(amount);
        HUDScript.UpdatePCHManaBar(PCHIndex, CurrentManaCap, CurrentMana); //Requires Unique Character Index
    }
    public override void DecreaseMana(float amount)
    {
        base.DecreaseMana(amount);
        HUDScript.UpdatePCHManaBar(PCHIndex, CurrentManaCap, CurrentMana); //Requires Unique Character Index
    }
    public override void IncreaseAggro(float amount)
    {
        base.IncreaseAggro(amount);
        HUDScript.UpdatePCHAggroCounter(PCHIndex, CurrentAggro); //Requires Unique Character Index
    }


    //Super Related (Currently Only Player Characters)
    public override void UseSuper(float cost)
    {
        PCScript.UseSuper(cost);
        ApplyColorEffect(SuperCEColor, SuperCEDuration);
    }


    //Health Related
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth < 0.0f)
        {
            CurrentHealth = 0.0f;
            PCScript.UnregisterPCH(PCHIndex);
        }
        HUDScript.UpdatePCHHealthBar(PCHIndex, CurrentHealthCap, CurrentHealth); //Requires Unique Character Index
    }
    public override void Heal(float amount)
    {
        base.Heal(amount);
        HUDScript.UpdatePCHHealthBar(PCHIndex, CurrentHealthCap, CurrentHealth); //Requires Unique Character Index
    }


    //Depends On Character Type
    public override void Sense()
    {
        SenseTimer -= Time.deltaTime;
        if (SenseTimer <= 0.0f)
        {
            SenseTimer = CurrentSenseTickRate;

            Blackboard BB = ControllingBT.GetBlackboard();
            Party FriendsGroup = PCScript.GetCurrentPCHParty();
            Party EnemiesGroup = PCScript.GetCurrentECHParty();

            BB.UpdateValue<Party>(FriendsPartyKey, FriendsGroup);
            BB.UpdateValue<Party>(EnemiesPartyKey, EnemiesGroup);
            BB.UpdateValue<int>(EnemiesCountKey, EnemiesGroup.GetCharactersCount());

            BB.UpdateValue<Character>(HealSpellTargetKey, PCScript.GetAllyInDanger(CurrentFHealthDangerThreshold));

            BB.UpdateValue<float>(SuperMeterKey, PCScript.GetCurrentSuper());

            //This one needs to be updated some other way that doesnt involve a tick
            HUDScript.UpdatePCHActionText(PCHIndex, ControllingBT.GetRunningTaskName());
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
