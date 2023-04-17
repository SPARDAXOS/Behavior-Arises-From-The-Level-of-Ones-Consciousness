using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private enum TurnType
    {
        PLAYER_CHARACTERS,
        ENEMY_CHARACTERS
    }

    private TurnType CurrentTurn = TurnType.PLAYER_CHARACTERS;
    public int CurrentCharacterTurn = -1;

    public bool[] PCHSlots = { false, false, false };
    public bool[] ECHSlots = { false, false, false };

    public bool[] PCHTurns = { false, false, false };
    public bool[] ECHTurns = { false, false, false };


    private EnemiesController EnemiesControllerScript = null;
    private PlayersController PlayersControllerScript = null;




    public void SetECScriptReference(EnemiesController script)
    {
        if (!script)
        {
            Debug.LogError("Null EnemiesController reference sent to GameMode at SetECScriptReference");
            return;
        }
        else
            EnemiesControllerScript = script;
    }
    public void SetPCScriptReference(PlayersController script)
    {
        if (!script)
        {
            Debug.LogError("Null PlayersController reference sent to GameMode at SetPCScriptReference");
            return;
        }
        else
            PlayersControllerScript = script;
    }


    private bool ValidateIndex(bool[] collection, int index)
    {
        if (index < 0 || index >= collection.Length)
            return false;
        return true;
    }
    private bool AnyTurnsActive(bool[] collection)
    {
        for (int i = 0; i < collection.Length; i++)
            if (collection[i]) // If any are not done yet
                return true;
        return false;
    }

    public int RegisterPCH()
    {
        for (int i = 0; i < PCHSlots.Length; i++)
        {
            if (!PCHSlots[i])
            {
                PCHSlots[i] = true;
                return i;
            }
        }

        Debug.LogError("No valid PCH slot was found and returned at GameMode");
        return -1;
    }
    public int RegisterECH()
    {
        for (int i = 0; i < ECHSlots.Length; i++)
        {
            if (!ECHSlots[i])
            {
                ECHSlots[i] = true;
                return i;
            }
        }

        Debug.LogError("No valid PCH slot was found and returned at GameMode");
        return -1;
    }

    public void UnregisterPCH(int index)
    {
        if (index < 0 || index >= PCHSlots.Length)
        {
            Debug.LogError("Out of range index set for UnregisterPCH " + index);
            return;
        }

        PCHSlots[index] = false;
    }
    public void UnregisterECH(int index)
    {
        if (index < 0 || index >= ECHSlots.Length)
        {
            Debug.LogError("Out of range index set for UnregisterECH " + index);
            return;
        }
        ECHSlots[index] = false;
    }

    public void EndPCHTurn(int index)
    {
        if (!ValidateIndex(PCHTurns, index))
        {
            Debug.LogError("Invalid index was sent to EndPCHTurn");
            return;
        }
        if (!PCHTurns[index])
        {
            Debug.LogWarning("Turn PCH " + index + " is already over!");
            return;
        }
        PCHTurns[index] = false;
        CheckTurnsSwitch();
    }
    public void EndECHTurn(int index)
    {
        if (!ValidateIndex(ECHTurns, index))
        {
            Debug.LogError("Invalid index was sent to EndECHTurn");
            return;
        }
        if (!ECHTurns[index])
        {
            Debug.LogWarning("Turn ECH " + index + " is already over!");
            //return;
        }
        ECHTurns[index] = false;
        CheckTurnsSwitch();
    }

    public void Activate()
    {
        SetupTurns();
    }

    private void CheckTurnsSwitch()
    {
        if (CurrentTurn == TurnType.PLAYER_CHARACTERS)
        {
            if (AnyTurnsActive(PCHTurns))
                return;

            CurrentTurn = TurnType.ENEMY_CHARACTERS; // Switch to ech turns if all pch turns are false
            SetupTurns();
        }
        else if (CurrentTurn == TurnType.ENEMY_CHARACTERS)
        {
            if (AnyTurnsActive(ECHTurns))
                return;

            CurrentTurn = TurnType.PLAYER_CHARACTERS; // Switch to pch turns if all ech turns are false
            SetupTurns();
        }
    }



    private int GetNextPCHTurnIndex()
    {
        for (int i = 0; i < PCHTurns.Length; i++)
            if (PCHTurns[i])//Turn has to be active.
                if (PCHSlots[i])//Slot has to be occupied.
                    return i;
        return -1;
    }
    private int GetNextECHTurnIndex()
    {
        for (int i = 0; i < ECHTurns.Length; i++)
            if (ECHTurns[i])//Turn has to be active.
                if (ECHSlots[i])//Slot has to be occupied.
                    return i;
        return -1;
    }


    private void SetupTurns()
    {
        if (CurrentTurn == TurnType.PLAYER_CHARACTERS)
        {
            for (int i = 0; i < PCHTurns.Length; i++)
                if (PCHSlots[i])
                    PCHTurns[i] = true;
            PlayersControllerScript.SetupTurns();
        }
        else if (CurrentTurn == TurnType.ENEMY_CHARACTERS)
        {
            for (int i = 0; i < ECHTurns.Length; i++)
                if (ECHSlots[i])
                    ECHTurns[i] = true;
            EnemiesControllerScript.SetupTurns();
        }
    }
    public void UpdateTurns()
    {
        if (CurrentTurn == TurnType.PLAYER_CHARACTERS)
        {
            CurrentCharacterTurn = GetNextPCHTurnIndex();
            if(CurrentCharacterTurn == -1)
            {
                Debug.Log("No PCH character is left!");
                return;
            }
            PlayersControllerScript.UpdateTurns(CurrentCharacterTurn);
        }
        else if (CurrentTurn == TurnType.ENEMY_CHARACTERS)
        {
            CurrentCharacterTurn = GetNextECHTurnIndex();
            if (CurrentCharacterTurn == -1)
            {
                Debug.LogError("No ECH character is left!");
                return;
            }
            EnemiesControllerScript.UpdateTurns(CurrentCharacterTurn);
        }
    }
}
