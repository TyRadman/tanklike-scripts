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
|--- LevelGeneration
|--- MainMenu
|--- Misc
|--- PlayTest
|--- PoolingSystem
|--- Snippets
|--- Sound
|--- Testing
|--- UI
|--- UnitControllers
|--- Utils
```

## Key Systems and Implementations
### Gameplay Systems
- **TankController.cs** – Handles player movement, aiming, and shooting.
- **EnemyAI.cs** – Governs enemy behavior, including pathfinding and attack patterns.
- **Projectile.cs** – Handles bullet movement, collision, and damage calculations.
- **HealthSystem.cs** – Manages health and damage interactions.

### Rogue-like Mechanics
- **UpgradeSystem.cs** – Implements skill progression and ability scaling.
- **RandomLevelGenerator.cs** – Procedural level generation for replayability.
- **ItemDrops.cs** – Handles enemy loot drops and power-ups.

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
