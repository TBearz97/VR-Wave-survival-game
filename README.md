# VR Wave Survival Game

A Unity 6 VR survival prototype focused on real-time combat,
terrain-aware spawning, and performance-conscious enemy management.

## Summary

This project is a first-person VR wave survival game built using Unity's
XR stack. The player navigates a terrain-based environment, manages
weapons with dual-hand interaction, and survives against continuously
spawning enemies while upgrading their abilities.

The implementation emphasizes **VR interaction fidelity**, **runtime
performance**, and **modular gameplay systems**.

## Key Features

-   VR locomotion using headset-relative movement and joystick input\
-   Dual-hand weapon interaction via XR Interaction Toolkit\
-   Upgrade system (health, damage, fire rate, accuracy, magazine size)\
-   Terrain-based spawning and NavMesh-aware positioning\
-   Enemy object pooling to reduce runtime allocation overhead\
-   Tower health system introducing a defend objective\
-   VR pause and inventory interaction system

## Technical Highlights

**XR Stack Integration** - OpenXR + Oculus XR support - XR Interaction
Toolkit for controller-based interaction - Unity Input System for action
mapping

**Performance-Oriented Design** - Enemy pooling system avoids costly
instantiation/destruction cycles - Active/inactive container pattern for
scene organization - Terrain-aware spawning reduces invalid placement
checks

**Gameplay Architecture** - `Player.cs` centralizes player state,
upgrades, and interaction flow\
- `TerrainEnvironmentSpawner.cs` handles environment and enemy
lifecycle\
- `Tower.cs` introduces an additional gameplay constraint (defense
objective)\
- Utility scripts support NavMesh obstacle baking from terrain data

## Tech Stack

-   **Engine:** Unity 6 (6000.0.43f1)\
-   **Language:** C#\
-   **Rendering:** Universal Render Pipeline (URP)\
-   **XR:** OpenXR, Oculus XR, XR Interaction Toolkit\
-   **Input:** Unity Input System\
-   **Navigation:** Unity NavMesh / AI Navigation

## Project Structure

    Assets/Main/
    ├── Scenes/          # Game and menu scenes
    ├── Scripts/         # Core gameplay systems
    ├── Guns/            # Weapon assets and logic
    ├── Zombie/          # Enemy assets

## Running the Project

1.  Clone the repository

        git clone https://github.com/TBearz97/VR-Wave-survival-game.git

2.  Open in Unity **6000.0.43f1**

3.  Load:

        Assets/Main/Scenes/Main 1.unity

4.  Ensure XR is configured for your headset (OpenXR recommended)

5.  Play in editor or build to device

## What This Project Demonstrates

-   Practical use of Unity's modern XR pipeline\
-   Ability to design gameplay systems with performance constraints in
    mind\
-   Understanding of VR-specific interaction patterns\
-   Experience structuring medium-scale Unity projects

## Known Limitations

-   Wave system is implicit (spawn-based) rather than formally staged\
-   Limited enemy variety and AI complexity\
-   Minimal UI/UX polish\
-   Some systems (e.g., spawn manager) are not fully utilized

## Next Steps

-   Formalize wave progression and difficulty scaling\
-   Expand enemy behaviors and combat depth\
-   Improve VR UI/UX and feedback systems\
-   Add audio and game feel polish\
-   Refactor into clearer system boundaries where needed

## Author

-   GitHub: https://github.com/TBearz97
