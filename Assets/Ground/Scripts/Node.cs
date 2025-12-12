using UnityEngine;

public class Node
{
    public int x;
    public int y;

    public bool isWalkable;
    public GameObject tile;

    public float gCost;
    public float hCost;
    public Node parent;
    
    public float fCost => gCost + hCost;

    public Node(int x, int y, bool isWalkable, GameObject tile)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.tile = tile;

        gCost = float.PositiveInfinity;
        hCost = 0f;
        parent = null;
    }

    public int ManhattanDistanceTo(Node other)
    {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
    }
}
