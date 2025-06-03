# The Ring Goes South - Development Log & Roadmap

## 1. Project Overview

"The Ring Goes South" is a 2D RPG featuring a hex-grid overworld exploration system and a turn-based tactical battle system. Players will manage a party of characters, explore the game world, and engage in strategic combat. The project is being developed using the Godot Engine with C#.

## 2. Current Progress (As of [Current Date])

The project has made significant strides in establishing core gameplay mechanics, particularly in overworld movement and the foundational elements of the battle system.

### 2.1. Overworld System

The overworld allows the player to navigate a hex-based map.

*   **Scene:** `res://scenes/screens/HexOverworld.tscn`
    *   This scene serves as the main container for the overworld experience.
    *   It integrates the hex grid, the player's party, and a camera.
*   **Party Movement:** `res://scripts/actors/PartyHexMovement.cs`
    *   Manages the player's party movement on the hex grid.
    *   Handles input for selecting a destination tile.
    *   Calculates and visualizes the path to the target.
    *   Executes movement along the calculated path with a tween for smooth animation.
    *   Includes debug options for highlighting adjacent tiles and lines.
*   **Tile Highlighting:** `res://scripts/utils/HighlightTileHelper.cs`
    *   Provides functionality to visually highlight hex tiles on the map.
    *   Used for indicating movement paths, selectable tiles, or areas of interest.
    *   Instantiates a `HighlightHexTile.tscn` scene for each highlighted tile.
    *   Supports clearing highlights.
*   **Hex Grid Utilities:** `res://scripts/utils/HexGridHelper.cs`
    *   A static helper class offering various functions for hex grid manipulation:
        *   `GetStraightTiles`: Finds tiles in straight lines from a point.
        *   `GetPathFromMovementArray`: Converts a series of movement vectors into a list of tile coordinates.
        *   `GetTilesInRange`: Implements a Breadth-First Search (BFS) to find all tiles within a certain range (cubic distance) from a starting coordinate.
        *   `GetRoute`: Calculates a path between two points on the hex grid, considering tile walkability (currently checks for tile existence).
        *   `Normalize`: Normalizes a vector to its directional components (-1, 0, or 1).
        *   `IsTileWalkable`: Checks if a tile at given coordinates exists and is considered walkable.
        *   `GetTileGlobalPosition`: Converts global world position to the global position of the tile's center.
    *   Defines standard hex direction vectors.
*   **Legacy Overworld (Square Tiles):** `res://OverworldScene.tscn`
    *   An older version of the overworld scene, likely using a square-based tilemap. This seems to have been superseded by the `HexOverworld.tscn`.

### 2.2. Battle System

A turn-based tactical battle system is under development, allowing characters to engage enemies on a separate hex grid.

*   **Main Battle Scene:** `res://scenes/screens/BattleScene.tscn`
    *   The root scene for battle encounters.
    *   Orchestrates various battle components and manages the battle flow.
*   **Battle Scene Logic:** `res://scripts/scenes/BattleScene.cs`
    *   Manages the overall battle state.
    *   Initializes and coordinates various helper components:
        *   `BattleTurnManager`: Handles turn order and active unit.
        *   `BattleGridHelper`: Provides utility functions specific to the battle grid (e.g., checking tile walkability considering unit positions).
        *   `UnitSpawner`: Responsible for creating and placing player and enemy units on the battle grid.
        *   `BattleInputHandler`: Manages player input during battles (e.g., selecting move targets, skipping turns).
        *   `BattleMovementController`: Handles unit movement logic, including pathfinding and execution.
        *   `BattleCameraController`: Controls camera movement, focusing on active units.
        *   `HighlightTileHelper`: Used for highlighting movement ranges, attack ranges, etc.
    *   Spawns player party members and predefined enemy units.
    *   Handles events for turn start, move target selection, and skipping turns.
*   **Battle Grid Helper:** `res://scripts/utils/Battle/BattleGridHelper.cs`
    *   Extends hex grid logic for battle scenarios.
    *   `IsTileWalkable`: Checks if a tile is walkable, considering the presence of other units (both player and enemy), excluding a specified unit if needed (e.g., the unit currently moving).
    *   `GetTilesInRange`: Finds reachable tiles within a given range, respecting unit obstructions and walkability.
    *   `GetMovementPath`: Leverages `HexGridHelper.GetRoute` for pathfinding on the battle grid.
    *   Provides `MapToWorld` and `WorldToMap` conversions specific to the battle grid.
