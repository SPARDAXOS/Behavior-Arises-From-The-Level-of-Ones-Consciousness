using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    //Related Systems References
    private Map MapScript = null;
    private HUD HUDScript = null;
    private GameMode GameModeScript = null;
    private PlayersController PlayersControllerScript = null;


    //Controlled Party
    private Party ECHParty = new Party();


    //Controlled Agents
    private GameObject MonsterAsset = null;

    private GameObject Monster1Ref = null;
    private Monster Monster1Script = null;

    private GameObject Monster2Ref = null;
    private Monster Monster2Script = null;

    private GameObject Monster3Ref = null;
    private Monster Monster3Script = null;


    //Controlling Behavior Trees
    private BehaviorTree MonsterBT1 = new BehaviorTree();
    private BehaviorTree MonsterBT2 = new BehaviorTree();
    private BehaviorTree MonsterBT3 = new BehaviorTree();


    //Initilization Related
    private void LoadResources()
    {
        MonsterAsset = Resources.Load<GameObject>("Entities/Monster");
        if (!MonsterAsset)
            Debug.LogError("Wrong path trying to load MonsterAsset at LoadResources - EnemiesController");
        else
        {
            Monster1Ref = Instantiate(MonsterAsset);
            Monster1Script = Monster1Ref.GetComponent<Monster>();

            Monster2Ref = Instantiate(MonsterAsset);
            Monster2Script = Monster2Ref.GetComponent<Monster>();

            Monster3Ref = Instantiate(MonsterAsset);
            Monster3Script = Monster3Ref.GetComponent<Monster>();
        }
    }
    private void SetupCharacters()
    {
        Monster1Script.SetECScriptReference(this);
        Monster1Script.SetHUDScriptReference(HUDScript);
        Monster1Script.SetBehaviorTree(MonsterBT1);
        Monster1Script.Init();

        Monster2Script.SetECScriptReference(this);
        Monster2Script.SetHUDScriptReference(HUDScript);
        Monster2Script.SetBehaviorTree(MonsterBT2);
        Monster2Script.Init();

        Monster3Script.SetECScriptReference(this);
        Monster3Script.SetHUDScriptReference(HUDScript);
        Monster3Script.SetBehaviorTree(MonsterBT3);
        Monster3Script.Init();
    }
    private void SetupParty()
    {
        ECHParty.AddCharacter(Monster1Script, Monster1Script.GetECHIndex());
        ECHParty.AddCharacter(Monster2Script, Monster2Script.GetECHIndex());
        ECHParty.AddCharacter(Monster3Script, Monster3Script.GetECHIndex());
    }
    private void SetupBehaviorTrees()
    {
        ConstructMonsterBT();
        ConstructMonsterBT2();
        ConstructMonsterBT3();
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
            HUDScript = script;
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
    public void SetPCScriptReference(PlayersController script)
    {
        if (!script)
        {
            Debug.LogError("Null PlayersController reference sent to PlayersController at SetPCScriptReference");
            return;
        }
        else
            PlayersControllerScript = script;
    }


    //Gets
    public Vector3 GetECHSpawnPosition(int index)
    {
        return MapScript.GetECHSpawnPoint(index);
    }
    public Party GetCurrentECHParty()
    {
        return ECHParty;
    }
    public Party GetCurrentPCHParty()
    {
        return PlayersControllerScript.GetCurrentPCHParty();
    }


    //To Register/Unregister Enemy Characters
    public int RegisterECH()
    {
        return GameModeScript.RegisterECH();
    }
    public void UnregisterECH(int index)
    {
        HUDScript.SetECHElementsState(index, false);
        ECHParty.GetCharacterAtIndex(index).gameObject.SetActive(false);
        ECHParty.RemoveCharacter(index);
        GameModeScript.UnregisterECH(index);
    }

    //Turn Related
    public void SetupTurns()
    {
        Character[] Characters = ECHParty.GetCharactersLeft();
        for (int i = 0; i < Characters.Length; i++)
            if (Characters[i])
                Characters[i].ResetCanPerformAction();

    }
    public void EndECHTurn(int index)
    {
        GameModeScript.EndECHTurn(index);
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
        Character[] Characters = ECHParty.GetCharactersLeft();
        if (Characters[index]) // If index exists
            Characters[index].Sense();
        else
            Debug.LogError("No entity with index " + index + " exists at EnemiesController - UpdateSenses");
    }
    private void UpdateEvaluations(int index)
    {
        Character[] Characters = ECHParty.GetCharactersLeft();
        if (Characters[index]) // If index exists
            Characters[index].Evaluate();
        else
            Debug.LogError("No entity with index " + index + " exists at EnemiesController - UpdateEvaluations");
    }
    private void UpdateExecutions(int index)
    {
        Character[] Characters = ECHParty.GetCharactersLeft();
        if (Characters[index]) // If index exists
            Characters[index].Execute();
        else
            Debug.LogError("No entity with index " + index + " exists at EnemiesController - UpdateExecutions");
    }


    //Constructing Behaviour Trees
    private void ConstructMonsterBT()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (buff, debuff)

        //All Sequences
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence AttackBuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence AttackDebuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If Enemy Count Is More Than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(Monster1Script.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If Can Perform An Action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(Monster1Script.GetCanPerformActionKey());

        //If Is Far Away From Spawn (Outside Combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(Monster1Script.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(Monster1Script.GetSlotPositionKey());

        //If Is Far Away From Spawn (In Combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(Monster1Script.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(Monster1Script.GetSlotPositionKey());

        //If Has Mana To Perform The Lowest Cost Spell
        DCompareKeyWithKeyFloat DHasEnoughMana = new DCompareKeyWithKeyFloat();
        DHasEnoughMana.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughMana.SetAValueKey(Monster1Script.GetManaKey());
        DHasEnoughMana.SetBValueKey(Monster1Script.GetLowestCostSpellKey());

        //If Friends Dont Have Buff Already Applied
        DIsStatusAilmentApplied IsAttackBuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackBuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackBuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.BUFF);
        IsAttackBuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackBuffNotApplied.SetBuffTypeKey(Monster1Script.GetAttackBuffSpellTypeKey());
        IsAttackBuffNotApplied.SetTargetPartyKey(Monster1Script.GetFriendsPartyKey());

        //If Enemies Dont Have Debuff Already Applied
        DIsStatusAilmentApplied IsAttackDebuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackDebuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackDebuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.DEBUFF);
        IsAttackDebuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackDebuffNotApplied.SetDebuffTypeKey(Monster1Script.GetAttackDebuffSpellTypeKey());
        IsAttackDebuffNotApplied.SetTargetPartyKey(Monster1Script.GetEnemiesPartyKey());


        //Tasks Construction
        //Normal Attack Sequence Nodes
        TFindTarget TFindLowestHPEnemy = new TFindTarget();
        TFindLowestHPEnemy.SetSearchType(TFindTarget.SearchType.HIGHEST_AGGRO);
        TFindLowestHPEnemy.SetEnemiesPartyKey(Monster1Script.GetEnemiesPartyKey());
        TFindLowestHPEnemy.SetResultCharacterKey(Monster1Script.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(Monster1Script.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(Monster1Script.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(Monster1Script.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(Monster1Script.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(Monster1Script.GetAggroIncreaseRateAKey());


        //Attack Buff Spell Sequence Nodes
        TApplyStatusAilments TAttackBuffSpell = new TApplyStatusAilments();
        TAttackBuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackBuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.BUFF);
        TAttackBuffSpell.SetSelfKey(Monster1Script.GetSelfKey());
        TAttackBuffSpell.SetTargetPartyKey(Monster1Script.GetFriendsPartyKey());
        TAttackBuffSpell.SetSpellCostKey(Monster1Script.GetAttackBuffSpellCostKey());
        TAttackBuffSpell.SetStatusAilmentRateKey(Monster1Script.GetAttackBuffSpellRateKey());
        TAttackBuffSpell.SetStatusAilmentTurnsKey(Monster1Script.GetAttackBuffSpellTurnsKey());
        TAttackBuffSpell.SetBuffTypeKey(Monster1Script.GetAttackBuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackBuff = new TEndTurn();
        TEndTurnAfterAttackBuff.SetSelfKey(Monster1Script.GetSelfKey());


        //Attack Debuff Spell Sequence Nodes
        TApplyStatusAilments TAttackDebuffSpell = new TApplyStatusAilments();
        TAttackDebuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackDebuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.DEBUFF);
        TAttackDebuffSpell.SetSelfKey(Monster1Script.GetSelfKey());
        TAttackDebuffSpell.SetTargetPartyKey(Monster1Script.GetEnemiesPartyKey());
        TAttackDebuffSpell.SetSpellCostKey(Monster1Script.GetAttackDebuffSpellCostKey());
        TAttackDebuffSpell.SetStatusAilmentRateKey(Monster1Script.GetAttackDebuffSpellRateKey());
        TAttackDebuffSpell.SetStatusAilmentTurnsKey(Monster1Script.GetAttackDebuffSpellTurnsKey());
        TAttackDebuffSpell.SetDebuffTypeKey(Monster1Script.GetAttackDebuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackDebuff = new TEndTurn();
        TEndTurnAfterAttackDebuff.SetSelfKey(Monster1Script.GetSelfKey());


        //Back To Spawn Nodes (Outside Combat)
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(Monster1Script.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(Monster1Script.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(Monster1Script.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(Monster1Script.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(Monster1Script.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(Monster1Script.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(Monster1Script.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();
        TEndTurnIC.SetSelfKey(Monster1Script.GetSelfKey());


        //Structure Setup
        MonsterBT1.ConnectToRoot(MainSelector);

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
        ActionTypeSelector.ConnectNode(DHasEnoughMana);
            DHasEnoughMana.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);

        //Magic selector
        MagicSelector.ConnectNode(IsAttackBuffNotApplied);
            IsAttackBuffNotApplied.ConnectNode(AttackBuffSpellSequence);
        MagicSelector.ConnectNode(IsAttackDebuffNotApplied);
            IsAttackDebuffNotApplied.ConnectNode(AttackDebuffSpellSequence);

        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go Back To Spawn If Outside Combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go Back To Spawn If In Combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Attack Buff Spell Sequence
        AttackBuffSpellSequence.ConnectNode(TAttackBuffSpell);
        AttackBuffSpellSequence.ConnectNode(TEndTurnAfterAttackBuff);

        //Attack Debuff Spell Sequence
        AttackDebuffSpellSequence.ConnectNode(TAttackDebuffSpell);
        AttackDebuffSpellSequence.ConnectNode(TEndTurnAfterAttackDebuff);

        //Normal Attack Sequence
        NormalAttackSequence.ConnectNode(TFindLowestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
    private void ConstructMonsterBT2()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (buff, debuff)

        //All Sequences
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence AttackBuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence AttackDebuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If Enemy Count Is More Than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(Monster2Script.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If Can Perform An Action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(Monster2Script.GetCanPerformActionKey());

        //If Is Far Away From Spawn (Outside Combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(Monster2Script.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(Monster2Script.GetSlotPositionKey());

        //If Is Far Away From Spawn (In Combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(Monster2Script.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(Monster2Script.GetSlotPositionKey());

        //If Has Mana To Perform The Lowest Cost Spell
        DCompareKeyWithKeyFloat DHasEnoughMana = new DCompareKeyWithKeyFloat();
        DHasEnoughMana.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughMana.SetAValueKey(Monster2Script.GetManaKey());
        DHasEnoughMana.SetBValueKey(Monster2Script.GetLowestCostSpellKey());

        //If Friends Dont Have Buff Already Applied
        DIsStatusAilmentApplied IsAttackBuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackBuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackBuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.BUFF);
        IsAttackBuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackBuffNotApplied.SetBuffTypeKey(Monster2Script.GetAttackBuffSpellTypeKey());
        IsAttackBuffNotApplied.SetTargetPartyKey(Monster2Script.GetFriendsPartyKey());

        //If Enemies Dont Have Debuff Already Applied
        DIsStatusAilmentApplied IsAttackDebuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackDebuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackDebuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.DEBUFF);
        IsAttackDebuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackDebuffNotApplied.SetDebuffTypeKey(Monster2Script.GetAttackDebuffSpellTypeKey());
        IsAttackDebuffNotApplied.SetTargetPartyKey(Monster2Script.GetEnemiesPartyKey());


        //Tasks Construction
        //Normal Attack Sequence Nodes
        TFindTarget TFindLowestHPEnemy = new TFindTarget();
        TFindLowestHPEnemy.SetSearchType(TFindTarget.SearchType.HIGHEST_AGGRO);
        TFindLowestHPEnemy.SetEnemiesPartyKey(Monster2Script.GetEnemiesPartyKey());
        TFindLowestHPEnemy.SetResultCharacterKey(Monster2Script.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(Monster2Script.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(Monster2Script.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(Monster2Script.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(Monster2Script.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(Monster2Script.GetAggroIncreaseRateAKey());


        //Attack Buff Spell Sequence Nodes
        TApplyStatusAilments TAttackBuffSpell = new TApplyStatusAilments();
        TAttackBuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackBuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.BUFF);
        TAttackBuffSpell.SetSelfKey(Monster2Script.GetSelfKey());
        TAttackBuffSpell.SetTargetPartyKey(Monster2Script.GetFriendsPartyKey());
        TAttackBuffSpell.SetSpellCostKey(Monster2Script.GetAttackBuffSpellCostKey());
        TAttackBuffSpell.SetStatusAilmentRateKey(Monster2Script.GetAttackBuffSpellRateKey());
        TAttackBuffSpell.SetStatusAilmentTurnsKey(Monster2Script.GetAttackBuffSpellTurnsKey());
        TAttackBuffSpell.SetBuffTypeKey(Monster2Script.GetAttackBuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackBuff = new TEndTurn();
        TEndTurnAfterAttackBuff.SetSelfKey(Monster2Script.GetSelfKey());


        //Attack Debuff Spell Sequence Nodes
        TApplyStatusAilments TAttackDebuffSpell = new TApplyStatusAilments();
        TAttackDebuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackDebuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.DEBUFF);
        TAttackDebuffSpell.SetSelfKey(Monster2Script.GetSelfKey());
        TAttackDebuffSpell.SetTargetPartyKey(Monster2Script.GetEnemiesPartyKey());
        TAttackDebuffSpell.SetSpellCostKey(Monster2Script.GetAttackDebuffSpellCostKey());
        TAttackDebuffSpell.SetStatusAilmentRateKey(Monster2Script.GetAttackDebuffSpellRateKey());
        TAttackDebuffSpell.SetStatusAilmentTurnsKey(Monster2Script.GetAttackDebuffSpellTurnsKey());
        TAttackDebuffSpell.SetDebuffTypeKey(Monster2Script.GetAttackDebuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackDebuff = new TEndTurn();
        TEndTurnAfterAttackDebuff.SetSelfKey(Monster2Script.GetSelfKey());


        //Back To Spawn Nodes (Outside Combat)
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(Monster2Script.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(Monster2Script.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(Monster2Script.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(Monster2Script.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(Monster2Script.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(Monster2Script.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(Monster2Script.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();
        TEndTurnIC.SetSelfKey(Monster2Script.GetSelfKey());


        //Structure Setup
        MonsterBT2.ConnectToRoot(MainSelector);

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
        ActionTypeSelector.ConnectNode(DHasEnoughMana);
        DHasEnoughMana.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);

        //Magic selector
        MagicSelector.ConnectNode(IsAttackBuffNotApplied);
        IsAttackBuffNotApplied.ConnectNode(AttackBuffSpellSequence);
        MagicSelector.ConnectNode(IsAttackDebuffNotApplied);
        IsAttackDebuffNotApplied.ConnectNode(AttackDebuffSpellSequence);

        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go Back To Spawn If Outside Combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go Back To Spawn If In Combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Attack Buff Spell Sequence
        AttackBuffSpellSequence.ConnectNode(TAttackBuffSpell);
        AttackBuffSpellSequence.ConnectNode(TEndTurnAfterAttackBuff);

        //Attack Debuff Spell Sequence
        AttackDebuffSpellSequence.ConnectNode(TAttackDebuffSpell);
        AttackDebuffSpellSequence.ConnectNode(TEndTurnAfterAttackDebuff);

        //Normal Attack Sequence
        NormalAttackSequence.ConnectNode(TFindLowestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
    private void ConstructMonsterBT3()
    {
        //All Selectors
        Selector MainSelector = new Selector(); //Selects either combat or go back if far away.
        Selector CombatSelector = new Selector(); //Selects whether to perform an action or go back.
        Selector ActionTypeSelector = new Selector(); //Selects the type of action to perform (Super, magic and melee)
        Selector MeleeSelector = new Selector(); //Selects which melee attack to perform (Normal attack only)
        Selector MagicSelector = new Selector(); //Selects which spell to use (buff, debuff)

        //All Sequences
        Sequence NormalAttackSequence = new Sequence(); //DecideTarget->MoveToIt->AttackIt
        Sequence AttackBuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence AttackDebuffSpellSequence = new Sequence(); //UseBuff->EndTurn
        Sequence GoBackIfInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn
        Sequence GoBackIfNotInCombatSequence = new Sequence(); //GetSpawnPoint->MoveToIt->EndTurn


        //Decorators Construction
        //If Enemy Count Is More Than 0
        DCompareKeyWithValueInt DAreThereEnemies = new DCompareKeyWithValueInt();
        DAreThereEnemies.SetCompareType(DCompareKeyWithValueInt.CompareType.A_GREATER_THAN_B);
        DAreThereEnemies.SetAValueKey(Monster3Script.GetEnemiesCountKey());
        DAreThereEnemies.SetBValue(0);

        //If Can Perform An Action
        DCondition DCanPerformAction = new DCondition();
        DCanPerformAction.SetConditionType(DCondition.ConditionType.BOOL);
        DCanPerformAction.SetSuccessCriteriaType(DCondition.SuccessCriteria.IF_TRUE);
        DCanPerformAction.SetConditionKey(Monster3Script.GetCanPerformActionKey());

        //If Is Far Away From Spawn (Outside Combat)
        DDistanceToPoint DIsAwayFromSpawnOC = new DDistanceToPoint();
        DIsAwayFromSpawnOC.SetSelfKey(Monster3Script.GetSelfKey());
        DIsAwayFromSpawnOC.SetTargetPointKey(Monster3Script.GetSlotPositionKey());

        //If Is Far Away From Spawn (In Combat)
        DDistanceToPoint DIsAwayFromSpawnIC = new DDistanceToPoint();
        DIsAwayFromSpawnIC.SetSelfKey(Monster3Script.GetSelfKey());
        DIsAwayFromSpawnIC.SetTargetPointKey(Monster3Script.GetSlotPositionKey());

        //If Has Mana To Perform The Lowest Cost Spell
        DCompareKeyWithKeyFloat DHasEnoughMana = new DCompareKeyWithKeyFloat();
        DHasEnoughMana.SetCompareType(DCompareKeyWithKeyFloat.CompareType.A_EQUAL_OR_HIGHER_THAN_B);
        DHasEnoughMana.SetAValueKey(Monster3Script.GetManaKey());
        DHasEnoughMana.SetBValueKey(Monster3Script.GetLowestCostSpellKey());

        //If Friends Dont Have Buff Already Applied
        DIsStatusAilmentApplied IsAttackBuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackBuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackBuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.BUFF);
        IsAttackBuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackBuffNotApplied.SetBuffTypeKey(Monster3Script.GetAttackBuffSpellTypeKey());
        IsAttackBuffNotApplied.SetTargetPartyKey(Monster3Script.GetFriendsPartyKey());

        //If Enemies Dont Have Debuff Already Applied
        DIsStatusAilmentApplied IsAttackDebuffNotApplied = new DIsStatusAilmentApplied();
        IsAttackDebuffNotApplied.SetCurrentTargetType(DIsStatusAilmentApplied.TargetType.PARTY);
        IsAttackDebuffNotApplied.SetCurrentStatusAilmentType(DIsStatusAilmentApplied.StatusAilmentType.DEBUFF);
        IsAttackDebuffNotApplied.SetCurrentSuccessCondition(DIsStatusAilmentApplied.SuccessCondition.IF_FALSE);
        IsAttackDebuffNotApplied.SetDebuffTypeKey(Monster3Script.GetAttackDebuffSpellTypeKey());
        IsAttackDebuffNotApplied.SetTargetPartyKey(Monster3Script.GetEnemiesPartyKey());


        //Tasks Construction
        //Normal Attack Sequence Nodes
        TFindTarget TFindLowestHPEnemy = new TFindTarget();
        TFindLowestHPEnemy.SetSearchType(TFindTarget.SearchType.HIGHEST_AGGRO);
        TFindLowestHPEnemy.SetEnemiesPartyKey(Monster3Script.GetEnemiesPartyKey());
        TFindLowestHPEnemy.SetResultCharacterKey(Monster3Script.GetTargetCharacterKey());

        TMoveTo TMoveToEnemy = new TMoveTo();
        TMoveToEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TMoveToEnemy.SetTargetType(TMoveTo.TargetType.CHARACTER);
        TMoveToEnemy.SetTargetCharacterKey(Monster3Script.GetTargetCharacterKey());
        TMoveToEnemy.SetMovementSpeedKey(Monster3Script.GetMovementSpeedKey());

        TAttackCharacter TAttackEnemy = new TAttackCharacter();
        TAttackEnemy.SetSelfKey(Monster1Script.GetSelfKey());
        TAttackEnemy.SetTargetCharacterKey(Monster3Script.GetTargetCharacterKey());
        TAttackEnemy.SetAttackDamageKey(Monster3Script.GetAttackDamageKey());
        TAttackEnemy.SetAggroIncreaseRateAKey(Monster3Script.GetAggroIncreaseRateAKey());


        //Attack Buff Spell Sequence Nodes
        TApplyStatusAilments TAttackBuffSpell = new TApplyStatusAilments();
        TAttackBuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackBuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.BUFF);
        TAttackBuffSpell.SetSelfKey(Monster3Script.GetSelfKey());
        TAttackBuffSpell.SetTargetPartyKey(Monster3Script.GetFriendsPartyKey());
        TAttackBuffSpell.SetSpellCostKey(Monster3Script.GetAttackBuffSpellCostKey());
        TAttackBuffSpell.SetStatusAilmentRateKey(Monster3Script.GetAttackBuffSpellRateKey());
        TAttackBuffSpell.SetStatusAilmentTurnsKey(Monster3Script.GetAttackBuffSpellTurnsKey());
        TAttackBuffSpell.SetBuffTypeKey(Monster3Script.GetAttackBuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackBuff = new TEndTurn();
        TEndTurnAfterAttackBuff.SetSelfKey(Monster3Script.GetSelfKey());


        //Attack Debuff Spell Sequence Nodes
        TApplyStatusAilments TAttackDebuffSpell = new TApplyStatusAilments();
        TAttackDebuffSpell.SetCurrentTargetType(TApplyStatusAilments.TargetType.PARTY);
        TAttackDebuffSpell.SetCurrentStatusAilmentType(TApplyStatusAilments.StatusAilmentType.DEBUFF);
        TAttackDebuffSpell.SetSelfKey(Monster3Script.GetSelfKey());
        TAttackDebuffSpell.SetTargetPartyKey(Monster3Script.GetEnemiesPartyKey());
        TAttackDebuffSpell.SetSpellCostKey(Monster3Script.GetAttackDebuffSpellCostKey());
        TAttackDebuffSpell.SetStatusAilmentRateKey(Monster3Script.GetAttackDebuffSpellRateKey());
        TAttackDebuffSpell.SetStatusAilmentTurnsKey(Monster3Script.GetAttackDebuffSpellTurnsKey());
        TAttackDebuffSpell.SetDebuffTypeKey(Monster3Script.GetAttackDebuffSpellTypeKey());

        TEndTurn TEndTurnAfterAttackDebuff = new TEndTurn();
        TEndTurnAfterAttackDebuff.SetSelfKey(Monster3Script.GetSelfKey());


        //Back To Spawn Nodes (Outside Combat)
        TMoveTo TMoveBackToSpawnOC = new TMoveTo();
        TMoveBackToSpawnOC.SetSelfKey(Monster3Script.GetSelfKey());
        TMoveBackToSpawnOC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnOC.SetTargetPositionKey(Monster3Script.GetSlotPositionKey());
        TMoveBackToSpawnOC.SetMovementSpeedKey(Monster3Script.GetMovementSpeedKey());

        TEndTurn TEndTurnOC = new TEndTurn();
        TEndTurnOC.SetSelfKey(Monster3Script.GetSelfKey());


        //Back To Spawn Nodes (In combat)
        TMoveTo TMoveBackToSpawnIC = new TMoveTo();
        TMoveBackToSpawnIC.SetSelfKey(Monster3Script.GetSelfKey());
        TMoveBackToSpawnIC.SetTargetType(TMoveTo.TargetType.POSITION);
        TMoveBackToSpawnIC.SetTargetPositionKey(Monster3Script.GetSlotPositionKey());
        TMoveBackToSpawnIC.SetMovementSpeedKey(Monster3Script.GetMovementSpeedKey());

        TEndTurn TEndTurnIC = new TEndTurn();
        TEndTurnIC.SetSelfKey(Monster3Script.GetSelfKey());


        //Structure Setup
        MonsterBT3.ConnectToRoot(MainSelector);

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
        ActionTypeSelector.ConnectNode(DHasEnoughMana);
        DHasEnoughMana.ConnectNode(MagicSelector);
        ActionTypeSelector.ConnectNode(MeleeSelector);

        //Magic selector
        MagicSelector.ConnectNode(IsAttackBuffNotApplied);
        IsAttackBuffNotApplied.ConnectNode(AttackBuffSpellSequence);
        MagicSelector.ConnectNode(IsAttackDebuffNotApplied);
        IsAttackDebuffNotApplied.ConnectNode(AttackDebuffSpellSequence);

        //Melee selector
        MeleeSelector.ConnectNode(NormalAttackSequence);


        //Sequences Setup
        //Go Back To Spawn If Outside Combat
        GoBackIfNotInCombatSequence.ConnectNode(TMoveBackToSpawnOC);
        GoBackIfNotInCombatSequence.ConnectNode(TEndTurnOC);

        //Go Back To Spawn If In Combat
        GoBackIfInCombatSequence.ConnectNode(TMoveBackToSpawnIC);
        GoBackIfInCombatSequence.ConnectNode(TEndTurnIC);

        //Attack Buff Spell Sequence
        AttackBuffSpellSequence.ConnectNode(TAttackBuffSpell);
        AttackBuffSpellSequence.ConnectNode(TEndTurnAfterAttackBuff);

        //Attack Debuff Spell Sequence
        AttackDebuffSpellSequence.ConnectNode(TAttackDebuffSpell);
        AttackDebuffSpellSequence.ConnectNode(TEndTurnAfterAttackDebuff);

        //Normal Attack Sequence
        NormalAttackSequence.ConnectNode(TFindLowestHPEnemy);
        NormalAttackSequence.ConnectNode(TMoveToEnemy);
        NormalAttackSequence.ConnectNode(TAttackEnemy);
    }
}
