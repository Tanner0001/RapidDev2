using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float attackRange = 30f;
    [SerializeField] private float fireRate = 1f; // shots per second
    [SerializeField] private int damage = 10;
    
    [Header("Setup")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform _firePoint;
    public Transform firePoint => _firePoint;
    [SerializeField] private Faction ownerFaction; // To avoid friendly fire

    private float _fireCooldown = 0f;

    public float AttackRange => attackRange;

    private void Update()
    {
        if (_fireCooldown > 0)
        {
            _fireCooldown -= Time.deltaTime;
        }
    }

    public bool CanFire()
    {
        return _fireCooldown <= 0;
    }

    public void Fire(Health target)
    {


        if (CanFire())
        {
            _fireCooldown = 1f / fireRate;
            Debug.Log($"{name}: Firing at {target.name}!");

            // --- Debug ---
            Vector3 direction = (target.transform.position - firePoint.position).normalized;
            // Debug.DrawRay(firePoint.position, direction * attackRange, Color.red, 0.5f);
            // --- End Debug ---

            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                projectile.Initialize(target, damage, ownerFaction, gameObject);
            }

        }
    }

    public void SetOwnerFaction(Faction faction)
    {
        ownerFaction = faction;
    }

    public void IncreaseFireRate(float percentage)
    {
        // Increase fire rate by a percentage, e.g., 0.1 for 10%
        fireRate *= (1 + percentage);
    }
}
