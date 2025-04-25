using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    List<Edge> edges = new List<Edge>();
    List<Node> nodes = new List<Node>();
    public List<Node> pathList = new List<Node>(); // list that gets populate by the A*
    public Graph() { }
    public void AddNode(GameObject id)
    {
        nodes.Add(new Node(id));
    }
    public void AddEdge(GameObject fromNode, GameObject toNode)
    {
        Node from = FindNode(fromNode);
        Node to = FindNode(toNode);
        if (from != null && to != null)
        {
            Edge edge = new Edge(from, to);
            edges.Add(edge);
            from.edges.Add(edge);
        }
    }
    Node FindNode(GameObject id)
    {
        foreach (Node n in nodes)
        {
            if (n.GetId().Equals(id))
            {
                return n;
            }
        }
        return null;
    }

    public void DrawGraph(float duration = 60f)
    {
        foreach (Edge edge in edges)
        {
            if (edge.start != null && edge.end != null)
            {
                Vector3 start = edge.start.GetId().transform.position;
                Vector3 end = edge.end.GetId().transform.position;
                Debug.DrawLine(start, end, Color.red, duration);
            }
        }
    }

    float Distance(Node a, Node b)
    {
        return Vector3.Distance(a.GetId().transform.position, b.GetId().transform.position);
    }

    int LowestF(List<Node> nodes)
    {
        float lowestF = 0;
        int count = 0;
        int iteratorCount = 0;
        lowestF = nodes[0].f;
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].f < lowestF)
            {
                lowestF = nodes[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }

    public bool AStar(GameObject startID, GameObject endID)
    {
        Node start = FindNode(startID);
        Node goal = FindNode(endID);
        if (start == null || goal == null)
            return false;
        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        start.g = 0;
        start.h = Distance(start, goal);
        start.f = start.g + start.h;
        start.cameFrom = null;
        openList.Add(start);
        while (openList.Count > 0)
        {
            // Find node with lowest f value
            Node current = openList.OrderBy(n => n.f).First();
            if (current == goal)
            {
                pathList = ReconstructPath(current); // Save it into pathList
                return true;
            }
            openList.Remove(current);
            closedSet.Add(current);
            foreach (Edge edge in current.edges)
            {
                Node neighbor = edge.end;
                if (closedSet.Contains(neighbor))
                    continue;
                float tentativeG = current.g + Distance(current, neighbor);
                if (!openList.Contains(neighbor) || tentativeG < neighbor.g)
                {
                    neighbor.cameFrom = current;
                    neighbor.g = tentativeG;
                    neighbor.h = Distance(neighbor, goal);
                    neighbor.f = neighbor.g + neighbor.h;
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        return false;
    }

    public bool AStar2(GameObject startID, GameObject endID)
    {
        Node start = FindNode(startID);
        Node end = FindNode(endID);
        if (start == null || end == null)
            return false;
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0;
        bool tentative_is_better;
        start.g = 0;
        start.h = Distance(start, end);
        start.f = start.g + start.h;
        open.Add(start);
        while (open.Count > 0)
        {
            int i = LowestF(open);
            Node thisNode = open[i];
            if (thisNode.GetId() == endID)
            {
                ReconstructPath2(start, thisNode);
                return true;
            }
            open.RemoveAt(i);
            closed.Add(thisNode);
            Node neighbour;
            foreach (Edge e in thisNode.edges)
            {
                neighbour = e.end;
                if (closed.IndexOf(neighbour) > -1)
                    continue;
                tentative_g_score = thisNode.g + Distance(thisNode, neighbour);
                if (open.IndexOf(neighbour) == -1)
                {
                    open.Add(neighbour);
                    tentative_is_better = true;
                }
                else if (tentative_g_score < neighbour.g)
                    tentative_is_better = true;
                else
                    tentative_is_better = false;
                if (tentative_is_better)
                {
                    neighbour.cameFrom = thisNode;
                    neighbour.g = tentative_g_score;
                    neighbour.h = Distance(thisNode, end);
                    neighbour.f = neighbour.g + neighbour.h;
                }
            }
        }
        return false;
    }

    List<Node> ReconstructPath(Node current)
    {
        List<Node> path = new List<Node>();
        while (current != null)
        {
            path.Add(current);
            current = current.cameFrom;
        }
        path.Reverse();
        return path;
    }

    public void ReconstructPath2(Node startID, Node endID)
    {
        pathList.Clear();
        pathList.Add(endID);
        Node p = endID.cameFrom;
        while (p != startID && p != null)
        {
            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startID);
    }
}