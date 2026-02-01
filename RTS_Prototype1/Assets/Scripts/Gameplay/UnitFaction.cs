using UnityEngine;

public class UnitFaction : MonoBehaviour
{
    [SerializeField] private Faction unitFactionType;
    public Faction UnitFactionType => unitFactionType;
}
