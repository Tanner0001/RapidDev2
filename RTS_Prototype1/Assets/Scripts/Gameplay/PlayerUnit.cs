using UnityEngine;
using System;

[RequireComponent(typeof(Health), typeof(Weapon), typeof(ShipMotor))]
public class PlayerUnit : MonoBehaviour
{
    public event Action OnStatsChanged;

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
            OnStatsChanged?.Invoke();
        }
        else
        {

        }
    }

    public void UpgradeFireRate()
    {
        if (GameManager.Instance.SpendCredits(fireRateUpgradeCost))
        {
            _weapon.IncreaseFireRate(fireRateUpgradePercent);
            OnStatsChanged?.Invoke();
        }
        else
        {

        }
    }

    // NOTE: ShipMotor does not have an upgrade method yet. This will be added.
    public void UpgradeMoveSpeed()
    {
        if (GameManager.Instance.SpendCredits(speedUpgradeCost))
        {
            _shipMotor.IncreaseSpeed(speedUpgradeAmount);
            OnStatsChanged?.Invoke();
        }
        else
        {

        }
    }
}

