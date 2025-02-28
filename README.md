# Tanklike – Code Documentation

## Overview
**Tanklike** is a top-down shooter rogue-like tank game developed in **Unity** using **C#**. The game is inspired by *Battle City*, it expands on strategic tank combat with procedural mechanics and more advanced gameplay.

## Role & Responsibilities
- **Gameplay Engineer** – Developed movement, aiming, shooting, and combat mechanics.
- **Tools Engineer** – Built custom in-editor tools to speed up level design.
- **Systems Designer** – Designed modular mechanics for scalable gameplay.
- **Tech Artist** – Implemented shaders and visual effects.
- **Game Designer** – Co-designed progression and player experience.

## Technologies Used
<a href="https://github.com/TyRadman/tanklike-scripts/">
<p>
  <img src="https://img.shields.io/badge/Unity-100000?logo=unity&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Blender-F5792A?logo=blender&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Adobe%20Photoshop-31A8FF?logo=adobephotoshop&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Visual%20Studio-5C2D91?logo=visualstudio&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Audacity-0000CC?logo=audacity&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Notion-000000?logo=notion&logoColor=white&style=for-the-badge" height="40">
<img src="https://img.shields.io/badge/Trello-0052CC?logo=trello&logoColor=white&style=for-the-badge" height="40">
</p>
</a>

- **Game Engine**: Unity  
- **Programming Language**: C#  
- **Tools & Software**:  
  - **Adobe Photoshop** – UI and texture work  
  - **Blender** – 3D modeling  
  - **GitHub** – Version control  
  - **Visual Studio** – Development environment  
  - **Audacity** – Audio editing  
  - **Notion & Trello** – Project management  

