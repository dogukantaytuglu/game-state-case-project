using System;

public class GameStateVariable<T> : IGameStateVariable
{
    private readonly Action<IGameStateVariable> _onValueChanged;
    public string DebugName;

    public GameStateVariable(T initValue, Action<IGameStateVariable> onValueChanged, string debugName)
    {
        _value = initValue;
        _onValueChanged = onValueChanged;
        DebugName = debugName;
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