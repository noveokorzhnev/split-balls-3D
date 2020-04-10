using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableStageEntity : MonoBehaviour, IInteractWithBallOnEnter, IInteractWithBallOnExit
{
    public Collider Collider { get; private set; }

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    public virtual void OnBallEnters(Ball ball)
    {

    }

    public virtual void OnBallExits(Ball ball)
    {

    }
}
