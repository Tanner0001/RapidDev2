using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ShipMotor), typeof(Weapon), typeof(PlayerUnit))]
public class PlayerCombat : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private Weapon _weapon;
    private Health _attackTarget;
    private UnitState _currentState;

    [Header("Behavior")]
    [SerializeField] private LayerMask obstacleLayers; // Layers that block line of sight

    // Auto-attack
    private float _findTargetCooldown;
    private const float AggroRadius = 40f;
    private const float FindTargetInterval = 1.0f;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
        _weapon = GetComponent<Weapon>();
        _currentState = UnitState.Idle;
    }

    void Update()
    {
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
                    _currentState = UnitState.Idle;
                }
                break;

            case UnitState.Attacking:
                if (_attackTarget == null || _attackTarget.IsDead)
                {
                    _currentState = UnitState.Idle;
                    _attackTarget = null;
                    break;
                }

                float distance = Vector3.Distance(transform.position, _attackTarget.transform.position);
                if (distance > _weapon.AttackRange)
                {
                    // Chase the target
                    _shipMotor.MoveTo(_attackTarget.transform.position);
                }
                else
                {
                    // In range, stop and check line of sight
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

    private bool HasLineOfSight()
    {
        if (_attackTarget == null) return false;

        // Use the weapon's fire point for a more accurate LOS check
        Transform firePoint = _weapon.firePoint; // Assuming firePoint is public or has a public getter in Weapon.cs
        Vector3 startPoint = firePoint != null ? firePoint.position : transform.position;
        
        RaycastHit hit;
        Vector3 direction = (_attackTarget.transform.position - startPoint).normalized;
        
        if (Physics.Raycast(startPoint, direction, out hit, _weapon.AttackRange, ~0, QueryTriggerInteraction.Ignore))
        {
            // Check if what we hit is our actual target.
            if (hit.collider.transform.IsChildOf(_attackTarget.transform) || hit.collider.transform == _attackTarget.transform)
            {
                return true; // Clear shot
            }
        }
        return false;
    }

    public void Move(Vector3 destination)
    {
        _currentState = UnitState.Moving;
        _attackTarget = null; // Clear attack target on move
        _shipMotor.MoveTo(destination);
    }

    public void Attack(Health target)
    {
        if (target != null && !target.IsDead)
        {
            _attackTarget = target;
            _currentState = UnitState.Attacking;
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
    private void OnDrawGizmosSelected()
    {
        if (_weapon == null) return;

        // Draw Aggro Radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AggroRadius);

        // Draw Weapon Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _weapon.AttackRange);

        // Draw line of sight
        if (_attackTarget != null)
        {
            if (HasLineOfSight())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.magenta;
            }
            Gizmos.DrawLine(transform.position, _attackTarget.transform.position);
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
