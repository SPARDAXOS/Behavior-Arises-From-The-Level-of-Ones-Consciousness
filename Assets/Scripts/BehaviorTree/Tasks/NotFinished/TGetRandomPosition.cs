using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGetRandomPosition : Task
{
    private string PositionKey = null;



    private float XMin = -10.0f;
    private float XMax = 10.0f;

    private float YMin = 0.0f;
    private float YMax = 0.0f;

    private float ZMin = -10.0f;
    private float ZMax = 10.0f;


    public void SetXLimits(float min, float max)
    {
        XMin = min;
        XMax = max;
    }
    public void SetYLimits(float min, float max)
    {
        YMin = min;
        YMax = max;
    }
    public void SetZLimits(float min, float max)
    {
        ZMin = min;
        ZMax = max;
    }
    public void SetPositionKey(string pos)
    {
        PositionKey = pos;
    }


    private Vector3 GetPosition()
    {
        Vector3 RandomPosition = new Vector3();
        RandomPosition.x = Random.Range(XMin, XMax);
        RandomPosition.y = Random.Range(YMin, YMax);
        RandomPosition.z = Random.Range(ZMin, ZMax);

        return RandomPosition;
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (PositionKey == null)
        {
            Debug.LogError("Null position key set at TGetRandomPosition");
            return BehaviorTree.ExecutionState.ERROR;
        }

        Blackboard BB = bt.GetBlackboard();
        BB.UpdateValue<Vector3>(PositionKey, GetPosition());

        return BehaviorTree.ExecutionState.SUCCESS;
    }
}
