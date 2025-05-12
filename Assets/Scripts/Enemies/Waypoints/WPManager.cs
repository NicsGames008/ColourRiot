using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Link
{
    public enum direction { BI, UNI };
    public GameObject node1;
    public GameObject node2;
    public direction dir;
}

public class WPManager : MonoBehaviour
{
    public GameObject[] waypoints;
    public Link[] links;
    public Graph graph = new Graph();
    // Start is called before the first frame update
    void Awake()
    {
        foreach (var waypoint in waypoints)
        {
           graph.AddNode(waypoint);
        }
        foreach(var link in links)
        {
            graph.AddEdge(link.node1, link.node2);
            if(link.dir == Link.direction.BI)
            {
                graph.AddEdge(link.node2, link.node1);  
            }

        }
       graph.DrawGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
