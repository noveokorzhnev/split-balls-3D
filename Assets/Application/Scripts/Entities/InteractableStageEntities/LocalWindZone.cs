using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalWindZone : InteractableStageEntity
{
    public static event System.Func<float> getWindForce;

    [SerializeField]
    private float maxWindForce;
    [SerializeField]
    private Vector3 windDirection;

    private Vector3 windVector;
    private HashSet<Ball> controlledBalls;

    protected override void Awake()
    {
        base.Awake();

        windVector = windDirection.normalized * maxWindForce;
        controlledBalls = new HashSet<Ball>();
    }

    private void FixedUpdate()
    {
        foreach (var ball in controlledBalls)
        {
            ball.AddForce(windVector * getWindForce());
        }
    }

    public override void OnBallEnters(Ball ball)
    {
        base.OnBallEnters(ball);

        controlledBalls.Add(ball);
    }

    public override void OnBallExits(Ball ball)
    {
        base.OnBallExits(ball);

        controlledBalls.Remove(ball);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, windDirection);
        Gizmos.color = Color.white;
    }
#endif
}
