using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShipMotor : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject selectionRing;

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Deselect();
    }

    public void MoveTo(Vector3 destination)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(destination);
    }
    
    public void Stop()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public bool HasReachedDestination()
    {
        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Select()
    {
        if (selectionRing != null)
        {
            selectionRing.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selectionRing != null)
        {
            selectionRing.SetActive(false);
        }
    }

    public void IncreaseSpeed(float amount)
    {
        _navMeshAgent.speed += amount;
    }
}

