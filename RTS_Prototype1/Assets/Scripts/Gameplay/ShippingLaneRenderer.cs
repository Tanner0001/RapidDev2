using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ShippingLaneRenderer : MonoBehaviour
{
    public List<Transform> lanePoints = new List<Transform>(); // New field
    public Color laneColor = Color.yellow; // New field, previously from data
    public float laneWidth = 0.5f; // New field, previously from data
    public Material lineMaterial;

    private LineRenderer _lineRenderer;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            Debug.LogError("ShippingLaneRenderer: LineRenderer component is missing!", this);
            return;
        }

        if (lineMaterial == null)
        {
            Debug.LogWarning("ShippingLaneRenderer: Line Material is not assigned. Using default.", this);
        }
        else
        {
            _lineRenderer.material = lineMaterial;
        }

        RenderLane();
    }

    void OnValidate()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        RenderLane();
    }

    private void RenderLane()
    {
        if (lanePoints.Count < 2) // Using local lanePoints
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        List<Vector3> positions = new List<Vector3>();
        foreach (Transform pointTransform in lanePoints) // Using local lanePoints
        {
            if (pointTransform != null)
            {
                positions.Add(pointTransform.position);
            }
            else
            {
                Debug.LogWarning("ShippingLaneRenderer: A null Transform was found in lanePoints. Skipping this point.", this);
            }
        }

        if (positions.Count < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        _lineRenderer.positionCount = positions.Count;
        _lineRenderer.SetPositions(positions.ToArray());

        _lineRenderer.startColor = laneColor; // Using local color
        _lineRenderer.endColor = laneColor;
        _lineRenderer.startWidth = laneWidth; // Using local width
        _lineRenderer.endWidth = laneWidth;
        
        if (LayerMask.NameToLayer("ShippingLanes") != -1)
        {
            gameObject.layer = LayerMask.NameToLayer("ShippingLanes");
        }
        else
        {
            Debug.LogWarning("ShippingLaneRenderer: 'ShippingLanes' layer not found. Please create it in Project Settings -> Tags and Layers.", this);
        }
    }
}
