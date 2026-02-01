using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(ShipMotor), typeof(Weapon), typeof(PlayerUnit))]
public class PlayerCombat : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private Weapon _weapon;
    private Health _attackTarget;
    private UnitState _currentState;
    private UnitState _previousState;

    [Header("Behavior")]
    [SerializeField] private LayerMask obstacleLayers; // Layers that block line of sight
    
    // --- Debug ---
    private float _debugLogCooldown = 1f; 
    // --- End Debug ---

    // Auto-attack
    private float _findTargetCooldown;
    private const float AggroRadius = 40f;
    private const float FindTargetInterval = 1.0f;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
        _weapon = GetComponent<Weapon>();
        SetState(UnitState.Idle);
    }

    void Update()
    {
        // --- Debug ---
        _debugLogCooldown -= Time.deltaTime;
        if (_debugLogCooldown <= 0)
        {
            Debug.Log($"[{gameObject.name}] State: {_currentState} | Target: {(_attackTarget != null ? _attackTarget.name : "null")}");
            _debugLogCooldown = 2f; // Log every 2 seconds
        }
        // --- End Debug ---

        
        _findTargetCooldown -= Time.deltaTime;

        switch (_currentState)
        {
            case UnitState.Idle:
                if (_findTargetCooldown <= 0)
                {
                    FindEnemyInAggroRadius();
                    _findTargetCooldown = FindTargetInterval;
                }
                break;
            
            case UnitState.Moving:
                if (_shipMotor.HasReachedDestination())
                {
                    SetState(UnitState.Idle);
                }
                break;

            case UnitState.MovingToAttack:
                if (_attackTarget == null || _attackTarget.IsDead)
                {
                    SetState(UnitState.Idle);
                    _shipMotor.Stop();
                    _shipMotor.ResetStoppingDistance();
                    break;
                }

                // Let the NavMeshAgent handle stopping. Once it has, we attack.
                if (_shipMotor.HasReachedDestination())
                {
                    SetState(UnitState.Attacking);
                } else {
                    // Make sure we are still tracking the target
                    _shipMotor.MoveTo(_attackTarget.transform.position);
                }
                break;

            case UnitState.Attacking:
                if (_attackTarget == null || _attackTarget.IsDead)
                {
                    SetState(UnitState.Idle);
                    _attackTarget = null;
                    _shipMotor.ResetStoppingDistance();
                    break;
                }

                float distance = Vector3.Distance(transform.position, _attackTarget.transform.position);
                // Use a small buffer to prevent rapidly switching between states
                if (distance > _weapon.AttackRange + 2f) 
                {
                    // Target moved out of range, chase it
                    SetState(UnitState.MovingToAttack);
                     _shipMotor.SetStoppingDistance(_weapon.AttackRange * 0.9f);
                    _shipMotor.MoveTo(_attackTarget.transform.position);
                }
                else
                {
                    // In range, stop and attack
                    _shipMotor.Stop();

                    Vector3 direction = (_attackTarget.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                    
                    if (HasLineOfSight())
                    {
                        if (_weapon.CanFire())
                        {
                            _weapon.Fire(_attackTarget);
                        }
                    }
                }
                break;
        }
    }

    private void SetState(UnitState newState)
    {
        if (_currentState == newState) return;

        _previousState = _currentState;
        _currentState = newState;
        Debug.Log($"[{gameObject.name}] State Change: {_previousState} -> {_currentState}");

    }

    private bool HasLineOfSight()
    {
        if (_attackTarget == null) return false;

        Transform firePoint = _weapon.firePoint;
        Vector3 startPoint = firePoint != null ? firePoint.position : transform.position;
        Vector3 targetPoint = _attackTarget.transform.position;
        
        Vector3 direction = (targetPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, targetPoint);

        // Raycast from the fire point towards the target, considering only obstacle layers.
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, distance, obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        
        return true;
    }


    public void Move(Vector3 destination)
    {
        _shipMotor.ResetStoppingDistance();
        SetState(UnitState.Moving);
        _attackTarget = null; // Clear attack target on move
        _shipMotor.MoveTo(destination);
    }

    public void Attack(Health target)
    {
        if (target != null && !target.IsDead)
        {
            Debug.Log($"[{gameObject.name}] New Attack Order! Target: {target.name}");

            _attackTarget = target;
            SetState(UnitState.MovingToAttack);

            // Set stopping distance to be within weapon range to avoid ramming
            _shipMotor.SetStoppingDistance(_weapon.AttackRange * 0.9f);
            _shipMotor.MoveTo(target.transform.position);
        }
    }

    private void FindEnemyInAggroRadius()
    {
        var colliders = Physics.OverlapSphere(transform.position, AggroRadius);
        if (colliders.Length == 0) return;

        var closestEnemy = colliders
            .Select(c => c.GetComponent<Health>())
            .Where(h => h != null && !h.IsDead && h.GetComponent<UnitFaction>()?.UnitFactionType == Faction.Enemy)
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
            .FirstOrDefault();

        if (closestEnemy != null)
        {
            Attack(closestEnemy);
        }
    }

    #region Debugging
    #if UNITY_EDITOR

    #endif
    #endregion
}