## Coding Standards
This project follows a structured coding standard for better maintainability and clarity across all team members. For detailed guidelines, see the [Coding Standards document](http://www.example.com). // FILL THIS

## Repository Structure
This repository only contains scripts. No assets, prefabs, or scenes are included.

<details>
  <summary>Show repo structure</summary>

```graphql
Scripts/
|--- Combat
|   |--- Abilities
|   |--- Ammunition
|   |--- Destructibles
|   |--- Editor
|   |--- Elements
|   |--- ShotsConfigurations
|   |--- Tools
|--- Editor
|   |--- CustomSearcher
|   |--- GameEditor
|   |--- Inspector
|   |--- RoomPainter
|--- EditorTools
|--- Environment
|   |--- Collectables
|   |--- Editor
|   |--- Interactables
|   |--- Items
|   |--- LevelGenerator
|   |   |--- LevelPainter
|   |   |--- MapMaker
|   |--- Props
|   |--- Rooms
|   |--- Shops
|--- Global
|   |--- Attributes
|   |--- Bosses
|   |--- Camera
|   |--- Cheats
|   |--- Editor
|   |--- Enemies
|   |--- Input
|   |--- Interfaces
|   |--- LoadingScreen
|   |--- MainMenu
|   |--- Others
|   |--- Players
|   |--- PoolingSystem
|   |--- Reports
|   |--- SceneControllers
|   |--- Screen
|--- Misc
|--- PlayTest
|--- Snippets
|--- Sound
|--- Testing
|   |--- LevelDesign
|   |--- Navigation
|   |--- Playground
|--- UI
|   |--- _Common
|   |--- AbilitySelection
|   |--- HealthBars
|   |--- HUD
|   |   |--- DamageScreen
|   |   |--- HealthBar
|   |   |--- Notifications
|   |   |--- OffScreenIndicators
|   |--- InGame
|   |   |--- DamagePopUp
|   |   |--- Crosshair
|   |--- Inputs
|   |--- Inventory
|   |--- MiniMap
|   |--- PauseMenu
|   |--- Shop
|   |--- SkillTree
|   |--- Tutorial
|--- UnitControllers
|   |--- BaseClasses
|   |   |--- TankParts
|   |   |--- SpecialPartsAnimations
|   |--- Bosses
|   |--- Core
|   |--- Editor
|   |--- Enemies
|   |--- MiniPlayers
|   |--- Players
|   |   |--- Crosshair
|   |   |--- Indicators
|   |   |--- MiniTankPlayer
|   |   |--- PlayerModifiableStats
|   |   |--- Vanguard
|   |--- Summons
|--- Utils
```
</details>


## Key Systems and Implementations
### Gameplay Systems
- **Unit Controller** – A component that governs all the components needed to control a unit in the game. It handles the unit's shooting, movement, visuals, stats, controls, constraints, and more.
- **Enemy AI** – Implemented using a [custom Behavior Tree Framework](https://github.com/TyRadman/BehaviorTree) that I developed. It's a decoupled system from the game's systems, which makes it easy to implement and change enemy behavior in run-time. 
- **Ammunition** – Handles projectile, rocket, and laser movement, collision, and damage calculations. These ammunition types have different modifiers in the form of `scriptable objects` that makes it easy to change their behaviors. For example, projectiles have an `OnImpact` modifier which can a `OneTargetModifier`, `AOEModifier`, `PiercingModifer`, `SpawnProjectileModifier`, and `MultipleImpactModifier` which can hold multiple modifier references.
- **Health System** – Manages health and damage interactions. Every method in the system that modifies an entity's health, calls `OnHealthChanged(current, previous, max)` event, which triggers UI changes, visual effects on the entity, and any other listeners to the event.

### Rogue-like Mechanics
- **Room Painters** –  A system that filters a room's tiles based on provided rules, such as identifying corners for crates or strategic placement for enemies where obstacles, other enemies placement and the players' positioning is taken into account.
- **Level Generator** – While specific rooms layouts are premade, the build of the overall map is procedural. The system is given a `Level Data` which holds the following:

|Data|Description|
|-|-|
|`String` LevelName | The name of the level.|
|`MapTileStyler` Styler| Holds data related to how the level generated should be styled. It determines the assets and textures to be used in the level generation process.|
|`MapTiles_SO` BossRoom | The map asset for the boss room, as different bosses will have different room layouts.|
|`BossRoomGate` BossGate | A reference to the prefab of the boss room gate. |
|`BossData` BossData | A Scriptable Object that holds data about the boss of the level, including the prefab and stats of the boss.|
|`Audio` LevelMusic | The background music to play in the level.|
|`List<MapTiles_SO>` MapsPool | A list of premade map assets that should be generated. The map asset holds an array of enum values representing the type of tile, and `x, y` coordinates to represent the tile's position on the room's map.|
|`Vector2Int` DroppersRange | The number of dropper instances that should be spawned in a room (such as crates or rocks).|
|`float` CratesToRockChance | The ratio between spawning a crate vs a rock for a dropper, with the crate being more valuable than the rock.|
|`Vector2Int` ExplosivesRange | The number of explosive props that should be placed in a room. Explosives are props that expode upon impact by either the players or enemies -- they deal damage to both parties.|
|`Vector2Int` DestructibleWallsRange | The number of destructible walls in a room room.|
|`List<WaveData>` Waves| Enemy waves that should be spawned in the level.|
|`ParticleSystem` WeatherVFX| The weather particle effects that should play in the level.|
- **Skill Tree** – Handles the players' in-run upgrades and progression. Unlike most of the Rogue-likes where players are given random options for upgrades or loot, in Tanklike, the player is given more agency over the type of upgrades they want to go for. The specific upgrade, of course, will be the player's choice from a random set of two options. For that, the UI representation of the skill tree has to be procedural and built and updated based on the player character at run-time which is what this system handles.
![](Tanklike-SkillTree.gif)


### Tools & Utilities
- **MapMaker.cs** – A custom in-editor level design tool for quick iteration.
- **DebugTools.cs** – Provides debugging utilities for performance monitoring.

### Visual & Effects
- **ShaderManager.cs** – Manages custom shaders for visual effects.
- **PostProcessingController.cs** – Controls post-processing effects to enhance visuals.

## Additional Notes
- This repository contains **code documentation**, not a public source code repository.
- For more details and some gameplay footage, visit my [Portfolio](https://tyradman.github.io/static-portfolio/) or view the [showcase video on Youtube](https://www.youtube.com/watch?v=EbcFn5lR5Ao&ab_channel=TyRadman).
- If you're interested in playing the game, **download the latest release from [Releases](https://github.com/TyRadman/Tanklike/releases).**

---
