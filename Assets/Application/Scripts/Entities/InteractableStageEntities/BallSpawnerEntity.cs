using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public abstract class BallSpawnerEntity : InteractableStageEntityWithValue, IBallSpawner
{
    protected abstract int AmountOfBallsToSpawn { get; }

    [SerializeField]
    protected float scaleOfBalls = 1;

    private Vector3 spawnPointMin;
    private Vector3 spawnPointMax;

    private Stack<Ball.Shared> ballsToSpawn;

    protected Transform animatedDisplay;

    public event System.Func<Vector3, float, Ball> spawnBallAtPositionWithScale;

    protected override void Awake()
    {
        base.Awake();

        ballsToSpawn = new Stack<Ball.Shared>();
        animatedDisplay = transform.Find("Attachments/Display");

        var colliderBounds = Collider.bounds;
        spawnPointMin = new Vector3(colliderBounds.min.x, colliderBounds.center.y, colliderBounds.center.z);
        spawnPointMax = new Vector3(colliderBounds.max.x, colliderBounds.center.y, colliderBounds.center.z);
    }

    private void Update()
    {
        if (ballsToSpawn.Any())
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        Vector3 testPosition = Vector3.Lerp(spawnPointMin, spawnPointMax, Random.value);

        if (spawnBallAtPositionWithScale != null)
        {
            var ball = spawnBallAtPositionWithScale(testPosition, scaleOfBalls);
            if (ball != null)
            {
                ball.SharedData = ballsToSpawn.Pop();
            }
        }
    }

    protected void MultiplyBall(Ball ball)
    {
        // Animate display.
        animatedDisplay.DOKill();
        animatedDisplay.localScale = Vector3.one;
        animatedDisplay.DOPunchScale(Vector3.one * 0.15f, 0.2f);

        // Apply new scale for the ball.
        ball.transform.localScale = Vector3.one * scaleOfBalls;

        // Mark the ball as multiplied.
        ball.AddToNewSharedData(Collider);

        // Spawn the required additional balls amount.
        for (int i = 0; i < AmountOfBallsToSpawn; i++)
        {
            ballsToSpawn.Push(ball.SharedData);
        }      
    }
}
