using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private LayerMask groundLayer;

    private Camera _mainCamera;
    
    // Cache components of the selected unit
    private PlayerUnit _selectedUnit;
    private ShipMotor _selectedShipMotor;
    private PlayerCombat _selectedPlayerCombat;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // --- Left Click: Selection ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleSelection();
        }

        // --- Right Click: Action ---
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleAction();
        }
    }

    private void HandleSelection()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, selectableLayer))
        {
            if (hit.collider.TryGetComponent<PlayerUnit>(out PlayerUnit newSelection))
            {
                // We hit a player unit, select it
                Deselect();

                _selectedUnit = newSelection;
                _selectedShipMotor = newSelection.GetComponent<ShipMotor>();
                _selectedPlayerCombat = newSelection.GetComponent<PlayerCombat>(); // Cache the combat component
                
                _selectedShipMotor.Select();
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowUpgradePanel(_selectedUnit.gameObject);
                }
            }
        }
        else
        {
            // We clicked something else (or nothing), deselect
            Deselect();
        }
    }

    private void HandleAction()
    {
        if (_selectedUnit == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Check for an enemy to attack first
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, selectableLayer))
        {
            // Ensure we have a combat component to issue attack orders
            if (_selectedPlayerCombat != null && hit.collider.TryGetComponent<Health>(out Health enemyHealth) && hit.collider.GetComponent<UnitFaction>()?.UnitFactionType == Faction.Enemy)
            {
                Debug.Log($"SelectionManager: Issuing Attack command to {_selectedUnit.name} on target {enemyHealth.name}.");
                _selectedPlayerCombat.Attack(enemyHealth);
                return; // Action is to attack, so we're done
            }
        }
        
        // If not attacking, check for a point on the ground to move to
        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            if (_selectedPlayerCombat != null)
            {
                Debug.Log($"SelectionManager: Issuing Move command to {_selectedUnit.name} to {hit.point}.");
                _selectedPlayerCombat.Move(hit.point);
            }
            else
            {
                // Fallback for non-combat units, though player units should have PlayerCombat
                _selectedShipMotor.MoveTo(hit.point);
            }
        }
    }

    private void Deselect()
    {
        Debug.Log("SelectionManager: Deselecting unit.");
        if (_selectedUnit != null)
        {
            _selectedShipMotor.Deselect();
            _selectedUnit = null;
            _selectedShipMotor = null;
            _selectedPlayerCombat = null;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideUpgradePanel();
            }
        }
    }

    public PlayerUnit GetSelectedUnit()
    {
        return _selectedUnit;
    }
}
