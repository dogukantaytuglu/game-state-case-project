
public class GameStateService
{
    private static readonly GameStateService _instance = new GameStateService();

    public static GameStateService Get()
    {
        return _instance;
    }

    public GameState State { get; private set; }

    public void Init(int coins, int stars, int enemyCount = 0)
    {
        State = new GameState(coins, stars, enemyCount);
    }
}