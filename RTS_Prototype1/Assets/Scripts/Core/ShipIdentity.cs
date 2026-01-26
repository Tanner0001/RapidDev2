using UnityEngine;

// This script should be attached to NPC ship prefabs.
public class ShipIdentity : MonoBehaviour
{
    // Using the existing ThreatLevel enum from Enums.cs
    [Header("Identity")]
    [SerializeField] private ThreatLevel _realIdentity;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer shipMeshRenderer;

    private bool _isRevealed = false;

    void Awake()
    {
        if (shipMeshRenderer == null)
        {
            Debug.Log($"ShipIdentity on {name}: MeshRenderer was not assigned in inspector, attempting to find it on the GameObject.");
            shipMeshRenderer = GetComponent<MeshRenderer>();
            if (shipMeshRenderer == null)
            {
                Debug.LogError($"ShipIdentity on {name}: MeshRenderer component NOT FOUND!", this);
            }
            else
            {
                Debug.Log($"ShipIdentity on {name}: Found MeshRenderer component successfully.");
            }
        }
    }
    
    // This should be called by the TrafficManager on spawn
    public void InitializeIdentity(ThreatLevel realIdentity)
    {
        _realIdentity = realIdentity;
        Debug.Log($"ShipIdentity on {name}: Real identity initialized to: {_realIdentity}");
    }

    // Public property to safely access the real identity
    public ThreatLevel RealIdentity => _realIdentity;
    public bool IsRevealed => _isRevealed;

    // This is called by the player's ship when it gets close
    public void Reveal()
    {
        Debug.Log($"ShipIdentity on {name}: Reveal() method called.");

        if (_isRevealed)
        {
            return;
        }

        Debug.Log($"ShipIdentity on {name}: Revealing real identity as {_realIdentity}.");
        _isRevealed = true;
    }
}
