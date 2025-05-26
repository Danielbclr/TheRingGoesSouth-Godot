using Godot;
using Godot.Collections;
using System.Collections.Generic;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.data;
using TheRingGoesSouth.scripts.utils;

public partial class UnitSpawner : Node
{
    [Export] public PackedScene PlayerUnitScene { get; set; }
    
    private TileMapLayer _battleGrid;
    private Node _parentNode;

    public void Initialize(TileMapLayer battleGrid, Node parentNode)
    {
        _battleGrid = battleGrid;
        _parentNode = parentNode;
    }

    public Array<PlayerUnit> SpawnPartyMembers()
    {
        Array<PlayerUnit> playerUnits = [];
        
        if (PartyManager.Instance == null)
        {
            GD.PrintErr("UnitSpawner: PartyManager instance is not available!");
            return playerUnits;
        }

        List<CharacterData> party = PartyManager.Instance.PartyMembers;
        Vector2I[] startingPositions = {
            new(2, 5), new(3, 6), new(2, 7), new(3, 8)
        };

        for (int i = 0; i < party.Count && i < startingPositions.Length; i++)
        {
            CharacterData character = party[i];
            Vector2I gridPosition = startingPositions[i];

            PlayerUnit playerUnit = PlayerUnitScene.Instantiate<PlayerUnit>();
            playerUnit.Setup(character, _battleGrid, gridPosition);

            _parentNode.AddChild(playerUnit);
            playerUnits.Add(playerUnit);

            GD.Print($"UnitSpawner: Placed {character.CharacterName} at map {gridPosition}, world {playerUnit.GlobalPosition}.");
        }
        
        GD.Print($"UnitSpawner: Placed {playerUnits.Count} party members on the grid.");
        return playerUnits;
    }

    public EnemyUnit SpawnEnemy(string enemyId, Vector2I spawnPosition)
    {
        EnemyData enemyData = EnemyDataLoader.GetEnemyData(enemyId);

        if (enemyData == null)
        {
            GD.PrintErr($"UnitSpawner: Could not find enemy data for ID '{enemyId}'. Skipping spawn.");
            return null;
        }

        PackedScene enemyScene = ResourceLoader.Load<PackedScene>(enemyData.ScenePath);
        if (enemyScene == null)
        {
            GD.PrintErr($"UnitSpawner: Could not load scene for enemy '{enemyData.Name}' at path '{enemyData.ScenePath}'. Skipping spawn.");
            return null;
        }

        EnemyUnit enemyUnit = enemyScene.Instantiate<EnemyUnit>();
        if (enemyUnit == null)
        {
            GD.PrintErr($"UnitSpawner: Failed to instantiate scene for enemy '{enemyData.Name}'. Ensure the root node has the EnemyUnit script.");
            return null;
        }

        enemyUnit.Setup(enemyData, _battleGrid, spawnPosition);
        _parentNode.AddChild(enemyUnit);
        
        GD.Print($"UnitSpawner: Spawned {enemyData.Name} at map {spawnPosition}, world {enemyUnit.GlobalPosition}.");
        return enemyUnit;
    }
}