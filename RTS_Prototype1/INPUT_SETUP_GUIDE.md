# Unity New Input System Setup Guide

This guide details how to configure the `InputSystem_Actions.inputactions` asset and integrate it with your `FleetManager.cs` script.

---

## 1. Locate and Configure Input Action Asset

1.  **Find the Asset**:
    *   In the `Assets` folder of your Unity project, locate the `InputSystem_Actions.inputactions` file.

2.  **Generate C# Class**:
    *   Select the `InputSystem_Actions.inputactions` asset.
    *   In the Inspector window, check the **`Generate C# Class`** checkbox.
    *   Ensure the `Class Name` is `InputSystemActions` (or your preferred name, but make a note of it).
    *   Click **`Apply`**. This will generate a C# script (e.g., `InputSystemActions.cs`) that provides an API to interact with your defined actions.

---

## 2. Set Up a Player Input Component

The `Player Input` component acts as the bridge between the physical input devices and your Input Action Asset.

1.  **Create an Input GameObject**:
    *   In your Hierarchy, create a new empty GameObject and name it `_InputManager`. This will hold the `Player Input` component.

2.  **Add `PlayerInput` Component**:
    *   With `_InputManager` selected, click `Add Component` in the Inspector and search for `Player Input`.
    *   Drag your `InputSystem_Actions.inputactions` asset from the Project window into the `Actions` field of the `Player Input` component.

3.  **Behavior Setting**:
    *   For direct C# access, set the `Behavior` dropdown to **`Invoke Unity Events`**. This is often the most flexible for scripting, though `Send Messages` or `Broadcast Messages` are also options depending on your project's needs. We will be using direct access in the `FleetManager`.

---

## 3. Configure Input Actions for FleetManager

Now that the C# class is generated, we can modify the `FleetManager.cs` script to use these new actions.

*(This step involves code modification which will be performed by the AI in the next turn.)*

1.  **Define Actions in Code**:
    *   In your `FleetManager.cs` script, you will need to:
        *   Instantiate the generated `InputSystemActions` class.
        *   Enable the desired action maps (e.g., "Player").
        *   Subscribe to the events of the specific input actions (e.g., "LeftClick", "RightClick").

2.  **Implement Callbacks**:
    *   Create methods in `FleetManager.cs` that will be called when the input actions are performed. These methods will contain the logic for selecting and moving ships.

---

Once `FleetManager.cs` is updated, your input system will be fully integrated using Unity's new Input System package.
