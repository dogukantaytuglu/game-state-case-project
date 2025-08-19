using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public readonly GameStateVariable<int> Coins;
    public readonly GameStateVariable<int> Stars;

    private readonly Dictionary<IGameStateVariable, HashSet<Action>> _variableListenersDictionary;
    private readonly Dictionary<Action, HashSet<IGameStateVariable>> _pendingVariableChangesDictionary;

    public GameState(int coins, int stars)
    {
        _variableListenersDictionary = new Dictionary<IGameStateVariable, HashSet<Action>>();
        _pendingVariableChangesDictionary = new Dictionary<Action, HashSet<IGameStateVariable>>();
        Coins = new GameStateVariable<int>(coins, OnAnyValueChanged, "Coins");
        Stars = new GameStateVariable<int>(stars, OnAnyValueChanged, "Stars");
    }

    private void OnAnyValueChanged(IGameStateVariable changedVariable)
    {
        var listeners = _variableListenersDictionary[changedVariable];
        foreach (var listener in listeners)
        {
            if (CanNotifyListener(changedVariable, listener))
            {
                NotifyListener(listener);
            }
        }
    }

    private void NotifyListener(Action action)
    {
        action.Invoke();
        _pendingVariableChangesDictionary.Remove(action);
    }

    private bool CanNotifyListener(IGameStateVariable changedVariable, Action action)
    {
        _pendingVariableChangesDictionary[action].Remove(changedVariable);
        return _pendingVariableChangesDictionary[action].Count < 1;
    }

    public void ListenFor<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        AddListenerToVariableDictionary(variable, listenerAction);

        AddVariableToPendingDictionary(variable, listenerAction);
    }

    private void AddVariableToPendingDictionary<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        if (_pendingVariableChangesDictionary.TryGetValue(listenerAction, out var variables))
        {
            if (!variables.Add(variable))
            {
                Debug.LogWarning($"{variable.DebugName} exists in {listenerAction.Method.Name} pending dictionary");
            }
        }

        else
        {
            _pendingVariableChangesDictionary[listenerAction] = new HashSet<IGameStateVariable>() { variable };
        }
    }

    private void AddListenerToVariableDictionary<T>(GameStateVariable<T> variable, Action listenerAction)
    {
        if (_variableListenersDictionary.TryGetValue(variable, out var listeners))
        {
            if (!listeners.Add(listenerAction))
            {
                Debug.LogWarning($"{listenerAction.Method.Name} exists in listener dictionary of {variable.DebugName}");
            }
        }

        else
        {
            _variableListenersDictionary[variable] = new HashSet<Action>() { listenerAction };
        }
    }
}