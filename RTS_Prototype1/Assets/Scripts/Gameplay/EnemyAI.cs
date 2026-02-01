using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ShipMotor), typeof(Weapon), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private Weapon _weapon;
    private Health _health;
    private Health _currentTarget;
    private Health _attacker;

    [Header("Behavior")]
    [SerializeField] private LayerMask obstacleLayers; // Layers that block line of sight

    private UnitState _currentState;
    private float _findTargetCooldown = 0f;
    private const float FindTargetInterval = 2.0f;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
        _weapon = GetComponent<Weapon>();
        _health = GetComponent<Health>();

        _health.OnDeath += HandleDeath;
        _health.OnDamaged += HandleDamaged;
    }

    void Start()
    {
        _currentState = UnitState.Idle;
    }

    void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDeath -= HandleDeath;
            _health.OnDamaged -= HandleDamaged;
        }
        if (_currentTarget != null)
        {
            _currentTarget.OnDeath -= OnTargetDeath;
        }
    }

    void Update()
    {
        // 1. Target acquisition
        if ((_currentTarget == null || _currentTarget.IsDead) && (_attacker == null || _attacker.IsDead))
        {
            _currentState = UnitState.Idle; // Go idle if no valid target
            _findTargetCooldown -= Time.deltaTime;
            if (_findTargetCooldown <= 0)
            {
                FindNewTarget();
                _findTargetCooldown = FindTargetInterval;
            }
        }

        if (_attacker != null && !_attacker.IsDead)
        {
             if (_currentTarget != _attacker)
            {
                SetTarget(_attacker);
            }
        }
        
        // 2. State behavior
        switch (_currentState)
        {
            case UnitState.Idle:
                // Do nothing, waiting for a target
                break;

            case UnitState.Attacking:
                if (_currentTarget == null) { _currentState = UnitState.Idle; break; }

                float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
                if (distance > _weapon.AttackRange)
                {
                    _shipMotor.MoveTo(_currentTarget.transform.position);
                }
                else
                {
                    _shipMotor.Stop();
                    Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                    
                    if (HasLineOfSight())
                    {
                        if (_weapon.CanFire())
                        {
                            _weapon.Fire(_currentTarget);
                        }
                    }
                }
                break;
        }
    }

    private bool HasLineOfSight()
    {
        if (_currentTarget == null) return false;
        
        // Use the weapon's fire point for a more accurate LOS check
        Transform firePoint = _weapon.firePoint;
        Vector3 startPoint = firePoint != null ? firePoint.position : transform.position;

        RaycastHit hit;
        Vector3 direction = (_currentTarget.transform.position - startPoint).normalized;
        
        if (Physics.Raycast(startPoint, direction, out hit, _weapon.AttackRange, ~0, QueryTriggerInteraction.Ignore))
        {
            // Check if what we hit is our actual target or one of its children (for complex prefabs)
            if (hit.collider.transform.IsChildOf(_currentTarget.transform) || hit.collider.transform == _currentTarget.transform)
            {
                return true; // Clear shot
            }
        }
        return false;
    }
    
    private void FindNewTarget()
    {
        GameObject[] cargoShips = GameObject.FindGameObjectsWithTag("CargoShip");
        if (cargoShips.Length == 0) return;

        Transform closestShip = cargoShips
            .OrderBy(ship => Vector3.Distance(transform.position, ship.transform.position))
            .FirstOrDefault()?.transform;

        if (closestShip != null)
        {
            SetTarget(closestShip.GetComponent<Health>());
        }
    }
    
    private void SetTarget(Health newTarget)
    {
        if (_currentTarget != null)
        {
            _currentTarget.OnDeath -= OnTargetDeath;
        }

        _currentTarget = newTarget;

        if (_currentTarget != null)
        {
            _currentTarget.OnDeath += OnTargetDeath;
            _currentState = UnitState.Attacking;

        }
        else
        {
            _currentState = UnitState.Idle;
            _attacker = null;
        }
    }

    private void OnTargetDeath(GameObject deadObject)
    {
        if (_currentTarget != null && _currentTarget.gameObject == deadObject)
        {
            SetTarget(null);
        }
    }
    
    private void HandleDamaged(GameObject victim, GameObject attacker)
    {
        if (victim == gameObject && attacker != null)
        {
            Health attackerHealth = attacker.GetComponent<Health>();
            UnitFaction attackerFaction = attacker.GetComponent<UnitFaction>();

            if (attackerHealth != null && attackerFaction != null && attackerFaction.UnitFactionType == Faction.Player)
            {
                _attacker = attackerHealth;
            }
        }
    }
    


    private void HandleDeath(GameObject deadObject)
    {
        // Debug.Log("Enemy AI shutting down."); // Was removed
    }

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        if (_weapon == null) return;

        // Draw Weapon Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _weapon.AttackRange);

        // Draw line of sight
        if (_currentTarget != null)
        {
            if (HasLineOfSight())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.magenta;
            }
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
        }
    }

    // This is expensive, for debug only.
    private void OnGUI()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 20), $"State: {_currentState}");
    }
    #endregion
}