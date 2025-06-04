# The Ring Goes South | Godot Prototype

A tactical roguelike inspired by Lord of the Rings, focusing on party management, exploration, and turn-based combat, being developed with the Godot Engine.

This project is a migration and continuation of a LibGDX-based prototype.

## Platforms

- Desktop (Windows, Linux, macOS) via Godot Engine's export capabilities.
- Other platforms supported by Godot can be targeted in the future.

## Engine & Language

- **Engine:** Godot Engine (version 4.x recommended)
- **Primary Language:** C#
- **Secondary/Scripting:** GDScript may be used for simpler node interactions or where C# overhead is not necessary.
- **Performance-Critical Modules:** C++ via GDExtension could be explored for highly demanding systems if needed.

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with your Godot C# version, typically .NET 6 or newer for Godot 4)
- Godot Engine (version 4.x, with .NET/C# support enabled)
- Git (for version control)

### Installation & Setup
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Danielbclr/TheRingGoesSouth-Godot.git 
    cd TheRingGoesSouth-Godot
    ```
2.  **Open in Godot Engine:**
    *   Launch the Godot Engine.
    *   Click "Import" and navigate to the cloned project's `project.godot` file.
3.  **Build C# Solution:**
    *   Godot should prompt you to build the C# solution upon first opening. If not, you can trigger a build via `Project > Tools > C# > Create C# solution` (if it doesn't exist) and then by clicking the build button in the Godot editor (usually near the top right).
4.  **Run the project:**
    *   Press F5 or click the "Play" button in the Godot editor.

---

## High-Level Features - Current Status & Godot Implementation Plan

This tracks features largely implemented in the original prototype and how they will be (or are being) realized in Godot.

### 🎮 1. Project Setup & Core Architecture
*   [x] Establish base Godot project structure.
*   [X] Configure C# for game logic.
⚙️ **Godot Implementation:**
    *   [x] Main scene setup (`Main.tscn` or similar).
    *   [x] Global managers (e.g., `ResourceManager`, `PartyManager`, `GameEventBus`) as AutoLoad singletons (C# scripts).
    *   [ ] Scene management system using `GetTree().ChangeSceneToFile()`.

### 🌍 2. Overworld Map System
*   [x] Grid-based map with a static layout loaded from Tiled.
*   [ ] Dynamic elements (weather, hazards).
⚙️ **Godot Implementation:**
    *   [x] `TileMap` node for rendering maps imported from TMX files (`battle_map.tmx`, `lotr_map.tmx`).
    *   [x] `Camera2D` node for map camera controls.
    *   [x] Party representation on map (e.g., a `Node2D` or `CharacterBody2D` based scene).
    *   [x] Input handling for party movement (C# script).
    *   [ ] Map interactions (e.g., quest givers) using `Area2D` nodes or by checking `TileMap` tile data from scripts.
    *   [ ] Fog of War (e.g., using a secondary `TileMap` for overlay, shaders, or light/shadow system).
    *   [ ] Route calculation (`AStar2D` or custom pathfinding on `TileMap` data).

### 🧭 3. Exploration & Resource System
*   [ ] Resource tracking (Food, Firewood, Gold, Hope).
⚙️ **Godot Implementation:**
    *   [ ] `ResourceManager.cs` (AutoLoad singleton) to manage resources.
    *   [ ] UI elements (Godot `Control` nodes like `Label`, `ProgressBar`) to display resources, updated from `ResourceManager`.
    *   [ ] Logic to tie resource consumption to actions/terrain.
    *   [ ] Node types with specific logic (e.g., `QuestGiverNode.tscn` with attached C# script).

### 🧑‍🤝‍🧑 4. Party System & RPG Logic
*   [ ] Party members, classes, skills, basic leveling.
⚙️ **Godot Implementation:**
    *   [x] `GameCharacter.cs` (or similar) C# class for character data (stats, skills, etc.). Consider using custom `Resource` C# classes for editor integration of character templates.
    *   [x] `PartyManager.cs` (AutoLoad singleton) to manage the player's party.
    *   [x] `Skill.cs` (and related classes like `SkillEffect.cs`) for skill definitions, potentially loaded from JSON or custom `Resource` files.
    *   [x] `GameClass.cs` for class definitions.
    *   [ ] Class Tree progression UI (using Godot `Control` nodes).
    *   [ ] XP and leveling mechanics fully integrated.
    *   [ ] Status effects system for characters.

### 💬 5. Dialog & Event System
*   [ ] Narrative choices, quest dialogs (loaded from JSON in original).
⚙️ **Godot Implementation:**
    *   [ ] `QuestManager.cs` (AutoLoad singleton) to load and track quest data (e.g., from JSON files).
    *   [ ] `DialogScreen.tscn` scene with C# script (`DialogScreen.cs`) using Godot `Control` nodes (`RichTextLabel`, `Button`s) for displaying dialog and choices.
    *   [ ] Event triggers on map (e.g., `Area2D` nodes linked to `QuestManager`).
    *   [ ] Quest objective tracking (e.g., kill counters updated by `BattleScreen`).

### 🔁 6. Turn-Based Combat (Tactics Style)
*   [ ] 2D grid system, turn-based actions, skills, basic enemy AI.
⚙️ **Godot Implementation:**
    *   [x] `BattleScreen.tscn` as the main combat scene.
    *   [x] `TileMap` node for the battle grid.
    *   [x] Combatant scenes (`PlayerUnit.tscn`, `EnemyUnit.tscn`) with C# scripts (`BattleActor.cs` base class, `PlayerUnit.cs`, `EnemyUnit.cs`).
        *   These scenes would include `Sprite2D` or `AnimatedSprite2D` for visuals.
    *   [x] `TurnManager.cs` (or logic within `BattleScreen.cs`) to handle turn order and flow.
    *   [x] Input handling for selecting units, movement, and skills (in `BattleScreen.cs` or dedicated input handler script).
    *   [ ] Skill range and AoE highlighting (e.g., drawing on a `CanvasItem` or using another `TileMap` overlay).
    *   [ ] Basic enemy AI logic in `EnemyUnit.cs`.
    *   [x] Combat UI (`BattleUiManager.cs` or directly in `BattleScreen.cs`) using Godot `Control` nodes for action popups, health bars, turn indicators.

### 🏕️ 7. Rest System + Interactions
*   [ ] Allow party to rest, restore, and trigger inter-character dialogs.
⚙️ **Godot Implementation (To Do):**
    *   [ ] `RestSystem.cs` logic (possibly an AutoLoad or part of a `CampScreen.cs`).
    *   [ ] `CampScreen.tscn` scene for rest UI and interactions.
    *   [ ] Event triggers for camp events (ambushes, dialogs).
    *   [ ] Resource consumption and recovery during rest.

### 📦 8. UI & UX
*   [ ] Basic UI for map, battle, dialogs.
⚙️ **Godot Implementation:**
    *   [ ] Scene2D.UI equivalent using Godot's `Control` nodes (e.g., `Panel`, `Button`, `Label`, `VBoxContainer`, `TextureRect`).
    *   [ ] `Theme` resources for consistent UI styling.
    *   [x] Battle action popup menu.
    *   [ ] Tooltips for skills, items, UI elements.
    *   [ ] Inventory display and management UI.
    *   [ ] Basic mouse input support. Keyboard for shortcuts can be added.

### 📁 9. Data & Content Pipeline
*   [x] Content loaded via JSON (Quests, Skills, Enemies, Classes in original).
⚙️ **Godot Implementation:**
    *   [x] Loading data from JSON files using Godot's `Json` class in C# or `JSON.ParseString()`.
    *   [x] Consider creating custom C# `Resource` types (e.g., `SkillResource.cs`, `EnemyResource.cs`, `ItemResource.cs`) that can be created and edited directly in the Godot Inspector for better workflow. These can still be serialized/deserialized from JSON if needed or use Godot's binary resource format (`.tres`, `.res`).
    *   [x] Tiled maps (`.tmx`) imported via Godot's `TileMap` importer.
    *   [x] Spritesheets/Textures imported into Godot.

### 🧪 10. Testing & Debug Tools
*   [ ] Basic logging.
⚙️ **Godot Implementation:**
    *   [ ] Extensive use of `GD.Print()` (or `Console.WriteLine` in C#) for logging.
    *   [ ] In-game debug console (a `Control` node scene with a `LineEdit` for commands).
        *   Commands to grant resources, spawn units, trigger events.
    *   [ ] Seeded randomness for repeatable testing.
    *   [ ] Consider using a testing framework like Godot C# Test (if available and mature for your version) or writing simple test scenes.

---

## 🗺️ Development Roadmap (Godot Port & Enhancement)

This outlines the planned phases for porting and further developing the project in Godot. Phases may overlap.

### Phase 1: Core Port & Foundation (Largely   Done from original, now for Godot)
*   [ ] Initial Godot project setup with C#.
*   [ ] Port `ResourceManager`, `PartyManager`, basic `GameCharacter` structure.
*   [ ] Implement `MapScreen` with Tiled map rendering and basic party movement.
*   [ ] Load basic character, class, skill, and enemy data (from JSON or initial Godot resources).
*   [ ] Port `QuestManager` and `DialogScreen` for basic quest interaction.

### Phase 2: Combat System Port (Largely   Done from original, now for Godot)
*   [ ] Implement `BattleScreen` with `TileMap` grid.
*   [ ] Port `BattleActor` logic for `PlayerUnit` and `EnemyUnit` scenes.
*   [ ] Implement turn management.
*   [ ] Port skill execution logic (single target, AoE) and damage calculation.
*   [ ] Basic enemy AI.
*   [ ] Combat UI (action menus, health bars, turn indicators).

### Phase 3: Item System (⏳ Next Up - Godot Implementation)
*   [ ] Define `Item.cs` class/resource and `ItemType` enum.
*   [ ] Create data files for items (JSON or Godot `Resource` files like `ItemResource.tres`).
*   [ ] Implement `ItemDataManager.cs` (if needed) or integrate loading into `ResourceManager`/`InventorySystem`.
*   [ ] Integrate item effects (e.g., health potion consumable in battle).
*   [ ] Basic inventory UI panel (`InventoryScreen.tscn`).

### Phase 4: Class Progression & Deeper RPG Elements
*   [ ] Full `ClassTree` implementation with data structures and UI (e.g., `ClassTreeScreen.tscn`).
*   [ ] `Perk` system: data structures, loading, and integration.
*   [ ] `Race` system: data structures, traits, and effects.
*   [ ] UI screens for Party Roster, Character Details (stats, skills, perks, equipment).

### Phase 5: Rest & Camp System
*   [ ] Implement `RestSystem.cs` logic.
*   [ ] Create `RestScreen.tscn` UI for camp interactions.
*   [ ] Define and implement `CampEvent`s (dialogs, ambushes).

### Phase 6: UI/UX Polish & Advanced Features
*   [ ] Advanced AI behavior for enemies (e.g., using Godot's navigation, behavior trees).
*   [ ] More varied event triggers.
*   [ ] Comprehensive tooltips for UI elements.
*   [ ] Sound effects and music integration (`AudioStreamPlayer` nodes).
*   [ ] Particle FX for skills and environment (`GPUParticles2D`/`CPUParticles2D`).
*   [ ] Save/Load game state (Godot's `FileAccess` or `ConfigFile`).
*   [ ] Implement in-game debug console.

---

## 📦 Data & Assets (Godot Structure)

*   **JSON Data:** `res://data/json/` for skills, quests, enemies, classes, items, etc.
    *   Can be parsed by C# scripts.
*   **Godot Resources:** `res://data/resources/` for custom `Resource` files (`.tres` or `.res`) like `SkillResource`, `EnemyTemplate`, `ItemDefinition`.
    *   These are powerful for editor integration.
*   **TileMaps:** `res://tilemaps/` for `.tmx` files and their imported Godot `TileSet` resources.
*   **Sprites & Textures:** `res://assets/graphics/sprites/`, `res://assets/graphics/ui/`
*   **UI Themes:** `res://assets/ui/themes/` for Godot `Theme` resources.
*   **Scenes:** `res://scenes/` (e.g., `res://scenes/screens/MapScreen.tscn`, `res://scenes/actors/PlayerUnit.tscn`).
*   **Scripts:** `res://scripts/` (organized by feature or type, e.g., `res://scripts/managers/`, `res://scripts/actors/`).

---

## 🎯 Next Steps / Stretch Goals (Long Term)

*   [ ] Analytics integration for playtesting.
*   [ ] Modular mod support.
*   [ ] Localization framework.
*   [ ] Accessibility options.

---