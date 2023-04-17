using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Composite
{
    public override void ConnectNode(Node node)
    {
        if(node == null)
        {
            Debug.LogError("Null node was sent to sequencer");
            return;
        }
        ConnectedNodes.Add(node);
    }



    public override BehaviorTree.EvaluationState Evaluate(BehaviorTree bt)
    {
        if (ConnectedNodes.Count <= 0)
        {
            Debug.LogError("Sequence does not have children nodes - Evaluate");
            return BehaviorTree.EvaluationState.FAILURE;
        }

        bt.SetCurrentNode(this); //Might need to remove from here

        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetLastTickedComposite(this); //HERE to avoid edge case of composite has a child that is composite
            BehaviorTree.EvaluationState ReturnState = ConnectedNodes[i].Evaluate(bt);

            //Return whatever you get unless its success then continue. This could be refactored.
            switch (ReturnState)
            {
                case BehaviorTree.EvaluationState.SUCCESS: //Try next option
                    {
                        continue;
                    }
                case BehaviorTree.EvaluationState.FAILURE: //End and return failure
                    {
                        return BehaviorTree.EvaluationState.FAILURE;
                    }
                case BehaviorTree.EvaluationState.RUNNING: // This is interesting here.... styopos at failure and runninga
                    {
                        return BehaviorTree.EvaluationState.RUNNING;
                    }
                case BehaviorTree.EvaluationState.ERROR: //End and return error
                    {
                        return BehaviorTree.EvaluationState.ERROR;
                    }
            }
        }

        return BehaviorTree.EvaluationState.SUCCESS; //All options have succeeded
    }

    public override BehaviorTree.ExecutionState Execute(BehaviorTree bt)
    {
        if (ConnectedNodes.Count <= 0)
        {
            Debug.LogError("Sequence does not have children nodes - Execute");
            return BehaviorTree.ExecutionState.FAILURE;
        }

        bt.SetCurrentNode(this); //Might need to remove from here

        for(int i = 0; i < ConnectedNodes.Count; i++)
        {
            bt.SetLastTickedComposite(this); //HERE to avoid edge case of composite has a child that is composite
            BehaviorTree.ExecutionState ReturnState = ConnectedNodes[i].Execute(bt);

            //Return whatever you get unless its success then continue. This could be refactored.
            switch (ReturnState)
            {
                case BehaviorTree.ExecutionState.SUCCESS: //Try next option
                    {
                        continue;
                    }
                case BehaviorTree.ExecutionState.FAILURE: //End and return failure
                    {
                        return BehaviorTree.ExecutionState.FAILURE;
                    }
                case BehaviorTree.ExecutionState.RUNNING: // This is interesting here.... styopos at failure and runninga
                    {
                        return BehaviorTree.ExecutionState.RUNNING;
                    }
                case BehaviorTree.ExecutionState.ERROR: //End and return error
                    {
                        return BehaviorTree.ExecutionState.ERROR;
                    } 
            }
        }

        return BehaviorTree.ExecutionState.SUCCESS; //All options have succeeded
    }
}
