using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : Node
{
    List<Node> ConnectedNodes = new List<Node>();

    public void ConnectNode(Node node)
    {
        if (node == null)
        {
            Debug.LogError("Null node sent to root");
            return;
        }
        else
            ConnectedNodes.Add(node);
    }

    //IMPORTANT IF YOU HAVE ONE TASK CONNECTED TO ROOT IMMEDIATELY AND ITS A RUNNING TASK
    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (ConnectedNodes.Count <= 0)
        {
            Debug.LogError("No nodes connected to root - Evaluate");
            return BehaviorTree.EvaluationState.ERROR;
        }

        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetCurrentNode(this);
            BehaviorTree.EvaluationState ReturnState = ConnectedNodes[i].Evaluate(bt);

            switch (ReturnState)
            {
                case BehaviorTree.EvaluationState.SUCCESS: //End and return success
                    {
                        return BehaviorTree.EvaluationState.SUCCESS;
                    }
                case BehaviorTree.EvaluationState.FAILURE: //Try next option
                    {
                        continue;
                    }
                case BehaviorTree.EvaluationState.RUNNING: //End and return running
                    {
                        return BehaviorTree.EvaluationState.RUNNING;
                    }
                case BehaviorTree.EvaluationState.ERROR: //End and return error
                    {
                        return BehaviorTree.EvaluationState.ERROR;
                    }
            }
        }

        return BehaviorTree.EvaluationState.FAILURE; //If it ran out of options to try out, it means they all failed.
    }
    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (ConnectedNodes.Count <= 0)
        {
            Debug.LogError("No nodes connected to root - Execute");
            return BehaviorTree.ExecutionState.ERROR;
        }

        for(int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetCurrentNode(this);
            BehaviorTree.ExecutionState ReturnState = ConnectedNodes[i].Execute(bt);

            switch (ReturnState)
            {
                case BehaviorTree.ExecutionState.SUCCESS: //End and return success
                    {
                        return BehaviorTree.ExecutionState.SUCCESS;
                    }
                case BehaviorTree.ExecutionState.FAILURE: //Try next option
                    {
                        continue;
                    }
                case BehaviorTree.ExecutionState.RUNNING: //End and return running
                    {
                        return BehaviorTree.ExecutionState.RUNNING;
                    }
                case BehaviorTree.ExecutionState.ERROR: //End and return error
                    {
                        return BehaviorTree.ExecutionState.ERROR;
                    }
            }
        }

        return BehaviorTree.ExecutionState.FAILURE; //If it ran out of options to try out, it means they all failed.
    }
}
