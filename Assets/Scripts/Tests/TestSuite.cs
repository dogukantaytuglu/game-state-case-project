using NUnit.Framework;

namespace Tests
{
    public class TestSuite
    {
        [Test]
        public void CanInitGameState()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;

            Assert.That(gameState.Coins.Value, Is.EqualTo(10));
            Assert.That(gameState.Stars.Value, Is.EqualTo(0));
        }

        [Test]
        public void CanObserveGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var stateObserverCalled = false;

            gameState.ListenFor(gameState.Coins, () =>
            {
                stateObserverCalled = true;
                Assert.That(gameState.Coins.Value, Is.EqualTo(8));
            });

            ShopService.Get().UseCoins(2);

            Assert.That(stateObserverCalled, "Obsever not called");
        }

        [Test]
        public void CanObserveConsistentGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);
            var callCount = 0;

            var stateObserverCalled = false;
            var gameState = gameStateService.State;

            void StateValidator()
            {
                callCount++;
                stateObserverCalled = true;
                Assert.That(gameState.Stars.Value, Is.EqualTo(1));
                Assert.That(gameState.Coins.Value, Is.EqualTo(9));
            }

            gameState.ListenFor(gameState.Coins, StateValidator);
            gameState.ListenFor(gameState.Stars, StateValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(1, 1);

            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(stateObserverCalled, "Obsever not called");
        }
    }
}