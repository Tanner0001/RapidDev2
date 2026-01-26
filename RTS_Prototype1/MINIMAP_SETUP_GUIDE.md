# Minimap Implementation Guide

This guide outlines the final steps to integrate the minimap into your Unity project.

## 1. Run Editor Scripts

Two editor scripts have been created to automate the setup process. You can run them from the Unity Editor's main menu:

-   **RTS > Create Minimap Prefab**: This will create a `Minimap.prefab` in the `Assets/prefabs/Minimap` folder. This prefab contains the necessary UI and camera setup for the minimap.
-   **RTS > Create Minimap Icon Data Assets**: This will create three `ScriptableObject` assets in the `Assets/Data/Minimap` folder: `PlayerIconData.asset`, `EnemyIconData.asset`, and `CivilianIconData.asset`. These assets will store the settings for the minimap icons.

## 2. Assign Sprites

You will need to create or import your own sprites for the different ship types. Once you have your sprites, you need to assign them to the `MinimapIconData` assets:

1.  Select `PlayerIconData.asset` in the `Assets/Data/Minimap` folder.
2.  In the Inspector, drag your player ship sprite to the **Icon** field.
3.  Repeat for `EnemyIconData.asset` and `CivilianIconData.asset` with their respective sprites.

## 3. Add Minimap to the Scene

1.  Drag the `Minimap.prefab` from the `Assets/prefabs/Minimap` folder into your scene hierarchy.

## 4. Assign Minimap to UIManager

1.  Select the `_UIManager` GameObject in your scene hierarchy.
2.  In the Inspector, find the `UIManager` component.
3.  Drag the `Minimap` GameObject from your scene hierarchy into the **Minimap** field.

## 5. Add MinimapIcon to Ships

For each of your ship prefabs, you need to add the `MinimapIcon` component:

1.  Open your ship prefab (e.g., `CoastGuardCutter`, `BadGuy`, `Civilian`).
2.  Click **Add Component** and search for `MinimapIcon`.
3.  In the `MinimapIcon` component, drag the corresponding `MinimapIconData` asset (e.g., `PlayerIconData`) from the `Assets/Data/Minimap` folder to the **Icon Data** field.

Once these steps are completed, the minimap should be fully functional in your game.
