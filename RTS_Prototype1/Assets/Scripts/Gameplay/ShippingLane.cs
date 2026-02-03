using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShippingLane
{
    public string name; // Added for identification and debugging
    public List<Transform> Waypoints = new List<Transform>();
}
