using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildButton : MonoBehaviour
{
    [SerializeField]
    private UnitData unitToBuild;

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (unitToBuild == null)
        {
            Debug.LogError("BuildButton has no UnitData assigned!", this);
            return;
        }

        UnitFactory.Instance.TryBuildUnit(unitToBuild);
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
