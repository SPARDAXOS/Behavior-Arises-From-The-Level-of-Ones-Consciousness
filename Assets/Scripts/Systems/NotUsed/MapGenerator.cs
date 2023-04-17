using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum CellType
    {
        SPAWNER,
        EMPTY
    }
    //Info for keys for blackboard flow
    //Assign ID to objects to creation in spawner.
    //ID + String for data = key in blackboard.

    [SerializeField] Vector3 MapOrigin = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] int WidthCellsCount = 3;
    [SerializeField] int HeightCellsCount = 3;
    [SerializeField] int SpawnersAmount = 3;
    [SerializeField] float SpawnersZOffset = 0.01f;

    private List<GameObject> Cells;
    private List<GameObject> Spawners;

    private int TotalCellsCount = -1;
    private float CellWidth = 0.0f;
    private float CellHeight = 0.0f;

    private GameObject CellAsset = null;
    private GameObject SpawnerAsset = null;

    private void LoadResources()
    {
        CellAsset = Resources.Load<GameObject>("Entities/Cell");
        if (!CellAsset)
        {
            Debug.LogError("Cell asset was not loaded correctly");
            return;
        }

        SpawnerAsset = Resources.Load<GameObject>("Entities/MonsterSpawner");
        if (!SpawnerAsset)
        {
            Debug.LogError("Spawner asset was not loaded correctly");
            return;
        }
    }
    private void CreateGridMap()
    {
        if (!CellAsset)
        {
            Debug.LogError("Null reference cell asset");
            return;
        }

        CalculateCellDimensions();
        TotalCellsCount = WidthCellsCount * HeightCellsCount;
        Cells = new List<GameObject>(TotalCellsCount);

        GameObject cell = null;
        Cell script = null;
        for (int i = 0; i < HeightCellsCount; i++)
        {
            for (int j = 0; j < WidthCellsCount; j++)
            {
                cell = Instantiate<GameObject>(CellAsset);
                script = cell.GetComponent<Cell>();
                float X = MapOrigin.x - (CellWidth * j);
                float Y = MapOrigin.y + (CellHeight * i);
                float Z = MapOrigin.z;
                cell.transform.position = new Vector3(X, Y, Z);
                script.SetCellType(CellType.EMPTY);
                Cells.Add(cell);
            }
        }
    }
    private void CreateSpawners()
    {
        Spawners = new List<GameObject>(SpawnersAmount);

        GameObject cell = null;
        Cell script = null;
        for (int i = 0; i < SpawnersAmount; i++)
        {
            GameObject spanwer = Instantiate<GameObject>(SpawnerAsset);
            Spawners.Add(spanwer);

            cell = GetRandomEmptyCell();
            script = cell.GetComponent<Cell>();
            Vector3 cellpos = cell.transform.position;

            spanwer.transform.position = new Vector3(cellpos.x, cellpos.y + SpawnersZOffset, cellpos.z);
            script.SetCellType(CellType.SPAWNER);
        }
    }


    private void CalculateCellDimensions()
    {
        SpriteRenderer CellSR = CellAsset.GetComponent<SpriteRenderer>();
        CellWidth = CellSR.size.x;
        CellHeight = CellSR.size.y;
    }





    private bool IsCellsPoolValid()
    {
        if (Cells.Count <= 0)
            return false;
        else
            return true;
    }
    private bool IsCellValid(int index)
    {
        if (Cells[index] == null)
            return false;
        else
            return true;
    }


    public GameObject GetRandomCell()
    {
        if (!IsCellsPoolValid())
        {
            Debug.LogError("Cells pool is not valid at GetRandomCell");
            return null;
        }

        int index = Random.Range(0, (TotalCellsCount - 1));
        if (!IsCellValid(index))
        {
            Debug.LogError("Invalid cell index sent to validate at GetRandomCell - " + index);
            return null;
        }

        return Cells[index];
    }
    public GameObject GetRandomEmptyCell()
    {
        if (!IsCellsPoolValid())
        {
            Debug.LogError("Cells pool is not valid at GetRandomEmptyCell");
            return null;
        }

        GameObject cell = null;
        Cell script = null;
        for(int i = 0; i < Cells.Count; i++)
        {
            int index = Random.Range(0, (TotalCellsCount - 1));
            if (IsCellValid(index))
            {
                cell = Cells[index];
                script = cell.GetComponent<Cell>();
                if (script.GetCellType() == CellType.EMPTY)
                {
                    Debug.Log(index);
                    return cell;
                }
            }
        }

        Debug.LogWarning("Couldnt return any empty cells at GetRandomEmptyCell");
        return null;
    }


    void Start()
    {
        LoadResources();
        CreateGridMap();
        CreateSpawners();
    }
}
