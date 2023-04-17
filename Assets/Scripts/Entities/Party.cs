using System.Collections.Generic;
using UnityEngine;

public class Party     //Used for grouping together squads!
{
    private Character[] Characters = new Character[3];
    private int CharactersInParty = 0;


    public Character GetCharacterInDanger(float threshold)
    {
        Character[] AvailableCharacters = GetCharactersLeft();
        for (int i = 0; i < AvailableCharacters.Length; i++)
        {
            if (AvailableCharacters[i])
            {
                if (AvailableCharacters[i].GetCurrentHealth() <= threshold)
                    return AvailableCharacters[i];
            }
        }
        return null;
    }

    private bool IsPartyFull()
    {
        if (CharactersInParty < Characters.Length)
            return false;
        return true;
    }

    public void AddCharacter(Character character, int index)
    {
        if (!character)
        {
            Debug.LogError("Null Character reference sent to Party");
            return;
        }
        if (!IsPartyFull())
        {
            if (Characters[index]) // Requested index is NOT empty
            {
                Debug.LogError("Index sent to party is already occupied in this party " + index + " Name: " + character.name);
                return;
            }
            else // Requested index is empty
            {
                Characters[index] = character;
                CharactersInParty++;
                return;
            }                     
        }

        Debug.LogError("Party is full");
    }
    public void RemoveCharacter(int index)
    {
        if(index < 0 || index >= Characters.Length)
        {
            Debug.LogError("Invalid index was sent to party for removal - " + index);
            return;
        }

        if (Characters[index])
        {
            CharactersInParty--;

            
            Characters[index] = default;
        }
        else
            Debug.LogError("No entry in party was found at index - " + index);
    }
    public Character[] GetCharacterSlots()
    {
        return Characters;
    }
    public Character[] GetCharactersLeft()
    {
        List<Character> ListOfCharacters = new List<Character>();
        Character[] AvailableCharacters = new Character[CharactersInParty];


        for (int i = 0; i < Characters.Length; i++)
            if (Characters[i])
                ListOfCharacters.Add(Characters[i]);

        if (ListOfCharacters.Count != CharactersInParty)
        {
            Debug.LogError("Error - Available characters amount isnt equal to CharactersInParty");
            return null;
        }

        for (int i = 0; i < ListOfCharacters.Count; i++)
        {
            AvailableCharacters[i] = ListOfCharacters[i];
        }
        //int num = 0;
        //for(int i = 0; i < Characters.Length; i++)
        //{
        //    if (Characters[i])
        //        num++;
        //}

        //Character[] AvailableCharacters = new Character[3];

        //for (int i = 0; i < Characters.Length; i++)
        //{
        //    if (Characters[i])
        //        AvailableCharacters[i] = Characters[i];
        //}




        return AvailableCharacters;
    }
    public int GetCharactersCount()
    {
        //int Count = 0;

        //for(int i = 0; i < Characters.Length; i++)
        //    if (Characters[i])
        //        Count++;

        return CharactersInParty;
    }

    public Character GetCharacterAtIndex(int index)
    {
        if (index < 0 || index >= Characters.Length)
        {
            return null;
        }



        return Characters[index];
    }
}
