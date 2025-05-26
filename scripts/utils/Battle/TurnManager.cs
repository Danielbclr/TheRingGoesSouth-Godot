using Godot;
using Godot.Collections;
using System;
using TheRingGoesSouth.scripts.actors;

public partial class BattleTurnManager : Node
{
    public enum BattleTurnState
    {
        None,
        PlayerTurnStarting,
        PlayerTurnReady,
        PlayerMoving,
        TurnEnding
    }

    public BattleTurnState CurrentBattleState { get; private set; } = BattleTurnState.None;
    public PlayerUnit ActivePlayerUnit { get; private set; }
    
    private Array<PlayerUnit> _playerUnits;
    private int _currentPlayerIndex = -1;

    public event Action<PlayerUnit> OnTurnStarted;
    public event Action OnTurnEnded;

    public void Initialize(Array<PlayerUnit> playerUnits)
    {
        _playerUnits = playerUnits;
        _currentPlayerIndex = -1;
        CurrentBattleState = BattleTurnState.TurnEnding;
    }

    public void StartBattle()
    {
        if (_playerUnits.Count > 0)
        {
            NextTurn();
        }
    }

    public void NextTurn()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerUnits.Count;
        ActivePlayerUnit = _playerUnits[_currentPlayerIndex];
        CurrentBattleState = BattleTurnState.PlayerTurnStarting;
        
        GD.Print($"--- {ActivePlayerUnit.CharacterData.CharacterName}'s Turn ---");
        
        CurrentBattleState = BattleTurnState.PlayerTurnReady;
        OnTurnStarted?.Invoke(ActivePlayerUnit);
    }

    public void EndTurn()
    {
        CurrentBattleState = BattleTurnState.TurnEnding;
        OnTurnEnded?.Invoke();
        NextTurn();
    }

    public void SetState(BattleTurnState newState)
    {
        CurrentBattleState = newState;
    }

    public void SkipTurn()
    {
        GD.Print($"{ActivePlayerUnit.CharacterData.CharacterName} skips turn.");
        EndTurn();
    }
}