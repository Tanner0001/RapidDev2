using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class ShipMotor : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject selectionRing;

    // State Machine
    private UnitState _currentState;
    private Transform _scanTarget;

    private List<Transform> _patrolPoints;
    private int _currentPatrolIndex;
    private bool _isFollowingLane = false;

    public UnitState CurrentState => _currentState;

    private UIManager _uiManager;
    private FXManager _fxManager;
    private SoundManager _soundManager;

    // Constants
    private const float HailDistance = 15.0f;
    private const float ScanTime = 2.0f;

    void Awake()
    {
        _currentState = UnitState.Idle;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogError($"ShipMotor on {name}: NavMeshAgent component not found!", this);
        }
        if (selectionRing == null)
        {
            Debug.LogWarning($"ShipMotor on {name}: SelectionRing is not assigned in the inspector.", this);
        }
    }

    void Start()
    {
        // Get singleton instances
        _uiManager = UIManager.Instance;
        if (_uiManager == null)
        {
            Debug.LogError("ShipMotor: UIManager instance not found in the scene!", this);
        }
        _fxManager = FXManager.Instance;
        if (_fxManager == null)
        {
            Debug.LogError("ShipMotor: FXManager instance not found in the scene!", this);
        }
        _soundManager = SoundManager.Instance;
        if (_soundManager == null)
        {
            Debug.LogError("ShipMotor: SoundManager instance not found in the scene!", this);
        }
    }

    void Update()
    {
        // --- State Machine Logic ---
        switch (_currentState)
        {
            case UnitState.Moving:
                // Check if we've arrived at our destination
                if (!_navMeshAgent.pathPending)
                {
                    if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                    {
                        // A check to ensure the agent is not moving.
                        if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                        {
                            Debug.Log($"[ShipMotor] {name} has arrived at its destination. Setting state to Idle.");
                            _currentState = UnitState.Idle;
                        }
                    }
                }
                break;

            case UnitState.Patrolling: // New state handling
                if (_isFollowingLane && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        Debug.Log($"[ShipMotor] {name}: Reached patrol point {_currentPatrolIndex}. Moving to next.");
                        _currentPatrolIndex++;
                        SetNextPatrolDestination();
                    }
                }
                break;
        }
    }

    private IEnumerator HailAndInspectRoutine()
    {
        // Stop the ship
        _navMeshAgent.ResetPath();
        Debug.Log($"[HailAndInspectRoutine] {name} has stopped to scan {_scanTarget.name}.");

        // Wait for scan time
        Debug.Log($"[HailAndInspectRoutine] Waiting for {ScanTime} seconds...");
        yield return new WaitForSeconds(ScanTime);
        Debug.Log($"[HailAndInspectRoutine] Wait finished.");

        // Get identity and reveal it
        ShipIdentity targetIdentity = _scanTarget.GetComponent<ShipIdentity>();
        if (targetIdentity != null)
        {
            Debug.Log($"[HailAndInspectRoutine] Scan complete. Attempting to call Reveal() on {_scanTarget.name}.");
            targetIdentity.Reveal();

            // Display the status on the UI
            if (_uiManager != null)
            {
                Debug.Log($"[ShipMotor] Attempting to call UIManager.DisplayShipStatus for {_scanTarget.name}.");
                _uiManager.DisplayShipStatus(targetIdentity.transform, targetIdentity.RealIdentity);
            }

            // Decide what to do based on the revealed identity
            switch (targetIdentity.RealIdentity)
            {
                case ThreatLevel.Red:
                    Debug.Log($"[HailAndInspectRoutine] Target {_scanTarget.name} is hostile. Destroying it.");
                    _fxManager.PlayExplosionFX(_scanTarget.position);
                    _soundManager.PlayExplosionSound(_scanTarget.position);
                    Destroy(_scanTarget.gameObject);
                    _currentState = UnitState.Idle;
                    _scanTarget = null;
                    break;

                case ThreatLevel.Green:
                default:
                    Debug.Log($"[HailAndInspectRoutine] Target {_scanTarget.name} is friendly. Returning to Idle.");
                    _currentState = UnitState.Idle;
                    _scanTarget = null;
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[HailAndInspectRoutine] {name} scanned a target, but it has no ShipIdentity component. Returning to Idle.");
            _currentState = UnitState.Idle;
            _scanTarget = null;
        }

        Debug.Log($"[HailAndInspectRoutine] Process finished. {name} is now {CurrentState}.");
    }

    // --- Public Methods ---

    public void MoveTo(Vector3 destination)
    {
        Debug.Log($"[ShipMotor] {name} received a MOVE command to {destination}.");
        StopAllCoroutines(); // Stop any scanning if we issue a new move order
        _currentState = UnitState.Moving;
        _scanTarget = null;
        if (_navMeshAgent != null)
        {
            _navMeshAgent.stoppingDistance = 2.0f; // Reset to a default for normal movement
            _navMeshAgent.SetDestination(destination);
            Debug.Log($"[ShipMotor] {name} NavMeshAgent destination set. Has Path: {_navMeshAgent.hasPath}, Is Calculating: {_navMeshAgent.pathPending}");
        }
        else
        {
            Debug.LogError($"ShipMotor on {name}: Cannot MoveTo because NavMeshAgent is missing!", this);
        }
    }

    public void Scan(Transform target)
    {
        Debug.Log($"[ShipMotor] {name} received a SCAN command for {target.name}.");
        StopAllCoroutines(); // Stop any previous scanning
        _currentState = UnitState.Scanning;
        _scanTarget = target;
        StartCoroutine(HailAndInspectRoutine());
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

    public void FollowLane(List<Transform> laneTransforms)
    {
        if (laneTransforms == null || laneTransforms.Count < 2)
        {
            Debug.LogWarning($"[ShipMotor] {name}: Cannot follow lane. Insufficient lane points provided.", this);
            _currentState = UnitState.Idle;
            _isFollowingLane = false;
            return;
        }

        StopAllCoroutines(); // Stop any other operations
        _currentState = UnitState.Patrolling; // Using Patrolling state for now
        _isFollowingLane = true;
        _patrolPoints = new List<Transform>(laneTransforms); // Copy the list
        _currentPatrolIndex = 0;
        _navMeshAgent.stoppingDistance = 0.5f; // Small stopping distance for precision

        SetNextPatrolDestination();
    }

    private void SetNextPatrolDestination()
    {
        if (_currentPatrolIndex >= _patrolPoints.Count)
        {
            Debug.Log($"[ShipMotor] {name}: Reached end of lane. Destroying ship.", this);
            _fxManager.PlayExplosionFX(transform.position); // Play explosion at ship's current position
            _soundManager.PlayExplosionSound(transform.position); // Play sound
            Destroy(gameObject); // Self-destruct
            return;
        }

        Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        if (targetPoint != null)
        {
            _navMeshAgent.SetDestination(targetPoint.position);
            Debug.Log($"[ShipMotor] {name}: Moving to patrol point {_currentPatrolIndex + 1}/{_patrolPoints.Count}: {targetPoint.name}", this);
        }
        else
        {
            Debug.LogWarning($"[ShipMotor] {name}: Patrol point {_currentPatrolIndex} is null. Skipping.", this);
            _currentPatrolIndex++;
            SetNextPatrolDestination(); // Try next point
        }
    }
}
