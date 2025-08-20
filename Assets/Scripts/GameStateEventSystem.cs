using System;
using System.Collections.Generic;

public class GameStateEventSystem
{
    private readonly Dictionary<IGameStateVariable, HashSet<Action>> _variableListenersDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _actionVariableDependencyDictionary = new();
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _actionVariableChangedDictionary = new();
    
    public void OnAnyValueChanged(IGameStateVariable changedVariable)
    {
        var listenersToNotify = _variableListenersDictionary[changedVariable];

        TryNotifyListeners(changedVariable, listenersToNotify);
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

    private void NotifyListener(Action action)
    {
        _actionVariableChangedDictionary[action].Clear();
        action.Invoke();
    }

    private bool CanNotifyListener(IGameStateVariable changedVariable, Action action)
    {
        var changedVariableSet = AddChangedVariableToAction(changedVariable, action);

        var dependencySet = _actionVariableDependencyDictionary[action];
        return changedVariableSet.Count >= dependencySet.Count;
    }

    private HashSet<IGameStateVariable> AddChangedVariableToAction(IGameStateVariable changedVariable, Action action)
    {
        if (_actionVariableChangedDictionary.TryGetValue(action, out var changedVariableSet))
        {
            changedVariableSet.Add(changedVariable);
        }

        else
        {
            changedVariableSet = new HashSet<IGameStateVariable>() { changedVariable };
            _actionVariableChangedDictionary[action] = changedVariableSet;
        }

        return changedVariableSet;
    }

    public void Subscribe<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        AddListenerToVariableDictionary(variable, listenerAction);
        AddVariableToPendingDictionary(variable, listenerAction);
    }

    public void Unsubscribe<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        _actionVariableDependencyDictionary.Remove(listenerAction);
        _variableListenersDictionary[variable].Remove(listenerAction);
    }

    private void AddVariableToPendingDictionary<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        if (_actionVariableDependencyDictionary.TryGetValue(listenerAction, out var variables))
        {
            variables.Add(variable);
        }

        else
        {
            _actionVariableDependencyDictionary[listenerAction] = new HashSet<IGameStateVariable>() { variable };
        }
    }

    private void AddListenerToVariableDictionary<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        if (_variableListenersDictionary.TryGetValue(variable, out var listeners))
        {
            listeners.Add(listenerAction);
        }

        else
        {
            _variableListenersDictionary[variable] = new HashSet<Action>() { listenerAction };
        }
    }
}