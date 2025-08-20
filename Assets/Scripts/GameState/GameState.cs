using System;

public class GameState
{
    public readonly GameStateVariable<int> Coins;
    public readonly GameStateVariable<int> Stars;

    private readonly GameStateEventSystem _gameStateEventSystem;

    public GameState(int coins, int stars)
    {
        _gameStateEventSystem = new GameStateEventSystem();
        
        Coins = new GameStateVariable<int>(coins, _gameStateEventSystem.OnAnyValueChanged);
        Stars = new GameStateVariable<int>(stars, _gameStateEventSystem.OnAnyValueChanged);
    }
    
    public void StartListening<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        _gameStateEventSystem.Subscribe(variable, listenerAction);
    }

    public void StopListening<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        _gameStateEventSystem.Unsubscribe(variable, listenerAction);
    }
}