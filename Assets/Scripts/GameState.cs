using System;
using System.Collections.Generic;

public class GameState
{
    public readonly GameStateVariable<int> Coins;
    public readonly GameStateVariable<int> Stars;

    private readonly Dictionary<IGameStateVariable, HashSet<Action>> _variableListenersDictionary;
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _actionVariableDependencyDictionary;
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _actionVariableChangedDictionary;

    public GameState(int coins, int stars)
    {
        _variableListenersDictionary = new Dictionary<IGameStateVariable, HashSet<Action>>();
        _actionVariableDependencyDictionary = new Dictionary<Action, HashSet<IGameStateVariable>>();
        _actionVariableChangedDictionary = new Dictionary<Action, HashSet<IGameStateVariable>>();

        Coins = new GameStateVariable<int>(coins, OnAnyValueChanged);
        Stars = new GameStateVariable<int>(stars, OnAnyValueChanged);
    }

    private void OnAnyValueChanged(IGameStateVariable changedVariable)
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