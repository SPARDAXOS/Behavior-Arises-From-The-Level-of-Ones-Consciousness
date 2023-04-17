using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public MapGenerator.CellType Type;


    public void SetCellType(MapGenerator.CellType type)
    {
        Type = type;
    }
    public MapGenerator.CellType GetCellType()
    {
        return Type;
    }
}
