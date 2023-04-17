using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private List<IDictionary> DataLibraries = new List<IDictionary>();
    private Dictionary<string, int> Addresses = new Dictionary<string, int>();

    private Dictionary<string, T> FindLibraryByType<T>()
    {
        if (DataLibraries.Count <= 0)
        {
            Debug.LogError("DataLibraries Empty at FindLibraryByType");
            return null;
        }

        System.Type ValueDataType = typeof(T);
        if (Addresses.ContainsKey(ValueDataType.ToString()))
        {
            int index = Addresses[ValueDataType.ToString()];
            return (Dictionary<string, T>)DataLibraries[index];
        }
        else
            return null;
    }
    private void AddToNewLibrary<T>(string key, T value)
    {
        Dictionary<string, T> NewLibrary = new Dictionary<string, T>(); // Creates a new library.                             
        NewLibrary.Add(key, value); // Adds the value to the new library.
        DataLibraries.Add(NewLibrary); // Adds new library to DataLibraries list.
        Addresses.Add(typeof(T).ToString(), DataLibraries.Count - 1); // If this fails then the same type of library was added twice
    }


    public void AddValue<T>(string key, T value)
    {
        //if(value == null)
        //{
        //    Debug.LogError("Null was sent to blackboard for adding new entry - Key: " + key);
        //    return;
        //}

        //DataLibraries is empty so create new library immediately without any searchs.
        if (DataLibraries.Count <= 0) 
        {
            AddToNewLibrary(key, value);
            return;
        }

        //DataLibraries is not empty so search for a library of the same datatype... 
        Dictionary<string, T> FoundDictionary = FindLibraryByType<T>();
        if (FoundDictionary != null)
        {
            if (FoundDictionary.ContainsKey(key))
            {
                Debug.LogError("Cant add key to blackboard - Key already exists - " + key);
                return;
            }
            else
            {
                FoundDictionary.Add(key, value);
                return;
            }
        }

        //A library with the same datatype was not found so create and add one...
        AddToNewLibrary(key, value);
    }
    public T GetValue<T>(string key)
    {
        Dictionary<string, T> FoundDictionary = FindLibraryByType<T>();
        if (FoundDictionary != null)
        {
            if (FoundDictionary.ContainsKey(key))
                return FoundDictionary[key];
            else
            {
                Debug.LogError("Key " + key + " does not exist in Blackboard - GetValue");
                return default(T); // Dont like this
            }
        }
        else
        {
            Debug.LogError("No library exists in Blackboard for datatype " + typeof(T) + " containing key " + key + " - GetValue");
            return default(T);
        }
    }
    public void UpdateValue<T>(string key, T value)
    {
        Dictionary<string, T> FoundDictionary = FindLibraryByType<T>();
        if (FoundDictionary != null)
        {
            if(FoundDictionary.ContainsKey(key))
                FoundDictionary[key] = value;
            else
                Debug.LogError("Key " + key + " does not exist in Blackboard - UpdateValue");
        }
        else
            Debug.LogError("No library exists in Blackboard for datatype " + typeof(T) + " containing key " + key + " - UpdateValue");
    }
}
