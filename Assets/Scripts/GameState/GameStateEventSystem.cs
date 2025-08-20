using System;
using System.Collections.Generic;

public class GameStateEventSystem
{
    private readonly Dictionary<IGameStateVariable, HashSet<Action>> _variableListenersDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _listenerPrerequisiteDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _listenerDirtyVariableDictionary = new();

    public void OnAnyValueChanged(IGameStateVariable changedVariable)
    {
        TryNotifyListeners(changedVariable, _variableListenersDictionary[changedVariable]);
    }

    private void TryNotifyListeners(IGameStateVariable variable, HashSet<Action> listeners)
    {
        foreach (var listener in listeners)
        {
            if (CanNotifyListener(variable, listener))
            {
                NotifyListener(listener);
            }
        }
    }

    private void NotifyListener(Action listener)
    {
        _listenerDirtyVariableDictionary[listener].Clear();
        listener.Invoke();
    }

    private bool CanNotifyListener(IGameStateVariable changedVariable, Action listener)
    {
        var changedVariableSet = AddDirtyVariableToListener(changedVariable, listener);

        var prerequisiteSet = _listenerPrerequisiteDictionary[listener];
        return changedVariableSet.Count >= prerequisiteSet.Count;
    }

    public void Subscribe<T>(GameStateVariable<T> variable, Action listener)
    {
        AddListenerToVariableDictionary(variable, listener);
        AddPrerequisiteToListener(variable, listener);
    }

    public void Unsubscribe<T>(GameStateVariable<T> variable, Action listener)
    {
        _listenerPrerequisiteDictionary.Remove(listener);
        _variableListenersDictionary[variable].Remove(listener);
    }
    
    private HashSet<IGameStateVariable> AddDirtyVariableToListener(IGameStateVariable changedVariable, Action listener)
    {
        _listenerDirtyVariableDictionary.AddItemToCollectionValue(listener, changedVariable);
        return _listenerDirtyVariableDictionary[listener];
    }

    private void AddPrerequisiteToListener(IGameStateVariable variable, Action listener)
    {
        _listenerPrerequisiteDictionary.AddItemToCollectionValue(listener, variable);
    }

    private void AddListenerToVariableDictionary<T>(GameStateVariable<T> variable, Action listener)
    {
        _variableListenersDictionary.AddItemToCollectionValue(variable, listener);
    }
}