using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public struct Shared
    {
        public HashSet<Collider> ignoringTriggerEnter;
    }

    public static event System.Action<Ball, Collider> onBallEnteredTrigger;
    public static event System.Action<Ball, Collider> onBallExitedTrigger;

    private Rigidbody mRigidbody;
    private Rigidbody Rigidbody
    {
        get
        {
            if (mRigidbody == null)
            {
                mRigidbody = GetComponent<Rigidbody>();
            }
            return mRigidbody;
        }
    }

    public Shared SharedData { get; set; }

    public void AddToNewSharedData(Collider trigger)
    {
        if (SharedData.ignoringTriggerEnter == null)
        {
            SharedData = new Shared() 
            { 
                ignoringTriggerEnter = new HashSet<Collider>() 
            };
        }
        else
        {
            SharedData = new Shared()
            {
                ignoringTriggerEnter = new HashSet<Collider>(SharedData.ignoringTriggerEnter)
            };
        }

        SharedData.ignoringTriggerEnter.Add(trigger);
    }

    public void AddForce(Vector3 force)
    {
        Rigidbody.AddForce(force, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (SharedData.ignoringTriggerEnter?.Contains(trigger) ?? false)
        {
            return;
        }

        onBallEnteredTrigger?.Invoke(this, trigger);
    }

    private void OnTriggerExit(Collider trigger)
    {
        onBallExitedTrigger?.Invoke(this, trigger);
    }
}
