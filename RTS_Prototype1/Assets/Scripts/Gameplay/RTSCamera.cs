using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCamera : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;
    [SerializeField] private Vector2 panLimit;

    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 120f;

    private InputSystem_Actions _inputSystemActions;

    void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _inputSystemActions.Enable();
    }

    void OnDisable()
    {
        _inputSystemActions.Disable();
    }

    void Update()
    {
        Vector3 initialPos = transform.position;
        Vector3 pos = initialPos;

        // Read pan input directly every frame for continuous movement
        Vector2 panInput = _inputSystemActions.Player.Move.ReadValue<Vector2>();

        // Keyboard Panning
        if (panInput != Vector2.zero)
        {
            Vector3 move = new Vector3(panInput.x, 0, panInput.y);
            pos += move * panSpeed * Time.deltaTime;
        }

        // ... (previous code unchanged)

        // Mouse Edge Panning
        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (mousePosition.y >= Screen.height - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (mousePosition.y <= panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (mousePosition.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
        }

        // Zooming - Read value directly and use Mathf.Sign to normalize it
        float scrollInput = _inputSystemActions.Player.Zoom.ReadValue<float>();
        
        // ... (rest of the script is unchanged)
        if (scrollInput != 0)
        {
            // Use Sign to get only the direction (1 or -1), not the large raw value
            float zoomDirection = Mathf.Sign(scrollInput);
            pos.y -= zoomDirection * scrollSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        if (pos != initialPos)
        {
            transform.position = pos;
        }
    }
}
