# ITU-CS-464-LAB-04-BSSE23040

Lab 04: player controller. The `Level_Warehouse` grey-box level from my Lab 3 project is exported as a Unity package, imported here, and given a player that you can walk through the level with a camera that follows it.

## What is in this repo

| What | Where |
|---|---|
| The exported level package (made from the Lab 3 project) | `LevelPackage/Level_Warehouse.unitypackage` |
| The level, imported from that package | `Assets/Scenes/Lab03/Level_Warehouse.unity` (with `Assets/Synty` and `Assets/Materials`) |
| **The scene to play** | `Assets/Scenes/Lab04_PlayerLevel.unity` |
| Player movement script | `Assets/Scripts/PlayerController.cs` |
| Camera that follows the player | `Assets/Scripts/ThirdPersonCamera.cs` |
| Goal trigger and on-screen hint | `Assets/Scripts/GoalTrigger.cs`, `Assets/Scripts/LevelHud.cs` |
| Screen recording of the level walk-through | `Walkthrough/Lab04_Walkthrough.mp4` |

## How to play

Open the project in Unity 6 (`6000.6.2f1`), open `Assets/Scenes/Lab04_PlayerLevel.unity` and press Play.

| Input | Action |
|---|---|
| W A S D / arrow keys / left stick | Move (relative to the camera) |
| Left Shift | Sprint |
| Space | Jump |
| Mouse | Turn the camera around the player |
| Esc | Free the mouse (click to lock it again) |

The player starts on the white pad at the south end. The light strip on the floor is the level's default route, and the flag on the upper floor is the goal. Reaching it shows "GOAL REACHED!" with your time.

## How it works

- **Player:** a 2 m tall capsule with a `CharacterController` and `PlayerController`. It moves relative to the camera, turns to face its direction of travel, and handles gravity, jumping, slopes and the stairs.
- **Camera:** `ThirdPersonCamera` follows the player from behind and slightly above with smoothing, orbits with the mouse, and moves closer when a wall is in the way.
- **Doorway:** the doorway piece in the Synty pack is only about 1 m wide, too narrow for the player, so in `Lab04_PlayerLevel` I replaced it with a 1.5 m by 2.5 m opening built from the pack's own wall piece (`Assets/Editor/Lab04Setup.cs`). The imported `Level_Warehouse` scene is untouched.
- **Walk-through video:** it was recorded in Play mode. `RouteAutopilot` feeds the same `PlayerController` along the level's default route, so the video shows the real movement code, camera and stairs. It stays off unless the recorder asks for it, so normal play uses the keyboard.
