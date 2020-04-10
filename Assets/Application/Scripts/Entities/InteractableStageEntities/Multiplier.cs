using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiplier : BallSpawnerEntity
{
    protected override string LabelFormat => base.LabelFormat + "x";

    // Multiplier should spawn (value - 1) additional balls.
    protected override int AmountOfBallsToSpawn => value - 1;  
    
    public override void OnBallEnters(Ball ball)
    {
        base.OnBallEnters(ball);

        MultiplyBall(ball);
    }
}
