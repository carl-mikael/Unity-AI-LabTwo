using UnityEngine;
using System.Collections.Generic;

public class AgentMover : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    private float _moveSpeed;

    private List<Node> currentPath;
    private int currentIndex;
    private uint f = 0;

    void Start()
    {
        List<Node> path = _gridManager.PathFromStartToEnd();
        FollowPath(path);
    }

    void Update()
    {
        f++;
        if (f % 100 == 0)
            currentPath = _gridManager.PathFromStartToEnd();

        if (currentPath == null || currentPath.Count == 0)
        {
            currentIndex = 0;
            return;
        }

        Node targetNode = currentPath[currentIndex];
        Vector3 targetPos = _gridManager.NodeToWorldPosition(targetNode);
        targetPos.y = this.transform.position.y;

        _moveSpeed = currentIndex == 0 ? 10f : 3f;
        Vector3 move = (targetPos-this.transform.position).normalized * _moveSpeed * Time.deltaTime;
        this.transform.Translate(move);

        const float DISTANCE_BUFFERT = 0.1f;
        if (Mathf.Abs(this.transform.position.magnitude - targetPos.magnitude) <= DISTANCE_BUFFERT)
            currentIndex++;

        // Done
        if (currentIndex >= currentPath.Count)
            currentIndex = 0;
    }

    public void FollowPath(List<Node> path)
    {
        currentPath = path;
        currentIndex = 0;
    }

}
