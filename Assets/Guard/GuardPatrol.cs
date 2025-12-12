using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;

public class GuardPatrol : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] _wayspoints;
    [SerializeField] private float _wayPointTolerance;

    [Header("Behavior")]
    [SerializeField] private float _chaseRange = 5f;
    [SerializeField] private float _looseRange = 7f;

    private enum State
    {
        Patrolling,
        Chasing,
        ReturningToPatrol,
    }

    private State _doing = State.Patrolling;
    private int _currentIndex = 0;
    private NavMeshAgent _agent;
    private GameObject _target;

    void Awake()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _target = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        _agent.SetDestination(_wayspoints[_currentIndex].position);
    }

    void Update()
    {
        switch (_doing)
        {
            case State.Patrolling:
                UpdatePatrol();
                break;
            case State.Chasing:
                UpdateChase();
                break;
            case State.ReturningToPatrol:
                UpdateReturning();
                break;
        }
    }

    private void UpdateReturning()
    {
        // Index of closest way point
        _currentIndex = Array.IndexOf(_wayspoints, _wayspoints.OrderBy(wp => 
                    Vector3.Distance(wp.position, _target.transform.position)).First());

        _doing = State.Patrolling;
    }

    private void UpdateChase()
    {
        if (Vector3.Distance(this.transform.position, _target.transform.position) > _looseRange)
        {
            _doing = State.ReturningToPatrol;
            return;
        }

        if (!_agent.pathPending)
            _agent.SetDestination(_target.transform.position);
    }

    private void UpdatePatrol()
    {
        if (Vector3.Distance(this.transform.position, _target.transform.position) <= _chaseRange)
        {
            _doing = State.Chasing;
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _wayPointTolerance)
        {
            _currentIndex = ++_currentIndex % _wayspoints.Length;
            _agent.SetDestination(_wayspoints[_currentIndex].position);
        }
    }
}
