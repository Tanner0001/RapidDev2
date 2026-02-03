using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShipMotor : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private float _defaultStoppingDistance;

    private Renderer _renderer;
    private Color _originalColor;
    private bool _isSelected;
    private bool _isTargeted;
    private MaterialPropertyBlock _propBlock; // For per-instance material properties

    [SerializeField] private Color selectionColor = Color.blue;
    [SerializeField] private Color targetColor = Color.red;

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _renderer = GetComponentInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();

        if (_renderer != null)
        {
            _renderer.GetPropertyBlock(_propBlock); // Get the current property block
            _originalColor = _propBlock.GetColor("_Color"); // Assuming the main color property is "_Color"
        }
        _defaultStoppingDistance = _navMeshAgent.stoppingDistance;
    }

    void Start()
    {
        UpdateHighlightColor();
    }
    
    void UpdateHighlightColor()
    {
        if (_renderer == null) return;

        _renderer.GetPropertyBlock(_propBlock); // Always get the current block to modify it

        if (_isTargeted)
        {
            _propBlock.SetColor("_Color", targetColor);
        }
        else if (_isSelected)
        {
            _propBlock.SetColor("_Color", selectionColor);
        }
        else
        {
            _propBlock.SetColor("_Color", _originalColor);
        }

        _renderer.SetPropertyBlock(_propBlock); // Apply the modified block
    }

    public void MoveTo(Vector3 destination)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(destination);
    }
    
    public void Stop()
    {
        if (_navMeshAgent.hasPath)
        {
            _navMeshAgent.isStopped = true;
        }
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

    public void SetStoppingDistance(float distance)
    {
        _navMeshAgent.stoppingDistance = distance;
    }

    public void ResetStoppingDistance()
    {
        _navMeshAgent.stoppingDistance = _defaultStoppingDistance;
    }

    public void Select()
    {
        _isSelected = true;
        UpdateHighlightColor();
    }

    public void Deselect()
    {
        _isSelected = false;
        UpdateHighlightColor();
    }
    
    public void Target()
    {
        _isTargeted = true;
        UpdateHighlightColor();
    }

    public void Untarget()
    {
        _isTargeted = false;
        UpdateHighlightColor();
    }

    public void IncreaseSpeed(float amount)
    {
        _navMeshAgent.speed += amount;
    }
}


