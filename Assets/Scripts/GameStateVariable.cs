using System;

public class GameStateVariable<T> : IGameStateVariable
{
    private readonly Action<IGameStateVariable> _onValueChanged;

    public GameStateVariable(T initValue, Action<IGameStateVariable> onValueChanged)
    {
        _value = initValue;
        _onValueChanged = onValueChanged;
    }

    public T Value
    {
        get => _value;

        set
        {
            _value = value;
            _onValueChanged?.Invoke(this);
        }
    }

    private T _value;
}