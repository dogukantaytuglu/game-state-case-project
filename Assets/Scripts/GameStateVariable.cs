using System;

public class GameStateVariable<T>
{
    public Action OnValueChanged;

    public GameStateVariable(T initValue)
    {
        _value = initValue;
    }

    public T Value
    {
        get => _value;

        set
        {
            _value = value;
            OnValueChanged?.Invoke();
        }
    }

    private T _value;
}