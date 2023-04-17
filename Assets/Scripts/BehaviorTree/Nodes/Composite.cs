using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composite : Node
{
    // Cant have this in interface cause leafs cant have children.
    protected List<Node> ConnectedNodes = new List<Node>();


    public virtual void ConnectNode(Node node)
    {
    }
}
