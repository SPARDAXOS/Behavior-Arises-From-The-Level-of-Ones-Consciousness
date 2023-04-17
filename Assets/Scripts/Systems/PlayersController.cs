using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersController : MonoBehaviour
{
    //Super Meter Attributes
    private float SuperIncreaseRate = 10.0f;
    private float SuperCap = 100.0f;


    //Related Systems References
    private Map MapScript = null;
    private HUD HUDScript = null;
    private GameMode GameModeScript = null;
    private EnemiesController EnemiesControllerScript = null;


    //Controlled Party
    private Party PCHParty = new Party();


    //Controlled Agents
    private GameObject TankAsset = null;
    private GameObject TankRef = null;
    private Tank TankScript = null;

    private GameObject DamageDealerAsset = null;
    private GameObject DamageDealerRef = null;
    private DamageDealer DamageDealerScript = null;

    private GameObject HealerAsset = null;
    private GameObject HealerRef = null;
    private Healer HealerScript = null;


    //Controlling Behavior Trees
    private BehaviorTree TankBT = new BehaviorTree();
    private BehaviorTree DamageDealerBT = new BehaviorTree();
    private BehaviorTree HealerBT = new BehaviorTree();


    //SuperMeter
    public float CurrentSuper = 0.0f;


    //Initilization Related
    private void LoadResources()
    {
        TankAsset = Resources.Load<GameObject>("Entities/Tank");
        if (!TankAsset)
            Debug.LogError("Wrong path trying to load TankAsset at LoadResources - PlayersController");
        else
        {
            TankRef = Instantiate(TankAsset);    
            TankScript = TankRef.GetComponent<Tank>();
        }

        DamageDealerAsset = Resources.Load<GameObject>("Entities/DamageDealer");
        if (!DamageDealerAsset)
            Debug.LogError("Wrong path trying to load DamageDealerAsset at LoadResources - PlayersController");
        else
        {
            DamageDealerRef = Instantiate(DamageDealerAsset);
            DamageDealerScript = DamageDealerRef.GetComponent<DamageDealer>();
        }

        HealerAsset = Resources.Load<GameObject>("Entities/Healer");
        if (!HealerAsset)
            Debug.LogError("Wrong path trying to load HealerAsset at LoadResources - PlayersController");
        else
        {
            HealerRef = Instantiate(HealerAsset);
            HealerScript = HealerRef.GetComponent<Healer>();
        }
    }
    private void SetupCharacters()
    {
        TankScript.SetPCScriptReference(this);
        TankScript.SetHUDScriptReference(HUDScript);
        TankScript.SetBehaviorTree(TankBT);
        TankScript.Init(); 

        DamageDealerScript.SetPCScriptReference(this);
        DamageDealerScript.SetHUDScriptReference(HUDScript);
        DamageDealerScript.SetBehaviorTree(DamageDealerBT);
        DamageDealerScript.Init();

        HealerScript.SetPCScriptReference(this);
        HealerScript.SetHUDScriptReference(HUDScript);
        HealerScript.SetBehaviorTree(HealerBT);
        HealerScript.Init();
    }
    private void SetupParty()
    {
        PCHParty.AddCharacter(TankScript, TankScript.GetPCHIndex());
        PCHParty.AddCharacter(DamageDealerScript, DamageDealerScript.GetPCHIndex());
        PCHParty.AddCharacter(HealerScript, HealerScript.GetPCHIndex());
    }
    private void SetupBehaviorTrees()
    {
        ConstructTankBT();
        ConstructDamageDealerBT();
        ConstructHealerBT();
    }
    public void Init()
    {
        LoadResources();
        SetupCharacters();
        SetupParty();
        SetupBehaviorTrees();
    }


    //Systems References Sets
    public void SetMapScriptReference(Map script)
    {
        if (!script)
        {
            Debug.LogError("Null map reference sent to PlayersController at SetMapScriptReference");
            return;
        }
        else
            MapScript = script;
    }
    public void SetHUDScriptReference(HUD script)
    {
        if (!script)
        {
            Debug.LogError("Null HUD reference sent to PlayersController at SetHUDScriptReference");
            return;
        }
        else
        {
            HUDScript = script;
            HUDScript.UpdateSuperBar(CurrentSuper); //On start update.
        }
    }
    public void SetGMScriptReference(GameMode script)
    {
        if (!script)
        {
            Debug.LogError("Null Game Mode reference sent to PlayersController at SetGMScriptReference");
            return;
        }
        else
            GameModeScript = script;
    }
    public void SetECScriptReference(EnemiesController script)
    {
        if (!script)
        {
            Debug.LogError("Null EnemiesController reference sent to PlayersController at SetECScriptReference");
            return;
        }
        else
            EnemiesControllerScript = script;
    }


    //Gets
    public Vector3 GetPCHSpawnPosition(int index)
    {
        return MapScript.GetPCHSpawnPoint(index);
    }
    public Party GetCurrentPCHParty()
    {
        return PCHParty;
    }
    public Party GetCurrentECHParty()
    {
        return EnemiesControllerScript.GetCurrentECHParty();
    }
    public Character GetAllyInDanger(float threshold)
    {
        return PCHParty.GetCharacterInDanger(threshold);
    }


    //Super Meter Related
    private void IncreaseSuper()
    {
        if (CurrentSuper < SuperCap)
        {
            CurrentSuper += SuperIncreaseRate;
            if (CurrentSuper > SuperCap)
                CurrentSuper = SuperCap;
            HUDScript.UpdateSuperBar(CurrentSuper / SuperCap);
        }
    }
    public float GetCurrentSuper()
    {
        return CurrentSuper;
    }
    public void UseSuper(float cost)
    {
        CurrentSuper -= cost;
        if (CurrentSuper < 0.0f)
            Debug.LogError("Super was reduced to lower than 0 - cost " + cost);
        HUDScript.UpdateSuperBar(CurrentSuper / SuperCap);
    }


    //To Register/Unregister Player Characters
    public int RegisterPCH()
    {
        return GameModeScript.RegisterPCH();
    }
    public void UnregisterPCH(int index)
    {
        HUDScript.SetPCHElementsState(index, false);
        PCHParty.GetCharacterAtIndex(index).gameObject.SetActive(false);
        PCHParty.RemoveCharacter(index);
        GameModeScript.UnregisterPCH(index);
    }


    //Turn Related
    public void SetupTurns()
    {
        Character[] Characters = PCHParty.GetCharactersLeft();
        if (Characters == null)
            Debug.Log("No characters are left in party!");
        for (int i = 0; i < Characters.Length; i++)
            if (Characters[i])
                Characters[i].ResetCanPerformAction();
    }
    public void EndPCHTurn(int index)
    {
        IncreaseSuper();
        GameModeScript.EndPCHTurn(index);
    }


    //For Updating Agents
    public void UpdateTurns(int index)
    {
        UpdateSenses(index);
        UpdateEvaluations(index);
        UpdateExecutions(index);
    }
    private void UpdateSenses(int index)
    {
        Character[] Characters = PCHParty.GetCharacterSlots();
        if (Characters[index]) // If index exists
            Characters[index].Sense();
        else
            Debug.LogError("No entity with index " + index + " exists at PlayersController - UpdateSenses");
    }
    private void UpdateEvaluations(int index)
    {
        Character[] Characters = PCHParty.GetCharacterSlots();
        if (Characters[index]) // If index exists
            Characters[index].Evaluate();
        else
            Debug.LogError("No entity with index " + index + " exists at PlayersController - UpdateEvaluations");
    }
    private void UpdateExecutions(int index)
    {
        Character[] Characters = PCHParty.GetCharacterSlots();
        if (Characters[index]) // If index exists
            Characters[index].Execute();
        else
            Debug.LogError("No entity with index " + index + " exists at PlayersController - UpdateExecutions");
    }


    //Constructing Behaviour Trees
    private void ConstructTankBT()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (Taunt spell only)

        //All Sequences
        Sequence UseSuperSequence = new Sequence(); //UseSuper->EndTurn
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence TauntSpellSequence = new Sequence(); //UseTaunt->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If enemy count is more than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(TankScript.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If can perform an action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(TankScript.GetCanPerformActionKey());

        //If is far away from spawn (Outside combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(TankScript.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(TankScript.GetSlotPositionKey());

        //If is far away from spawn (In combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(TankScript.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(TankScript.GetSlotPositionKey());

        //If self does not have the highest aggro in party
        DCompareSelfStatWithParty DNotHighestAggro = new DCompareSelfStatWithParty();
        DNotHighestAggro.SetConditionType(DCompareSelfStatWithParty.SuccessConditionType.NOT_HIGHEST_IN_PARTY);
        DNotHighestAggro.SetStatType(DCompareSelfStatWithParty.CompareStatType.AGGRO);
        DNotHighestAggro.SetSelfKey(TankScript.GetSelfKey());
        DNotHighestAggro.SetTargetPartyKey(TankScript.GetFriendsPartyKey());
        DNotHighestAggro.SetAggroKey(TankScript.GetAggroKey());

        //If has mana to taunt
        DCompareKeyWithKeyFloat DHasEnoughManaToTaunt = new DCompareKeyWithKeyFloat();
        DHasEnoughManaToTaunt.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughManaToTaunt.SetAValueKey(TankScript.GetManaKey());
        DHasEnoughManaToTaunt.SetBValueKey(TankScript.GetTauntSpellCostKey());

        //If super bar is full enough to use
        DCompareKeyWithKeyFloat DCanUseSuper = new DCompareKeyWithKeyFloat();
        DCanUseSuper.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DCanUseSuper.SetAValueKey(TankScript.GetSuperMeterKey());
        DCanUseSuper.SetBValueKey(TankScript.GetSuperCostKey());

        //If friends party is above health danger threshold
        DComparePartyStatWithValue DIsFriendsPartyHPHighEnough = new DComparePartyStatWithValue();
        DIsFriendsPartyHPHighEnough.SetCompareType(DComparePartyStatWithValue.CompareType.HIGHER_THAN_VALUE);
        DIsFriendsPartyHPHighEnough.SetStatType(DComparePartyStatWithValue.StatType.HEALTH);
        DIsFriendsPartyHPHighEnough.SetTargetPartyKey(TankScript.GetFriendsPartyKey());
        DIsFriendsPartyHPHighEnough.SetValue(TankScript.GetFHealthDangerThreshold());

        //If enemy party is above health danger threshold
        DComparePartyStatWithValue DIsEnemyPartyHPHighEnough = new DComparePartyStatWithValue();
        DIsEnemyPartyHPHighEnough.SetCompareType(DComparePartyStatWithValue.CompareType.HIGHER_THAN_VALUE);
        DIsEnemyPartyHPHighEnough.SetStatType(DComparePartyStatWithValue.StatType.HEALTH);
        DIsEnemyPartyHPHighEnough.SetTargetPartyKey(TankScript.GetEnemiesPartyKey());
        DIsEnemyPartyHPHighEnough.SetValue(TankScript.GetEHealthDangerThreshold());

        //If friends dont have buff already applied
        DIsStatusAilmentApplied IsSuperBuffNotApplied = new DIsStatusAilmentApplied();
        IsSuperBuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsSuperBuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.BUFF);
        IsSuperBuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE); //Success if they dont have it
        IsSuperBuffNotApplied.SetBuffTypeKey(TankScript.GetSuperBuffTypeKey());
        IsSuperBuffNotApplied.SetTargetPartyKey(TankScript.GetFriendsPartyKey());


        //Tasks Construction
        //Use Super Sequnce Nodes
        TUseSuper UseSuper = new TUseSuper();
        UseSuper.SetCurrentSuperType(TUseSuper.SuperType.BUFF);
        UseSuper.SetSelfKey(TankScript.GetSelfKey());
        UseSuper.SetTargetPartyKey(TankScript.GetFriendsPartyKey());
        UseSuper.SetSuperCostKey(TankScript.GetSuperCostKey());
        UseSuper.SetSuperBuffTypeKey(TankScript.GetSuperBuffTypeKey());
        UseSuper.SetSuperBuffRateKey(TankScript.GetSuperBuffRateKey());
        UseSuper.SetSuperBuffTurnsKey(TankScript.GetSuperBuffTurnsKey());

        //End turn after super
        TEndTurn TEndTurnS = new TEndTurn();
        TEndTurnS.SetSelfKey(TankScript.GetSelfKey());


        //Normal attack Sequence Nodes
        TFindTarget TFindHighestHPEnemy = new TFindTarget();
        TFindHighestHPEnemy.SetSearchType(TFindTarget.SearchType.HIGHEST_HEALTH);
        TFindHighestHPEnemy.SetEnemiesPartyKey(TankScript.GetEnemiesPartyKey());
        TFindHighestHPEnemy.SetResultCharacterKey(TankScript.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(TankScript.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(TankScript.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(TankScript.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(TankScript.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(TankScript.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(TankScript.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(TankScript.GetAggroIncreaseRateAKey());


        //Taunt Spell Sequence Nodes
        TTaunt TTauntSpell = new TTaunt();
        TTauntSpell.SetSelfKey(TankScript.GetSelfKey());
        TTauntSpell.SetSpellCostKey(TankScript.GetTauntSpellCostKey());
        TTauntSpell.SetTauntRateKey(TankScript.GetTauntSpellRateKey());

        TEndTurn TEndTurnAfterTaunt = new TEndTurn(); //Reusage?
        TEndTurnAfterTaunt.SetSelfKey(TankScript.GetSelfKey());


        //Back To Spawn Nodes (Outside combat) Reuse?
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(TankScript.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(TankScript.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(TankScript.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(TankScript.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(TankScript.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(TankScript.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(TankScript.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();// Reuse???
        TEndTurnIC.SetSelfKey(TankScript.GetSelfKey());


        //Structure Setup (Keep connecting till you hit another selector or sequence)
        TankBT.ConnectToRoot(MainSelector);

        //Main selector
        MainSelector.ConnectNode(DAreThereEnemies);
            DAreThereEnemies.ConnectNode(CombatSelector);
        MainSelector.ConnectNode(DIsAwayFromSpawnOC);
            DIsAwayFromSpawnOC.ConnectNode(GoBackIfNotInCombatSequence);

        //Combat selector
        CombatSelector.ConnectNode(DCanPerformAction);
            DCanPerformAction.ConnectNode(ActionTypeSelector);
        CombatSelector.ConnectNode(DIsAwayFromSpawnIC);
            DIsAwayFromSpawnIC.ConnectNode(GoBackIfInCombatSequence);

        //Action type selector
        ActionTypeSelector.ConnectNode(DCanUseSuper);
            DCanUseSuper.ConnectNode(DIsFriendsPartyHPHighEnough);
                DIsFriendsPartyHPHighEnough.ConnectNode(DIsEnemyPartyHPHighEnough);
                    DIsEnemyPartyHPHighEnough.ConnectNode(IsSuperBuffNotApplied);
                        IsSuperBuffNotApplied.ConnectNode(UseSuperSequence);
        ActionTypeSelector.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);
        
        //Magic selector
        MagicSelector.ConnectNode(DNotHighestAggro);
            DNotHighestAggro.ConnectNode(DHasEnoughManaToTaunt);
                DHasEnoughManaToTaunt.ConnectNode(TauntSpellSequence);

        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go back to spawn if outside combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go back to spawn if in combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Super sequence
        UseSuperSequence.ConnectNode(UseSuper);
        UseSuperSequence.ConnectNode(TEndTurnS);

        //Taunt spell sequence
        TauntSpellSequence.ConnectNode(TTauntSpell);
        TauntSpellSequence.ConnectNode(TEndTurnAfterTaunt);

        //Normal attack sequence
        NormalAttackSequence.ConnectNode(TFindHighestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
    private void ConstructDamageDealerBT()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (Attack buff spell only)

        //All Sequences
        Sequence UseSuperSequence = new Sequence(); //UseSuper->EndTurn
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence AttackBuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If Enemy Count Is More Than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(DamageDealerScript.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If Can Perform An Action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(DamageDealerScript.GetCanPerformActionKey());

        //If Is Far Away From Spawn (Outside Combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(DamageDealerScript.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(DamageDealerScript.GetSlotPositionKey());

        //If Is Far Away From Spawn (In Combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(DamageDealerScript.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(DamageDealerScript.GetSlotPositionKey());

        //If Has Mana To Cast Buff
        DCompareKeyWithKeyFloat DHasEnoughManaForBuff = new DCompareKeyWithKeyFloat();
        DHasEnoughManaForBuff.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughManaForBuff.SetAValueKey(DamageDealerScript.GetManaKey());
        DHasEnoughManaForBuff.SetBValueKey(DamageDealerScript.GetAttackBuffSpellCostKey());

        //If Super Bar Is Full Enough To Use
        DCompareKeyWithKeyFloat DCanUseSuper = new DCompareKeyWithKeyFloat();
        DCanUseSuper.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DCanUseSuper.SetAValueKey(DamageDealerScript.GetSuperMeterKey());
        DCanUseSuper.SetBValueKey(DamageDealerScript.GetSuperCostKey());

        //If Friends Party Is Above Health Danger Threshold
        DComparePartyStatWithValue DIsFriendsPartyHPHighEnough = new DComparePartyStatWithValue();
        DIsFriendsPartyHPHighEnough.SetCompareType(DComparePartyStatWithValue.CompareType.HIGHER_THAN_VALUE);
        DIsFriendsPartyHPHighEnough.SetStatType(DComparePartyStatWithValue.StatType.HEALTH);
        DIsFriendsPartyHPHighEnough.SetTargetPartyKey(DamageDealerScript.GetFriendsPartyKey());
        DIsFriendsPartyHPHighEnough.SetValue(DamageDealerScript.GetFHealthDangerThreshold());

        //If Enemy Party Is below Health Danger Threshold
        DComparePartyStatWithValue DIsEnemyPartyHPLowEnough = new DComparePartyStatWithValue();
        DIsEnemyPartyHPLowEnough.SetCompareType(DComparePartyStatWithValue.CompareType.LOWER_THAN_VALUE);
        DIsEnemyPartyHPLowEnough.SetStatType(DComparePartyStatWithValue.StatType.HEALTH);
        DIsEnemyPartyHPLowEnough.SetTargetPartyKey(DamageDealerScript.GetEnemiesPartyKey());
        DIsEnemyPartyHPLowEnough.SetValue(DamageDealerScript.GetEHealthDangerThreshold());

        //If Friends Dont Have Buff Already Applied
        DIsStatusAilmentApplied IsAttackBuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackBuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackBuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.BUFF);
        IsAttackBuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE); //Success if they dont have it
        IsAttackBuffNotApplied.SetBuffTypeKey(DamageDealerScript.GetAttackBuffSpellTypeKey());
        IsAttackBuffNotApplied.SetTargetPartyKey(DamageDealerScript.GetFriendsPartyKey());


        //Tasks Construction
        //Use Super Sequnce Nodes
        TUseSuper UseSuper = new TUseSuper();
        UseSuper.SetCurrentSuperType(TUseSuper.SuperType.DAMAGE);
        UseSuper.SetSelfKey(DamageDealerScript.GetSelfKey());
        UseSuper.SetTargetPartyKey(DamageDealerScript.GetFriendsPartyKey());
        UseSuper.SetSuperCostKey(DamageDealerScript.GetSuperCostKey());
        UseSuper.SetSuperDamageRateKey(DamageDealerScript.GetSuperDamageRateKey());

        //End Turn After Super
        TEndTurn TEndTurnS = new TEndTurn();
        TEndTurnS.SetSelfKey(DamageDealerScript.GetSelfKey());


        //Normal Attack Sequence Nodes
        TFindTarget TFindLowestHPEnemy = new TFindTarget();
        TFindLowestHPEnemy.SetSearchType(TFindTarget.SearchType.LOWEST_HEALTH);
        TFindLowestHPEnemy.SetEnemiesPartyKey(DamageDealerScript.GetEnemiesPartyKey());
        TFindLowestHPEnemy.SetResultCharacterKey(DamageDealerScript.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(DamageDealerScript.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(DamageDealerScript.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(DamageDealerScript.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(DamageDealerScript.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(DamageDealerScript.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(DamageDealerScript.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(DamageDealerScript.GetAggroIncreaseRateAKey());


        //Attack Buff Spell Sequence Nodes
        TApplyStatusAilments TAttackBuffSpell = new TApplyStatusAilments();
        TAttackBuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackBuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.BUFF);
        TAttackBuffSpell.SetSelfKey(DamageDealerScript.GetSelfKey());
        TAttackBuffSpell.SetTargetPartyKey(DamageDealerScript.GetFriendsPartyKey());
        TAttackBuffSpell.SetSpellCostKey(DamageDealerScript.GetAttackBuffSpellCostKey());
        TAttackBuffSpell.SetStatusAilmentRateKey(DamageDealerScript.GetAttackBuffSpellRateKey());
        TAttackBuffSpell.SetStatusAilmentTurnsKey(DamageDealerScript.GetAttackBuffSpellTurnsKey());
        TAttackBuffSpell.SetBuffTypeKey(DamageDealerScript.GetAttackBuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackBuff = new TEndTurn();
        TEndTurnAfterAttackBuff.SetSelfKey(DamageDealerScript.GetSelfKey());


        //Back To Spawn Nodes (Outside Combat)
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(DamageDealerScript.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(DamageDealerScript.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(DamageDealerScript.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(DamageDealerScript.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(DamageDealerScript.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(DamageDealerScript.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(DamageDealerScript.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();
        TEndTurnIC.SetSelfKey(DamageDealerScript.GetSelfKey());


        //Structure Setup
        DamageDealerBT.ConnectToRoot(MainSelector);

        //Main selector
        MainSelector.ConnectNode(DAreThereEnemies);
        DAreThereEnemies.ConnectNode(CombatSelector);
        MainSelector.ConnectNode(DIsAwayFromSpawnOC);
        DIsAwayFromSpawnOC.ConnectNode(GoBackIfNotInCombatSequence);

        //Combat selector
        CombatSelector.ConnectNode(DCanPerformAction);
        DCanPerformAction.ConnectNode(ActionTypeSelector);
        CombatSelector.ConnectNode(DIsAwayFromSpawnIC);
        DIsAwayFromSpawnIC.ConnectNode(GoBackIfInCombatSequence);

        //Action type selector
        ActionTypeSelector.ConnectNode(DCanUseSuper);
        DCanUseSuper.ConnectNode(DIsFriendsPartyHPHighEnough);
        DIsFriendsPartyHPHighEnough.ConnectNode(DIsEnemyPartyHPLowEnough);
        DIsEnemyPartyHPLowEnough.ConnectNode(UseSuperSequence);
        ActionTypeSelector.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);

        //Magic selector
        MagicSelector.ConnectNode(IsAttackBuffNotApplied);
            IsAttackBuffNotApplied.ConnectNode(DHasEnoughManaForBuff);
                DHasEnoughManaForBuff.ConnectNode(AttackBuffSpellSequence);

        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go Back To Spawn If Outside Combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go Back To Spawn If In Combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Super Sequence
        UseSuperSequence.ConnectNode(UseSuper);
        UseSuperSequence.ConnectNode(TEndTurnS);

        //Attack Buff Spell Sequence
        AttackBuffSpellSequence.ConnectNode(TAttackBuffSpell);
        AttackBuffSpellSequence.ConnectNode(TEndTurnAfterAttackBuff);

        //Normal Attack Sequence
        NormalAttackSequence.ConnectNode(TFindLowestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
    private void ConstructHealerBT()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (Fireball, heal, defense debuff)

        //All Sequences
        Sequence UseSuperSequence = new Sequence(); //UseSuper->EndTurn
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence FireballSpellSequence = new Sequence(); //DecideTarget->CastFireball->EndTurn
        Sequence HealSpellSequence = new Sequence(); //DecideTarget->CastHeal->EndTurn
        Sequence DefenseDebuffSpellSequence = new Sequence(); //DecideTarget->CastDebuff->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If enemy count is more than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(HealerScript.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If can perform an action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(HealerScript.GetCanPerformActionKey());

        //If An Ally Needs Healing
        DCondition DAllyNeedsHealing = new DCondition();
        DAllyNeedsHealing.SetConditionType(DCondition.ConditionType.CHARACTER);
        DAllyNeedsHealing.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DAllyNeedsHealing.SetCharacterKey(HealerScript.GetHealSpellTargetKey());

        //If is far away from spawn (Outside combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(HealerScript.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(HealerScript.GetSlotPositionKey());

        //If is far away from spawn (In combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(HealerScript.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(HealerScript.GetSlotPositionKey());

        //If has mana to cast heal
        DCompareKeyWithKeyFloat DHasEnoughManaForHeal = new DCompareKeyWithKeyFloat();
        DHasEnoughManaForHeal.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughManaForHeal.SetAValueKey(HealerScript.GetManaKey());
        DHasEnoughManaForHeal.SetBValueKey(HealerScript.GetHealSpellCostKey());

        //If has mana to cast debuff
        DCompareKeyWithKeyFloat DHasEnoughManaForDebuff = new DCompareKeyWithKeyFloat();
        DHasEnoughManaForDebuff.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughManaForDebuff.SetAValueKey(HealerScript.GetManaKey());
        DHasEnoughManaForDebuff.SetBValueKey(HealerScript.GetDefenseDebuffSpellCostKey());

        //If has mana to cast FireBall
        DCompareKeyWithKeyFloat DHasEnoughManaForFireball = new DCompareKeyWithKeyFloat();
        DHasEnoughManaForFireball.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughManaForFireball.SetAValueKey(HealerScript.GetManaKey());
        DHasEnoughManaForFireball.SetBValueKey(HealerScript.GetFireballSpellCostKey());


        //If Enemies Dont Have Debuff Already Applied
        DIsStatusAilmentApplied DIsDefenseDebuffNotApplied = new DIsStatusAilmentApplied();
        DIsDefenseDebuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        DIsDefenseDebuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.DEBUFF);
        DIsDefenseDebuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        DIsDefenseDebuffNotApplied.SetDebuffTypeKey(HealerScript.GetDefenseDebuffSpellTypeKey());
        DIsDefenseDebuffNotApplied.SetTargetPartyKey(HealerScript.GetEnemiesPartyKey());

        //If super bar is full enough to use
        DCompareKeyWithKeyFloat DCanUseSuper = new DCompareKeyWithKeyFloat();
        DCanUseSuper.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DCanUseSuper.SetAValueKey(HealerScript.GetSuperMeterKey());
        DCanUseSuper.SetBValueKey(HealerScript.GetSuperCostKey());

        //If friends party is below health danger threshold
        DComparePartyStatWithValue DIsFriendsPartyHPLowEnough = new DComparePartyStatWithValue();
        DIsFriendsPartyHPLowEnough.SetCompareType(DComparePartyStatWithValue.CompareType.LOWER_OR_EQUALS_VALUE);
        DIsFriendsPartyHPLowEnough.SetStatType(DComparePartyStatWithValue.StatType.HEALTH);
        DIsFriendsPartyHPLowEnough.SetTargetPartyKey(HealerScript.GetFriendsPartyKey());
        DIsFriendsPartyHPLowEnough.SetValue(HealerScript.GetFHealthDangerThreshold());


        //Tasks Construction
        //Defense Debuff Spell Sequence Nodes
        TApplyStatusAilments TCastDefenseDebuffSpell = new TApplyStatusAilments();
        TCastDefenseDebuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TCastDefenseDebuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.DEBUFF);
        TCastDefenseDebuffSpell.SetSelfKey(HealerScript.GetSelfKey());
        TCastDefenseDebuffSpell.SetTargetPartyKey(HealerScript.GetEnemiesPartyKey());
        TCastDefenseDebuffSpell.SetSpellCostKey(HealerScript.GetDefenseDebuffSpellCostKey());
        TCastDefenseDebuffSpell.SetStatusAilmentRateKey(HealerScript.GetDefenseDebuffSpellRateKey());
        TCastDefenseDebuffSpell.SetStatusAilmentTurnsKey(HealerScript.GetDefenseDebuffSpellTurnsKey());
        TCastDefenseDebuffSpell.SetDebuffTypeKey(HealerScript.GetDefenseDebuffSpellTypeKey());

        //End Turn After Defense Debuff Spell
        TEndTurn TEndTurnAfterDDS = new TEndTurn();
        TEndTurnAfterDDS.SetSelfKey(HealerScript.GetSelfKey());


        //Heal Spell Sequence Nodes
        TCastHeal THealAlly = new TCastHeal();
        THealAlly.SetSelfKey(HealerScript.GetSelfKey());
        THealAlly.SetTargetCharacterKey(HealerScript.GetHealSpellTargetKey());
        THealAlly.SetHealSpellCostKey(HealerScript.GetHealSpellCostKey());
        THealAlly.SetHealSpellRateKey(HealerScript.GetHealSpellRateKey());

        //End Turn After Heal Spell
        TEndTurn TEndTurnAfterHS = new TEndTurn();
        TEndTurnAfterHS.SetSelfKey(HealerScript.GetSelfKey());


        //Use Super Sequnce Nodes
        TUseSuper UseSuper = new TUseSuper();
        UseSuper.SetCurrentSuperType(TUseSuper.SuperType.HEAL);
        UseSuper.SetSelfKey(HealerScript.GetSelfKey());
        UseSuper.SetTargetPartyKey(HealerScript.GetFriendsPartyKey());
        UseSuper.SetSuperCostKey(HealerScript.GetSuperCostKey());
        UseSuper.SetSuperHealRateKey(HealerScript.GetSuperHealRateKey());

        //End turn after super
        TEndTurn TEndTurnS = new TEndTurn();
        TEndTurnS.SetSelfKey(HealerScript.GetSelfKey());


        //Normal Attack Sequence Nodes
        TFindTarget TFindLowestHPEnemy = new TFindTarget();
        TFindLowestHPEnemy.SetSearchType(TFindTarget.SearchType.LOWEST_HEALTH);
        TFindLowestHPEnemy.SetEnemiesPartyKey(HealerScript.GetEnemiesPartyKey());
        TFindLowestHPEnemy.SetResultCharacterKey(HealerScript.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(HealerScript.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(HealerScript.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(HealerScript.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(HealerScript.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(HealerScript.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(HealerScript.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(HealerScript.GetAggroIncreaseRateAKey());


        //Cast Fireball Spell Sequence Nodes
        TFindTarget TFindHighestHPEnemy = new TFindTarget();
        TFindHighestHPEnemy.SetSearchType(TFindTarget.SearchType.HIGHEST_HEALTH);
        TFindHighestHPEnemy.SetEnemiesPartyKey(HealerScript.GetEnemiesPartyKey());
        TFindHighestHPEnemy.SetResultCharacterKey(HealerScript.GetTargetCharacterKey());

        TShootProjectileAtCharacter TCastFireball = new TShootProjectileAtCharacter();
        TCastFireball.SetSelfKey(HealerScript.GetSelfKey());
        TCastFireball.SetTargetCharacterKey(HealerScript.GetTargetCharacterKey());
        TCastFireball.SetProjectileKey(HealerScript.GetFireballProjectileKey());
        TCastFireball.SetProjectileCostKey(HealerScript.GetFireballSpellCostKey());
        TCastFireball.SetProjectileSpeedKey(HealerScript.GetFireballSpellSpeedKey());
        TCastFireball.SetProjectileDamageKey(HealerScript.GetFireballSpellDamageKey());
        TCastFireball.SetAggroIncreaseRateKey(HealerScript.GetFireballAggroRateKey());
        TCastFireball.SetSpawnPositionKey(HealerScript.GetSlotPositionKey());


        TEndTurn TEndTurnAfterFS = new TEndTurn();
        TEndTurnAfterFS.SetSelfKey(HealerScript.GetSelfKey());


        //Back To Spawn Nodes (Outside combat)
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(HealerScript.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(HealerScript.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(HealerScript.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(HealerScript.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(HealerScript.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(HealerScript.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(HealerScript.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();
        TEndTurnIC.SetSelfKey(HealerScript.GetSelfKey());


        //Structure Setup
        HealerBT.ConnectToRoot(MainSelector);

        //Main selector
        MainSelector.ConnectNode(DAreThereEnemies);
        DAreThereEnemies.ConnectNode(CombatSelector);
        MainSelector.ConnectNode(DIsAwayFromSpawnOC);
        DIsAwayFromSpawnOC.ConnectNode(GoBackIfNotInCombatSequence);

        //Combat selector
        CombatSelector.ConnectNode(DCanPerformAction);
        DCanPerformAction.ConnectNode(ActionTypeSelector);
        CombatSelector.ConnectNode(DIsAwayFromSpawnIC);
        DIsAwayFromSpawnIC.ConnectNode(GoBackIfInCombatSequence);

        //Action type selector
        ActionTypeSelector.ConnectNode(DCanUseSuper);
            DCanUseSuper.ConnectNode(DIsFriendsPartyHPLowEnough);
                DIsFriendsPartyHPLowEnough.ConnectNode(UseSuperSequence);
        ActionTypeSelector.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);

        //Magic selector
        MagicSelector.ConnectNode(DAllyNeedsHealing);
            DAllyNeedsHealing.ConnectNode(DHasEnoughManaForHeal);
                DHasEnoughManaForHeal.ConnectNode(HealSpellSequence);
        MagicSelector.ConnectNode(DIsDefenseDebuffNotApplied);
            DIsDefenseDebuffNotApplied.ConnectNode(DHasEnoughManaForDebuff);
                DHasEnoughManaForDebuff.ConnectNode(DefenseDebuffSpellSequence);
        MagicSelector.ConnectNode(DHasEnoughManaForFireball);
            DHasEnoughManaForFireball.ConnectNode(FireballSpellSequence);
 
        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go back to spawn if outside combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go back to spawn if in combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Super Sequence
        UseSuperSequence.ConnectNode(UseSuper);
        UseSuperSequence.ConnectNode(TEndTurnS);

        //Defense Debuff Spell Sequence
        DefenseDebuffSpellSequence.ConnectNode(TCastDefenseDebuffSpell);
        DefenseDebuffSpellSequence.ConnectNode(TEndTurnAfterDDS);

        //Heal Spell Sequence
        HealSpellSequence.ConnectNode(THealAlly);
        HealSpellSequence.ConnectNode(TEndTurnAfterHS);

        //Fireball Spell Sequence
        FireballSpellSequence.ConnectNode(TFindHighestHPEnemy);
        FireballSpellSequence.ConnectNode(TCastFireball);
        FireballSpellSequence.ConnectNode(TEndTurnAfterFS);

        //Normal Attack Sequence
        NormalAttackSequence.ConnectNode(TFindLowestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
}
