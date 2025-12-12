using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private Material walkableMaterial;
    [SerializeField] private Material wallMaterial;

    [SerializeField] private Material startMat;
    [SerializeField] private Material goalMat;
    [SerializeField] private Material pathMat;
    [SerializeField] private Material exploreMat;
    [SerializeField] private Material processedMat;

    private Node[,] nodes;
    private Dictionary<GameObject, Node> tileToNode = new();
    // Input action for click
    private InputAction clickAction;
    private InputAction pathAction;
    private AStar _aStar;
    public AStar aStar => _aStar;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    public IEnumerable<Node> Nodes
    {
        get
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return GetNode(x, y);
                }
            }
        }
    }

    private void Awake()
    {
        _aStar = new() { gridManager = this };
        GenerateGrid();
    }

    private void OnEnable()
    {
        clickAction = new InputAction(
                name: "Click",
                type: InputActionType.Button,
                binding: "<Mouse>/leftButton"
                );
        clickAction.performed += OnClickPerformed;
        clickAction.Enable();

        pathAction = new InputAction(
                name: "RightClick",
                type: InputActionType.Button,
                binding: "<Mouse>/rightButton"
                );
        pathAction.performed += OnPathPerformed;
        pathAction.Enable();
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.performed -= OnClickPerformed;
            clickAction.Disable();
        }
        if (pathAction != null)
        {
            pathAction.performed -= OnPathPerformed;
            pathAction.Disable();
        }
    }

    private void OnPathPerformed(InputAction.CallbackContext ctx)
    {
        PathFromStartToEnd();
    }

    public List<Node> PathFromStartToEnd()
    {
        foreach(Node node in Nodes)
            if (node.isWalkable)
                SetTileMaterial(node, walkableMaterial);

        Node start = nodes[0, 0];
        Node goal = nodes[width-1, height-1];

        List<Node> path = _aStar.FindPath(start, goal);

        foreach(Node node in _aStar.explore)
            SetTileMaterial(node, exploreMat);

        foreach(Node node in _aStar.processed)
            SetTileMaterial(node, processedMat);

        foreach(Node node in path)
            SetTileMaterial(node, pathMat);

        SetTileMaterial(start, startMat);
        SetTileMaterial(goal, goalMat);

        return path;
    }

    private void GenerateGrid()
    {
        nodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0f,
                        y * cellSize);
                GameObject tileGO = Instantiate(tilePrefab,
                        worldPos, Quaternion.identity, transform);
                tileGO.name = $"Tile_{x}_{y}";
                Node node = new Node(x, y, true, tileGO);
                nodes[x, y] = node;
                tileToNode[tileGO] = node;
                SetTileMaterial(node, walkableMaterial);
            }
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext
            ctx)
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        Ray ray =
            cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;
            if (tileToNode.TryGetValue(clicked, out Node node))
            {
                bool newWalkable = !node.isWalkable;
                SetWalkable(node, newWalkable);
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;
        return nodes[x, y];
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.z / cellSize);
        return GetNode(x, y);
    }

    public Vector3 NodeToWorldPosition(Node node)
    {
        return new Vector3(node.x * cellSize, transform.position.y, node.y * cellSize);
    }

    public IEnumerable<Node> GetNeighbours(Node node, bool
            allowDiagonals = false)
    {
        int x = node.x;
        int y = node.y;
        // 4-neighbour
        yield return GetNode(x + 1, y);
        yield return GetNode(x - 1, y);
        yield return GetNode(x, y + 1);
        yield return GetNode(x, y - 1);
        if (allowDiagonals)
        {
            yield return GetNode(x + 1, y + 1);
            yield return GetNode(x - 1, y + 1);
            yield return GetNode(x + 1, y - 1);
            yield return GetNode(x - 1, y - 1);
        }
    }

    public void SetWalkable(Node node, bool isWalkable)
    {
        node.isWalkable = isWalkable;
        SetTileMaterial(node, isWalkable ? walkableMaterial :
                wallMaterial);
    }

    private void SetTileMaterial(Node node, Material mat)
    {
        var renderer = node.tile.GetComponent<MeshRenderer>();
        if (renderer != null && mat != null)
        {
            renderer.material = mat;
        }
    }
}
