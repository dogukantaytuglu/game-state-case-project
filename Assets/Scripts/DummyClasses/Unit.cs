using System.Collections.Generic;

public class Unit : BaseStateVariable
{
    public int EnemyCount => _enemies.Count;
    
    private List<Enemy> _enemies;

    public Unit(int enemyCount, IValueChangeListener valueChangeListener) : base(valueChangeListener)
    {
        AddDummyEnemies(enemyCount);
    }

    public void KillEnemy(int index)
    {
        _enemies.RemoveAt(index);
        OnValueChanged.Invoke(this);
    }

    private void AddDummyEnemies(int enemyCount)
    {
        _enemies = new List<Enemy>();

        for (int i = 0; i < enemyCount; i++)
        {
            _enemies.Add(new Enemy($"enemy {i + 1}"));
        }
    }
}