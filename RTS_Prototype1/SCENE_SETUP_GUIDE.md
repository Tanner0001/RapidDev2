# Naval RTS Prototype - Scene Setup Guide

This guide details how to set up the main game scene in Unity using the provided scripts.

## 1. Layers and Tags

First, ensure the following Layers and Tags are configured in `Edit > Project Settings > Tags and Layers`.

### Layers:
- **Ground:** Used for the water/ground plane for movement commands.
- **Selectable:** Used for player units and any other object that can be clicked on.

### Tags:
- **Player:** For player-controlled unit prefabs.
- **Enemy:** For enemy AI unit prefabs.
- **CargoShip:** For the allied NPC cargo ship prefabs.

## 2. Manager Objects

Create empty GameObjects in your scene for each manager. It's best practice to have these as singletons that persist through the scene.

- **`@GameManager`**:
    - Attach the `GameManager.cs` script.
    - Configure starting credits and nation health in the Inspector.

- **`@UIManager`**:
    - Attach the `UIManager.cs` script.
    - Create a UI Canvas (`GameObject > UI > Canvas`).
    - Inside the Canvas, create the following UI elements and link them to the `UIManager` inspector fields:
        - **`CreditsText`**: A `TextMeshPro - Text` element.
        - **`NationHealthText`**: A `TextMeshPro - Text` element.
        - **`UpgradePanel`**: A `Panel` UI object.
            - Attach the `UpgradePanel.cs` script to this panel.
            - Inside the panel, create three `Button` elements for Health, Fire Rate, and Speed.
            - For each button, in the `OnClick()` event in the Inspector:
                - Drag the `UpgradePanel` GameObject into the object field.
                - From the dropdown, select `UpgradePanel > OnUpgradeHealthClicked()` (or the corresponding method for the other buttons).
        - **`GameOverPanel`**: A `Panel` with a "Game Over" message. Initially, set it to inactive.

- **`@SelectionManager`**:
    - Attach the `SelectionManager.cs` script.
    - In the Inspector, set:
        - `Selectable Layer` to the **Selectable** layer.
        - `Ground Layer` to the **Ground** layer.

- **`@Spawner`**:
    - Attach the `Spawner.cs` script.
    - Create empty GameObjects in the scene to act as spawn points.
    - In the Inspector, link the following:
        - `Cargo Ship Prefabs` and `Enemy Ship Prefabs` (see Prefab setup below).
        - `Cargo Spawn Points`: The transforms where cargo ships will appear.
        - `Home Port`: A transform representing the destination for cargo ships.
        - `Enemy Spawn Points`: Transforms where enemies will appear (typically at the edges of the map).

## 3. Prefab Setup

You will need to create prefabs for your player, enemy, and cargo ships.

### Base Components (for all ships):
- A root GameObject with a `NavMeshAgent`.
- A 3D model as a child.
- `ShipMotor.cs` script.
- `Health.cs` script.
- `UnitFaction.cs` script.
- A `Capsule Collider` or other collider.

### Player Unit Prefab (`CoastGuardCutter`):
- **Tag**: `Player`
- **Layer**: `Selectable`
- **`UnitFaction`**: Set to `Player`.
- **Add Components**:
    - `PlayerUnit.cs`
    - `Weapon.cs`
        - **Projectile Prefab**: Link a projectile prefab (see below).
        - **Fire Point**: An empty child GameObject marking where projectiles spawn from.
        - **Owner Faction**: `Player`.
- **`ShipMotor`**: Link the `selectionRing` GameObject (a simple ring mesh or UI element that appears under the ship when selected).

### Enemy Unit Prefab (`BadGuy`):
- **Tag**: `Enemy`
- **Layer**: `Selectable`
- **`UnitFaction`**: Set to `Enemy`.
- **Add Components**:
    - `EnemyAI.cs`
    - `Weapon.cs`
        - **Projectile Prefab**: Link a projectile prefab.
        - **Fire Point**: A fire point transform.
        - **Owner Faction**: `Enemy`.

### Cargo Ship Prefab (`Civilian`):
- **Tag**: `CargoShip`
- **Layer**: `Selectable`
- **`UnitFaction`**: Set to `Civilian`.
- **Add Components**:
    - `CargoShipAI.cs`
- **`Health`**: Set `Max Health` to a low value (e.g., 25).

### Projectile Prefab:
- A simple 3D object (e.g., a sphere or capsule).
- `Projectile.cs` script.
- `Rigidbody` component (set to `Is Kinematic`).
- `Collider` (set to `Is Trigger`).

## 4. Scene Environment

- **Water/Ground**: A large plane with the `Ground` layer assigned.
- **NavMesh**: You must bake a NavMesh for ship movement (`Window > AI > Navigation`). Ensure the water/ground plane is marked as `Navigation Static`.
- **Camera**: Ensure your main camera has a `Physics Raycaster` component to interact with the UI.

Once these steps are complete, you can start the scene. The `Spawner` will begin creating ships, and the player can select their units and issue commands.
