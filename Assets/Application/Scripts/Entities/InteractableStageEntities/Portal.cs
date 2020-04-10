using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : InteractableStageEntity
{
    [SerializeField]
    private Transform @out;

    public override void OnBallEnters(Ball ball)
    {
        base.OnBallEnters(ball);

        ball.transform.position = @out.position;
    }
}
