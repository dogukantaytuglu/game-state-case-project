using NUnit.Framework;
using UnityEngine;

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

            gameState.StartListening(gameState.Coins, () =>
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

            gameState.StartListening(gameState.Coins, StateValidator);
            gameState.StartListening(gameState.Stars, StateValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(1, 1);

            gameState.StopListening(gameState.Coins, StateValidator);
            gameState.StopListening(gameState.Stars, StateValidator);
            
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(stateObserverCalled, "Obsever not called");
        }

        [Test]
        public void CanObserveMultipleConsistentGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            var shopService = ShopService.Get();
            
            var initCoins = 100;
            var currentStars = 0;
            var starCost = 1;
            var currentLoopCount = 0;
            
            gameStateService.Init(initCoins, currentStars);
            var gameState = gameStateService.State;

            var buyCount = Mathf.RoundToInt(initCoins / starCost);
            
            void StateValidator()
            {
                var stars = currentLoopCount;
                var coins = initCoins - (starCost * currentLoopCount);
                
                Assert.That(gameState.Stars.Value, Is.EqualTo(stars));
                Assert.That(gameState.Coins.Value, Is.EqualTo(coins));
            }
            
            gameState.StartListening(gameState.Coins, StateValidator);
            gameState.StartListening(gameState.Stars, StateValidator);

            for (int i = 0; i < buyCount; i++)
            {
                currentLoopCount++;
                shopService.BuyStars(1, starCost);
            }
            
            gameState.StopListening(gameState.Coins, StateValidator);
            gameState.StopListening(gameState.Stars, StateValidator);
            
            Assert.That(currentLoopCount, Is.EqualTo(buyCount));
        }

        [Test]
        public void CaObserveComplexStructures()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(1, 1, 4);
            var state = gameStateService.State;
            var unit = state.Unit;
            var currentEnemyCount = unit.EnemyCount;
            
            void StateValidator()
            {
                Assert.That(unit.EnemyCount, Is.LessThan(currentEnemyCount));
                currentEnemyCount = unit.EnemyCount;
            }
            
            state.StartListening(state.Unit, StateValidator);

            while (state.Unit.EnemyCount > 0)
            {
                unit.KillEnemy(unit.EnemyCount - 1);
            }
            
            Assert.That(unit.EnemyCount, Is.EqualTo(0));
        }
    }
}