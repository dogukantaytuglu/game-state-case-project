using System;
using System.Collections.Generic;

public class GameStateEventSystem : IValueChangeListener
{
    private readonly Dictionary<IGameStateVariable, HashSet<Action>> _variableObserversDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _observerPrerequisiteDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _observerDirtyVariableDictionary = new();

    public void OnAnyValueChanged(IGameStateVariable changedVariable)
    {
        TryNotifyObservers(changedVariable, _variableObserversDictionary[changedVariable]);
    }

    private void TryNotifyObservers(IGameStateVariable variable, HashSet<Action> observers)
    {
        foreach (var observer in observers)
        {
            if (CanNotifyObserver(variable, observer))
            {
                NotifyObserver(observer);
            }
        }
    }

    private void NotifyObserver(Action observer)
    {
        _observerDirtyVariableDictionary[observer].Clear();
        observer.Invoke();
    }

    private bool CanNotifyObserver(IGameStateVariable changedVariable, Action observer)
    {
        var changedVariableSet = AddDirtyVariableToObserver(changedVariable, observer);

        var prerequisiteSet = _observerPrerequisiteDictionary[observer];
        return changedVariableSet.Count >= prerequisiteSet.Count;
    }

    public void Subscribe(IGameStateVariable variable, Action observer)
    {
        AddObserverToVariableDictionary(variable, observer);
        AddPrerequisiteToObserver(variable, observer);
    }

    public void Unsubscribe(IGameStateVariable variable, Action observer)
    {
        _observerPrerequisiteDictionary.Remove(observer);
        _variableObserversDictionary[variable].Remove(observer);
    }
    
    private HashSet<IGameStateVariable> AddDirtyVariableToObserver(IGameStateVariable changedVariable, Action observer)
    {
        _observerDirtyVariableDictionary.AddItemToCollectionValue(observer, changedVariable);
        return _observerDirtyVariableDictionary[observer];
    }

    private void AddPrerequisiteToObserver(IGameStateVariable variable, Action observer)
    {
        _observerPrerequisiteDictionary.AddItemToCollectionValue(observer, variable);
    }

    private void AddObserverToVariableDictionary(IGameStateVariable variable, Action observer)
    {
        _variableObserversDictionary.AddItemToCollectionValue(variable, observer);
    }
}