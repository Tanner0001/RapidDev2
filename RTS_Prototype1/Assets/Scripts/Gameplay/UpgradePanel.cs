using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public void OnUpgradeHealthClicked()
    {
        PlayerUnit selectedUnit = SelectionManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            selectedUnit.UpgradeHealth();
        }
    }

    public void OnUpgradeFireRateClicked()
    {
        PlayerUnit selectedUnit = SelectionManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            selectedUnit.UpgradeFireRate();
        }
    }

    public void OnUpgradeSpeedClicked()
    {
        PlayerUnit selectedUnit = SelectionManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            selectedUnit.UpgradeMoveSpeed();
        }
    }
}
