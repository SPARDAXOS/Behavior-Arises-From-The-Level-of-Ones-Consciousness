using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Vector3[] PCHSpawnPoints = new Vector3[3];
    public Vector3[] ECHSpawnPoints = new Vector3[3];

    //Should i keep track of which were used? like a bool array for it like the pch and ech slots?

    private void SetupReferences()
    {
        Transform SpawnPoint = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        SpawnPoint = transform.Find("PCHSpawnPoint1");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - PCHSpawnPoint1 - Map");
                    }
                    break;
                case 1:
                    {
                        SpawnPoint = transform.Find("PCHSpawnPoint2");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - PCHSpawnPoint2 - Map");
                    }
                    break;
                case 2:
                    {
                        SpawnPoint = transform.Find("PCHSpawnPoint3");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - PCHSpawnPoint3 - Map");
                    }
                    break;
            }
            if (!SpawnPoint)
                Debug.LogError("Component was not found - " + i + " - Map");
            PCHSpawnPoints[i] = SpawnPoint.position;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        SpawnPoint = transform.Find("ECHSpawnPoint1");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - ECHSpawnPoint1 - Map");
                    }
                    break;
                case 1:
                    {
                        SpawnPoint = transform.Find("ECHSpawnPoint2");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - ECHSpawnPoint2 - Map");
                    }
                    break;
                case 2:
                    {
                        SpawnPoint = transform.Find("ECHSpawnPoint3");
                        if (!SpawnPoint)
                            Debug.LogError("Child was not found - ECHSpawnPoint3 - Map");
                    }
                    break;
            }
            if (!SpawnPoint)
                Debug.LogError("Component was not found - " + i + " - Map");
            ECHSpawnPoints[i] = SpawnPoint.position;
        }
    }

    public Vector3 GetPCHSpawnPoint(int index)
    {
        if (!ValidateIndex(PCHSpawnPoints, index))
        {
            Debug.LogError("Invalid PCH index was sent to GetPCHSpawnPoint - Map");
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        return PCHSpawnPoints[index];
    }
    public Vector3 GetECHSpawnPoint(int index)
    {
        if (!ValidateIndex(ECHSpawnPoints, index))
        {
            Debug.LogError("Invalid ECH index was sent to GetECHSpawnPoint - Map");
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        return ECHSpawnPoints[index];
    }
    private bool ValidateIndex(Vector3[] collection, int index)
    {
        if (index < 0 || index >= collection.Length)
            return false;
        return true;
    }
    public void Init()
    {
        SetupReferences();
    }
}
