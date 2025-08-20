using System;

public class GameState
{
    public readonly SingleValueVariable<int> Coins;
    public readonly SingleValueVariable<int> Stars;
    public readonly Unit Unit;

    private readonly GameStateEventSystem _gameStateEventSystem;

    public GameState(int coins, int stars, int enemyCount)
    {
        _gameStateEventSystem = new GameStateEventSystem();

        Coins = new SingleValueVariable<int>(coins, _gameStateEventSystem);
        Stars = new SingleValueVariable<int>(stars, _gameStateEventSystem);
        Unit = new Unit(enemyCount, _gameStateEventSystem);
    }

    public void Subscribe(IGameStateVariable variable, Action observer)
    {
        _gameStateEventSystem.Subscribe(variable, observer);
    }

    public void Unsubscribe(IGameStateVariable variable, Action observer)
    {
        _gameStateEventSystem.Unsubscribe(variable, observer);
    }
}