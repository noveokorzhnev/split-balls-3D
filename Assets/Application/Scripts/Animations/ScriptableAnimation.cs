using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableAnimation : MonoBehaviour
{
    [Header("Auto-play settings")]
    [SerializeField]
    private bool playOnEnable;
    [SerializeField]
    private bool stopOnDisable;

    public abstract void Play();
    public abstract void Stop();

    protected virtual void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    protected virtual void OnDisable()
    {
        if (stopOnDisable)
        {
            Stop();
        }
    }
}
