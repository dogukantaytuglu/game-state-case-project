using System;

public class BaseStateVariable : IGameStateVariable
{
    protected readonly IValueChangeListener ValueChangeListener;
    protected Action<IGameStateVariable> OnValueChanged => ValueChangeListener.OnAnyValueChanged;

    protected BaseStateVariable(IValueChangeListener valueChangeListener)
    {
        ValueChangeListener = valueChangeListener;
    }
}