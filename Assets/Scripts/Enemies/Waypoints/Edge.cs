using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Node start;
    public Node end;

    public Edge(Node start, Node end)
    {
        this.start = start;
        this.end = end;
    }
}
