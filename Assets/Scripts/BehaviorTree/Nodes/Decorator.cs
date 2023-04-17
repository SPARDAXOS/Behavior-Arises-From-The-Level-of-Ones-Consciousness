using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : Node
{
    protected Node ConnectedNode = null;

    public void ConnectNode(Node node)
    {
        if (node == null)
        {
            Debug.LogError("Null node connected to decorator");
            return;
        }
        else
            ConnectedNode = node;
    }
}
