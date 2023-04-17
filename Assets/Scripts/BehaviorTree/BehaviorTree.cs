using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    public enum EvaluationState
    {
        SUCCESS,
        FAILURE,
        RUNNING,
        ERROR
    }
    public enum ExecutionState
    {
        SUCCESS,
        FAILURE,
        RUNNING,
        ERROR
    }

    private ExecutionState CurrentExecutionState = ExecutionState.ERROR;
    private EvaluationState CurrentEvaluationState = EvaluationState.FAILURE;

    private string CurrentTaskName = "Nothing";

    private Root RootNode = new Root();
    private Blackboard MainBlackBoard = new Blackboard();

    private Node CurrentNode = null;
    private Composite LastTickedComposite = null;
    private Task RunningTask = null;

    public void Reset()
    {
        CurrentNode = null;
        LastTickedComposite = null;
        RunningTask = null;

        CurrentExecutionState = ExecutionState.ERROR;
        CurrentEvaluationState = EvaluationState.FAILURE;
    }

    public void SetCurrentTaskName(string name)
    {
        CurrentTaskName = name;
    }
    public void SetCurrentNode(Node node)
    {
        CurrentNode = node;
    }
    public void SetLastTickedComposite(Composite node)
    {
        LastTickedComposite = node;
    }


    public string GetRunningTaskName()
    {
        return CurrentTaskName;
    }
    public Blackboard GetBlackboard()
    {
        if (MainBlackBoard == null)
            Debug.LogError("Blackboard reference is null at behavior tree");
        return MainBlackBoard;
    }

    public void ConnectToRoot(Node node)
    {
        RootNode.ConnectNode(node);
    }

    public void Evaluate() // The states and this could be dumbed the fuck down
    {
        if (RunningTask != null) // If there is a running task. TO CHECK IF RUNNING TASK GOT INTERRUPTED!!
        {
            CurrentEvaluationState = RootNode.Evaluate(this);
            switch (CurrentEvaluationState)
            {
                case EvaluationState.ERROR: //If an error was encountered
                    {
                        //Doesnt matter if before, It or after.
                        if (CurrentNode == RunningTask) //If the node that returned error is the running task. - it interrupts itself when it sends back the error
                        {
                            RunningTask = null;
                        }
                        else
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " was interrupted due to an error. - Evaluation");
                            RunningTask.Interrupt();
                            RunningTask = null;
                        }
                    }
                    break;
                case EvaluationState.FAILURE: //If a node ran and returned failure
                    {
                        if (CurrentNode == RunningTask) //If the node that returned failure is the running task. - it interrupts itself when it sends back the error
                        {
                            RunningTask = null;
                        }
                        else //If not then its gotta be before somewhere else. (There is no before and after now cause this doesnt tick it)
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to a failure. - Evaluation");
                            RunningTask.Interrupt();
                            RunningTask = null;  
                        }   
                    }   
                    break;
                case EvaluationState.SUCCESS: //If a node ran and returned success
                    {
                        if (CurrentNode == RunningTask) //If the node that returned success is the running task.
                        {
                            break;
                        }
                        else //If not then something else at some point
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to a success. - Evaluation");
                            RunningTask.Interrupt();
                            RunningTask = null;
                        }
                    }
                    break;
                case EvaluationState.RUNNING: //If a node ran and returend running
                    {
                        if (CurrentNode == RunningTask) //If the node that returned running is the running task.
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " passed the evaluation and is still running - Evaluation");
                            break;
                        }
                        else //If not then it was something else
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to start running - Evaluation");
                            RunningTask.Interrupt(); //Interrupt old one.
                        }
                    }
                    break;
            }
        }
        else
            Debug.Log("Evaluation did not start cause there was no running task set!");
    }
    public void Execute()
    {
        if (RunningTask == null) // If there is no running task set. TO RUN NORMALLY OR FIND A RUNNING TASK.!!!!
        {
            CurrentExecutionState = RootNode.Execute(this);
            switch (CurrentExecutionState)
            {
                case ExecutionState.ERROR: //If it did not run correctly
                    {
                        Debug.Log("Error is returned to the behavior tree - Execution - No running task");
                    }
                    break;
                case ExecutionState.FAILURE: //If it ran but failed the last available task
                    {
                        Debug.Log("Failure is returned to the behavior tree - Execution - No running task + " + CurrentNode.ToString());
                    }
                    break;
                case ExecutionState.SUCCESS: //If it ran and succeeded at the first task
                    {
                        Debug.Log("Success is returned to the behavior tree - Execution - No running task");
                    }
                    break;
                case ExecutionState.RUNNING: //If it ran and is currently running a task
                    {
                        //Set running task to current if any returns running
                        RunningTask = (Task)CurrentNode; //Questionable as fuck
                        Debug.Log("Running task " + RunningTask.ToString() + " started running - Execution - No running task");
                    }
                    break;
            }
        }
        else if (RunningTask != null) //If there is a running task then its status is running. TO RUN RUNNING TASK!
        {
            if (LastTickedComposite == null)
            {
                Debug.LogError("Running task is set but there is no lasttickedcomponent - error");
                return;
            } //Just to catch error

            CurrentExecutionState = LastTickedComposite.Execute(this);
            switch (CurrentExecutionState)
            {
                case ExecutionState.ERROR: //If the task did not run correctly
                    {
                        if (CurrentNode == RunningTask) //If the node that encountered an error is the running task.
                        {
                            RunningTask = null;
                        }
                        else //If not
                        {
                            if (RunningTask.GetStatus() == Task.RunningStatus.NOT_RUNNING) //If the task status is not running. It finished and something afterwards returned error.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was finished and " + CurrentNode.ToString() + " was called and returned error - Execution - Running task");
                                RunningTask = null;
                            }
                            else //If it is running then something before returned error.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to an error - Execution - Running task");
                                RunningTask.Interrupt();
                                RunningTask = null;
                            }
                        }
                    }
                    break;
                case ExecutionState.FAILURE: //If the task failed while running
                    {
                        if (CurrentNode == RunningTask) //If the node that returned failure is the running task.
                        {
                            RunningTask = null;
                        }
                        else //If not
                        {
                            if (RunningTask.GetStatus() == Task.RunningStatus.NOT_RUNNING) //If the task status is not running. It finished and something afterwards failed.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was finished and " + CurrentNode.ToString() + " was called and failed - Execution - Running task");
                                RunningTask = null;
                            }
                            else //If it is running then something before it failed.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to a failure - Execution - Running task");
                                RunningTask.Interrupt();
                                RunningTask = null;
                            }
                        }
                    }
                    break;
                case ExecutionState.SUCCESS: //If the task ran and succeeded
                    {
                        if (CurrentNode == RunningTask) //If the node that returned success is the running task.
                        {
                            RunningTask = null;
                        }
                        else //If not
                        {
                            if (RunningTask.GetStatus() == Task.RunningStatus.NOT_RUNNING) //If the task status is not running. It finished and something afterwards returned success.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was finished and " + CurrentNode.ToString() + " was called and succeded - Execution - Running task");
                                RunningTask = null;
                            }
                            else //If it is running then something before it was called and succeded.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it to a success - Execution - Running task");
                                RunningTask.Interrupt();
                                RunningTask = null;
                            }
                        }
                    }
                    break;
                case ExecutionState.RUNNING: //If the task is currently running
                    {
                        if (CurrentNode == RunningTask) //If the node that returned running is the running task.
                        {
                            Debug.Log("Running task " + RunningTask.ToString() + " is running currently - Execution - Running task");
                            break;
                        }
                        else //If not
                        {
                            if (RunningTask.GetStatus() == Task.RunningStatus.NOT_RUNNING) //If the task status is not running. It finished and something afterwards started running.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was finished and " + CurrentNode.ToString() + " was called and started running - Execution - Running task");
                                RunningTask = (Task)CurrentNode; //Questionable as fuck.
                            }
                            else //If it is running then something before it was called and started running.
                            {
                                Debug.Log("Running task " + RunningTask.ToString() + " was interrupted and " + CurrentNode.ToString() + " was called before it and started running - Execution - Running task");
                                RunningTask.Interrupt(); //Interrupt old one.
                                RunningTask = (Task)CurrentNode; //Questionable as fuck.
                            }
                        }
                    }
                    break;
            }
        }
    }
}