*   **Unit Spawner:** `res://scripts/utils/Battle/UnitSpawner.cs`
    *   `SpawnPartyMembers`: Instantiates `PlayerUnit` scenes based on data from `PartyManager.Instance.PartyMembers` and places them at predefined starting positions.
    *   `SpawnEnemy`: Loads enemy data using `EnemyDataLoader`, instantiates the enemy's scene, and places it on the grid.
*   **Player Unit Scene:** `res://scenes/actors/PlayerUnit.tscn` (Referenced in `BattleScene.cs`)
    *   The PackedScene used to instantiate player-controlled units in battle. (Actual script `PlayerUnit.cs` not provided but implied).
*   **Highlight Tile Scene:** `res://scenes/utils/HightlightHexTile.tscn` (Referenced in `HexOverworld.tscn` and `HighlightTileHelper.cs`)
    *   A simple scene, likely a `Sprite2D` or `Polygon2D`, used to visually mark a hex tile.

### 2.3. Data Management & Character System

A data-driven approach is used for defining characters, classes, and enemies.

*   **Party Management:** `res://scripts/autoloads/PartyManager.cs`
    *   Singleton (`_instance`) for global access to party data.
    *   Manages the player's `PartyMembers` (a `List<CharacterData>`).
    *   Defines `maxPartySize` and `battlePartySize`.
    *   Loads an initial party in `LoadInitialParty` by creating characters using `CharacterDataManager`.
    *   Provides methods to add, remove, and retrieve party members.
    *   `CreateAndAddCharacter`: A helper to create a character from a class ID and add them to the party.
