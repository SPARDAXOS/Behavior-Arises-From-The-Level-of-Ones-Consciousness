using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Composite
{
    //This func is exact same as sequence. Maybe make it in composite? Maybe leave it for separation and debugging puposes
    public override void ConnectNode(Node node)
    {
        if (node == null)
        {
            Debug.LogError("Null node was sent to selector");
            return;
        }
        ConnectedNodes.Add(node);
    }



    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (ConnectedNodes.Count <= 0)
        {
            Debug.LogError("Selector does not have children nodes - Evaluate");
            return BehaviorTree.EvaluationState.ERROR;
        }

        bt.SetCurrentNode(this);

        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetLastTickedComposite(this); //HERE to avoid edge case of composite has a child that is composite
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
            Debug.LogError("Selector does not have children nodes - Execute");
            return BehaviorTree.ExecutionState.ERROR;
        }

        bt.SetCurrentNode(this);

        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetLastTickedComposite(this); //HERE to avoid edge case of composite has a child that is composite
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
