# AI_INSTRUCTIONS.md

> **Instruction to AI:** Read this file before generating code to ensure consistency with the project architecture, camera perspective, and gameplay goals.

## 1. Project Overview
* **Genre:** Real-Time Strategy (RTS) / Simulation.
* **Theme:** Naval Coast Guard (Law Enforcement, Search & Rescue, Interdiction).
* **Perspective:** 3D Top-Down / Isometric (Perspective Camera angled ~60°).
* **Engine:** Unity (C#).
* **Core Loop:**
&nbsp;   1.  **Select** fleet ships.
&nbsp;   2.  **Patrol** designated shipping lanes.
&nbsp;   3.  **Intercept** unidentified radar contacts.
&nbsp;   4.  **Hail/Scan** to reveal "Ship Identity" (Civilian, Smuggler, Hostile).
&nbsp;   5.  **Engage** or release based on the reveal.

---

## 2. Technical Architecture

### A. The "Manager" Layer (Global Logic)
* **`GameManager`**: Singleton. Handles global game states (Peace, Combat, Game Over).
* **`SelectionManager`**: Handles all Mouse Input.
&nbsp;   * **Raycasting**: Uses LayerMasks to distinguish between "Water" (Movement commands) and "Ships" (Interaction commands).
&nbsp;   * **Box Select**: Draws a UI rectangle to select multiple `ShipMotor` units.
* **`TrafficManager`**: Procedural generation.
&nbsp;   * Spawns NPC ships at "Entry Nodes".
&nbsp;   * Despawns them at "Exit Nodes".
&nbsp;   * Assigns hidden `ThreatLevel` data upon spawn.

### B. The "Unit" Layer (Component Composition)
* **`ShipMotor`**:
&nbsp;   * **Component:** Requires `NavMeshAgent`.
&nbsp;   * **Role:** Handles movement, stopping, and rotation.
&nbsp;   * **Methods:** `MoveTo(Vector3)`, `SetPatrolRoute(List<Vector3>)`.
* **`ShipIdentity` (NPC)**:
&nbsp;   * **Role:** Holds the "Truth" about the vessel.
&nbsp;   * **Data:** `Faction` (Civilian/Enemy), `isUnknown` (bool).
&nbsp;   * **Visuals:** Handles changing materials/flags when `Reveal()` is called.
* **`SensorSuite` (Player)**:
&nbsp;   * **Role:** The "Hail" ability.
&nbsp;   * **Logic:** Checks distance to target. If `dist < range`, starts a Coroutine (Scan Timer). On complete, calls `target.Reveal()`.
* **`Billboard` (UI Helper)**:
&nbsp;   * **Role:** Forces World Space Canvases (Health/Icon) to face the Main Camera to prevent "flat" looking UI in 3D.
* **`RepairZone` (Gameplay Volume)**:
&nbsp;   * **Role:** A trigger volume that repairs any friendly `PlayerUnit` inside it over time.
&nbsp;   * **Logic:** Uses `OnTriggerEnter`/`OnTriggerExit` to track units. In `Update`, it applies healing to all units currently inside the zone.

---

## 3. Unity Implementation Details

### Physics & Layers
* **Layer 6: Water** (Baked NavMesh Surface).
* **Layer 7: Units** (Ships, obstacles).
* **Layer 8: UI** (World Space interfaces).

### Movement (NavMesh)
* **Strict Rule:** Never use `transform.Translate` or `Rigidbody.AddForce` for ship movement.
* **Standard:** Always use `NavMeshAgent.SetDestination(point)`.
* **Turning:** Configure `NavMeshAgent.angularSpeed` for realistic ship turning radii.

### Visuals
* **Camera:** Perspective Projection.
&nbsp;   * Position: `(0, 50, -50)`
&nbsp;   * Rotation: `(50, 0, 0)`
* **Feedback:** Use simple colored Rings (Projectors or Sprites) on the water surface to indicate selection.

---

## 4. Coding Standards

### Syntax & Style
* **Variables:** `camelCase` (e.g., `moveSpeed`).
* **Properties/Methods:** `PascalCase` (e.g., `RevealIdentity()`).
* **Private Fields:** Prefix with underscore (e.g., `_navMeshAgent`).
* **Serialization:** Use `[SerializeField] private` to expose variables to the Inspector while keeping them private in code.

### Required Data Structures
Use these Enums to maintain state consistency across scripts.

```csharp
public enum Faction 
{ 
&nbsp;   CoastGuard, 
&nbsp;   Civilian, 
&nbsp;   Smuggler, 
&nbsp;   HostileMilitary 
}

public enum UnitState 
{ 
&nbsp;   Idle, 
&nbsp;   Moving, 
&nbsp;   Patrolling, 
&nbsp;   Intercepting, // Moving toward a moving target
&nbsp;   Scanning      // Stationary, performing action
}

public enum ThreatLevel 
{ 
&nbsp;   Unknown, // Grey
&nbsp;   Green,   // Clear
&nbsp;   Yellow,  // Suspicious
&nbsp;   Red      // Hostile
}
```