*   **Enemy Data:**
    *   **Definition:** `res://scripts/data/EnemyData.cs`
        *   POCO (Plain Old C# Object) defining the structure for enemy attributes (Id, Name, MaxHealth, Attack, Defense, Speed, ScenePath, MoveRange).
    *   **Loading:** `res://scripts/data/EnemyDataLoader.cs`
        *   Static class to load enemy definitions from a JSON file.
        *   `LoadEnemyData`: Reads `res://data/enemies.json`, deserializes it into a list of `EnemyData` objects, and stores them in a dictionary keyed by `Id`.
        *   `GetEnemyData`: Retrieves `EnemyData` by ID.
    *   **JSON Data File:** `res://data/enemies.json` (Content not provided, but its existence and structure are implied by the loader).
*   **Class Data:**
    *   **Definition:** `res://scripts/data/ClassData.cs`
        *   POCO defining the structure for character class attributes (Id, Name, Description, BaseStats, ScenePath, ClassSkills, ClassPerks).
    *   **Loading:** `res://scripts/data/ClassDataLoader.cs`
        *   Static class to load class definitions from `res://data/classes.json`.
        *   Similar loading and retrieval mechanism as `EnemyDataLoader`.
    *   **JSON Data File:** `res://data/classes.json` (Content not provided, but its existence and structure are implied by the loader).
*   **Character Data & Manager:** (Implied by `PartyManager.cs` and `UnitSpawner.cs`)
    *   `CharacterData.cs`: (Not provided) Assumed to exist, holding information about individual characters, including their stats, class(es), skills, current health, etc. Likely incorporates `ClassData`.
    *   `CharacterDataManager.cs`: (Not provided) Assumed to be an autoload or singleton responsible for creating `CharacterData` instances, possibly by combining base character info with `ClassData`. `PartyManager.Instance.LoadCharacterFromClass` points to its existence.

### 2.4. Utilities

*   **Logging:** `ILoggable` interface and `Logger` class (seen in `PartyHexMovement.cs`)
    *   A simple logging utility to enable/disable debug messages per script via a `DEBUG_TAG`.

## 3. Future Roadmap

This section outlines potential future development directions, categorized for clarity.

### 3.1. Core Gameplay Loop

*   **Objective System:**
    *   Implement a quest or main story objective system to guide the player.
    *   Introduce side quests and optional content.
*   **Overworld Interactions:**
    *   Event Triggers: Place triggers on overworld tiles to initiate dialogues, battles, item discoveries, or scene transitions.
    *   NPCs: Add non-player characters to the overworld for interaction, quests, and lore.
    *   Towns/Settlements: Create safe zones with shops, inns, and quest givers.
    *   Dungeons/Points of Interest: Design explorable areas with unique challenges and rewards.
*   **Battle System Enhancements (Phase 1 - Core Combat):**
    *   **Actions:**
        *   Basic Attack: Implement a standard attack action.
        *   Skills/Abilities: Allow units to use skills defined in their `ClassData` or `CharacterData`. This will require a Skill system (data, effects, targeting).
        *   Defend: Implement a "defend" action to reduce incoming damage.
        *   Items: Allow use of consumable items during battle.
    *   **Targeting:**
        *   Develop UI and logic for selecting targets for attacks and skills (single target, AoE).
        *   Highlight valid target tiles.
    *   **Damage Calculation:**
        *   Implement formulas for damage (e.g., `Attack - Defense`).
        *   Consider critical hits, misses, elemental affinities.
    *   **Status Effects:**
        *   Design and implement a system for status effects (e.g., poison, stun, buff, debuff).
    *   **Victory/Defeat Conditions:**
        *   Define and implement conditions for winning or losing a battle (e.g., defeat all enemies, protect an NPC, survive X turns).
        *   Transition back to overworld or a game over screen.
*   **Saving/Loading:**
    *   Implement a system to save and load game progress (party state, player position, quest progress).

### 3.2. Character System & Progression

*   **Experience and Leveling:**
    *   Award experience points after battles.
    *   Implement a leveling system where characters improve stats and potentially learn new skills upon leveling up.
*   **Inventory System:**
    *   Develop an inventory for the party to hold items (consumables, equipment, quest items).
    *   UI for managing inventory.
*   **Equipment System:**
    *   Allow characters to equip weapons, armor, and accessories.
    *   Equipment should affect character stats and potentially grant abilities.
*   **Skill System (Detailed):**
    *   `SkillData.cs`: Define skill properties (name, description, cost, range, targeting type, effects).
    *   `SkillDataLoader.cs`: Load skills from JSON.
    *   Integrate with `CharacterData` and `ClassData` to determine which skills a character has.
    *   Implement skill effects (damage, healing, status effects, buffs/debuffs).

### 3.3. Battle System (Phase 2 - Advanced Tactics)

*   **Enemy AI:**
    *   Develop more sophisticated AI for enemy units (e.g., prioritize targets, use skills effectively, move strategically).
    *   Consider different AI profiles for various enemy types.
*   **Environmental Factors:**
    *   Tiles with special properties (e.g., cover, difficult terrain, healing aura).
    *   Line of sight for ranged attacks.
*   **Advanced Combat Mechanics:**
    *   Opportunity attacks, flanking bonuses, height advantages (if applicable).
    *   Turn order manipulation abilities.
    *   Summoning units.

### 3.4. User Interface (UI) & User Experience (UX)

*   **Overworld UI:**
    *   Party status display.
    *   Minimap.
    *   Quest log.
    *   Menu for inventory, character stats, options.
*   **Battle UI:**
    *   Unit information display (HP, MP, status effects).
    *   Action menu (Attack, Skills, Defend, Item, Wait).
    *   Turn order display.
    *   Damage numbers and visual feedback for actions.
*   **Dialogue System:**
    *   Implement a system for displaying conversations with NPCs.
    *   Support for branching dialogues and player choices.
*   **Tooltips and Help:**
    *   Provide contextual information for UI elements, skills, items, etc.

### 3.5. Art & Audio

*   **Character Sprites & Animations:**
    *   Create or acquire distinct sprites for different characters and enemies.
    *   Implement idle, walk, attack, hit, and death animations.
*   **Tilemap Art:**
    *   Design visually appealing tilesets for the overworld and battle environments.
*   **Visual Effects (VFX):**
    *   Effects for spells, attacks, status changes.
*   **Sound Effects (SFX):**
    *   Sounds for movement, attacks, UI interactions, spells.
*   **Music:**
    *   Background music for overworld, battles, towns.

### 3.6. Technical & Refinements

*   **Code Refactoring:**
    *   Continuously review and refactor code for clarity, efficiency, and maintainability.
    *   Ensure adherence to C# and Godot best practices.
*   **Performance Optimization:**
    *   Profile and optimize performance, especially in scenes with many nodes or complex calculations.
*   **Input System Refinement:**
    *   Improve input handling for responsiveness and intuitiveness (keyboard, mouse, potentially gamepad).
*   **Testing & Debugging:**
    *   Implement a robust testing strategy.
    *   Expand debug tools as needed.
*   **Build & Deployment:**
    *   Set up build pipelines for different platforms.

## 4. Long-Term Vision

*   **Expanded World:** More regions, dungeons, and secrets to discover.
*   **Deeper Storyline:** Rich narrative with compelling characters and plot developments.
*   **Advanced Class/Job System:** Multi-classing, promotions, unique class abilities.
*   **Crafting System:** Allow players to craft items or equipment.
*   **Modding Support:** If feasible, allow community contributions.

This document should be updated regularly as the project progresses and priorities shift.
