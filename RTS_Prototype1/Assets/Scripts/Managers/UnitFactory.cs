using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public static UnitFactory Instance { get; private set; }

    [SerializeField]
    private Transform spawnPoint;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool TryBuildUnit(UnitData unitData)
    {
        if (GameManager.Instance.SpendCredits(unitData.cost))
        {
            if (spawnPoint == null)
            {
                Debug.LogError("UnitFactory has no spawnPoint assigned!", this);
                // Refund credits if spawn fails
                GameManager.Instance.AddCredits(unitData.cost); 
                return false;
            }

            Instantiate(unitData.unitPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Built {unitData.unitName} for {unitData.cost} credits.");
            return true;
        }
        else
        {
            Debug.Log($"Not enough credits to build {unitData.unitName}.");
            return false;
        }
    }
}
