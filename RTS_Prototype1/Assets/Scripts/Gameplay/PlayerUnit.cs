using UnityEngine;

[RequireComponent(typeof(Health), typeof(Weapon), typeof(ShipMotor))]
public class PlayerUnit : MonoBehaviour
{
    private Health _health;
    private Weapon _weapon;
    private ShipMotor _shipMotor;

    [Header("Upgrade Costs")]
    [SerializeField] private int healthUpgradeCost = 100;
    [SerializeField] private int fireRateUpgradeCost = 150;
    [SerializeField] private int speedUpgradeCost = 75;

    [Header("Upgrade Values")]
    [SerializeField] private float healthUpgradeAmount = 25f;
    [SerializeField] private float fireRateUpgradePercent = 0.1f; // 10%
    [SerializeField] private float speedUpgradeAmount = 5f;

    void Awake()
    {
        _health = GetComponent<Health>();
        _weapon = GetComponent<Weapon>();
        _shipMotor = GetComponent<ShipMotor>();
    }

    public void UpgradeHealth()
    {
        if (GameManager.Instance.SpendCredits(healthUpgradeCost))
        {
            _health.IncreaseMaxHealth(healthUpgradeAmount);
            Debug.Log($"{name} health upgraded!");
        }
        else
        {
            Debug.Log("Not enough credits for health upgrade.");
        }
    }

    public void UpgradeFireRate()
    {
        if (GameManager.Instance.SpendCredits(fireRateUpgradeCost))
        {
            _weapon.IncreaseFireRate(fireRateUpgradePercent);
            Debug.Log($"{name} fire rate upgraded!");
        }
        else
        {
            Debug.Log("Not enough credits for fire rate upgrade.");
        }
    }

    // NOTE: ShipMotor does not have an upgrade method yet. This will be added.
    public void UpgradeMoveSpeed()
    {
        if (GameManager.Instance.SpendCredits(speedUpgradeCost))
        {
            _shipMotor.IncreaseSpeed(speedUpgradeAmount);
            Debug.Log($"{name} speed upgraded!");
        }
        else
        {
            Debug.Log("Not enough credits for speed upgrade.");
        }
    }
}
