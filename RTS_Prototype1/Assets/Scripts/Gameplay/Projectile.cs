using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Health _target;
    private int _damage;
    private Faction _ownerFaction;
    private GameObject _owner;

    [SerializeField] private float speed = 50f;
    [SerializeField] private GameObject hitEffect;

    public void Initialize(Health target, int damage, Faction ownerFaction, GameObject owner)
    {
        _target = target;
        _damage = damage;
        _ownerFaction = ownerFaction;
        _owner = owner;
    }

    void Update()
    {
        if (_target == null || _target.IsDead)
        {
            // If target is gone, just destroy the projectile
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        Vector3 direction = (_target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(_target.transform);

        // Check for collision (simple distance check)
        if (Vector3.Distance(transform.position, _target.transform.position) < 1.0f)
        {
            // Check target's faction to avoid friendly fire
            UnitFaction unitFaction = _target.GetComponent<UnitFaction>();
            if (unitFaction != null && unitFaction.UnitFactionType == _ownerFaction)
            {
                // Friendly fire, do nothing and self-destruct
                Destroy(gameObject);
                return;
            }

            _target.TakeDamage(_damage, _owner);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            SoundManager.Instance.PlayExplosionSound(transform.position);
            Destroy(gameObject);
        }
    }
}
