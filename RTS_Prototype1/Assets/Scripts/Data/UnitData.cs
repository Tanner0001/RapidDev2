using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New UnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName;
    public Sprite icon;
    
    [Header("Production")]
    public GameObject unitPrefab;
    public int cost;
}
