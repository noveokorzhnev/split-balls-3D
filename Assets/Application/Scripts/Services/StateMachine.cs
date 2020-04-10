using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<S> where S : System.Enum
{
    protected S previousState;
    protected S currentState;
    protected bool isStateValid;

    protected HashSet<(S, S)> transitions;
    protected Dictionary<S, System.Action> exitCallbacks;
    protected Dictionary<S, System.Action> enterCallbacks;
    protected Dictionary<(S, S), System.Action> transitionCallbacks;

    public StateMachine()
    {
        transitions = new HashSet<(S, S)>();
        exitCallbacks = new Dictionary<S, System.Action>();
        enterCallbacks = new Dictionary<S, System.Action>();
        transitionCallbacks = new Dictionary<(S, S), System.Action>();
    }

    protected void AddTransition(S from, S to)
    {
        transitions.Add((from, to));
    }

    protected void AddCallbackOnExit(S state, System.Action callback)
    {
        if (exitCallbacks.ContainsKey(state))
        {
            exitCallbacks[state] += callback;
        }
        else
        {
            exitCallbacks.Add(state, callback);
        }
    }

    protected void AddCallbackOnTransition(S from, S to, System.Action callback)
    {
        var key = (from, to);
        if (transitionCallbacks.ContainsKey(key))
        {
            transitionCallbacks[key] += callback;
        }
        else
        {
            transitionCallbacks.Add(key, callback);
        }
    }

    protected void AddCallbackOnEnter(S state, System.Action callback)
    {
        if (enterCallbacks.ContainsKey(state))
        {
            enterCallbacks[state] += callback;
        }
        else
        {
            enterCallbacks.Add(state, callback);
        }
    }

    protected void Exit()
    {
        if (!isStateValid)
        {
            // Could not exit from invalid state.
            return;
        }

        previousState = currentState;
        isStateValid = false;

        FireCallbacks(onExit: true);
    }

    protected void Enter(S nextState)
    {
        if (isStateValid)
        {
            // Could not enter state while current state is valid.
            return;
        }

        currentState = nextState;
        isStateValid = true;

        FireCallbacks(onEnter: true);
    }

    protected void Advance(S nextState)
    {
        if (!isStateValid)
        {
            // Could not advance from invalid state.
            return;
        }

        if (!transitions.Contains((currentState, nextState)))
        {
            // Undefined transition.
            return;
        }

        previousState = currentState;
        currentState = nextState;

        FireCallbacks(onExit: true, onTransition: true, onEnter: true);
    }

    private void FireCallbacks(bool onExit = false, bool onTransition = false, bool onEnter = false)
    {
        System.Action callback;
        if (onExit && exitCallbacks.TryGetValue(previousState, out callback))
        {
            callback?.Invoke();
        }
        if (onTransition && transitionCallbacks.TryGetValue((previousState, currentState), out callback))
        {
            callback?.Invoke();
        }
        if (onEnter && enterCallbacks.TryGetValue(currentState, out callback))
        {
            callback?.Invoke();
        }
    }
}
