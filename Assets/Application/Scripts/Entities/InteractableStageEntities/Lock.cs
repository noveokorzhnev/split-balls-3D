using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Lock : BallSpawnerEntity, IBallDestroyer
{
    [SerializeField]
    private int burst;
    protected override int AmountOfBallsToSpawn => burst - 1;

    public event System.Action onBallDestroyed;

    public override void OnBallEnters(Ball ball)
    {
        base.OnBallEnters(ball);

        if (value > 0)
        {
            value--;
            OnValueSet();

            if (value == 0)
            {
                foreach (var renderer in GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }

                // TODO: Play effect.
                MultiplyBall(ball);
            }
            else
            {
                Destroy(ball.gameObject);

                animatedDisplay.DOKill();
                animatedDisplay.localScale = Vector3.one;
                animatedDisplay.DOPunchScale(Vector3.one * 0.15f, 0.2f);

                onBallDestroyed?.Invoke();
            }
        }
    }
}
