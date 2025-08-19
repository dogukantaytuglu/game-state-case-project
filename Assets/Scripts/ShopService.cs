public class ShopService
{
    private static readonly ShopService _instance = new ShopService();

    public static ShopService Get()
    {
        return _instance;
    }

    public void BuyStars(int stars, int forCoins)
    {
        EarnStars(stars);
        UseCoins(forCoins);
    }

    private static void EarnStars(int stars)
    {
        GameStateService.Get().State.Stars.Value += stars;
    }

    public void UseCoins(int coins)
    {
        GameStateService.Get().State.Coins.Value -= coins;
    }
}