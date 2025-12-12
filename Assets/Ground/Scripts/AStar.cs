using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AStar
{
    public GridManager gridManager;

    public List<Node> explore = new();
    public HashSet<Node> processed = new();

    public List<Node> FindPath(Node startNode, Node goalNode)
    {
        List<Node> path = new();

        foreach(Node node in gridManager.Nodes)
        {
            node.gCost = float.PositiveInfinity;
            node.hCost = node.ManhattanDistanceTo(goalNode);
            node.parent = null;
        }

        explore = new();
        processed = new();

        startNode.gCost = 0;
        explore.Add(startNode);
        Node currentNode = null;

        while(explore.Count != 0)
        {
            explore = explore.OrderByDescending(n => n.fCost).ToList();

            int lastIndex = explore.Count-1;
            currentNode = explore[lastIndex];
            explore.RemoveAt(lastIndex);

            //explore = explore.OrderBy(n => n.fCost).ToList();
            //currentNode = explore[0];
            //explore.RemoveAt(0);

            if (currentNode == goalNode)
                break;

            processed.Add(currentNode);
            foreach(Node neighbour in gridManager.GetNeighbours(currentNode))
            {
                if (neighbour == null || !neighbour.isWalkable || processed.Contains(neighbour))
                    continue;

                float tentativeG = currentNode.gCost+1;
                if (tentativeG < neighbour.gCost)
                {
                    neighbour.gCost = tentativeG;
                    neighbour.parent = currentNode;
                }

                explore.Add(neighbour);
            }
        }

        if (currentNode == null || currentNode != goalNode)
            return path;

        while(currentNode.parent != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
}
