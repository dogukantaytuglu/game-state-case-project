using System;

public class SingleValueVariable<T> : BaseStateVariable where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
{
    public SingleValueVariable(T initValue, IValueChangeListener valueChangeListener) : base(valueChangeListener)
    {
        _value = initValue;
    }

    public T Value
    {
        get => _value;

        set
        {
            _value = value;
            OnValueChanged.Invoke(this);
        }
    }

    private T _value;
}