
using UnityEngine;
using UnityEngine.InputSystem;

public class FleetManager : MonoBehaviour
{
    [SerializeField] private LayerMask shipLayer;
    [SerializeField] private LayerMask waterLayer;

    private ShipMotor _selectedShip;
    private Camera _mainCamera;
    private InputSystem_Actions _inputSystemActions;
    private UIManager _uiManager;

    void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("FleetManager: Main Camera not found! Ensure your camera is tagged 'MainCamera'.");
        }
    }

    void Start()
    {
        _uiManager = UIManager.Instance;
        if (_uiManager == null)
        {
            Debug.LogError("FleetManager: UIManager instance not found in the scene!", this);
        }
    }

    void Update()
    {
        if (_selectedShip != null && _uiManager != null)
        {
            _uiManager.UpdateSelectedShipStatus(_selectedShip.name, _selectedShip.CurrentState);
        }
    }

    void OnEnable()
    {
        _inputSystemActions.Enable();
        _inputSystemActions.Player.LeftClick.performed += OnLeftClick;
        _inputSystemActions.Player.RightClick.performed += OnRightClick;
    }

    void OnDisable()
    {
        _inputSystemActions.Player.LeftClick.performed -= OnLeftClick;
        _inputSystemActions.Player.RightClick.performed -= OnRightClick;
        _inputSystemActions.Disable();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        if (Mouse.current == null) return; // Ignore if no mouse is present

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Raycast only for selectable ships (player ships)
        if (Physics.Raycast(ray, out hit, 1000, shipLayer))
        {
            Debug.Log($"[FleetManager] Left-click hit '{hit.collider.name}' on layer '{LayerMask.LayerToName(hit.collider.gameObject.layer)}'.");
            ShipMotor ship = hit.collider.GetComponent<ShipMotor>();
            if (ship != null)
            {
                SelectShip(ship);
            }
        }
        else
        {
            // If we click something other than a selectable ship, deselect
            Debug.Log("[FleetManager] Left-click did not hit a selectable ship. Deselecting.");
            DeselectShip();
        }
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (_selectedShip == null)
        {
            Debug.Log("[FleetManager] Right-click ignored: No ship selected.");
            return;
        }

        if (Mouse.current == null) return; // Ignore if no mouse is present

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            // Check if we hit a ship with an identity (an NPC)
            ShipIdentity targetIdentity = hit.collider.GetComponent<ShipIdentity>();
            if (targetIdentity != null)
            {
                // Check if the target is already revealed and friendly
                if (targetIdentity.IsRevealed && targetIdentity.RealIdentity == ThreatLevel.Green)
                {
                    Debug.Log($"[FleetManager] Target {targetIdentity.name} is already identified as friendly (Green). No action taken.");
                    return; // Do not issue an intercept order
                }

                // It's an NPC ship, issue a scan command
                Debug.Log($"[FleetManager] Issuing SCAN command to {_selectedShip.name} for target {targetIdentity.name}.");
                _selectedShip.Scan(targetIdentity.transform);
            }
            // Check if we hit the water layer
            else if (waterLayer == (waterLayer | (1 << hit.collider.gameObject.layer)))
            {
                // It's water, issue a move command
                Debug.Log($"[FleetManager] Issuing MOVE command to {_selectedShip.name} at position {hit.point}.");
                _selectedShip.MoveTo(hit.point);
            }
        }
        else
        {
            Debug.Log("[FleetManager] Right-click did not hit any valid layer (Water or Ship).");
        }
    }
    
    // ... (rest of the script is unchanged)
    
    void SelectShip(ShipMotor ship)
    {
        if (_selectedShip != null)
        {
            _selectedShip.Deselect();
        }
        _selectedShip = ship;
        _selectedShip.Select();
        Debug.Log($"[FleetManager] Ship selected: {ship.name}");
    }

    void DeselectShip()
    {
        if (_selectedShip != null)
        {
            _selectedShip.Deselect();
            Debug.Log($"[FleetManager] Ship deselected: {_selectedShip.name}");
        }
        _selectedShip = null;
        
        if (_uiManager != null)
        {
            _uiManager.ClearSelectedShipStatus();
        }
    }
}

