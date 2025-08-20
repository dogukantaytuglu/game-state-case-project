using System;

public class BaseStateVariable : IGameStateVariable
{
    protected Action<IGameStateVariable> OnValueChanged => _valueChangeListener.OnAnyValueChanged;
    private readonly IValueChangeListener _valueChangeListener;

    protected BaseStateVariable(IValueChangeListener valueChangeListener)
    {
        _valueChangeListener = valueChangeListener;
    }
}