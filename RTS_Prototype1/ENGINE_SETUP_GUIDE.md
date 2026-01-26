# Unity Engine Setup Guide: Part 1

This guide details how to configure the foundational scripts (`RTSCamera`, `FleetManager`, `ShipMotor`) in the Unity Editor.

---

## 1. Scene and Navigation Setup

1.  **Create a Plane for Water**:
    *   In the Hierarchy, go to `Create > 3D Object > Plane`.
    *   Rename it to `WaterSurface`.
    *   Set its `Scale` to something large, like `(50, 1, 50)`.
    *   **Crucially**, set its `Layer` to **Layer 6: Water**. If you haven't created this layer yet, go to `Edit > Project Settings > Tags and Layers` to add it.

2.  **Bake the NavMesh**:
    *   Open the AI > Navigation window (`Window > AI > Navigation`).
    *   Select the `WaterSurface` object. In the Inspector, go to the `Object` tab and check **`Navigation Static`**.
    *   In the Navigation window, go to the `Bake` tab.
    *   Click the **`Bake`** button. You should see the plane turn blue in the Scene view, indicating a walkable surface.

---

## 2. Camera Configuration

1.  **Position the Main Camera**:
    *   Select the `Main Camera` in the Hierarchy.
    *   Set its `Transform` to the values specified in the project overview:
        *   **Position**: `(X: 0, Y: 50, Z: -50)`
        *   **Rotation**: `(X: 50, Y: 0, Z: 0)`
    *   Ensure its `Projection` is set to **`Perspective`**.

2.  **Attach the Camera Script**:
    *   With the `Main Camera` selected, drag the `Assets/Scripts/RTSCamera.cs` script onto it in the Inspector.
    *   Adjust the `Pan Limit` and `Scroll` settings as desired. A good starting `Pan Limit` might be `(X: 250, Y: 250)`.

---

## 3. FleetManager Setup

1.  **Create a Manager Object**:
    *   Create an empty GameObject in the Hierarchy (`Create > Create Empty`).
    *   Rename it to `_FleetManager`.
    *   Drag the `Assets/Scripts/FleetManager.cs` script onto it.

2.  **Assign Layers**:
    *   In the Inspector for `_FleetManager`, you will see two unassigned layer masks on the `Fleet Manager` component.
    *   Set the **`Ship Layer`** to **Layer 7: Units**.
    *   Set the **`Water Layer`** to **Layer 6: Water**.
    *   *(You may need to add the "Units" layer in `Project Settings > Tags and Layers` if you haven't already)*.

---

## 4. Ship Prefab Creation

This is the most important part. You will create a reusable "template" for your ships.

1.  **Create the Base Ship**:
    *   Create a basic 3D object like a Capsule or Cube (`Create > 3D Object > Capsule`).
    *   Rename it `CoastGuardCutter`.
    *   Set its `Layer` to **Layer 7: Units**.

2.  **Add the `ShipMotor` Script**:
    *   Drag the `Assets/Scripts/ShipMotor.cs` script onto `CoastGuardCutter`.

3.  **Add and Configure `NavMeshAgent`**:
    *   The `ShipMotor` script automatically adds a `NavMeshAgent`.
    *   Adjust the agent's properties for ship-like movement. Good starting values:
        *   **`Speed`**: 10
        *   **`Angular Speed`**: 120 (This is key for turning)
        *   **`Acceleration`**: 8
        *   **`Stopping Distance`**: 2
    *   Ensure the `NavMeshAgent`'s `Agent Type` corresponds to the NavMesh you baked.

4.  **Create the Selection Ring**:
    *   Right-click on `CoastGuardCutter` in the Hierarchy and choose `3D Object > Quad`.
    *   Rename this new object to `SelectionRing`.
    *   Zero out its `Position` so it's at the base of the ship.
    *   Set its `Rotation` to `(X: 90, Y: 0, Z: 0)` so it lies flat.
    *   Scale it to be slightly larger than the ship model (e.g., `(2, 2, 2)`).
    *   **Disable it by default** by unchecking the box next to its name in the Inspector.
    *   *Optional: Create a simple, unlit material with a circle texture (or just a green/blue color) and assign it to the `SelectionRing`'s `MeshRenderer`.*

5.  **Assign the Selection Ring**:
    *   Select the parent `CoastGuardCutter` object.
    *   In the `Ship Motor` component, drag the `SelectionRing` GameObject from the Hierarchy into the `Selection Ring` field.

6.  **Create the Prefab**:
    *   Drag the configured `CoastGuardCutter` GameObject from your Hierarchy into the `Assets` folder (or a new `Assets/Prefabs` folder). This creates the prefab.
    *   You can now delete the `CoastGuardCutter` from your scene and drag multiple instances of the prefab back in.

---

With these steps completed, you can enter Play mode. You should be able to pan and zoom the camera, left-click to select ships (which will show the selection ring), and right-click on the water to move them.
